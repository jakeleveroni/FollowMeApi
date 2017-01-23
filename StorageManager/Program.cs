using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3;
using System.Drawing;

using StorageManager.S3Wrapper;

namespace StorageManager
{
    class Program
    {
        static void Main(string[] args)
        {
            string EnvName = Environment.UserName;

            S3 s3 = new S3();

            // upload testing content
            //s3.UploadProfileImage("00000000-0000-0000-000000000000", string.Format("C:\\Users\\{0}\\documents\\visual studio 2015\\Projects\\FollowMeApi\\StorageManager\\TestAssets\\testProfile.jpg", EnvName));
            //s3.UploadTripContent("11111111-1111-1111-111111111111", string.Format("C:\\Users\\{0}\\documents\\visual studio 2015\\Projects\\FollowMeApi\\StorageManager\\TestAssets\\testImage1.jpg", EnvName));
            //s3.UploadTripContent("11111111-1111-1111-111111111111", string.Format("C:\\Users\\{0}\\documents\\visual studio 2015\\Projects\\FollowMeApi\\StorageManager\\TestAssets\\testImage2.jpg", EnvName));
            //s3.UploadTripContent("11111111-1111-1111-111111111111", string.Format("C:\\Users\\{0}\\documents\\visual studio 2015\\Projects\\FollowMeApi\\StorageManager\\TestAssets\\testImage3.jpg", EnvName));

            Console.ReadKey();
        }
    }
}
