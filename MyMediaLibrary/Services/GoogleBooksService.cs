using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MyMediaLibrary.Models;

namespace MyMediaLibrary.Services
{
    public class GoogleBooksService
    {
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "https://www.googleapis.com/books/v1/volumes";

        public GoogleBooksService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<MediaItem>> SearchBooksAsync(string query)
        {
            var url = $"{ApiUrl}?q={Uri.EscapeDataString(query)}&maxResults=5";
            var response = await _httpClient.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();
            return ParseBooks(json);
        }

        private List<MediaItem> ParseBooks(string json)
        {
            var books = new List<MediaItem>();
            try
            {
                var data = JsonConvert.DeserializeObject<JObject>(json);
                if (data?["items"] == null) return books;

                foreach (var item in data["items"])
                {
                    var volumeInfo = item["volumeInfo"];
                    books.Add(new MediaItem
                    {
                        Title = volumeInfo["title"]?.ToString().Trim() ?? "Без названия",
                        Author = volumeInfo["authors"] != null
                            ? string.Join(", ", volumeInfo["authors"].ToObject<string[]>())
                            : "Неизвестен",
                        Genre = volumeInfo["categories"]?[0]?.ToString() ?? "Книга",
                        Description = volumeInfo["description"]?.ToString(),
                        Rating = 0.0,
                        IsVisited = false,
                        ImagePath = volumeInfo["imageLinks"]?["thumbnail"]?.ToString()
                    });
                }
            }
            catch
            {
                // Если ошибка парсинга - возвращаем пустой список
            }
            return books;
        }   
    }
}