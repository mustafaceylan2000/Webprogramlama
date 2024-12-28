using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text.Json.Nodes;

namespace KuaforWeb.Controllers
{
    public class SuggestionsController : Controller
    {
        private readonly HttpClient _httpClient;

        public SuggestionsController()
        {
            _httpClient = new HttpClient();
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                ViewBag.Error = "Açıklama alanı boş bırakılamaz!";
                return View();
            }

            // Gemini API URL ve API anahtarı
            string apiKey = "AIzaSyBkZt4gygXJzzbuzgtJiqgH9-nk4S2zwns"; // Gemini API anahtarınızı buraya ekleyin
            //string apiUrl = $"https://generativelanguage.googleapis.com/v1beta2/models/gemini-1.5:generateMessage?key={apiKey}";
            //string apiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={apiKey}";
            string apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={apiKey}";
            // API'ye gönderilecek veri
            var prompt = $"Bir kullanıcı, '{description}' tarzında bir saç modeli istiyor. Ona uygun birkaç farklı saç modeli önerin ve açıklamalarını yapın.";
            var requestBody = new
            {
                contents = new[]
                   {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            //var requestBody = new
            //{
            //    model = "gemini-1.5-flash",
            //    messages = new[]
            // {
            //        new
            //        {
            //            role = "user",
            //            content = $"Bir kullanıcı, '{description}' tarzında bir saç modeli istiyor. Ona uygun birkaç farklı saç modeli önerin ve açıklamalarını yapın."
            //        }
            //    },
            //    temperature = 0.7,
            //    max_tokens = 100
            //};

            var jsonRequest = JsonSerializer.Serialize(requestBody);
            Console.WriteLine("Gönderilen JSON:");
            Console.WriteLine(jsonRequest);


            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            // API isteğini gönder
            var response = await _httpClient.PostAsync(apiUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = $"API isteği başarısız oldu: {response.StatusCode}";
                return View();
            }

            // API yanıtını al ve deserialize et
            var result = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<object>(result);

            var errorResponse = await response.Content.ReadAsStringAsync();
            Console.WriteLine("API Hata Yanıtı:");
            Console.WriteLine(errorResponse);
            // Yanıtı ViewBag üzerinden view'a gönder
            Console.WriteLine("API Yanıtı:");
            Console.WriteLine(result);
            // API yanıtını al
            //var result = await response.Content.ReadAsStringAsync();
            var jsonResponse = JsonNode.Parse(result);
            // Yanıtı doğrudan ViewBag'e string olarak ekleyin
            string suggestionsText = jsonResponse?["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString() ?? "Öneriler alınamadı.";
            //ViewBag.Suggestions = suggestionsText;

            string[] suggestionsList = suggestionsText.Split(new[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
            ViewBag.SuggestionsList = suggestionsList;
            //ViewBag.Suggestions = apiResponse;
            return View();
        }
    }
}
