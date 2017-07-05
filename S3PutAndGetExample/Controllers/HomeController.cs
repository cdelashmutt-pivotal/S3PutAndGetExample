using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace S3PutAndGetExample.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult S3Test()
        {
            string bucketName = "s3test";
            string keyName = "Data.txt";
            using (var client = new AmazonS3Client("<your-access-key>", "<your-secret-key>", 
                new AmazonS3Config { ServiceURL = "<your-minio-hostname>", SignatureVersion="2", ForcePathStyle = true }))
            {
                //Create the bucket if it isn't there
                if (!(AmazonS3Util.DoesS3BucketExist(client, bucketName)))
                {
                    CreateABucket(client, bucketName);
                }

                //Shove the current date/time into a key called "Data.txt"
                client.PutObject( new PutObjectRequest { 
                    BucketName = bucketName,
                    Key = keyName,
                    ContentBody = DateTime.Now.ToString()
                });

                using (GetObjectResponse response = client.GetObject(bucketName, keyName))
                {
                    using (StreamReader reader = new StreamReader(response.ResponseStream))
                    {
                        ViewBag.Message = reader.ReadToEnd();
                    }
                }

            }

            return View();
        }

        private void CreateABucket(IAmazonS3 client, string bucketName)
        {
            try
            {
                PutBucketRequest putRequest1 = new PutBucketRequest
                {
                    BucketName = bucketName,
                    UseClientRegion = true
                };

                PutBucketResponse response1 = client.PutBucket(putRequest1);
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                    ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    Console.WriteLine("Check the provided AWS Credentials.");
                    Console.WriteLine(
                        "For service sign up go to http://aws.amazon.com/s3");
                }
                else
                {
                    Console.WriteLine(
                        "Error occurred. Message:'{0}' when writing an object"
                        , amazonS3Exception.Message);
                }
            }
        }
    }
}