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
        private string m_UserBucketName = "followme.usercontent";

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
                BucketName = m_UserBucketName,
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
                BucketName = m_UserBucketName,
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

            return true;
        }

    }
}
