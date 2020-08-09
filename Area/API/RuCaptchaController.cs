using AutoBot.Area.CollectingСryptocurrencies.Interface;
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
    public class RuCaptchaController : ControllerBase, IRuCaptchaController
    {
        private readonly IRuCaptcha _ruCaptcha;
        private readonly Uri _urlRuCaptcha = new Uri("http://rucaptcha.com");
        private readonly string _urlForPostQuery = "/in.php";
        private readonly string _urlForGetQuery = "/res.php";
        private readonly string _APIKey = "bdeac7db70fa1dfbabc955a4826f7775"; //TODO: реализовать авто смену ключа при нулевом балансе
        private string _captchaId;

        public RuCaptchaController(IRuCaptcha ruCaptcha)
        {
            _ruCaptcha = ruCaptcha;
        }

        public RuCaptchaController()
        {
        }

        ///<inheritdoc/>
        [HttpPost]
        public async Task<string> SendCaptchaImage(string byteImage)
        {
            string status = string.Empty;
            while (status != "OK")
            {
                using (var client = new HttpClient { BaseAddress = _urlRuCaptcha })
                {
                    var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("key", $"{_APIKey}"),
                        new KeyValuePair<string, string>("body", $"{byteImage}"),
                        new KeyValuePair<string, string>("method", "base64")
                    });

                    var result = await client.PostAsync(_urlForPostQuery, content);
                    status = GetStatusRequest(result).Result;

                }

                if (status == "ERROR_NO_SLOT_AVAILABLE")
                {
                    await _ruCaptcha.GoTo();
                }
                else
                {
                    var array = status.Split("|");
                    status = array[0];
                    if (status == "OK")
                    {
                        _captchaId = array[1];
                        return await GetResponse(_captchaId);
                    }
                }
            }

            return string.Empty;
        }
        ///<inheritdoc/>
        [HttpPost]
        public async Task<string> SendRecaptcha_v2(string token, string url)
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
                _captchaId = array[1];
                Thread.Sleep(15000);
                return await GetResponse(array[1]);
            }

            return string.Empty;
        }
        ///<inheritdoc/>
        public string GetCaptchaQueryId()
        {
            return _captchaId;
        }
        ///<inheritdoc/>
        [HttpGet]
        public async void SendReport(string captchaId, string typeReport)
        {
            var url = $"{_urlRuCaptcha + _urlForGetQuery}?key={_APIKey}&action={typeReport}&id={captchaId}";
            using (var client = new HttpClient { BaseAddress = _urlRuCaptcha })
            {
                var result = await client.GetAsync(url);
                string status = await GetStatusRequest(result);
            }
        }


        /// <summary>
        /// Получить ответ
        /// </summary>
        /// <param name="captchaId">Id-капчи</param>
        /// <returns>Ключ расшифровки</returns>
        [HttpGet]
        protected async Task<string> GetResponse(string captchaId)
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
                        await _ruCaptcha.GoTo();
                    }
                }

                Thread.Sleep(5000);
            }

            return status.Replace("OK|", string.Empty);
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
    }
}
