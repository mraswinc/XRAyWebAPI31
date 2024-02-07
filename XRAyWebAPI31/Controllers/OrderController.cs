using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System.Threading.Tasks;
using XRAyWebAPI31.Models;

namespace XRAyWebAPI31.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly RestClient _client;

        public OrderController()
        {
            _client = new RestClient("https://www.amazon.com");
        }

        [HttpGet]
        public async Task<Order> Get()
        {
            // send GET request with RestSharp
            var client = new RestClient("https://testapi.jasonwatmore.com");
            var request = new RestRequest("products/1");
            var response = await client.ExecuteGetAsync<Order>(request);

            return response.Data;
        }
    }
}
