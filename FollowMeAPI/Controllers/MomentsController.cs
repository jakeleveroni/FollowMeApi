using System;
using System.Linq;
using System.Web.Http;
using System.Collections.Generic;
using FollowMeDataBase.Models;
using Newtonsoft.Json;

namespace FollowMeAPI.Controllers
{
	[RoutePrefix("moments")]
	public class MomentsController : ApiController
	{
		[HttpGet]
		[Route("get")]
		public MomentModel GetMoment()
		{
			if (Request.Headers.Contains("guid"))
			{
				var momentId = Request.Headers.GetValues("guid").FirstOrDefault();

				try
				{
					return JsonConvert.DeserializeObject<MomentModel>(WebApiApplication.Db.GetMoment(momentId));
				}
				catch (Exception ex)
				{
					Tools.logger.Error("Could not post moment to Db, " + ex.Message);
					return null;
				}
			}

			Tools.logger.Error("[MOMENTS CONTROLLER GET][ERROR] : No token provided");
			return null;
		}

		[HttpPost]
		[Route("new")]
		public MomentModel PostMoment([FromBody] MomentModel jsonModel)
		{
		    string type = null;

			if (Request.Headers.Contains("guid"))
			{
				var tripId = Request.Headers.GetValues("guid").FirstOrDefault();

				if (Request.Headers.Contains("type"))
				{
					type = Request.Headers.GetValues("type").FirstOrDefault();
				}

				if (tripId == null)
				{
				    return null;
				}

				try
				{
					jsonModel.Type = type;
					WebApiApplication.Db.AddNewMoment(jsonModel, new Guid(tripId));

					return jsonModel;
				}
				catch (Exception ex)
				{
					Tools.logger.Error("[MOMENTS CONTROLLER POST][ERROR] : Could not post moment to Db, " + ex.Message);
					return null;
				}
			}

			Tools.logger.Error("[MOMENTS CONTROLLER POST][ERROR] : No trip id to add the moment to");
			return null;
		}

		[HttpDelete]
		[Route("delete")]
		public string DeleteMoment()
		{
			if (Request.Headers.Contains("guid"))
			{
				var momentId = Request.Headers.GetValues("guid").FirstOrDefault();

				try
				{
					WebApiApplication.Db.DeleteMoment(momentId);
					return momentId;
				}
				catch (Exception ex)
				{
					Tools.logger.Error("[MOMENTS CONTROLLER DELETE][ERROR] : Could not delete moment from Db, " + ex.Message);
					return null;
				}
			}

			Tools.logger.Error("[MOMENTS CONTROLLER DELETE][ERROR] : No moment id provided");
			return null;
		}

		[HttpPatch]
		[Route("update")]
		public string UpdateMoment()
		{
			string userId = null;
			Dictionary<string, string> kvp = new Dictionary<string, string>();

			if (Request.Headers.Contains("guid"))
			{
				userId = Request.Headers.GetValues("guid").FirstOrDefault();
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
							WebApiApplication.Db.UpdateMoment(userId, entry.Value, updateType);
							return $"updating {userId}'s {entry.Key} with {entry.Value}";
						}
					}
				}
				catch (Exception ex)
				{
					Tools.logger.Error("[UPDATE MOMENT][ERROR] : Could not update moment in Db, " + ex.Message);
					return null;
				}
			}

			Tools.logger.Error("[UPDATE MOMENT][ERROR] : no token provided");
			return null;
		}
	}
}
