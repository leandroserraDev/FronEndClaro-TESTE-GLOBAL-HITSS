using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using Presentation.Helpers;
using Presentation.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace Presentation.Services
{
    public  class Services : IServiceApi
    {
        private IConfiguration _config;
        private string localHost = "";

        public Services(IConfiguration config)
        {
            _config = config;
            localHost = _config.GetSection("enderecoAPI").Value;
        }


        public async  Task<List<Cell>> GetAll(HttpClient client)
        {

            string baseUrl = localHost;
            client.BaseAddress = new Uri(baseUrl);
            var contentType = new MediaTypeWithQualityHeaderValue
        ("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);


            HttpResponseMessage response = await client.GetAsync
        ("/api/claro/mobile");
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
            string baseUrl = localHost;
            client.BaseAddress = new Uri(baseUrl);
            var contentType = new MediaTypeWithQualityHeaderValue
        ("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);


            string stringData = JsonSerializer.Serialize(cell);
            var contentData = new StringContent(stringData,
        System.Text.Encoding.UTF8, "application/json");


            HttpResponseMessage response = await client.PostAsync("/api/claro/mobile", contentData);
            string stringResult = response.Content.
        ReadAsStringAsync().Result;

            stringResult.Contains("True");

            return true;

        }

        public  async Task<bool> Edit(string code, CellViewModel cell, HttpClient client)
        {
            string baseUrl = localHost;
            client.BaseAddress = new Uri(baseUrl);
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);


            string stringData = JsonSerializer.Serialize(cell);
            var contentData = new StringContent(stringData,System.Text.Encoding.UTF8, "application/json");


            HttpResponseMessage response = await client.PutAsync($@"/api/claro//mobile/{code}", contentData);

            string stringResult = response.Content.ReadAsStringAsync().Result;

            stringResult.Contains("True");

            return true;

        }


        public  async Task<Cell> GetReal(String code, HttpClient client)
        {
            string baseUrl = localHost;

            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);


            UriBuilder builder = new UriBuilder($@"{localHost}claro/mobile/{code}");

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

            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);


            UriBuilder builder = new UriBuilder($@"{localHost}claro/mobile/{code}");

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


            UriBuilder builder = new UriBuilder($@"{localHost}claro/mobile/{code}");

            var response = await  client.DeleteAsync(builder.Uri);

            string stringCells = await  response.Content.ReadAsStringAsync();

            return true;
        }

        public async Task<bool> CreateUser(UserViewModel user)
        {
            string baseUrl = localHost;
            var client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);


            string stringData = JsonSerializer.Serialize(user);
            var contentData = new StringContent(stringData,System.Text.Encoding.UTF8, "application/json");


            HttpResponseMessage response = await client.PostAsync("auth", contentData);
            string stringResult = await  response.Content.ReadAsStringAsync();

            stringResult.Contains("True");

            return true;

        }

        public async Task<JWT> LoginUser(UserViewModel user)
        {

            string baseUrl = localHost;
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");client.DefaultRequestHeaders.Accept.Add(contentType);


            string stringData = JsonSerializer.Serialize(user);
            var contentData = new StringContent(stringData,System.Text.Encoding.UTF8, "application/json");


            HttpResponseMessage response = await client.PostAsync("/api/auth/login", contentData);
            string stringJWT = await response.Content.ReadAsStringAsync();
            JWT jwt = JsonSerializer.Deserialize<JWT>(stringJWT);

            if(jwt != null && !string.IsNullOrEmpty(jwt.token))
            {
                return jwt;
            }

            return null;
        }
    }
}
