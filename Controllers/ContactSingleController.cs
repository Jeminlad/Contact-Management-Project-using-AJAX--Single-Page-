// using ContactProject.Models;
// using ContactProject.Repositories.Interface;
// using Microsoft.AspNetCore.Mvc;
// using Npgsql;

// namespace MVC.Controllers
// {
//     // [Route("[controller]")]
//     public class ContactSingleController : Controller
//     {
//         //NpgsqlConnection con = new NpgsqlConnection("Server=cipg01;port=5432;Database=Intern_011;User Id=postgres;Password=123456");
//         private readonly IContactInterface _contactRepo;
//         private readonly string _connectionString;

//         public ContactSingleController(IContactInterface contactRepo)
//         {
//             _contactRepo = contactRepo;
//             _connectionString = "Server=cipg01;port=5432;Database=Intern_011;User Id=postgres;Password=123456"; // Initialize your connection string here
//         }
//         public async Task<ActionResult> Index()
//         {
//             if (HttpContext.Session.GetInt32("UserId") != null)
//             {
//                 return View();
//             }
//             else
//             {
//                 return RedirectToAction("Index", "Home");
//             }
//         }
//         public async Task<ActionResult> KendoIndex()
//         {
//             if (HttpContext.Session.GetInt32("UserId") != null)
//             {
//                 return View();
//             }
//             else
//             {
//                 return RedirectToAction("Index", "Home");
//             }
//         }

//         [HttpGet]
//         public async Task<ActionResult> List()
//         {
//             List<t_Contact> contacts = await
//             _contactRepo.GetAllByUser(HttpContext.Session.GetInt32("UserId").ToString());
//             return Json(contacts);
//         }

//         public async Task<ActionResult> Logout()
//         {
//             HttpContext.Session.Clear();
//             if (HttpContext.Session.GetInt32("UserId") != null)
//             {
//                 return RedirectToAction("Index", "Contact");
//             }
//             else
//             {
//                 return RedirectToAction("Index", "Home");
//             }
//         }

//         public async Task<IActionResult> GetContactById(string id)
//         {
//             var contact = await _contactRepo.GetOne(id);
//             if (contact == null)
//                 return BadRequest(new { success = false, message = "There was no contact found" });
//             return Ok(contact);
//         }

//         // [HttpPost]
//         // public async Task<ActionResult> Create(t_Contact contact)
//         // {
//         //     if (ModelState.IsValid)
//         //     {
//         //         if (HttpContext.Session.GetInt32("UserId") != null)
//         //         {
//         //             if (contact.ContactPicture != null && contact.ContactPicture.Length > 0)
//         //             {
//         //                 var fileName = contact.c_Email + Path.GetExtension(contact.ContactPicture.FileName);
//         //                 var filePath = Path.Combine("../MVC/wwwroot/contact_images", fileName);
//         //                 Directory.CreateDirectory(Path.Combine("../MVC/wwwroot/contact_images"));
//         //                 contact.c_Image = fileName;
//         //                 System.IO.File.Delete(filePath);
//         //                 using (var stream = new FileStream(filePath, FileMode.Create))
//         //                 {
//         //                     contact.ContactPicture.CopyTo(stream);
//         //                 }
//         //             }
//         //             contact.c_UserId = (int)HttpContext.Session.GetInt32("UserId");
//         //             var result = 0;
//         //             if (contact.c_ContactId == 0)
//         //             {
//         //                 result = await _contactRepo.Add(contact);
//         //             }

//         //             else
//         //             {
//         //                 result = await _contactRepo.Update(contact);
//         //             }
//         //             if (result == 0)
//         //             {
//         //                 return BadRequest(new { success = false, message = "There was some error while adding the contact" });
//         //             }
//         //             else
//         //             {
//         //                 return Ok(new { success = true, message = "contact Insterted Successfully!!!!!" });
//         //             }
//         //         }
//         //         else
//         //         {
//         //             return BadRequest(new { success = false, message = "There was some error while adding the contact" });
//         //         }
//         //     }
//         //     var errors = ModelState.Where(e => e.Value.Errors.Count > 0)
//         //     .ToDictionary(
//         //     kvp => kvp.Key,
//         //     kvp => kvp.Value.Errors.Select(err => err.ErrorMessage).ToArray()
//         //     );
//         //     return BadRequest(new { success = false, message = errors });
//         // }

//         public IActionResult Create()
//         {
//             return View();
//         }

