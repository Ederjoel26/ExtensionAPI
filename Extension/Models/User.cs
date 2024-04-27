using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Extension.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id {  get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
        public string OperatorLine { get; set; }

    }
}
