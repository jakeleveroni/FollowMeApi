using System;
using System.Runtime.Serialization;
using Amazon.DynamoDBv2.DataModel;
using Newtonsoft.Json;

namespace FollowMeAPI.DataModels
{
    [DataContract]
    [DynamoDBTable("Moments")]
    public class MomentModel
	{
		[DataMember(Name = "Title")]
		[DynamoDBProperty("Title")]
		public string Title { get; set; }

        [DataMember(Name = "SubTitle")]
        [DynamoDBProperty("SubTitle")]
        public string SubTitle { get; set; }

        [DataMember(Name = "Guid")]
		[DynamoDBHashKey("Guid")]
		public Guid Guid { get; set; }

		[DataMember(Name = "Longitude")]
		[DynamoDBProperty("Longitude")]
		public string Longitude { get; set; }

		[DataMember(Name = "Latitude")]
		[DynamoDBProperty("Latitude")]
		public string Latitude { get; set; }

		[DataMember(Name = "Creator")]
		[DynamoDBProperty("Creator")]
		public string Creator { get; set; }

		[DataMember(Name = "ContentId")]
		[DynamoDBProperty("ContentId")]
		public Guid ContentId { get; set; }

        [DataMember(Name="CreationTime")]
        [DynamoDBProperty("CreationTime")]
        public DateTime CreationTime { get; set; }

		[DataMember(Name = "Type")]
		[DynamoDBProperty("Type")]
		public string Type { get; set; }

        public MomentModel() { }

		public MomentModel(string title, string subTitle, Guid guid, Guid contentId, string owner, string longitude, string latitude, DateTime creationTime, string type)
		{
			Title = title;
		    SubTitle = subTitle;
			Longitude = longitude;
			Latitude = latitude;
			Guid = guid;
			ContentId = contentId;
			Creator = owner;
		    CreationTime = creationTime;
			Type = type;
		}

		public MomentModel(MomentModel other)
		{
			Title = other.Title;
			Longitude = other.Longitude;
			Latitude = other.Latitude;
			Creator = other.Creator;
			Guid = other.Guid;
			ContentId = other.ContentId;
			Type = other.Type;
		    CreationTime = other.CreationTime;
		    SubTitle = other.SubTitle;
		}

	    public string SerializeToJson()
	    {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public static bool operator ==(MomentModel a, MomentModel b)
        {
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
            {
                return false;
            }

            if (a.Title == b.Title && a.Longitude == b.Longitude && a.Latitude == b.Latitude &&
                a.Guid == b.Guid && a.SubTitle == b.SubTitle && a.CreationTime == b.CreationTime &&
                a.ContentId == b.ContentId && a.Creator == b.Creator && a.Type == b.Type)
            {
                return true;
            }

            return false;
        }

        public static bool operator !=(MomentModel a, MomentModel b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