//         [HttpPost("Create")]
//         public IActionResult Create(t_Contact contact)
//         {
//             if (contact == null)
//                 return BadRequest(new { message = "Invalid contact data" });

//             using (var conn = new NpgsqlConnection(_connectionString))
//             {
//                 conn.Open();
//                 string query;

//                 if (contact.c_ContactId == 0)  // Insert new contact
//                 {
//                     query = "INSERT INTO t_Contact (c_ContactName, c_Email, c_Mobile, c_Address) VALUES (@Name, @Email, @Mobile, @Address)";
//                 }
//                 else  // Update existing contact
//                 {
//                     query = "UPDATE t_Contact SET c_ContactName=@Name, c_Email=@Email, c_Mobile=@Mobile, c_Address=@Address WHERE c_ContactId=@Id";
//                 }

//                 using (var cmd = new NpgsqlCommand(query, conn))
//                 {
//                     cmd.Parameters.AddWithValue("@Name", contact.c_ContactName);
//                     cmd.Parameters.AddWithValue("@Email", contact.c_Email);
//                     cmd.Parameters.AddWithValue("@Mobile", contact.c_Mobile);
//                     cmd.Parameters.AddWithValue("@Address", contact.c_Address);

//                     if (contact.c_ContactId != 0)
//                         cmd.Parameters.AddWithValue("@Id", contact.c_ContactId);

//                     cmd.ExecuteNonQuery();
//                 }
//             }

//             return Ok(new { message = "Contact saved successfully" });
//         }

//         [HttpGet]
//         public async Task<ActionResult> Delete(string id)
//         {
//             int status = await _contactRepo.Delete(id);
//             if (status == 1)
//             {
//                 return Ok(new { success = true, message = "contact Insterted Successfully!!!!!" });
//             }
//             else
//             {
//                 return BadRequest(new { success = false, message = "There was some error while deleting the contact" });
//             }
//         }

//         public IActionResult Error()
//         {
//             return View("Error!");
//         }
//     }
// }

using ContactProject.Models;
using ContactProject.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace MVC.Controllers
{
    [Route("ContactSingle")]
    public class ContactSingleController : Controller
    {
        private readonly IContactInterface _contactRepo;
        private readonly string _connectionString;

        public ContactSingleController(IContactInterface contactRepo)
        {
            _contactRepo = contactRepo;
            _connectionString = "Server=cipg01;port=5432;Database=Intern_011;User Id=postgres;Password=123456";
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpGet]
        public async Task<ActionResult> List()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return Unauthorized(new { message = "Session expired, please log in again." });

            var contacts = await _contactRepo.GetAllByUser(HttpContext.Session.GetInt32("UserId").ToString());
            return Json(contacts);
        }

        [HttpPost]
        [Route("ContactSingle/Create")]
        public IActionResult Create([FromBody] t_Contact contact)
        {
            if (contact == null)
                return BadRequest(new { message = "Invalid contact data" });

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                                       .SelectMany(v => v.Errors)
                                       .Select(e => e.ErrorMessage)
                                       .ToList();
                return BadRequest(new { message = "Validation failed", errors });
            }

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                string query;

                if (contact.c_ContactId == 0)  // Insert new contact
                {
                    query = "INSERT INTO t_Contact (c_ContactName, c_Email, c_Mobile, c_Address) VALUES (@Name, @Email, @Mobile, @Address)";
                }
                else  // Update existing contact
                {
                    query = "UPDATE t_Contact SET c_ContactName=@Name, c_Email=@Email, c_Mobile=@Mobile, c_Address=@Address WHERE c_ContactId=@Id";
                }

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", contact.c_ContactName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", contact.c_Email ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Mobile", contact.c_Mobile ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Address", contact.c_Address ?? (object)DBNull.Value);

                    if (contact.c_ContactId != 0)
                        cmd.Parameters.AddWithValue("@Id", contact.c_ContactId);

                    cmd.ExecuteNonQuery();
                }
            }

            return Ok(new { message = "Contact saved successfully" });
        }


        [HttpPost("Delete")]
        public async Task<ActionResult> Delete([FromBody] t_Contact contact)
        {
            if (contact == null || contact.c_ContactId == 0)
                return BadRequest(new { success = false, message = "Invalid contact ID" });

            try
            {
                int status = await _contactRepo.Delete(contact.c_ContactId.ToString());

                if (status == 1)
                    return Ok(new { success = true, message = "Contact deleted successfully!" });

                return BadRequest(new { success = false, message = "Error deleting contact" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Error deleting contact: " + ex.Message });
            }
        }
    }
}
