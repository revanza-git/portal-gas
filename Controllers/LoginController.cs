using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Admin.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Views/Login.cshtml");
        }

        public IActionResult ForgotPassword()
        {
            return View("~/Views/ForgotPassword.cshtml");
        }
    }
}
