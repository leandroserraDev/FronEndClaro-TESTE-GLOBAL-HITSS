using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Presentation.Models;
using Presentation.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Presentation.Controllers
{
    public class CellController : Controller
    {
        // GET: CellController
        private readonly IWebHostEnvironment _hostingEnv;
        private readonly IServiceApi _serviceApi;

        public CellController(IWebHostEnvironment hostingEnv, IServiceApi serviceApi)
        {
            _hostingEnv = hostingEnv;
            _serviceApi = serviceApi;
        }

        public async Task<ActionResult> Index()
        {
            var client = new HttpClient();

            string token = Request.Cookies["bearer"];

            if (string.IsNullOrEmpty(token))
            {
                TempData["TokenDefault"] = "Token Expired";
                return RedirectToAction("Index", "Auth");
            }
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var cells = await _serviceApi.GetAll(client);
            if (cells == null) return View();

            return View(cells);
        }

        // GET: CellController/Details/5
        public async Task<ActionResult> Details(string code)
        {
            var client = new HttpClient();

            string token = Request.Cookies["bearer"];
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            if (string.IsNullOrEmpty(token))
            {
                TempData["LoginFailure"] = "Login failure";
                return RedirectToAction("Index", "Auth");
            }


            var entity = await _serviceApi.Get(code, client);
            if (entity == null) return RedirectToAction("Index", "Cell");

            return View(entity);
        }

        // GET: CellController/Create
        public async Task<ActionResult> Create()
        {
            return View();
        }

        // POST: CellController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Cell cell)
        {

            try
            {
                var client = new HttpClient();

                string token = Request.Cookies["bearer"];

                if (string.IsNullOrEmpty(token))
                {
                    TempData["LoginFailure"] = "Login failure";
                    return RedirectToAction("Index", "Auth");
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                //Save image to wwwroot/image
                string wwwRootPath = _hostingEnv.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(cell.ImageFile.FileName);


                string extension = Path.GetExtension(cell.ImageFile.FileName);
                List<string> extensions = new List<string>();

                var contentType = cell.ImageFile.ContentType.Split("/")[1];

                if (contentType.ToUpper() != "JPG" && contentType.ToUpper() != "JPEG"
                    && contentType.ToUpper() != "PNG" && contentType.ToUpper() != "GIF")
                {

                    TempData["LoginFailure"] = "Extension file failure";
                    return View();
                }
                string path = Path.Combine(wwwRootPath + "\\Image", Guid.NewGuid().ToString().Substring(0, 6)).Trim() + "." + contentType;
                cell.Photo = path;
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await cell.ImageFile.CopyToAsync(fileStream);
                }
                var viewEntity = new CellViewModel()
                {
                    code = cell.Code,
                    model = cell.Model,
                    price = cell.Price,
                    brand = cell.Brand,
                    photo = cell.Photo,
                    date = cell.Date

                };
                await _serviceApi.Create(viewEntity, client);

                return RedirectToAction("Index", "Cell");
            }
            catch (Exception e)
            {
                return RedirectToAction("Index", "Cell");
            }
        }

        // GET: CellController/Edit/5
        public async Task<ActionResult> Edit(string code)
        {

            var client = new HttpClient();

            string token = Request.Cookies["bearer"];
            if (string.IsNullOrEmpty(token))
            {
                TempData["LoginFailure"] = "Login failure";
                return RedirectToAction("Index", "Auth");
            }

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var entity = await _serviceApi.Get(code, client);
            if (entity == null) return RedirectToAction("Index", "Cell");

            return View(entity);
        }

        // POST: CellController/Edit/5
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditConfirmed(string code, IFormCollection formCollection)
        {

            var client = new HttpClient();

            string token = Request.Cookies["bearer"];
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            if (string.IsNullOrEmpty(token))
            {
                TempData["LoginFailure"] = "Login failure";
                return RedirectToAction("Index", "Auth");
            }


            string model = formCollection["Model"];
            var price = formCollection["Price"];
            var brand = formCollection["Brand"];
            var date = formCollection["Date"];
            var imageFile = formCollection["ImageFile"];


            //Save image to wwwroot/image
            string wwwRootPath = _hostingEnv.WebRootPath;
            string fileName = Path.GetFileNameWithoutExtension(formCollection.Files[0].FileName);
            string extension = Path.GetExtension(formCollection.Files[0].FileName);

            var contentType = formCollection.Files[0].ContentType.Split("/")[1];

            if (contentType.ToUpper() != "JPG" && contentType.ToUpper() != "JPEG"
               && contentType.ToUpper() != "PNG" && contentType.ToUpper() != "GIF")
            {

                TempData["LoginFailure"] = "Extension file failure";
                return  View();
            }

            string path = Path.Combine(wwwRootPath + "/Image", Guid.NewGuid().ToString().Substring(0, 6)).Trim() + "." + contentType;

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await formCollection.Files[0].CopyToAsync(fileStream);
            }
            var viewEntity = new CellViewModel()
            {
                model = model,
                price = Convert.ToInt32(price),
                brand = brand,
                photo = path,
                date = Convert.ToDateTime(date)

            };
            var entity = await _serviceApi.GetReal(code, client);

            if (entity == null) return RedirectToAction("Index", "Cell");

            if (System.IO.File.Exists(entity.Photo))
            {
                System.IO.File.Delete(entity.Photo);
            }

            client = new HttpClient();

            token = Request.Cookies["bearer"];
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            await _serviceApi.Edit(code, viewEntity, client);

            return RedirectToAction("Index", "Cell");

        }

        public async Task<ActionResult> Delete(string code)
        {
            var client = new HttpClient();

            string token = Request.Cookies["bearer"];
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            if (string.IsNullOrEmpty(token))
            {
                TempData["LoginFailure"] = "Login failure";
                return RedirectToAction("Index", "Auth");
            }
     

            if (string.IsNullOrEmpty(code))
            {
                return View();
            }
            var entity = await _serviceApi.Get(code, client);


            if (entity == null) return View();

            return View(entity);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string code, IFormCollection collection)
        {
            var client = new HttpClient();

            string token = Request.Cookies["bearer"];
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            if (string.IsNullOrEmpty(token))
            {
                TempData["LoginFailure"] = "Login failure";
                return RedirectToAction("Index", "Auth");
            }


            if (string.IsNullOrEmpty(code))
            {
                return RedirectToAction("Index", "Cell");
            }
            var entity = await _serviceApi.GetReal(code, client);

            if (entity == null) return View();

            if (System.IO.File.Exists(entity.Photo))
            {
                System.IO.File.Delete(entity.Photo);
            }

            var cell = await  _serviceApi.Delete(code, client);
            if (!cell) return View();


            return RedirectToAction("Index", "Cell");
        }
    }
}
