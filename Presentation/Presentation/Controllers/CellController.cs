using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.InkML;
using HAdministradora.Infra.CrossCutting.Aws.Interfaces.Services;
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
    [Route("claro/mobile")]
    public class CellController : Controller
    {
        // GET: CellController
        private readonly IWebHostEnvironment _hostingEnv;
        private readonly IServiceApi _serviceApi;
        private readonly IBucketS3Service _bucketS3Service;

        public CellController(IWebHostEnvironment hostingEnv, IServiceApi serviceApi, IBucketS3Service bucketS3Service)
        {
            _hostingEnv = hostingEnv;
            _serviceApi = serviceApi;
            _bucketS3Service = bucketS3Service;
        }

        public async Task<ActionResult> Index()
        {
             var client = new HttpClient();



            string token = Request.Cookies["bearer"];
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            if (string.IsNullOrEmpty(token))
            {
                TempData["LoginFailure"] = "Login failure";
                return RedirectToAction("Index", "Auth");
            }

            var cells = await _serviceApi.GetAll(client);
            
            if (cells == null) return View();

            return View(cells);
        }

        [Route("details")]
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

        [Route("create")]
        public async Task<ActionResult> Create()
        {
            return await Task.FromResult(View());
        }

        [Route("create")]
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

                string extension = Path.GetExtension(cell.ImageFile.FileName);
                List<string> extensions = new List<string>();

                var contentType = cell.ImageFile.ContentType.Split("/")[1];

                if (contentType.ToUpper() != "JPG" && contentType.ToUpper() != "JPEG"
                    && contentType.ToUpper() != "PNG" && contentType.ToUpper() != "GIF")
                {

                    TempData["LoginFailure"] = "EXTENSION NOT APPLICABLE";
                    return View();
                }

                string pathAWS = (Guid.NewGuid().ToString().Substring(0, 6) + "." + contentType).Replace(" ", "");

                //Save image in s3 amazon

                var stream = cell.ImageFile.OpenReadStream().Length > 7000000;
                if (stream)
                {
                    TempData["LoginFailure"] = "Image length is not applicable, please, insert files with max length 7mb";
                    return View();

                }

                await _bucketS3Service.UploadObjectAsync(cell.ImageFile.OpenReadStream(), pathAWS);

                cell.Photo = pathAWS;

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
            catch
            {
                return RedirectToAction("Index", "Cell");
            }
        }

        [Route("edit")]
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
        [HttpPost, ActionName("edit")]
        [Route("edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string code, IFormCollection formCollection)
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

            var contentType = formCollection.Files[0].ContentType.Split("/")[1];

            if (contentType.ToUpper() != "JPG" && contentType.ToUpper() != "JPEG"
               && contentType.ToUpper() != "PNG" && contentType.ToUpper() != "GIF")
            {

                TempData["LoginFailure"] = "Extension file failure";
                return View();
            }

            string pathAWS = (Guid.NewGuid().ToString().Substring(0, 6) + "." + contentType).Replace(" ", "");

            var stream = formCollection.Files[0].OpenReadStream().Length > 8000;
            if (stream)
            {
                TempData["LoginFailure"] = "Image length is not applicable, please, insert files with max length 7mb";
                return View();

            }

            await _bucketS3Service.UploadObjectAsync(formCollection.Files[0].OpenReadStream(), pathAWS);


            var viewEntity = new CellViewModel()
            {
                model = model,
                price = Convert.ToInt32(price),
                brand = brand,
                photo = pathAWS,
                date = Convert.ToDateTime(date)

            };
            var entity = await _serviceApi.GetReal(code, client);

            if (entity == null) return RedirectToAction("Index", "Cell");


            await _bucketS3Service.DeleteSingleObject(entity.photo);


            client = new HttpClient();

            token = Request.Cookies["bearer"];
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            await _serviceApi.Edit(code, viewEntity, client);

            return RedirectToAction("Index", "Cell");

        }
        [Route("delete")]
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
        [Route("delete")]
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


            await _bucketS3Service.DeleteSingleObject(entity.photo);


            var cell = await _serviceApi.Delete(code, client);
            if (!cell) return View();

            return RedirectToAction("Index", "Cell");
        }
    }
}
