using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ContactProject.Models;
using ContactProject.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Npgsql;
using Npgsql.Replication;

namespace ContactProject.Controllers
{
    // [Route("[controller]")]
    public class ContactSingleController : Controller
    {
        private readonly ILogger<ContactSingleController> _logger;
        private readonly IContactInterface _contactRepo;
        private readonly NpgsqlConnection  _conn;
        public ContactSingleController(ILogger<ContactSingleController> logger, IContactInterface contactRepo,NpgsqlConnection conn)
        {
            _conn=conn;
            _contactRepo = contactRepo;
            _logger = logger;
        }
        // private readonly string connectionString = "Server=cipg01;port=5432;Database=Intern_011;User Id=postgres;Password=123456";

        private readonly IContactInterface _cityHelper;

        public ContactSingleController(IContactInterface cityHelper)
        {
            _cityHelper = cityHelper;
        }

        // private readonly NpgsqlConnection con;


        public async Task<ActionResult> Index()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                List<t_Contact> contacts = await _contactRepo.GetAllByUser(HttpContext.Session.GetInt32("UserId").ToString());
                return View(contacts);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<ActionResult> Logout()
        {
            HttpContext.Session.Clear();
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                return RedirectToAction("Index", "Contact");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }


        public async Task<IActionResult> GetContactById(string id)
        {
            var contact = await _contactRepo.GetOne(id);
            if (contact == null)
                return BadRequest(new { success = false, message = "There was no contact found" });
            return Ok(contact);
        }


        // [HttpPost]
        // public async Task<ActionResult> Create(t_Contact contact)
        // {
        //     if (ModelState.IsValid)
        //     {
        //         if (HttpContext.Session.GetInt32("UserId") != null)
        //         {
        //             if (contact.ContactPicture != null && contact.ContactPicture.Length > 0)
        //             {
        //                 var fileName = contact.c_Email +
        //                 Path.GetExtension(contact.ContactPicture.FileName);
        //                 var filePath = Path.Combine("wwwroot/contact_images", fileName);
        //                 Directory.CreateDirectory(Path.Combine("wwwroot/contact_images"));
        //                 contact.c_Image = fileName;
        //                 System.IO.File.Delete(filePath);
        //                 using (var stream = new FileStream(filePath, FileMode.Create))
        //                 {
        //                     contact.ContactPicture.CopyTo(stream);
        //                 }
        //             }
        //             contact.c_UserId = (int)HttpContext.Session.GetInt32("UserId");
        //             var result = 0;
        //             if (contact.c_ContactId == 0)
        //             {
        //                 result = await _contactRepo.Add(contact);
        //             }
        //             else
        //             {
        //                 result = await _contactRepo.Update(contact);
        //             }
        //             if (result == 0)
        //             {
        //                 return BadRequest(new { success = false, message = "There was some error while adding the contact" });
        //             }
        //             else
        //             {
        //                 return Ok(new { success = true, message = "contact Insterted Successfully!!!!!" });
        //             }
        //         }
        //         else
        //         {
        //             return BadRequest(new { success = false, message = "There was some error while adding the contact" });
        //         }
        //     }
        //     var errors = ModelState.Where(e => e.Value.Errors.Count > 0)
        //     .ToDictionary(
        //     kvp => kvp.Key,
        //     kvp => kvp.Value.Errors.Select(err => err.ErrorMessage).ToArray()
        //     );
        //     return BadRequest(new { success = false, message = errors });
        // }


        [HttpGet]
        public async Task<ActionResult> Delete(string id)
        {
            int status = await _contactRepo.Delete(id);
            if (status == 1)
            {
                return Ok(new { success = true, message = "contact Insterted Successfully!!!!!" });
            }
            else
            {
                return BadRequest(new { success = false, message = "There was some error while adding the contact" });
            }
        }
        public IActionResult Error()
        {
            return View("Error!");
        }


        #region List of contact using Kendo UI

        public IActionResult contact_Read()
        {
            var cities = _cityHelper.GetAll();
            return Json(cities);
        }

        public IActionResult Details(int id)
        {
            var city = _cityHelper.GetOne(id);
            if (city == null)
            {
                return NotFound();
            }
            return View(city);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(t_Contact city)
        {
            if (ModelState.IsValid)
            {
                _cityHelper.Insert(city);
                return Json(new { success = true, message = "Data Inserted", newCityId = city.c_ContactId });
            }
            return Json(new { success = false, message = "Invalid data." });
        }

        public IActionResult Edit(int id)
        {
            var city = _cityHelper.GetOne(id);
            if (city == null)
            {
                return NotFound();
            }
            return View(city);
        }

        [HttpPost]
        public IActionResult Edit(t_Contact city)
        {
            if (ModelState.IsValid)
            {
                _cityHelper.Update(city);
                return Json(new { success = true, message = "Data Successfully Updated" });
            }
            return Json(new { success = false, message = "Invalid data." });
        }

        [HttpPost]
        public IActionResult Delete(int c_ContactId)
        {
            try
            {
                var city = _cityHelper.GetOne(c_ContactId);
                if (city == null)
                {
                    return Json(new { success = false, message = "City not found." });
                }

                _cityHelper.Delete(city);
                return Json(new { success = true, message = "City Deleted" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while deleting the city." });
            }
        }

        public List<t_Contact> GetAll()
        {
            var cityList = new List<t_Contact>();
            using (var cmd = new NpgsqlCommand("Select * from t_contact", _conn))
            {
                var dt = new DataTable();
                _conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        dt.Load(reader);
                    }
                }
                cityList = (from DataRow dr in dt.Rows
                            select new t_Contact
                            {
                                c_ContactId = Convert.ToInt32(dr["c_contactid"]),
                                c_ContactName = dr["c_contactname"].ToString()
                            }).ToList();
                _conn.Close();
            }
            return cityList;
        }

        #endregion

    }
}