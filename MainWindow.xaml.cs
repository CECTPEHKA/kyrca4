using System;
using System.Linq;
using System.Windows;
using MyMediaLibrary.Data;
using MyMediaLibrary.Models;
using MyMediaLibrary.Services;

namespace MyMediaLibrary
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            LoadData();
        }

        private void LoadData()
        {
            using (var db = new MediaLibraryContext())
            {
                db.Database.EnsureCreated();
                MediaListView.ItemsSource = null; // Очистка списка перед обновлением
                MediaListView.ItemsSource = db.MediaItems.ToList();
            }
        }

        private void AddTestItem_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new MediaLibraryContext())
            {
                var newItem = new MediaItem
                {
                    Title = "Тестовая книга",
                    Genre = "Фантастика",
                    Author = "Автор Теста",
                    Date = DateTime.Now,
                    Rating = 8,
                    IsVisited = false,
                    Description = "Описание тестовой книги",
                    ImagePath = "",
                    Type = MediaType.Book
                };

                db.MediaItems.Add(newItem);
                db.SaveChanges();
            }
            LoadData();
        }

        private async void SearchBook_Click(object sender, RoutedEventArgs e)
        {
            string query = SearchTextBox.Text;
            if (string.IsNullOrWhiteSpace(query))
            {
                MessageBox.Show("Введите название книги для поиска!");
                return;
            }

            GoogleBooksService service = new GoogleBooksService();
            var bookItem = await service.GetBookByQueryAsync(query);
            if (bookItem != null)
            {
                using (var db = new MediaLibraryContext())
                {
                    db.MediaItems.Add(bookItem);
                    db.SaveChanges();
                }
                LoadData();
            }
            else
            {
                MessageBox.Show("Не удалось найти книгу по запросу!");
            }
        }

        private void EditMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (MediaListView.SelectedItem is MediaItem selectedItem)
            {
                EditMediaItemWindow editWindow = new EditMediaItemWindow(selectedItem);
                if (editWindow.ShowDialog() == true)
                {
                    using (var db = new MediaLibraryContext())
                    {
                        var dbItem = db.MediaItems.FirstOrDefault(x => x.Id == selectedItem.Id);
                        if (dbItem != null)
                        {
                            dbItem.Title = editWindow.MediaItem.Title;
                            dbItem.Genre = editWindow.MediaItem.Genre;
                            dbItem.Author = editWindow.MediaItem.Author;
                            dbItem.Description = editWindow.MediaItem.Description;
                            dbItem.Date = editWindow.MediaItem.Date;

                            db.SaveChanges();
                        }
                    }
                    LoadData();
                }
            }
            else
            {
                MessageBox.Show("Выберите элемент для редактирования!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (MediaListView.SelectedItem is MediaItem selectedItem)
            {
                using (var db = new MediaLibraryContext())
                {
                    var dbItem = db.MediaItems.FirstOrDefault(x => x.Id == selectedItem.Id);
                    if (dbItem != null)
                    {
                        db.MediaItems.Remove(dbItem);
                        db.SaveChanges();
                    }
                }
                LoadData();
            }
        }
    }
}
