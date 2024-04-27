using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Extension.Models
{
    public class Contact
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id {  get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Date { get; set; }
        public bool IsRegistered { get; set; }
        public string IdGroup { get; set; }
        public string OperatorUser { get; set; }
        public string UserLine { get; set; }
    }
}
