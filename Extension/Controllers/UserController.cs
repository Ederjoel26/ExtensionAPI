using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver.Core.Configuration;
using MongoDB.Driver;
using Extension.Models;

namespace Extension.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly MongoClient client;

        public UserController()
        {
            // Replace the connection string with your MongoDB connection string
            string connectionString = "mongodb+srv://ederjoel55:FmGKSLwCaXJKJGmn@savecontactgp.bzatzi6.mongodb.net/?retryWrites=true&w=majority";
            client = new MongoClient(connectionString);
        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            try
            {
                var users = client
                    .GetDatabase("SaveContactGP")
                    .GetCollection<User>("Users");

                List<User> lstUsers = users.Find(user => user.IsActive).ToList();

                return Ok(lstUsers);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] User newUser)
        {
            try
            {
                var users = client
                    .GetDatabase("SaveContactGP")
                    .GetCollection<User>("Users");

                var lines = client
                    .GetDatabase("SaveContactGP")
                    .GetCollection<Line>("Lines");

                List<User> lstUsers = users.Find(_ => true).ToList();

                User oUser = lstUsers.Where(user => user.UserName ==  newUser.UserName).FirstOrDefault();

                if (oUser != null)
                {
                    return BadRequest($"El usuario {oUser.UserName} ya ha sido agregado anteriormente");
                }

                if (newUser.IsAdmin)
                {
                    Line line = new()
                    {
                        LineName = newUser.UserName,
                        KeyGroup = Guid.NewGuid().ToString()
                    };

                    lines.InsertOne(line);
                }

                users.InsertOne(newUser);

                return Ok(newUser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult GetUser(User oUser)
        {
            try
            {
                var users = client
                    .GetDatabase("SaveContactGP")
                    .GetCollection<User>("Users");

                User User = users.Find(user => user.UserName == oUser.UserName).FirstOrDefault();

                if (User == null)
                {
                    return BadRequest("User no exists");
                }

                if (User.Password == oUser.Password)
                {
                    return Ok("Logged");
                }

                return BadRequest("Password Incorrect");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public IActionResult UpdateUser([FromBody] User updatedUser)
        {
            try
            {
                var users = client
                    .GetDatabase("SaveContactGP")
                    .GetCollection<User>("Users");

                var filter = Builders<User>.Filter.Eq("UserName", updatedUser.UserName);

                var update = Builders<User>.Update
                    .Set("Password", updatedUser.Password);

                users.UpdateOne(filter, update);

                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{UserName}")]
        public IActionResult DeleteUser(string UserName)
        {
            try
            {
                var users = client
                    .GetDatabase("SaveContactGP")
                    .GetCollection<User>("Users");

                var filter = Builders<User>.Filter.Eq("UserName", UserName);

                var update = Builders<User>.Update
                    .Set("IsActive", false);

                users.UpdateOne(filter, update);

                return Ok($"User with ID {UserName} deleted");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
