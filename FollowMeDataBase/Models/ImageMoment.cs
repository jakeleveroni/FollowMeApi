using System;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using Amazon.DynamoDBv2.DataModel;

namespace FollowMeDataBase.Models
{
	public class ImageMoment : MomentModel
	{
		[DataMember(Name = "ImageContent")]
		[DynamoDBProperty("ImageContent")]
		public byte[] ImageContent { get; set; }

		public ImageMoment()
		{
			ImageContent = null;
		}

		public ImageMoment(byte[] content, string name, Guid momentId, Guid contentId, string creator, string longitude, string latitude)
			: base(name, momentId, contentId, creator, longitude, latitude)
		{
			ImageContent = content;
			Type = "image";
		}

		public override string SerializeToJson()
		{
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}
	}
}
