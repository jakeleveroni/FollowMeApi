using System;
using System.Runtime.Serialization;
using Amazon.DynamoDBv2.DataModel;

namespace FollowMeAPI.DataModels
{
    [DataContract]
    [DynamoDBTable("Badges")]
    public class BadgeModel
    {
        [DataMember(Name="Id")]
        [DynamoDBProperty("Id")]
        public int Id { get; set; }

        [DataMember(Name="Descprtion")]
        [DynamoDBProperty("Descprtion")]
        public string Description { get; set; }

        [DataMember(Name = "Value")]
        [DynamoDBProperty("Value")]
        public int Value { get; set; }

        [DataMember(Name="Image")]
        [DynamoDBProperty("Image")]
        public Uri Image { get; set; }

        public BadgeModel()
        {
            
        }

        public BadgeModel(int id, string desc, int val, string imgSrc)
        {
            Id = id;
            Description = desc;
            Value = val;
            Image = new Uri(imgSrc);
        }

    }
}