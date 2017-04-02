using System;
using Amazon.DynamoDBv2.DataModel;
using System.Runtime.Serialization;

namespace FollowMeDataBase.Models
{
	public abstract class MomentModel
	{
		[DataMember(Name = "Title")]
		[DynamoDBProperty("Title")]
		public string Title { get; set; }

		[DataMember(Name = "Guid")]
		[DynamoDBProperty("Guid")]
		public Guid MomentId { get; set; }

		[DataMember(Name = "Longitude")]
		[DynamoDBProperty("Longitude")]
		public string Longitude { get; set; }

		[DataMember(Name = "Latitude")]
		[DynamoDBProperty("Latitude")]
		public string Latitude { get; set; }

		[DataMember(Name = "Creator")]
		[DynamoDBProperty("Creator")]
		public string Creator { get; set; }

		[DataMember(Name = "Type")]
		[DynamoDBProperty("Type")]
		public string Type { get; set; }

		public MomentModel()
		{
			Title = "Uninitalized";
			Creator = "None";
			Longitude = Latitude = "None";
			Type = "Base";
		}

		public MomentModel(string title, Guid id, string owner, string longitude, string latitude)
		{
			Title = title;
			Longitude = longitude;
			Latitude = latitude;
			MomentId = id;
			Creator = owner;
			Type = "Base";
		}

		public MomentModel(MomentModel other)
		{
			Title = other.Title;
			Longitude = other.Longitude;
			Latitude = other.Latitude;
			Creator = other.Creator;
			MomentId = other.MomentId;
			Type = "Base";
		}

		public void GenerateARN()
		{
			//TODO
			throw new NotImplementedException();
		}

		public abstract string SerializeToJson();

        public static bool operator ==(MomentModel a, MomentModel b)
        {
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
            {
                return false;
            }

            if (a.Title == b.Title && a.Longitude == b.Longitude && a.Latitude == b.Latitude)
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
