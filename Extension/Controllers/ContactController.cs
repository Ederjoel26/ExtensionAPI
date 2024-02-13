using Extension.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Extension.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly MongoClient client;

        public ContactController()
        {
            string connectionString = "mongodb+srv://ederjoel55:FmGKSLwCaXJKJGmn@savecontactgp.bzatzi6.mongodb.net/?retryWrites=true&w=majority";
            client = new MongoClient(connectionString);
        }

        [HttpGet]
        public IActionResult GetContacts()
        {
            try
            {
                var contacts = client
                    .GetDatabase("SaveContactGP")
                    .GetCollection<Contact>("Contacts");

                List<Contact> lstContacts = contacts.Find(_ => true).ToList();

                return Ok(lstContacts);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult CreateContact([FromBody] Contact contact)
        {
            try
            {
                var contacts = client
                    .GetDatabase("SaveContactGP")
                    .GetCollection<Contact>("Contacts");

                contacts.InsertOne(contact);

                return Ok(contact);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public IActionResult UpdateContact([FromBody] Contact updatedContact)
        {
            try
            {
                var contacts = client
                    .GetDatabase("SaveContactGP")
                    .GetCollection<Contact>("Contacts");

                var filter = Builders<Contact>.Filter.Eq("Name", updatedContact.Name);

                var update = Builders<Contact>.Update
                    .Set("Name", updatedContact.Name)
                    .Set("PhoneNumber", updatedContact.PhoneNumber)
                    .Set("Date", updatedContact.Date)
                    .Set("IsRegistered", updatedContact.IsRegistered)
                    .Set("IdGroup", updatedContact.IdGroup)
                    .Set("UserLine", updatedContact.UserLine);

                contacts.UpdateOne(filter, update);

                return Ok(updatedContact);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{Name}")]
        public IActionResult DeleteContact(string Name)
        {
            try
            {
                var contacts = client
                    .GetDatabase("SaveContactGP")
                    .GetCollection<Contact>("Contacts");

                var filter = Builders<Contact>.Filter.Eq("Name", Name);

                contacts.DeleteOne(filter);

                return Ok($"Contact with ID {Name} deleted");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
