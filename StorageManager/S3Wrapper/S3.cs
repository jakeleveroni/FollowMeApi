using System;
using System.Collections.Generic;
using Amazon.S3;
using Amazon.S3.Transfer;
using System.IO;
using Amazon.S3.Model;
using System.Drawing;

namespace StorageManager.S3Wrapper
{
    
    public class S3
    {
        TransferUtility TransUtil;
        public IAmazonS3 client;
		private string m_S3Bucket = "followme.usercontent";

        public S3()
        {
            try
            {
                client = new AmazonS3Client(Amazon.RegionEndpoint.USWest2);
                TransUtil = new TransferUtility(client);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[S3-CONSTRUCTOR][ERROR] : Could not create S3 client,) " + ex.Message);
            }
        }


        public bool UploadProfileImage(string userId, string pathToFile)
        {
            TransferUtilityUploadRequest uploadReq = new TransferUtilityUploadRequest
            {
                BucketName = m_S3Bucket,
                FilePath = pathToFile,
                StorageClass = S3StorageClass.ReducedRedundancy,
                Key = "Users/" + userId + "/Profile/ProfileImage",
            };

            try 
            {
                TransUtil.Upload(uploadReq);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[S3-UPLOAD_PROFILE_MAGE][ERROR] : Could not upload profile image, " + ex.Message);
            }

            return true;
        }

        public bool UploadTripContent(string tripId, string pathToFile)
        {
            string fileName = string.Empty;
            try
            {
                fileName = Path.GetFileName(pathToFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[S3-UPLOAD_TRIP_CONTENT][ERROR] : Could not upload trip content, " + ex.Message);
                return false;
            }

            TransferUtilityUploadRequest uploadReq = new TransferUtilityUploadRequest
            {
                BucketName = m_S3Bucket,
                FilePath = pathToFile,
                StorageClass = S3StorageClass.ReducedRedundancy,
                Key = "Trips/" + tripId + "/Content/" + fileName,
            };

            try
            {
                TransUtil.Upload(uploadReq);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[S3-UPLOAD_TRIP_CONTENT][ERROR] : Could not upload content, " + ex.Message);
            }

            return true;
        }

		// returns the ARN of the users profile image
		public string GetUserProfileImageARN(string userId)
		{
			ListObjectsV2Response response = null;
			ListObjectsV2Request request = new ListObjectsV2Request
			{
				BucketName = m_S3Bucket + "/Users/" + userId + "/Profile/",
				MaxKeys = 1,
			};

			try
			{
				response = client.ListObjectsV2(request);
			}
			catch (AmazonS3Exception amazonS3Exception)
			{
				if (amazonS3Exception.ErrorCode != null && (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") || amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
				{
					Console.WriteLine("[S3-GET_USER_PROFILE][ERROR] : AWS credentials are not valid");
				}
				else
				{
					Console.WriteLine("[S3-GET_USER_PROFILE][ERROR] : AWS error message, {0}", amazonS3Exception.Message);
				}

				return null;
			}

			return response.S3Objects[0].Key;
 		}

		// returns a list of ARNS that point to trip content, if error occurs returns empty list
		public List<string> GetTripContentARNList(string tripId)
		{
			List<string> fileNames = new List<string>();
			ListObjectsV2Response response;

			try
			{
				ListObjectsV2Request request = new ListObjectsV2Request
				{
					BucketName = m_S3Bucket + "/Trips/" + tripId + "/Content/",
					MaxKeys = 100,
				};

				do
				{
					response = client.ListObjectsV2(request);

					foreach (S3Object entry in response.S3Objects)
					{
						fileNames.Add(entry.Key);
					}

					request.ContinuationToken = response.NextContinuationToken;

				} while (response.IsTruncated);
			}
			catch (AmazonS3Exception amazonS3Exception)
			{
				if (amazonS3Exception.ErrorCode != null && (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") ||amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
				{
					Console.WriteLine("[S3-GET_TRIP_CONTENTS][ERROR] : AWS credentials are not valid");
				}
				else
				{
					Console.WriteLine("[S3-GET_TRIP_CONTENTS][ERROR] : AWS error message, {0}", amazonS3Exception.Message);
				}
			}

			return fileNames;
		}

    }
}
