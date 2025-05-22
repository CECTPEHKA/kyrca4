using System;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using MyMediaLibrary.Data;
using MyMediaLibrary.Models;
using MyMediaLibrary.Services;
using System.Collections.ObjectModel;

namespace MyMediaLibrary
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<MediaItem> _mediaItems;
        private MediaLibraryContext _dbContext;

        public MainWindow()
        {
            InitializeComponent();
            _dbContext = new MediaLibraryContext();
            _mediaItems = new ObservableCollection<MediaItem>();
            MediaListView.ItemsSource = _mediaItems;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                _mediaItems.Clear();
                foreach (var item in _dbContext.MediaItems.ToList())
                {
                    _mediaItems.Add(item);
                }
                StatusText.Text = $"Загружено {_mediaItems.Count} элементов";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddTestItem_Click(object sender, RoutedEventArgs e)
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

            try
            {
                _dbContext.MediaItems.Add(newItem);
                _dbContext.SaveChanges();
                _mediaItems.Add(newItem);
                StatusText.Text = "Тестовый элемент добавлен";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void SearchBook_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                MessageBox.Show("Введите название книги для поиска!", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var service = new GoogleBooksService();
                var bookItem = await service.GetBookByQueryAsync(SearchTextBox.Text);

                if (bookItem != null)
                {
                    _dbContext.MediaItems.Add(bookItem);
                    _dbContext.SaveChanges();
                    _mediaItems.Add(bookItem);
                    StatusText.Text = $"Добавлена книга: {bookItem.Title}";
                }
                else
                {
                    MessageBox.Show("Книга не найдена", "Информация",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка поиска: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (MediaListView.SelectedItem is MediaItem selectedItem)
            {
                var editWindow = new EditMediaItemWindow(selectedItem);
                if (editWindow.ShowDialog() == true)
                {
                    try
                    {
                        var dbItem = _dbContext.MediaItems.Find(selectedItem.Id);
                        if (dbItem != null)
                        {
                            dbItem.Title = editWindow.MediaItem.Title;
                            dbItem.Genre = editWindow.MediaItem.Genre;
                            dbItem.Author = editWindow.MediaItem.Author;
                            dbItem.Description = editWindow.MediaItem.Description;
                            dbItem.Date = editWindow.MediaItem.Date;
                            dbItem.Rating = editWindow.MediaItem.Rating;
                            dbItem.IsVisited = editWindow.MediaItem.IsVisited;
                            dbItem.ImagePath = editWindow.MediaItem.ImagePath;
                            dbItem.Type = editWindow.MediaItem.Type;

                            _dbContext.SaveChanges();

                            // Обновляем элемент в коллекции
                            var index = _mediaItems.IndexOf(selectedItem);
                            _mediaItems[index] = dbItem;

                            StatusText.Text = $"Изменения сохранены: {dbItem.Title}";
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                                      MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (MediaListView.SelectedItem is MediaItem selectedItem)
            {
                var confirm = MessageBox.Show(
                    $"Вы действительно хотите удалить '{selectedItem.Title}'?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (confirm == MessageBoxResult.Yes)
                {
                    try
                    {
                        var dbItem = _dbContext.MediaItems.Find(selectedItem.Id);
                        if (dbItem != null)
                        {
                            _dbContext.MediaItems.Remove(dbItem);
                            _dbContext.SaveChanges();
                            _mediaItems.Remove(selectedItem);
                            StatusText.Text = $"Удалено: {selectedItem.Title}";
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка",
                                      MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}