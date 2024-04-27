using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Extension.Models
{
    public class Line
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string LineName { get; set; }
        public string KeyGroup { get; set; }
    }
}
