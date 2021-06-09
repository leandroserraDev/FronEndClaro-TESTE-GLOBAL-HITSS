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
    public  class Services : IServiceApi
    {
        public async  Task<List<Cell>> GetAll(HttpClient client)
        {
            string baseUrl = "https://localhost:44384/api/";
            client.BaseAddress = new Uri(baseUrl);
            var contentType = new MediaTypeWithQualityHeaderValue
        ("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);


            HttpResponseMessage response = await client.GetAsync
        ("/api/claro");
            string stringCells =await  response.Content.ReadAsStringAsync();
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
                    Photo = ConvertToBase64.ConvertImageToBase64(cell.photo),
                    Date = cell.date
                };

                newEntity.Date.ToString("dd/MM/yyyy");

                listCell.Add(newEntity);
            }

            return await Task.FromResult(listCell);
        }

        public async Task<bool> Create(CellViewModel cell, HttpClient client)
        {
            string baseUrl = "https://localhost:44384/api/";
            client.BaseAddress = new Uri(baseUrl);
            var contentType = new MediaTypeWithQualityHeaderValue
        ("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);


            string stringData = JsonSerializer.Serialize(cell);
            var contentData = new StringContent(stringData,
        System.Text.Encoding.UTF8, "application/json");


            HttpResponseMessage response = await client.PostAsync
        ("/api/claro", contentData);
            string stringResult = response.Content.
        ReadAsStringAsync().Result;

            stringResult.Contains("True");

            return true;

        }

        public  async Task<bool> Edit(string code, CellViewModel cell, HttpClient client)
        {
            string baseUrl = "https://localhost:44384/api/";
            client.BaseAddress = new Uri(baseUrl);
            var contentType = new MediaTypeWithQualityHeaderValue
        ("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);


            string stringData = JsonSerializer.Serialize(cell);
            var contentData = new StringContent(stringData,
        System.Text.Encoding.UTF8, "application/json");


            HttpResponseMessage response = await client.PutAsync($@"/api/claro/{code}", contentData);

            string stringResult = response.Content.ReadAsStringAsync().Result;

            stringResult.Contains("True");

            return true;

        }


        public  async Task<Cell> GetReal(String code, HttpClient client)
        {
            string baseUrl = "https://localhost:44384/api/";

            var contentType = new MediaTypeWithQualityHeaderValue
        ("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);


            UriBuilder builder = new UriBuilder($@"https://localhost:44384/api/claro/{code}");

            var response = await client.GetAsync(builder.Uri);

            string stringCells = await response.Content.ReadAsStringAsync();

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



        public  async Task<Cell> Get(String code, HttpClient client)
        {

            var contentType = new MediaTypeWithQualityHeaderValue
        ("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);


            UriBuilder builder = new UriBuilder($@"https://localhost:44384/api/claro/{code}");

            var response = await  client.GetAsync(builder.Uri);

            string stringCells = await response.Content.ReadAsStringAsync();

            CellViewModel cell = JsonSerializer.Deserialize<CellViewModel>(stringCells);

            if (cell == null) return null;

            var newEntity = new Cell()
            {
                Code = cell.code,
                Model = cell.model,
                Price = cell.price,
                Brand = cell.brand,
                Photo = ConvertToBase64.ConvertImageToBase64(cell.photo),
                Date = cell.date
            };

            return newEntity;
        }
     
        public  async Task<bool> Delete(String code, HttpClient client)
        {

            var contentType = new MediaTypeWithQualityHeaderValue
        ("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);


            UriBuilder builder = new UriBuilder($@"https://localhost:44384/api/claro/{code}");

            var response = await  client.DeleteAsync(builder.Uri);

            string stringCells = await  response.Content.ReadAsStringAsync();

            return true;
        }

        public async Task<bool> CreateUser(UserViewModel user)
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


            HttpResponseMessage response = await client.PostAsync("auth", contentData);
            string stringResult = response.Content.ReadAsStringAsync().Result;

            stringResult.Contains("True");

            return true;

        }


    }
}
