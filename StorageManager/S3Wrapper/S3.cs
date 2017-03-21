using System;
using System.Collections.Generic;
using Amazon.S3;
using Amazon.S3.Transfer;
using System.IO;
using Amazon.S3.Model;
using Utility;
using System.Drawing;
using System.Text;

namespace StorageManager.S3Wrapper
{

    public class S3 : IDisposable
    {
		private TransferUtility m_TransUtil;
		private IAmazonS3 m_Client;
		private string m_S3Bucket = "followme.usercontent";

        public S3()
        {
            try
            {
                m_Client = new AmazonS3Client(Amazon.RegionEndpoint.USWest2);
                m_TransUtil = new TransferUtility(m_Client);
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
                m_Client = new AmazonS3Client(accessKey, secretAccessKey, token, Amazon.RegionEndpoint.USWest2);
                m_TransUtil = new TransferUtility(m_Client);
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
                BucketName = m_S3Bucket,
                InputStream = memStream,
                StorageClass = S3StorageClass.ReducedRedundancy,
                Key = "Users/" + userId + "/Profile/ProfileImage",
            };

            try
            {
                m_TransUtil.Upload(uploadReq);
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
                BucketName = m_S3Bucket,
                InputStream = new MemoryStream(imageInBytes),
                StorageClass = S3StorageClass.ReducedRedundancy,
                Key = "Users/" + userId + "/Profile/ProfileImage",
            };

            try
            {
                m_TransUtil.Upload(uploadReq);
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
		public string GetUserProfileImageURL(string userId)
		{
            GetPreSignedUrlRequest req = new GetPreSignedUrlRequest()
            {
                BucketName = m_S3Bucket,
                Key = string.Format("Users/{0}/Profile/ProfileImage", userId),
                Expires = DateTime.Now.AddHours(1),
                Protocol = Protocol.HTTP,
            };

			try
			{
                return m_Client.GetPreSignedURL(req);
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
            string responseBody = "";

            GetObjectRequest request = new GetObjectRequest
            {
                BucketName = m_S3Bucket,
                Key = "Users/" + userId + "/Profile" + "/ProfileImage"
            };

            try
            {
                using (GetObjectResponse s3Res = m_Client.GetObject(request))
                using (Stream responseStream = s3Res.ResponseStream)
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    responseBody = reader.ReadToEnd();
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
                Key = string.Format("Trips/{}/{}/{}", tripId, userId, contentGuid),
                AutoCloseStream = true,
                StorageClass = S3StorageClass.ReducedRedundancy
            };

            try
            {
                m_TransUtil.Upload(req);
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
            string responseBody = string.Empty;

            GetPreSignedUrlRequest req = new GetPreSignedUrlRequest
            {
                BucketName = m_S3Bucket,
                Key = string.Format("Trips/{0}/{1}/{2}", tripId, userId, contentGuid),
                Expires = DateTime.Now.AddHours(1),
                Protocol = Protocol.HTTP,
            };

            try
            {
                return m_Client.GetPreSignedURL(req);
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
                BucketName = m_S3Bucket,
                MaxKeys = 50,
                StartAfter = string.Format("Trips/{0}/{1}/", tripId, userId),
            };

            ListObjectsV2Response listRes = m_Client.ListObjectsV2(listReq);

            foreach (var item in listRes.S3Objects)
            {
                GetPreSignedUrlRequest urlReq = new GetPreSignedUrlRequest
                {
                    BucketName = m_S3Bucket,
                    Key = item.Key,
                    Expires = DateTime.Now.AddHours(1),
                    Protocol = Protocol.HTTP
                };

                urls.Add(m_Client.GetPreSignedURL(urlReq));
            }

            return (urls.Count > 0) ? urls : null; 
        }

        public void Dispose()
		{
			m_Client.Dispose();
			m_TransUtil.Dispose();
		}
	}
}

