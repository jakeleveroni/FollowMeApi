using System;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using Amazon.DynamoDBv2.DataModel;

namespace FollowMeDataBase.Models
{
	public class TextMoment : MomentModel
	{
		[DataMember(Name = "TextContent")]
		[DynamoDBProperty("TextContent")]
		public string TextContent { get; set; }

		public TextMoment()
		{
			TextContent = null;
		}

		public TextMoment(string content, string name, Guid id, string creator, string longitude, string latitude)
			: base(name, id, creator, longitude, latitude)
		{
			TextContent = content;
			Type = "text";
		}

		public override string SerializeToJson()
		{
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}
	}
}
