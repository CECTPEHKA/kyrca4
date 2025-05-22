using System;
using System.Windows;
using MyMediaLibraryModels;

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
                Date = item.Date,
                Rating = item.Rating,
                IsVisited = item.IsVisited,
                ImagePath = item.ImagePath,
                Type = item.Type
            };

            // Заполняем поля формы
            TitleTextBox.Text = MediaItem.Title ?? "";
            GenreTextBox.Text = MediaItem.Genre ?? "";
            AuthorTextBox.Text = MediaItem.Author ?? "";
            DescriptionTextBox.Text = MediaItem.Description ?? "";
            DatePicker.SelectedDate = MediaItem.Date;
            RatingTextBox.Text = MediaItem.Rating.ToString();
            IsVisitedCheckBox.IsChecked = MediaItem.IsVisited;
            ImagePathTextBox.Text = MediaItem.ImagePath ?? "";
            TypeComboBox.SelectedItem = MediaItem.Type;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MediaItem.Title = string.IsNullOrWhiteSpace(TitleTextBox.Text) ? null : TitleTextBox.Text;
                MediaItem.Genre = string.IsNullOrWhiteSpace(GenreTextBox.Text) ? null : GenreTextBox.Text;
                MediaItem.Author = string.IsNullOrWhiteSpace(AuthorTextBox.Text) ? null : AuthorTextBox.Text;
                MediaItem.Description = string.IsNullOrWhiteSpace(DescriptionTextBox.Text) ? null : DescriptionTextBox.Text;
                MediaItem.Date = DatePicker.SelectedDate ?? DateTime.Now;
                MediaItem.Rating = int.TryParse(RatingTextBox.Text, out int rating) ? rating : 0;
                MediaItem.IsVisited = IsVisitedCheckBox.IsChecked ?? false;
                MediaItem.ImagePath = string.IsNullOrWhiteSpace(ImagePathTextBox.Text) ? null : ImagePathTextBox.Text;
                MediaItem.Type = TypeComboBox.SelectedItem as MediaType? ?? MediaType.Book;

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
    }
}