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
        private readonly ObservableCollection<MediaItem> _mediaItems;
        private readonly MediaLibraryContext _dbContext;

        public MainWindow()
        {
            InitializeComponent();
            _dbContext = new MediaLibraryContext();
            _mediaItems = new ObservableCollection<MediaItem>();
            MediaListView.ItemsSource = _mediaItems;
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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ОБНОВЛЕННЫЙ метод для чекбокса
        private void VisitedCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.DataContext is MediaItem item)
            {
                item.IsVisited = checkBox.IsChecked ?? false;

                try
                {
                    using (var db = new MediaLibraryContext())
                    {
                        var dbItem = db.MediaItems.Find(item.Id);
                        if (dbItem != null)
                        {
                            dbItem.IsVisited = item.IsVisited;
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

        // ОБНОВЛЕННЫЙ метод для звёзд
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
                    var dbItem = _dbContext.MediaItems.Find(item.Id);
                    if (dbItem != null)
                    {
                        dbItem.Rating = item.Rating;
                        _dbContext.SaveChanges();

                        // Принудительное обновление `ListView`
                        RefreshListView();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка сохранения рейтинга: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
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


        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.DataContext is MediaItem item)
            {
                try
                {
                    var dbItem = _dbContext.MediaItems.Find(item.Id);
                    if (dbItem != null)
                    {
                        dbItem.IsVisited = checkBox.IsChecked ?? false;
                        _dbContext.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            e.Handled = true;
        }

        private void EditMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (MediaListView.SelectedItem is not MediaItem selectedItem) return;

            var editWindow = new EditMediaItemWindow(selectedItem);
            if (editWindow.ShowDialog() == true)
            {
                try
                {
                    var dbItem = _dbContext.MediaItems.Find(selectedItem.Id);
                    if (dbItem != null)
                    {
                        dbItem.Title = editWindow.MediaItem.Title;
                        dbItem.Author = editWindow.MediaItem.Author;
                        dbItem.Genre = editWindow.MediaItem.Genre;
                        dbItem.Description = editWindow.MediaItem.Description;
                        _dbContext.SaveChanges();
                        RefreshListView();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (MediaListView.SelectedItem is not MediaItem selectedItem) return;

            var confirm = MessageBox.Show(
                $"Удалить '{selectedItem.Title}'?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirm == MessageBoxResult.Yes)
            {
                try
                {
                    _dbContext.MediaItems.Remove(selectedItem);
                    _dbContext.SaveChanges();
                    _mediaItems.Remove(selectedItem);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void RefreshListView()
        {
            var tempList = new ObservableCollection<MediaItem>(_dbContext.MediaItems.ToList());
            MediaListView.ItemsSource = null;
            MediaListView.ItemsSource = tempList;
            _mediaItems.Clear();
            foreach (var item in tempList)
            {
                _mediaItems.Add(item);
            }
        }
    }
}
