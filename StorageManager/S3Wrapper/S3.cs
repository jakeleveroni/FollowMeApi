using System;
using System.Collections.Generic;
using Amazon.S3;
using Amazon.S3.Transfer;
using System.IO;
using Amazon.S3.Model;
using Utility;

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
                Logger.logger.Error("[S3-CONSTRUCTOR][ERROR] : Could not create S3 client,) " + ex.Message);
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
                m_TransUtil.Upload(uploadReq);
            }
            catch (Exception ex)
            {
                Logger.logger.Error("[S3-UPLOAD_PROFILE_MAGE][ERROR] : Could not upload profile image, " + ex.Message);
            }

            return true;
        }

        public bool UploadTripContent(string userId, string tripId, string pathToFile)
        {
            string fileName = string.Empty;
            try
            {
                fileName = Path.GetFileName(pathToFile);
            }
            catch (Exception ex)
            {
                Logger.logger.Error("[S3-UPLOAD_TRIP_CONTENT][ERROR] : Could not upload trip content, " + ex.Message);
                return false;
            }

            TransferUtilityUploadRequest uploadReq = new TransferUtilityUploadRequest
            {
                BucketName = m_S3Bucket,
                FilePath = pathToFile,
                StorageClass = S3StorageClass.ReducedRedundancy,
                Key = "Trips/" + tripId + "/Content/" + fileName,
            };

			uploadReq.Metadata.Add("content-owner", userId);

            try
            {
                m_TransUtil.Upload(uploadReq);
            }
            catch (Exception ex)
            {
                Logger.logger.Error("[S3-UPLOAD_TRIP_CONTENT][ERROR] : Could not upload content, " + ex.Message);
            }

            return true;
        }

		// NOTE: Untested code, not sure if working correctly at the moment

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
				response = m_Client.ListObjectsV2(request);
			}
			catch (AmazonS3Exception amazonS3Exception)
			{
				if (amazonS3Exception.ErrorCode != null && (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") || amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
				{
                    Logger.logger.Error("[S3-GET_USER_PROFILE][ERROR] : AWS credentials are not valid");
				}
				else
				{
                    Logger.logger.Error("[S3-GET_USER_PROFILE][ERROR] : AWS error message, " + amazonS3Exception.Message);
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
					response = m_Client.ListObjectsV2(request);

					foreach (S3Object entry in response.S3Objects)
					{
						fileNames.Add(m_S3Bucket + "/Trips/" + tripId + "/Content/" + entry.Key);
					}

					request.ContinuationToken = response.NextContinuationToken;

				} while (response.IsTruncated);
			}
			catch (AmazonS3Exception amazonS3Exception)
			{
				if (amazonS3Exception.ErrorCode != null && (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") ||amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
				{
                    Logger.logger.Error("[S3-GET_TRIP_CONTENTS][ERROR] : AWS credentials are not valid");
				}
				else
				{
                    Logger.logger.Error("[S3-GET_TRIP_CONTENTS][ERROR] : AWS error message, " +  amazonS3Exception.Message);
				}
			}

			return fileNames;
		}

		public List<string> GetTripContentARNListByOwner(string userId, string tripId)
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
					response = m_Client.ListObjectsV2(request);

					foreach (S3Object entry in response.S3Objects)
					{
						if (response.ResponseMetadata.Metadata["owner"] == userId)
						{
							fileNames.Add(m_S3Bucket + "/Trips/" + tripId + "/Content/" + entry.Key);
						}
					}


					request.ContinuationToken = response.NextContinuationToken;
				} while (response.IsTruncated);
			}
			catch (AmazonS3Exception amazonS3Exception)
			{
				if (amazonS3Exception.ErrorCode != null && (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") || amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
				{
                    Logger.logger.Error("[S3-GET_TRIP_CONTENTS][ERROR] : AWS credentials are not valid");
				}
				else
				{
                    Logger.logger.Error("[S3-GET_TRIP_CONTENTS][ERROR] : AWS error message, " + amazonS3Exception.Message);
				}
			}

			return fileNames;
		}

		public void Dispose()
		{
			m_Client.Dispose();
			m_TransUtil.Dispose();
		}
	}
}

