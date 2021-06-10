using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;
using Presentation.Services;
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
        private readonly IServiceApi _serviceApi;

        public AuthController(IServiceApi serviceApi)
        {
            _serviceApi = serviceApi;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Login(User user)
        {
            var userViewModel = new UserViewModel()
            {
                login = user.Login,
                password = user.Password
            };
            var jwt = await _serviceApi.LoginUser(userViewModel);

            // HttpContext.Response.Cookies.Append(
            HttpContext.Response.Cookies.Append("bearer", jwt.token, new CookieOptions { Path = "/" });

            ViewBag.Message = "User logged in successfully!";

            return RedirectToAction("Index", "Cell");
        }

        public async Task<ActionResult> Logout()
        {
            HttpContext.Response.Cookies.Append("bearer", "", new CookieOptions { Path = "/" });

            return await Task.FromResult(RedirectToAction("Index", "Auth"));
        }

        // GET: AuthController
        public async Task<ActionResult> Index()
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
        public async Task<ActionResult> Create(UserViewModel user)
        {
            await _serviceApi.CreateUser(user);

            return RedirectToAction("Index", "Auth");
        }


    }
}
