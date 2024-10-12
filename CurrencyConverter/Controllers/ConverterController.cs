using CurrencyConverter.Models;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices.JavaScript;
using Newtonsoft.Json.Linq;

namespace CurrencyConverter.Controllers
{
    public class ConverterController : Controller
    {
        private readonly string url = "https://openexchangerates.org/api/latest.json";
        private readonly string apiKey = "06e3e3e78dc740c4a9160bb610d398a7";
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task< IActionResult> Convert(Currency currency)
        {
            if(ModelState.IsValid)
            {
                var ExchangeRates = await GetExchangeRate();
                var ConvertedAmount = (currency.Amount) *
                    (ExchangeRates[currency.ToCurrency] / ExchangeRates[currency.FromCurrency]);
                currency.ConvertedAmount = decimal.Round(ConvertedAmount, 2);
                ViewBag.Converted = currency.ConvertedAmount;
                return View("Result");
            }
            return View("Index");
        }

        private async Task<Dictionary<string,decimal>> GetExchangeRate()
        {
            using(var client = new HttpClient())
            {
                // يقوم بارسال طلب الى url ويقوم باعاده النتيجه string
                var response = await client.GetStringAsync($"{url}?app_id={apiKey}");
                // يحوله الى json
                var data = JObject.Parse(response);
                // يحول json to dictionary
                var rates = data["rates"].ToObject<Dictionary<string, decimal>>();
                return rates;
            }
        }
    }
}
