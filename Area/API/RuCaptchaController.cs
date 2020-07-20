using AutoBot.Area.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoBot.Area.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class RuCaptchaController : ControllerBase
    {
        private readonly IRuCaptcha _ruCaptcha = new RuCaptcha();
        private readonly Uri _urlRuCaptcha = new Uri("http://rucaptcha.com");
        private readonly string _urlForPostQuery = "/in.php";
        private readonly string _urlForGetQuery = "/res.php";
        private readonly string _APIKey = "bdeac7db70fa1dfbabc955a4826f7775";
        private string _keyCaptcha;

        [HttpPost]
        public async Task<string> SendCaptcha(string image)
        {
            string status = string.Empty;
            while (status != "OK")
            {
                using (var client = new HttpClient { BaseAddress = _urlRuCaptcha })
                {
                    var content = new FormUrlEncodedContent(new[]
                    {
                    new KeyValuePair<string, string>("key", $"{_APIKey}"),
                    new KeyValuePair<string, string>("body", $"{image}"),
                    new KeyValuePair<string, string>("method", "base64")
                });

                    var result = await client.PostAsync(_urlForPostQuery, content);
                    status = GetStatusRequest(result).Result;

                }

                if (status == "ERROR_NO_SLOT_AVAILABLE")
                {
                    _ruCaptcha.GoTo();
                }
                else
                {
                    var array = status.Split("|");
                    status = array[0];
                    if (status == "OK")
                    {
                        _keyCaptcha = array[1];
                        return await GetResponseOnCaptcha(_keyCaptcha);
                    }
                }
            }

            return string.Empty;
        }

        [HttpGet]
        public async Task<string> GetResponseOnCaptcha(string captchaId)
        {
            var url = $"{_urlRuCaptcha + _urlForGetQuery}?key={_APIKey}&action=get&id={captchaId}";
            string status = "CAPCHA_NOT_READY";

            while (status == "CAPCHA_NOT_READY")
            {
                using (var client = new HttpClient { BaseAddress = _urlRuCaptcha })
                {
                    var result = await client.GetAsync(url);

                    status = await GetStatusRequest(result);
                    if (status == "ERROR_NO_SLOT_AVAILABLE")
                    {
                        _ruCaptcha.GoTo();
                    }
                }

                Thread.Sleep(5000);
            }

            return status.Replace("OK|", string.Empty);
        }

        [HttpGet]
        public async void SendReportOnCaptcha(string captchaId, string typeReport)
        {
            var url = $"{_urlRuCaptcha + _urlForGetQuery}?key={_APIKey}&action={typeReport}&id={captchaId}";
            using (var client = new HttpClient { BaseAddress = _urlRuCaptcha })
            {
                var result = await client.GetAsync(url);
                string status = await GetStatusRequest(result);
            }
        }

        [HttpPost]
        public async Task<string> SendCaptcha_v2(string token, string url)
        {
            string status;
            using (var client = new HttpClient { BaseAddress = _urlRuCaptcha })
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("key", _APIKey),
                    new KeyValuePair<string, string>("method", "userrecaptcha"),
                    new KeyValuePair<string, string>("googlekey", token),
                    new KeyValuePair<string, string>("pageurl", url),
                });

                var result = await client.PostAsync(_urlForPostQuery, content);
                status = GetStatusRequest(result).Result;

            }
            var array = status.Split("|");
            if (array[0] == "OK")
            {
                _keyCaptcha = array[1];
                Thread.Sleep(15000);
                return await GetResponseOnCaptcha(array[1]);
            }

            return string.Empty;
        }



        /// <summary>
        /// Получить статус запроса
        /// </summary>
        /// <param name="responseMessage">Ответ от сервиса</param>
        /// <returns>Код статуса</returns>
        protected async Task<string> GetStatusRequest(HttpResponseMessage responseMessage)
        {
            var bytes = await responseMessage.Content.ReadAsByteArrayAsync();
            return Encoding.GetEncoding("utf-8").GetString(bytes, 0, bytes.Length).ToString();
        }

        public string GetKeyCaptcha()
        {
            return _keyCaptcha;
        }
    }
}
