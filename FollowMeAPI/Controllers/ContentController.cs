using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FollowMeAPI.Sessions;
using System.Threading.Tasks;

namespace FollowMeAPI.Controllers
{
    public class ContentController : ApiController
    {
        [HttpGet]
        [Route("url-user-content/get")]
        public string GetProfileImageUrl()
        {
            string url = null;
            string token = null, userId = null;

            if (Request.Headers.Contains("Token"))
            {
                token = Request.Headers.GetValues("Token").FirstOrDefault();
            }

            if (SessionManager.ValidateSession(token))
            {
                if (Request.Headers.Contains("UserId"))
                {
                    userId = Request.Headers.GetValues("UserId").FirstOrDefault();
                }

                if (string.IsNullOrEmpty(userId))
                {
                    url = WebApiApplication.s3.GetUserProfileImageUrl(userId);
                }
            }

            return url;
        }

        [HttpGet]
        [Route("base64string-user-content/get")]
        public string GetProfileImageBase64()
        {
            string response = null;
            string token = null, userId = null;


            if (Request.Headers.Contains("Token"))
            {
                token = Request.Headers.GetValues("Token").FirstOrDefault();
            }

            if (!SessionManager.ValidateSession(token))
            {

            }
            else
            {
                if (Request.Headers.Contains("UserId"))
                {
                    userId = Request.Headers.GetValues("UserId").FirstOrDefault();
                }

                if (string.IsNullOrEmpty(userId))
                {
                    response = WebApiApplication.s3.GetUserProfileImageBase64(userId);
                }
            }

            return response;
        }

        [HttpPost]
        [Route("frombody-user-content/upload")]
        public HttpResponseMessage FromBodyPostProfileImage([FromBody] string content)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            string token = null, userId = null;

            if (string.IsNullOrEmpty(content))
            {
                response.StatusCode = HttpStatusCode.NoContent;
                return response;
            }

            if (Request.Headers.Contains("Token"))
            {
                token = Request.Headers.GetValues("Token").FirstOrDefault();
            }

            if (SessionManager.ValidateSession(token))
            {
                if (Request.Headers.Contains("UserId"))
                {
                    userId = Request.Headers.GetValues("UserId").FirstOrDefault();
                }

                if (!string.IsNullOrEmpty(userId))
                {
                    WebApiApplication.s3.UploadProfileImageFromBase64String(userId, content);
                }
                else
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    return response;
                }
            }

            response.StatusCode = HttpStatusCode.OK;
            return response;
        }

        // NOTE: random error codes for debugging purposes
        [HttpPost]
        [Route("user-content/upload")]
        public async Task<HttpResponseMessage> MultiPartPostProfileImage()
        {
            string token = null, userId = null;

            if (Request.Headers.Contains("Token"))
            {
                token = Request.Headers.GetValues("Token").FirstOrDefault();
            }

            if (SessionManager.ValidateSession(token))
            {
                if (Request.Headers.Contains("UserId"))
                {
                    userId = Request.Headers.GetValues("UserId").FirstOrDefault();
                }

                if (!string.IsNullOrEmpty(userId))
                {
                    // verify request is multipart form data type
                    if (!Request.Content.IsMimeMultipartContent())
                    {
                        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                    }

                    try
                    {
                        MultipartMemoryStreamProvider incomingFiles = await Request.Content.ReadAsMultipartAsync();

                        foreach (var stream in incomingFiles.Contents)
                        {
                            byte[] fileAsBytes = await stream.ReadAsByteArrayAsync();
                            string base64Image = Convert.ToBase64String(fileAsBytes);
                            WebApiApplication.s3.UploadProfileImageFromBase64String(userId, base64Image);
                        }

                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    catch (Exception e)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NoContent, e.Message);
                    }
                }
            }

            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }
    }
}
