using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Presentation.Models;
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

        public CellController(IWebHostEnvironment hostingEnv)
        {
            _hostingEnv = hostingEnv;
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

            var cells = await Services.Services.GetAll(client);
            if (cells == null) return View();

            return View(cells);
        }

        // GET: CellController/Details/5
        public ActionResult Details(string code)
        {
            var client = new HttpClient();

            string token = Request.Cookies["bearer"];
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var entity = Services.Services.Get(code, client);
            if (entity == null) return RedirectToAction("Index", "Cell");

            return View(entity);
        }

        // GET: CellController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CellController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Cell cell)
        {

            try
            {
                var client = new HttpClient();

                string token = Request.Cookies["bearer"];
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
                string path = Path.Combine(wwwRootPath + "/Image", Guid.NewGuid().ToString().Substring(0, 6)).Trim() + "." + contentType;
                cell.Photo = path;
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    cell.ImageFile.CopyToAsync(fileStream).GetAwaiter();
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
                Services.Services.Create(viewEntity, client);

                return RedirectToAction("Index", "Cell");
            }
            catch (Exception e)
            {
                return View();
            }
        }

        // GET: CellController/Edit/5
        public ActionResult Edit(string code)
        {

            var client = new HttpClient();

            string token = Request.Cookies["bearer"];
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var entity = Services.Services.Get(code, client);
            if (entity == null) return RedirectToAction("Index", "Cell");

            return View(entity);
        }

        // POST: CellController/Edit/5
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditConfirmed(string code, IFormCollection formCollection)
        {

            var client = new HttpClient();

            string token = Request.Cookies["bearer"];
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

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

            string path = Path.Combine(wwwRootPath + "/Image", Guid.NewGuid().ToString().Substring(0, 6)).Trim() + "." + contentType;

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                formCollection.Files[0].CopyToAsync(fileStream);
            }
            var viewEntity = new CellViewModel()
            {
                model = model,
                price = Convert.ToInt32(price),
                brand = brand,
                photo = path,
                date = Convert.ToDateTime(date)

            };
            var entity = Services.Services.GetReal(code, client);

            if (entity == null) return RedirectToAction("Index", "Cell");

            if (System.IO.File.Exists(entity.Photo))
            {
                System.IO.File.Delete(entity.Photo);
            }

            client = new HttpClient();

            token = Request.Cookies["bearer"];
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            Services.Services.Edit(code, viewEntity, client);

            return RedirectToAction("Index", "Cell");

        }

        public ActionResult Delete(string code)
        {
            var client = new HttpClient();

            string token = Request.Cookies["bearer"];
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


            if (string.IsNullOrEmpty(code))
            {
                return View();
            }
            var entity = Services.Services.Get(code, client);


            if (entity == null) return View();

            return View(entity);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string code, IFormCollection collection)
        {
            var client = new HttpClient();

            string token = Request.Cookies["bearer"];
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            if (string.IsNullOrEmpty(code))
            {
                return RedirectToAction("Index", "Cell");
            }
            var entity = Services.Services.GetReal(code, client);

            if (entity == null) return View();

            if (System.IO.File.Exists(entity.Photo))
            {
                System.IO.File.Delete(entity.Photo);
            }

            var cell = Services.Services.Delete(code, client);
            if (!cell) return View();


            return RedirectToAction("Index", "Cell");
        }
    }
}
