using System;
using System.Windows;
using MyMediaLibrary.Models;

namespace MyMediaLibrary
{
    public partial class EditMediaItemWindow : Window
    {
        public MediaItem MediaItem { get; private set; }

        public EditMediaItemWindow(MediaItem item)
        {
            InitializeComponent();

            // Клонируем объект
            MediaItem = new MediaItem
            {
                Id = item.Id,
                Title = item.Title,
                Genre = item.Genre,
                Author = item.Author,
                Description = item.Description,
       
                Rating = item.Rating,
                IsVisited = item.IsVisited,
            };

            // Заполняем поля формы
            TitleTextBox.Text = MediaItem.Title ?? "";
            GenreTextBox.Text = MediaItem.Genre ?? "";
            AuthorTextBox.Text = MediaItem.Author ?? "";
            DescriptionTextBox.Text = MediaItem.Description ?? "";
            IsVisitedCheckBox.IsChecked = MediaItem.IsVisited;

        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MediaItem.Title = string.IsNullOrWhiteSpace(TitleTextBox.Text) ? null : TitleTextBox.Text;
                MediaItem.Genre = string.IsNullOrWhiteSpace(GenreTextBox.Text) ? null : GenreTextBox.Text;
                MediaItem.Author = string.IsNullOrWhiteSpace(AuthorTextBox.Text) ? null : AuthorTextBox.Text;
                MediaItem.Description = string.IsNullOrWhiteSpace(DescriptionTextBox.Text) ? null : DescriptionTextBox.Text;
                MediaItem.IsVisited = IsVisitedCheckBox.IsChecked ?? false;

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"EDIT WINDOW ERROR: {ex.Message}");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void TypeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}