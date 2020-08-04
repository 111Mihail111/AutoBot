using System.Threading.Tasks;

namespace AutoBot.Area.CollectingСryptocurrencies.Interface
{
    public interface IRuCaptchaController
    {
        /// <summary>
        /// Послать изображение капчи
        /// </summary>
        /// <param name="byteImage">Байты изображения</param>
        /// <returns>Символы расшифровки</returns>
        public Task<string> SendCaptchaImage(string byteImage);
        /// <summary>
        /// Послать Рекапчу_v2
        /// </summary>
        /// <param name="token">Токен рекапчи</param>
        /// <param name="url">Url-адрес страницы</param>
        /// <returns>Ключ расшифровки</returns>
        public Task<string> SendRecaptcha_v2(string token, string url);
        /// <summary>
        /// Получить идентификатор запроса капчи
        /// </summary>
        /// <returns></returns>
        public string GetCaptchaQueryId();
        /// <summary>
        /// Послать отчет
        /// </summary>
        /// <param name="captchaId">Идентификатор капчи</param>
        /// <param name="typeReport">Тип отчета</param>
        public void SendReport(string captchaId, string typeReport);
    }
}
