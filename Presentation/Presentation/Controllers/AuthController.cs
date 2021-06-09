using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    public class AuthController : Controller
    {
        [AllowAnonymous]
        public IActionResult Login(User user)
        {

            string baseUrl = "https://localhost:44384/api/";
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);
            var contentType = new MediaTypeWithQualityHeaderValue
        ("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);


            string stringData = JsonSerializer.Serialize(user);
            var contentData = new StringContent(stringData,
        System.Text.Encoding.UTF8, "application/json");


            HttpResponseMessage response = client.PostAsync
            ("/api/auth/login", contentData).Result;
            string stringJWT = response.Content.
        ReadAsStringAsync().Result;
            JWT jwt = JsonSerializer.Deserialize<JWT>(stringJWT);

            if (string.IsNullOrEmpty(jwt.token))
            {
                TempData["LoginFailure"] = "Login failure";
                return RedirectToAction("Index", "Auth");
            }
            // HttpContext.Response.Cookies.Append(
            HttpContext.Response.Cookies.Append("bearer", jwt.token, new CookieOptions { Path = "/" });

            ViewBag.Message = "User logged in successfully!";

            return RedirectToAction("Index", "Cell");
        }

        public ActionResult Logout()
        {
            HttpContext.Response.Cookies.Append("bearer", "", new CookieOptions { Path = "/" });

            return RedirectToAction("Index", "Auth");
        }

        // GET: AuthController
        public ActionResult Index()
        {

            string token = Request.Cookies["bearer"];

            if (!string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index", "Cell");
            }

            return View();
        }

        // GET: AuthController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AuthController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserViewModel user)
        {
            Services.Services.CreateUser(user);

            return RedirectToAction("Index", "Auth");
        }


    }
}
