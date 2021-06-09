using Presentation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Presentation.Services
{
    public interface IServiceApi
    {
        Task<List<Cell>> GetAll(HttpClient client);
        Task<bool> Create(CellViewModel cell, HttpClient client);
        Task<bool> Edit(string code, CellViewModel cell, HttpClient client);
        Task<Cell> GetReal(String code, HttpClient client);
        Task<Cell> Get(String code, HttpClient client);
        Task<bool> Delete(String code, HttpClient client);
        Task<bool> CreateUser(UserViewModel user);
        Task<JWT> LoginUser(UserViewModel user);

    }
}
