using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.ObjectModel;
using MyMediaLibrary.Data;
using MyMediaLibrary.Services;
using MyMediaLibrary.Models;

namespace MyMediaLibrary
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<MediaItem> _mediaItems;
        private MediaLibraryContext _dbContext;

        public MainWindow()
        {
            InitializeComponent();
            InitializeDatabase();
            LoadData();
        }

        private void InitializeDatabase()
        {
            _dbContext = new MediaLibraryContext();
            _dbContext.Database.EnsureCreated();
        }
        private async void SearchBook_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                MessageBox.Show("Введите название книги");
                return;
            }

            try
            {
                var service = new GoogleBooksService();
                var books = await service.SearchBooksAsync(SearchTextBox.Text);

                if (books == null || books.Count == 0)
                {
                    MessageBox.Show("Книги не найдены");
                    return;
                }

                var window = new SearchResultsWindow(books);
                if (window.ShowDialog() == true && window.SelectedBook != null)
                {
                    using (var db = new MediaLibraryContext())
                    {
                        db.MediaItems.Add(window.SelectedBook);
                        db.SaveChanges();
                        _mediaItems.Add(window.SelectedBook);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void LoadData()
        {
            try
            {
                _mediaItems = new ObservableCollection<MediaItem>(_dbContext.MediaItems.ToList());
                MediaListView.ItemsSource = _mediaItems;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (MediaListView.SelectedItem is not MediaItem selectedItem) return;

            var confirm = MessageBox.Show($"Удалить '{selectedItem.Title}'?",
                                        "Подтверждение",
                                        MessageBoxButton.YesNo);
            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                using (var db = new MediaLibraryContext())
                {
                    var itemToDelete = db.MediaItems.Find(selectedItem.Id);
                    if (itemToDelete != null)
                    {
                        db.MediaItems.Remove(itemToDelete);
                        db.SaveChanges();
                    }
                }
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления: {ex.Message}", "Критическая ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (MediaListView.SelectedItem is not MediaItem selectedItem) return;

            var editWindow = new EditMediaItemWindow(selectedItem);
            if (editWindow.ShowDialog() == true)
            {
                try
                {
                    using (var db = new MediaLibraryContext())
                    {
                        var dbItem = db.MediaItems.Find(selectedItem.Id);
                        if (dbItem != null)
                        {
                            dbItem.Title = editWindow.MediaItem.Title;
                            dbItem.Genre = editWindow.MediaItem.Genre;
                            dbItem.Author = editWindow.MediaItem.Author;
                            db.SaveChanges();
                        }
                    }

                    // Принудительно обновляем UI
                    selectedItem.Title = editWindow.MediaItem.Title;
                    selectedItem.Genre = editWindow.MediaItem.Genre;
                    selectedItem.Author = editWindow.MediaItem.Author;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private void VisitedCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.DataContext is MediaItem item)
            {
                try
                {
                    using (var db = new MediaLibraryContext())
                    {
                        var dbItem = db.MediaItems.Find(item.Id);
                        if (dbItem != null)
                        {
                            dbItem.IsVisited = checkBox.IsChecked ?? false;
                            db.SaveChanges();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void RatingStar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button &&
                button.Tag is string tagStr &&
                double.TryParse(tagStr, out double newRating) &&
                button.DataContext is MediaItem item)
            {
                bool isHalfRating = Keyboard.IsKeyDown(Key.LeftCtrl) ||
                                   Keyboard.IsKeyDown(Key.RightCtrl);

                item.Rating = isHalfRating ? newRating - 0.5 : newRating;

                try
                {
                    using (var db = new MediaLibraryContext())
                    {
                        var dbItem = db.MediaItems.Find(item.Id);
                        if (dbItem != null)
                        {
                            dbItem.Rating = item.Rating;
                            db.SaveChanges();
                        }
                    }
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка сохранения рейтинга: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
