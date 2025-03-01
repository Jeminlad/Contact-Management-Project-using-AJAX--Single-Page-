using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ContactProject.Models;
using FluentValidation;
using FluentValidation.Results;

namespace ContactProject.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IValidator<RegisterVM> _validator;

    public HomeController(ILogger<HomeController> logger, IValidator<RegisterVM> validator )
    {
        _logger = logger;
        _validator = validator;
    }

    public IActionResult Index()
    {
        return View();
    }



    public IActionResult Privacy()
    {
        return View();
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }



    public IActionResult FRegister()
    {
        return View();
    }
    [HttpPost]
    public IActionResult FRegister(RegisterVM model)
    {
        ValidationResult result = _validator.Validate(model);   
        if (!result.IsValid)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
            return View(model);
        }
        TempData["SuccessMessage"] = "Registration successful!";
        return RedirectToAction("FRegister");
    }

}
