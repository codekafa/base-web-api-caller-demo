using Core.Infrastructure.Base;
using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiDesignPattern
{
    class Program
    {
        static void Main(string[] args)
        {

            var  apiHelp = new ApiHandler<ApiRequestModel, ApiResponseModel>();

            var result = apiHelp.Get("https://jsonplaceholder.typicode.com/todos/1", new ApiRequestModel());

            var resultsync = apiHelp.GetAsync("https://jsonplaceholder.typicode.com/todos/1", new ApiRequestModel());
        }
    }
}
