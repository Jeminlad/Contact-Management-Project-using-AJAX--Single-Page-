using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ContactProject.Models;
using ContactProject.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ContactProject.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserInterface reg;

        public UserController(IUserInterface reg)
        {
            this.reg = reg;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(t_User user)
        {
            if (ModelState.IsValid)
            {
                if (user.ProfilePicture != null && user.ProfilePicture.Length > 0)
                {
                    // Save the uploaded file 
                    var fileName = user.c_Email + Path.GetExtension(user.ProfilePicture.FileName);
                    var filePath = Path.Combine("wwwroot/profile_images", fileName);
                    Directory.CreateDirectory(Path.Combine("wwwroot/profile_images"));
                    user.c_Image = fileName;
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        user.ProfilePicture.CopyTo(stream);
                    }
                }
                Console.WriteLine("user.c_fname: " + user.c_UserName);
                var status = await reg.Register(user);
                if (status == 1)
                {
                    ViewData["message"] = "User Registred";
                    return RedirectToAction("Index");
                }
                else if (status == 0)
                {
                    ViewData["message"] = "User Already Registred";
                }
                else
                {
                    ViewData["message"] = "There was some error while Registration";
                }
            }
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(vm_Login login)
        {
            t_User UserData = await reg.Login(login);
            if (ModelState.IsValid)
            {
                if (UserData.c_UserId != 0)
                {
                    HttpContext.Session.SetInt32("UserId", UserData.c_UserId);
                    HttpContext.Session.SetString("UserName", UserData.c_UserName);

                    string imageFileName = $"{UserData.c_Email}.png";
                    string userImagePath = $"/profile_images/{imageFileName}";
                    string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "profile_images", imageFileName);

                    if (!System.IO.File.Exists(fullPath))
                    {
                        userImagePath = "/profile_images/default-user.png";
                    }

                    HttpContext.Session.SetString("UserImage", userImagePath);
                    ViewData["message"] = "Login suceesful";
                    return RedirectToAction("Index", "ContactSingle");
                }
                else
                {
                    ViewData["message"] = "Invalid Username and Password";
                }
            }
            return View(login);
        }


        #region Login with Kendo UI

        [HttpGet]
        public ActionResult KendoLogin()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> KendoLogin([FromBody] vm_Login user)
        {

            t_User UserData = await reg.Login(user);

            if (UserData != null && UserData.c_UserId != 0)
            {
                HttpContext.Session.SetInt32("UserId", UserData.c_UserId);
                HttpContext.Session.SetString("UserName", UserData.c_UserName);
                HttpContext.Session.SetString("UserProfilePicture", UserData.c_Image ?? "user.png");

                return Json(new { success = true, message = "User logged in successfully!", redirectUrl = Url.Action("Index", "Home") });
            }
            else
            {
                return Json(new { success = false, message = "Invalid Username or Password" });
            }
        }

        #endregion

        #region Register with Kendo UI

        [HttpGet]
        public IActionResult KendoRegister()
        {
            return View();
        }


        [HttpPost]
        public async Task<JsonResult> KendoRegister(t_User user)
        {
            try
            {
                if (user.ProfilePicture != null && user.ProfilePicture.Length > 0)
                {
                    var fileName = user.c_Email + Path.GetExtension(user.ProfilePicture.FileName);
                    var filePath = Path.Combine("wwwroot/profile_images", fileName);

                    Directory.CreateDirectory(Path.Combine("wwwroot/profile_images"));
                    user.c_Image = fileName;

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await user.ProfilePicture.CopyToAsync(stream);
                    }
                }

                // _user.Add(user);
                // return Json(new { success = true, message = "Registration Successful!!", redirectUrl = Url.Action("KendoLogin", "User") });
                int result = await reg.Register(user);

                if (result == 0)
                {
                    return Json(new { success = false, message = "Email already exists. Please use a different email." });
                }
                else if (result == 1)
                {
                    return Json(new { success = true, message = "Registration Successful!!", redirectUrl = Url.Action("KendoLogin", "User") });
                }
                else
                {
                    return Json(new { success = false, message = "Registration failed due to an unexpected error." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        #endregion
    
    
    }
}