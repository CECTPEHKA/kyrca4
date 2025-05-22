using System;
using System.Net.Http;
using System.Threading.Tasks;
using MyMediaLibrary.Models;
using Newtonsoft.Json;

namespace MyMediaLibrary.Services
{
    public class GoogleBooksService
    {
        private readonly HttpClient _httpClient;

        public GoogleBooksService()
        {
            _httpClient = new HttpClient(); // Создание HTTP-клиента для API-запросов
        }

        public async Task<MediaItem> GetBookByQueryAsync(string query)
        {
            string url = $"https://www.googleapis.com/books/v1/volumes?q={query}";
            try
            {
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    dynamic data = JsonConvert.DeserializeObject(json);

                    if (data.items != null && data.items.Count > 0)
                    {
                        var volumeInfo = data.items[0].volumeInfo;

                        return new MediaItem
                        {
                            Title = volumeInfo.title,
                            Author = volumeInfo.authors != null ? string.Join(", ", volumeInfo.authors.ToObject<string[]>()) : "Неизвестно",
                            Description = volumeInfo.description,
                            ImagePath = volumeInfo.imageLinks != null ? volumeInfo.imageLinks.thumbnail : "",
                            Type = MediaType.Book,
                            Date = DateTime.TryParse((string)volumeInfo.publishedDate, out DateTime dt) ? dt : DateTime.Now,
                            Genre = volumeInfo.categories != null ? string.Join(", ", volumeInfo.categories.ToObject<string[]>()) : "Не указано",
                            Rating = 0,
                            IsVisited = false
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка запроса к Google Books API: " + ex.Message);
            }
            return null;
        }
    }
}
