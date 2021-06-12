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
        Task<List<CellViewModel>> GetAll(HttpClient client);
        Task<bool> Create(CellViewModel cell, HttpClient client);
        Task<bool> Edit(string code, CellViewModel cell, HttpClient client);
        Task<CellViewModel> GetReal(String code, HttpClient client);
        Task<CellViewModel> Get(String code, HttpClient client);
        Task<bool> Delete(String code, HttpClient client);
        Task<bool> CreateUser(UserViewModel user);
        Task<JWT> LoginUser(UserViewModel user);

    }
}
