using System;
using System.Linq;
using System.Web.Http;
using System.Collections.Generic;
using FollowMeDataBase.Models;
using Newtonsoft.Json;
using Utility;
using FollowMeAPI.Sessions;

namespace FollowMeAPI.Controllers
{
	[RoutePrefix("Moments")]
	public class MomentsController : ApiController
	{
		[HttpGet]
		[Route("moments/get")]
		public MomentModel GetMoment()
		{
			string token = null;
			string momentId = null;

			if (Request.Headers.Contains("Token"))
			{
				token = Request.Headers.GetValues("Token").FirstOrDefault();
			}

			if (SessionManager.ValidateSession(token))
			{
				if (Request.Headers.Contains("MomentGuid"))
				{
					momentId = Request.Headers.GetValues("MomentGuid").FirstOrDefault();

					try
					{
						return JsonConvert.DeserializeObject<MomentModel>(WebApiApplication.db.GetMoment(momentId));
					}
					catch (Exception ex)
					{
						Tools.logger.Error("Could not post moment to db, " + ex.Message);
						return null;
					}
				}
				else
				{
					Tools.logger.Error("[MOMENTS CONTROLLER GET][ERROR] : No moment id provided");
					return null;
				}
			}
			else
			{
				Tools.logger.Error("[MOMENTS CONTROLLER GET][ERROR] : No token provided");
				return null;
			}
		}

		[HttpPost]
		[Route("moments/new")]
		public MomentModel PostMoment([FromBody] string jsonModel)
		{
			string token = null;
			string tripId = null;
			string type = null;

			if (Request.Headers.Contains("Token"))
			{
				token = Request.Headers.GetValues("Token").FirstOrDefault();
			}

			if (SessionManager.ValidateSession(token))
			{
				dynamic momentObj;

				if (Request.Headers.Contains("TripGuid"))
				{
					tripId = Request.Headers.GetValues("TripGuid").FirstOrDefault();

					if (Request.Headers.Contains("Type"))
					{
						type = Request.Headers.GetValues("Type").FirstOrDefault();
					}

					try
					{
						switch (type)
						{
							case "image":
								momentObj = JsonConvert.DeserializeObject<ImageMoment>(jsonModel);
								WebApiApplication.db.AddNewMoment(momentObj, new Guid(tripId));
								break;
							case "video":
								momentObj = JsonConvert.DeserializeObject<VideoMoment>(jsonModel);
								WebApiApplication.db.AddNewMoment(momentObj, new Guid(tripId));
								break;
							case "text":
								momentObj = JsonConvert.DeserializeObject<TextMoment>(jsonModel);
								WebApiApplication.db.AddNewMoment(momentObj, new Guid(tripId));
								break;
							default:
								throw new InvalidProgramException();
						}

						return momentObj;
					}
					catch (Exception ex)
					{
						Tools.logger.Error("[MOMENTS CONTROLLER POST][ERROR] : Could not post moment to db, " + ex.Message);
						return null;
					}
				}
				else
				{
					Tools.logger.Error("[MOMENTS CONTROLLER POST][ERROR] : No trip id to add the moment to");
					return null;
				}
			}
			else
			{
				Tools.logger.Error("[MOMENTS CONTROLLER POST][ERROR] : No token provided");
				return null;
			}
		}

		[HttpDelete]
		[Route("moments/delete")]
		public string DeleteMoment()
		{
			string token = null;
			string momentId = null;

			if (Request.Headers.Contains("Token"))
			{
				token = Request.Headers.GetValues("Token").FirstOrDefault();
			}

			if (SessionManager.ValidateSession(token))
			{
				if (Request.Headers.Contains("Guid"))
				{
					momentId = Request.Headers.GetValues("Guid").FirstOrDefault();

					try
					{
						WebApiApplication.db.DeleteMoment(momentId);
						return momentId;
					}
					catch (Exception ex)
					{
						Tools.logger.Error("[MOMENTS CONTROLLER DELETE][ERROR] : Could not delete moment from db, " + ex.Message);
						return null;
					}
				}
				else
				{
					Tools.logger.Error("[MOMENTS CONTROLLER DELETE][ERROR] : No moment id provided");
					return null;
				}
			}
			else
			{
				Tools.logger.Error("[MOMENTS CONTROLLER DELETE][ERROR] : No token provided");
				return null;
			}
		}

		[HttpPatch]
		[Route("moments/update")]
		public string UpdateMoment()
		{
			string userId = null;
			string token = null;
			Dictionary<string, string> kvp = new Dictionary<string, string>();

			if (Request.Headers.Contains("Token"))
			{
				token = Request.Headers.GetValues("Token").FirstOrDefault();
			}

			if (SessionManager.ValidateSession(token))
			{
				if (Request.Headers.Contains("Guid"))
				{
					userId = Request.Headers.GetValues("Guid").FirstOrDefault();
				}

				if (userId == null)
				{
					return null;
				}

				// the header is parsed out this way because the header collection is implemented in 
				// a fucking stupid way where you cant get the keys of the header, they are just hard coded string 
				// comparisons on the header collection. Fucking unbelievable...
				if (Request.Headers.Contains("Title"))
				{
					kvp.Add("Title", Request.Headers.GetValues("Title").FirstOrDefault());
				}
				if (Request.Headers.Contains("Longitude"))
				{
					kvp.Add("Longitude", Request.Headers.GetValues("Longitude").FirstOrDefault());
				}
				if (Request.Headers.Contains("Latitude"))
				{
					kvp.Add("Latitude", Request.Headers.GetValues("Latitude").FirstOrDefault());
				}
				if (Request.Headers.Contains("Creator"))
				{
					kvp.Add("Creator", Request.Headers.GetValues("Creator").FirstOrDefault());
				}

				if (kvp.Count > 0)
				{
					try
					{

						foreach (KeyValuePair<string, string> entry in kvp)
						{
							MomentItemEnums updateType = Tools.GetMomentItemEnum(entry.Key);

							if (updateType != MomentItemEnums.InvalidUpdate && entry.Value != null)
							{
								WebApiApplication.db.UpdateMoment(userId, entry.Value, updateType);
								return $"updating {userId}'s {entry.Key} with {entry.Value}";
							}
						}
					}
					catch (Exception ex)
					{
						Tools.logger.Error("[UPDATE MOMENT][ERROR] : Could not update moment in db, " + ex.Message);
						return null;
					}
				}
			}

			Tools.logger.Error("[UPDATE MOMENT][ERROR] : no token provided");
			return null;
		}
	}
}
