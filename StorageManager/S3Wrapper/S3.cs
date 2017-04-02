using System;
using System.Collections.Generic;
using Amazon.S3;
using Amazon.S3.Transfer;
using System.IO;
using Amazon.S3.Model;
using Utility;
using System.Drawing;

namespace StorageManager.S3Wrapper
{
    public class S3 : IDisposable
    {
		private readonly TransferUtility _mTransUtil;
		private readonly IAmazonS3 _mClient;
		private const string Ms3Bucket = "followme.usercontent";

        public S3()
        {
            try
            {
                _mClient = new AmazonS3Client(Amazon.RegionEndpoint.USWest2);
                _mTransUtil = new TransferUtility(_mClient);
            }
            catch (Exception ex)
            {
                Tools.logger.Error("[S3-CONSTRUCTOR][ERROR] : Could not create S3 client, " + ex.Message);
            }
        }

        public S3(string accessKey, string secretAccessKey, string token)
        {
            try
            {
                _mClient = new AmazonS3Client(accessKey, secretAccessKey, token, Amazon.RegionEndpoint.USWest2);
                _mTransUtil = new TransferUtility(_mClient);
            }
            catch(Exception ex)
            {
                Tools.logger.Error("[S3-CONSTRUCTOR][ERROR] : Could not create S3 client, " + ex.Message);
            }
        }

        /*
         * UPLOAD AND DOWNLOAD FOR USER PROFILE PICTURES TO S3 
        */    

        // uploads a base 64 string representation of an image to the 
        // amazon s3 bucket
        public bool UploadProfileImageFromBase64String(string userId, string base64Image)
        {
            MemoryStream memStream = new MemoryStream();
            StreamWriter writer = new StreamWriter(memStream);
            writer.Write(base64Image);
            writer.Flush();
            memStream.Position = 0;

            TransferUtilityUploadRequest uploadReq = new TransferUtilityUploadRequest
            {
                BucketName = Ms3Bucket,
                InputStream = memStream,
                StorageClass = S3StorageClass.ReducedRedundancy,
                Key = "Users/" + userId + "/Profile/ProfileImage",
            };

            try
            {
                _mTransUtil.Upload(uploadReq);
            }
            catch (Exception ex)
            {
                Tools.logger.Error("[S3-UPLOAD_PROFILE_MAGE][ERROR] : Could not convert image to bytes, " + ex.Message);
                return false;
            }

            return true;
        }

        // USED WHEN FILES ARE SENT TO SERVER THEN SAVED 
        // LOCALLY ON SERVER BEFORE BEING SENT TO S3
        public bool UploadProfileImageFromLocalStorage(string userId, string localFilePath)
        {
            Image img = Image.FromFile(localFilePath);
            byte[] imageInBytes;

            using (var ms = new MemoryStream())
            {
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                imageInBytes = ms.ToArray();
            }

            TransferUtilityUploadRequest uploadReq = new TransferUtilityUploadRequest
            {
                BucketName = Ms3Bucket,
                InputStream = new MemoryStream(imageInBytes),
                StorageClass = S3StorageClass.ReducedRedundancy,
                Key = "Users/" + userId + "/Profile/ProfileImage",
            };

            try
            {
                _mTransUtil.Upload(uploadReq);
                return true;
            }
            catch (Exception ex)
            {
                Tools.logger.Error("[S3-UPLOAD_PROFILE_MAGE][ERROR] : Could not convert image to bytes, " + ex.Message);
                return false;
            }
        }

