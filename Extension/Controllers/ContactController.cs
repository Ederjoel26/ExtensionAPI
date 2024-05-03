using Extension.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Extension.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly MongoClient client;
        private Dictionary<string, int> month = new Dictionary<string, int>()
        {
            {"Enero", 1},
            {"Febrero", 2},
            {"Marzo", 3},
            {"Abril", 4},
            {"Mayo", 5},
            {"Junio", 6},
            {"Julio", 7},
            {"Agosto", 8},
            {"Septiembre", 9},
            {"Octubre", 10},
            {"Noviembre", 11},
            {"Diciembre", 12}
        };

        public ContactController()
        {
            string connectionString = "mongodb+srv://bdprogamble:BfrsjWmUcoJ4HiHW@savegamblingcontact.tvw2vqq.mongodb.net/";
            client = new MongoClient(connectionString);
        }

        [HttpGet]
        public IActionResult GetContacts()
        {
            try
            {
                var contactsCollection = client
                    .GetDatabase("SaveContactGP")
                    .GetCollection<Contact>("Contacts");

                List<Contact> lstContacts = contactsCollection.Find(_ => true).ToList();

                // Ordenar la lista por fecha utilizando la fecha construida con expresión regular
                List<Contact> lstContactsShorted = lstContacts
                    .OrderByDescending(contact =>
                    {
                        var match = Regex.Match(contact.Date, @"(\d{1,2})/(\d{1,2})/(\d{4}), (\d{1,2}):(\d{1,2}):(\d{1,2})");
                        if (match.Success)
                        {
                            var day = int.Parse(match.Groups[1].Value);
                            var month = int.Parse(match.Groups[2].Value);
                            var year = int.Parse(match.Groups[3].Value);
                            var parsedDate = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
                            return parsedDate;
                        }
                        return DateTime.MinValue; // Otra opción podría ser devolver DateTime.MinValue si la expresión regular no tiene éxito
                    })
                    .ToList();

                return Ok(lstContactsShorted);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetHundredContacts()
        {
            try
            {
                var contactsCollection = client
                    .GetDatabase("SaveContactGP")
                    .GetCollection<Contact>("Contacts");

                List<Contact> lstContacts = contactsCollection.Find(_ => true).ToList();

                // Filtrar contactos con fechas mal formateadas y construir la fecha
                List<Contact> validContacts = lstContacts
                    .Where(contact =>
                    {
                        var match = Regex.Match(contact.Date, @"(\d{1,2})/(\d{1,2})/(\d{4}), (\d{1,2}):(\d{1,2}):(\d{1,2})");
                        return match.Success;
                    })
                    .ToList();

                validContacts.Reverse();

                return Ok(validContacts.Take(50).ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("{Name}")]
        public IActionResult GetContactByName(string Name)
        {
            try
            {
                var contacts = client
                    .GetDatabase("SaveContactGP")
                    .GetCollection<Contact>("Contacts");

                List<Contact> lstContacts = contacts.Find(_ => true).ToList();

                List<Contact> oContact = lstContacts.Where(contact => contact.Name == Name).ToList();

                return Ok(oContact);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("MonthName")]
        public IActionResult GetContactsCountPerMonth(string MonthName)
        {
            try
            {
                var contactsCollection = client
                    .GetDatabase("SaveContactGP")
                    .GetCollection<Contact>("Contacts");

                var startDate = new DateTime(DateTime.Now.Year, month[MonthName], 1, 0, 0, 0, DateTimeKind.Utc);
                var endDate = startDate.AddMonths(1).AddDays(-1).AddDays(1).AddMilliseconds(-1);

                var contacts = contactsCollection
                    .Find(c => c.Date != null)
                    .ToList();

                var contactsCountPerDay = contacts
                    .Select(c =>
                    {
                        var match = Regex.Match(c.Date, @"(\d{1,2})/(\d{1,2})/(\d{4}), (\d{1,2}):(\d{1,2}):(\d{1,2})");
                        if (match.Success)
                        {
                            var day = int.Parse(match.Groups[1].Value);
                            var month = int.Parse(match.Groups[2].Value);
                            var year = int.Parse(match.Groups[3].Value);
                            var parsedDate = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
                            return new { ParsedDate = parsedDate, OriginalDate = c.Date };
                        }
                        return null;
                    })
                    .Where(c => c != null && startDate <= c.ParsedDate && c.ParsedDate <= endDate)
                    .GroupBy(c => c.ParsedDate.Date)
                    .Select(g => new ContactsCount
                    {
                        Date = g.Key.ToString("yyyy-MM-dd"),
                        Contacts = g.Count()
                    })
                    .ToList();

                return Ok(contactsCountPerDay);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("MonthName")]
        public IActionResult GetContactsUserLinePerMonth(string MonthName)
        {
            try
            {
                var contactsCollection = client
                    .GetDatabase("SaveContactGP")
                    .GetCollection<Contact>("Contacts");

                var startDate = new DateTime(DateTime.Now.Year, month[MonthName], 1, 0, 0, 0, DateTimeKind.Utc);
                var endDate = startDate.AddMonths(1).AddDays(-1).AddDays(1).AddMilliseconds(-1);

                var contacts = contactsCollection
                    .Find(c => c.Date != null)
                    .ToList();

                var contactsPerSpecifiedMonth = contacts
                    .Select(c =>
                    {
                        var match = Regex.Match(c.Date, @"(\d{1,2})/(\d{1,2})/(\d{4}), (\d{1,2}):(\d{1,2}):(\d{1,2})");
                        if (match.Success)
                        {
                            var day = int.Parse(match.Groups[1].Value);
                            var month = int.Parse(match.Groups[2].Value);
                            var year = int.Parse(match.Groups[3].Value);
                            var parsedDate = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
                            if (startDate <= parsedDate && parsedDate <= endDate)
                                return c;

                            return null;
                        }
                        return null;
                    })
                    .Where(c => c != null)
                    .ToList();

                List<UserLineContacts> contactsPerUserLine = new();

                foreach (var contact in contactsPerSpecifiedMonth)
                {
                    bool flag = false;
                    int index = 0;

                    for (int i = 0; i < contactsPerUserLine.Count; i++)
                    {
                        if (contact.UserLine == contactsPerUserLine[i].UserName)
                        {
                            flag = true;
                            index = i;
                            break;
                        }
                    }

                    if (flag)
                    {
                        int count = contactsPerSpecifiedMonth.Where(c =>
                        {
                            string dateOne = c.Date.Split(",")[0];
                            string dateTwo = contact.Date.Split(",")[0];

                            return dateOne == dateTwo && c.UserLine == contact.UserLine;
                        }).Count();

                        ContactsCount Contacts = new()
                        {
                            Date = contact.Date.Split(",")[0],
                            Contacts = count
                        };

                        var AddedContacts = contactsPerUserLine[index].ContactsCount.Where(c => c.Date == Contacts.Date).FirstOrDefault();

                        if (AddedContacts == null)
                        {
                            contactsPerUserLine[index].ContactsCount.Add(Contacts);
                        }
                    }
                    else
                    {
                        int count = contactsPerSpecifiedMonth.Where(c =>
                        {
                            string dateOne = c.Date.Split(",")[0];
                            string dateTwo = contact.Date.Split(",")[0];

                            return dateOne == dateTwo && c.UserLine == contact.UserLine;
                        }).Count();

                        ContactsCount Contacts = new()
                        {
                            Date = contact.Date.Split(",")[0],
                            Contacts = count
                        };


                        UserLineContacts userLineContacts = new()
                        {
                            UserName = contact.UserLine
                        };

                        userLineContacts.ContactsCount.Add(Contacts);

                        contactsPerUserLine.Add(userLineContacts);
                    }
                }

                return Ok(contactsPerUserLine);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("MonthName")]
        public IActionResult GetContactsOperatorPerMonth(string MonthName)
        {
            try
            {
                var contactsCollection = client
                    .GetDatabase("SaveContactGP")
                    .GetCollection<Contact>("Contacts");

                var startDate = new DateTime(DateTime.Now.Year, month[MonthName], 1, 0, 0, 0, DateTimeKind.Utc);
                var endDate = startDate.AddMonths(1).AddDays(-1).AddDays(1).AddMilliseconds(-1);

                var contacts = contactsCollection
                    .Find(c => c.Date != null)
                    .ToList();

                var contactsPerSpecifiedMonth = contacts
                    .Select(c =>
                    {
                        var match = Regex.Match(c.Date, @"(\d{1,2})/(\d{1,2})/(\d{4}), (\d{1,2}):(\d{1,2}):(\d{1,2})");
                        if (match.Success)
                        {
                            var day = int.Parse(match.Groups[1].Value);
                            var month = int.Parse(match.Groups[2].Value);
                            var year = int.Parse(match.Groups[3].Value);
                            var parsedDate = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
                            if (startDate <= parsedDate && parsedDate <= endDate)
                                return c;

                            return null;
                        }
                        return null;
                    })
                    .Where(c => c != null)
                    .ToList();

                List<UserLineContacts> contactsPerUserLine = new();

                foreach (var contact in contactsPerSpecifiedMonth)
                {
                    bool flag = false;
                    int index = 0;

                    for (int i = 0; i < contactsPerUserLine.Count; i++)
                    {
                        if (contact.OperatorUser == contactsPerUserLine[i].UserName)
                        {
                            flag = true;
                            index = i;
                            break;
                        }
                    }

                    if (flag)
                    {
                        int count = contactsPerSpecifiedMonth.Where(c =>
                        {
                            string dateOne = c.Date.Split(",")[0];
                            string dateTwo = contact.Date.Split(",")[0];

                            return dateOne == dateTwo && c.OperatorUser == contact.OperatorUser;
                        }).Count();

                        ContactsCount Contacts = new()
                        {
                            Date = contact.Date.Split(",")[0],
                            Contacts = count
                        };

                        var AddedContacts = contactsPerUserLine[index].ContactsCount.Where(c => c.Date == Contacts.Date).FirstOrDefault();

                        if (AddedContacts == null)
                        {
                            contactsPerUserLine[index].ContactsCount.Add(Contacts);
                        }
                    }
                    else
                    {
                        int count = contactsPerSpecifiedMonth.Where(c =>
                        {
                            string dateOne = c.Date.Split(",")[0];
                            string dateTwo = contact.Date.Split(",")[0];

                            return dateOne == dateTwo && c.OperatorUser == contact.OperatorUser;
                        }).Count();

                        ContactsCount Contacts = new()
                        {
                            Date = contact.Date.Split(",")[0],
                            Contacts = count
                        };


                        UserLineContacts userLineContacts = new()
                        {
                            UserName = contact.OperatorUser
                        };

                        userLineContacts.ContactsCount.Add(Contacts);

                        contactsPerUserLine.Add(userLineContacts);
                    }
                }

                return Ok(contactsPerUserLine);
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

                List<Contact> lstContacts = contacts.Find(_ => true).ToList();

                Contact oContact = lstContacts.Where(cont => cont.PhoneNumber == contact.PhoneNumber).FirstOrDefault();

                if(oContact != null)
                {
                    return BadRequest($"El numero {oContact.PhoneNumber} ya está registrado");
                }

                oContact = lstContacts.Where(cont => cont.Name == contact.Name).FirstOrDefault();

                if (oContact != null)
                {
                    return BadRequest($"El Nombre {oContact.Name} ya está registrado");
                }

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

        [HttpDelete]
        public IActionResult DeleteContact(string Name, string Number)
        {
            try
            {
                var contacts = client
                     .GetDatabase("SaveContactGP")
                     .GetCollection<Contact>("Contacts");

                var contact = contacts.Find(c => c.PhoneNumber == Number && c.Name == Name && !c.IsRegistered);

                var filter = Builders<Contact>.Filter.And(
                   Builders<Contact>.Filter.Eq(c => c.Name, Name),
                   Builders<Contact>.Filter.Eq(c => c.PhoneNumber, Number)
               );

                contacts.DeleteOne(filter);

                return Ok($"Contact with ID {Name} deleted");
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
