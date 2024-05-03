using Extension.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Extension.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LineController : ControllerBase
    {
        private readonly MongoClient client;

        public LineController()
        {
            // Replace the connection string with your MongoDB connection string
            string connectionString = "mongodb+srv://bdprogamble:BfrsjWmUcoJ4HiHW@savegamblingcontact.tvw2vqq.mongodb.net/";
            client = new MongoClient(connectionString);
        }

        [HttpGet]
        public IActionResult GetLines() 
        {
            try
            {
                var lines = client
                    .GetDatabase("SaveContactGP")
                    .GetCollection<Line>("Lines");

                List<Line> Lines = lines.Find(_ => true).ToList();

                return Ok(Lines);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetLine(string LineName) 
        {
            try
            {
                var lines = client
                    .GetDatabase("SaveContactGP")
                    .GetCollection<Line>("Lines");

                Line Line = lines
                    .Find(line => line.LineName == LineName)
                    .FirstOrDefault();

                return Ok(Line);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult CreateLine([FromBody]Line line)
        {
            try
            {
                var lines = client
                    .GetDatabase("SaveContactGP")
                    .GetCollection<Line>("Lines");

                lines.InsertOne(line);

                return Ok(line);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
