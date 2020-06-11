using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AutoBot.Area.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class RuCaptchaController : ControllerBase
    {
        private readonly string _urlRuCaptcha = "https://rucaptcha.com/res.php";
        private readonly string _APIKey = "bdeac7db70fa1dfbabc955a4826f7775";

        [HttpPost]
        public async void SendCaptcha(string image)
        {
            WebRequest request = WebRequest.Create(_urlRuCaptcha + $"?key={_APIKey}&body={image}");
            request.Method = "POST";

            WebResponse webResponse = await request.GetResponseAsync();
        }
    }
}
