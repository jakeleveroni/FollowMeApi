using System;
using System.Collections.Generic;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DataModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace FollowMeDataBase.Models
{
    public class Moment
    {
        [DataMember(Name = "Title")]
        [DynamoDBProperty("Title")]
        public string Title { get; set; }

        [DataMember(Name = "Arn")]
        [DynamoDBProperty("Arn")]
        public string Arn { get; set; }

        [DataMember(Name = "TextContent")]
        [DynamoDBProperty("TextContent")]
        public string TextContent { get; set; }

        [DataMember(Name = "ImageContent")]
        [DynamoDBProperty("ImageContent")]
        public byte[] ImageContent { get; set; }

        [DataMember(Name = "VideoContent")]
        [DynamoDBProperty("VideoContent")]
        public byte[] VideoContent { get; set; }

        [DataMember(Name = "MetaData")]
        [DynamoDBProperty("MetaData")]
        Dictionary<string, string> MetaData { get; set; }

        public Moment()
        {
            Title = "Uninitalized";
            MetaData = new Dictionary<string, string>();
        }

        public Moment(string title, string textContent, Dictionary<string, string> meta)
        {
            Title = title;
            TextContent = textContent;
            MetaData = new Dictionary<string, string>(meta);
        }

        public Moment(string title, byte[] mediaContent, Dictionary<string, string> meta)
        {
            Title = title;
            MetaData = new Dictionary<string, string>(meta);

            if (MetaData["Content-Type"] == "Video")
            {
                VideoContent = mediaContent;
            }
            else if (MetaData["Content-Type"] == "Image")
            {
                ImageContent = mediaContent;
            }
        }

        public Moment(Moment other)
        {
            Title = other.Title;
            TextContent = other.TextContent;
            ImageContent = other.ImageContent;
            VideoContent = other.VideoContent;
            Arn = other.Arn;
            MetaData = new Dictionary<string, string>(other.MetaData);
        }

        public void GenerateARN()
        {
            //TODO
            throw new NotImplementedException();
        }

        public string SerializeToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public static bool operator ==(Moment a, Moment b)
        {
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
            {
                return false;
            }

            if (a.Title == b.Title && a.TextContent == b.TextContent && a.ImageContent == b.ImageContent &&
                a.VideoContent == b.VideoContent && a.MetaData == b.MetaData && a.Arn == b.Arn)
            {
                return true;
            }

            return false;
        }

        public static bool operator !=(Moment a, Moment b)
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