        // returns a string URL that points to the requested profile picture
        // this means that the asset must be made public when its uploaded
		public string GetUserProfileImageUrl(string userId)
		{
            GetPreSignedUrlRequest req = new GetPreSignedUrlRequest()
            {
                BucketName = Ms3Bucket,
                Key = $"Users/{userId}/Profile/ProfileImage",
                Expires = DateTime.Now.AddHours(1),
                Protocol = Protocol.HTTP,
            };

			try
			{
                return _mClient.GetPreSignedURL(req);
			}
			catch (AmazonS3Exception amazonS3Exception)
			{
				if (amazonS3Exception.ErrorCode != null && (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") || amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
				{
                    Tools.logger.Error("[S3-GET_USER_PROFILE][ERROR] : AWS credentials are not valid");
				}
				else
				{
                    Tools.logger.Error("[S3-GET_USER_PROFILE][ERROR] : AWS error message, " + amazonS3Exception.Message);
				}

				return null;
			}
 		}

        // Returns a base 64 encoded string that represents the user
        // profile image
        public string GetUserProfileImageBase64(string userId)
        {
            GetObjectRequest request = new GetObjectRequest
            {
                BucketName = Ms3Bucket,
                Key = "Users/" + userId + "/Profile" + "/ProfileImage"
            };

            try
            {
                using (GetObjectResponse s3Res = _mClient.GetObject(request))
                using (Stream responseStream = s3Res.ResponseStream)
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    string responseBody = reader.ReadToEnd();
                    return responseBody;
                }
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null && (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") || amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    Tools.logger.Error("[S3-GET_USER_PROFILE][ERROR] : AWS credentials are not valid");
                }
                else
                {
                    Tools.logger.Error("[S3-GET_USER_PROFILE][ERROR] : AWS error message, " + amazonS3Exception.Message);
                }

                return null;
            }
        }

        /*
         * UPLOAD AND DOWNLOAD FOR USER TRIP CONTENT 
        */

        public bool UploadContentToTrip(string tripId, string userId, string contentGuid, string base64Content)
        {
            MemoryStream memStream = new MemoryStream();
            StreamWriter writer = new StreamWriter(memStream);
            writer.Write(base64Content);
            writer.Flush();
            memStream.Position = 0;

            TransferUtilityUploadRequest req = new TransferUtilityUploadRequest
            {
                InputStream = memStream,
                Key = $"Trips/{tripId}/{userId}/{contentGuid}",
                AutoCloseStream = true,
                StorageClass = S3StorageClass.ReducedRedundancy
            };

            try
            {
                _mTransUtil.Upload(req);
            }
            catch (Exception ex)
            {
                Tools.logger.Error("[S3-UPLOAD_PROFILE_MAGE][ERROR] : Could not upload the content to the trip, " + ex.Message);
                return false;
            }

            return true;
        }

        public string DownloadSingleItemFromTrip(string userId, string tripId, string contentGuid)
        {
            GetPreSignedUrlRequest req = new GetPreSignedUrlRequest
            {
                BucketName = Ms3Bucket,
                Key = $"Trips/{tripId}/{userId}/{contentGuid}",
                Expires = DateTime.Now.AddHours(1),
                Protocol = Protocol.HTTP,
            };

            try
            {
                return _mClient.GetPreSignedURL(req);
            }
            catch (Exception ex)
            {
                Tools.logger.Error("[S3-DOWNLOAD-SINGLE-ITEM][ERROR] : Could not get pre-signed content URL, " + ex.Message);
                return string.Empty;
            }
        }

        public List<string> DownloadAllUserTripContent(string userId, string tripId)
        {
            List<string> urls = new List<string>();
            ListObjectsV2Request listReq = new ListObjectsV2Request
            {
                BucketName = Ms3Bucket,
                MaxKeys = 50,
                StartAfter = $"Trips/{tripId}/{userId}/",
            };

            ListObjectsV2Response listRes = _mClient.ListObjectsV2(listReq);

            foreach (var item in listRes.S3Objects)
            {
                GetPreSignedUrlRequest urlReq = new GetPreSignedUrlRequest
                {
                    BucketName = Ms3Bucket,
                    Key = item.Key,
                    Expires = DateTime.Now.AddHours(1),
                    Protocol = Protocol.HTTP
                };

                urls.Add(_mClient.GetPreSignedURL(urlReq));
            }

            return (urls.Count > 0) ? urls : null; 
        }

        public void Dispose()
		{
			_mClient.Dispose();
			_mTransUtil.Dispose();
		}
	}
}

