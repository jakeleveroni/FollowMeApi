using System;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using Amazon.DynamoDBv2.DataModel;

namespace FollowMeDataBase.Models
{
	public class VideoMoment : MomentModel
	{
		[DataMember(Name = "VideoContent")]
		[DynamoDBProperty("VideoContent")]
		public byte[] VideoContent { get; set; }

		public VideoMoment()
		{
			VideoContent = null;
		}

		public VideoMoment(byte[] content, string name, Guid momentId, Guid contentId, string creator, string longitude, string latitude)
			: base(name, momentId, contentId, creator, longitude, latitude)
		{
			VideoContent = content;
			Type = "video";
		}

		public override string SerializeToJson()
		{
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}
	}
}
