using Microsoft.AspNetCore.Mvc;
using Presentation.Helpers;
using Presentation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace Presentation.Services
{
    public static class Services
    {
        public static List<Cell> GetAll(HttpClient client)
        {
            string baseUrl = "https://localhost:44384/api/";
            client.BaseAddress = new Uri(baseUrl);
            var contentType = new MediaTypeWithQualityHeaderValue
        ("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);


            HttpResponseMessage response = client.GetAsync
        ("/api/claro").Result;
            string stringCells = response.Content.
        ReadAsStringAsync().Result;
            List<CellViewModel> cells = JsonSerializer.Deserialize<List<CellViewModel>>(stringCells);

            var listCell = new List<Cell>();
            foreach (var cell in cells)
            {
                var newEntity = new Cell()
                {
                    Code = cell.code,
                    Model = cell.model,
                    Price = cell.price,
                    Brand = cell.brand,
                    Photo = ConvertToBase64.ConvertImageToBase64(cell.photo).Result,
                    Date = cell.date
                };

                newEntity.Date.ToString("dd/MM/yyyy");

                listCell.Add(newEntity);
            }

            return listCell;
        }

        public static bool Create(CellViewModel cell, HttpClient client)
        {
            string baseUrl = "https://localhost:44384/api/";
            client.BaseAddress = new Uri(baseUrl);
            var contentType = new MediaTypeWithQualityHeaderValue
        ("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);


            string stringData = JsonSerializer.Serialize(cell);
            var contentData = new StringContent(stringData,
        System.Text.Encoding.UTF8, "application/json");


            HttpResponseMessage response = client.PostAsync
        ("/api/claro", contentData).Result;
            string stringResult = response.Content.
        ReadAsStringAsync().Result;

            stringResult.Contains("True");

            return true;

        }

        public static bool Edit(string code, CellViewModel cell, HttpClient client)
        {
            string baseUrl = "https://localhost:44384/api/";
            client.BaseAddress = new Uri(baseUrl);
            var contentType = new MediaTypeWithQualityHeaderValue
        ("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);


            string stringData = JsonSerializer.Serialize(cell);
            var contentData = new StringContent(stringData,
        System.Text.Encoding.UTF8, "application/json");


            HttpResponseMessage response = client.PutAsync
        ($@"/api/claro/{code}", contentData).Result;
            string stringResult = response.Content.
        ReadAsStringAsync().Result;

            stringResult.Contains("True");

            return true;

        }


        public static Cell GetReal(String code, HttpClient client)
        {
            string baseUrl = "https://localhost:44384/api/";

            var contentType = new MediaTypeWithQualityHeaderValue
        ("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);


            UriBuilder builder = new UriBuilder($@"https://localhost:44384/api/claro/{code}");

            var response = client.GetAsync(builder.Uri).Result;

            string stringCells = response.Content.ReadAsStringAsync().Result;

            CellViewModel cell = JsonSerializer.Deserialize<CellViewModel>(stringCells);

            if (cell == null) return null;

            var newEntity = new Cell()
            {
                Code = cell.code,
                Model = cell.model,
                Price = cell.price,
                Brand = cell.brand,
                Photo = cell.photo,
                Date = cell.date
            };

            return newEntity;
        }



        public static Cell Get(String code, HttpClient client)
        {
            string baseUrl = "https://localhost:44384/api/";

            var contentType = new MediaTypeWithQualityHeaderValue
        ("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);


            UriBuilder builder = new UriBuilder($@"https://localhost:44384/api/claro/{code}");

            var response = client.GetAsync(builder.Uri).Result;

            string stringCells = response.Content.ReadAsStringAsync().Result;

            CellViewModel cell = JsonSerializer.Deserialize<CellViewModel>(stringCells);

            if (cell == null) return null;

            var newEntity = new Cell()
            {
                Code = cell.code,
                Model = cell.model,
                Price = cell.price,
                Brand = cell.brand,
                Photo = ConvertToBase64.ConvertImageToBase64(cell.photo).Result,
                Date = cell.date
            };

            return newEntity;
        }
     
        public static bool Delete(String code, HttpClient client)
        {
            string baseUrl = "https://localhost:44384/api/";

            var contentType = new MediaTypeWithQualityHeaderValue
        ("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);


            UriBuilder builder = new UriBuilder($@"https://localhost:44384/api/claro/{code}");

            var response = client.DeleteAsync(builder.Uri).Result;

            string stringCells = response.Content.ReadAsStringAsync().Result;

            return true;
        }

        public static bool CreateUser(UserViewModel user)
        {
            string baseUrl = "https://localhost:44384/api/";
            var client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);
            var contentType = new MediaTypeWithQualityHeaderValue
        ("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);


            string stringData = JsonSerializer.Serialize(user);
            var contentData = new StringContent(stringData,
        System.Text.Encoding.UTF8, "application/json");


            HttpResponseMessage response = client.PostAsync
        ("auth", contentData).Result;
            string stringResult = response.Content.
        ReadAsStringAsync().Result;

            stringResult.Contains("True");

            return true;

        }


    }
}
