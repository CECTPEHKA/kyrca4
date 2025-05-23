using System.Collections.Generic;
using System.Windows;
using MyMediaLibrary.Models;

namespace MyMediaLibrary
{
    public partial class SearchResultsWindow : Window
    {
        public MediaItem SelectedBook { get; private set; }

        public SearchResultsWindow(List<MediaItem> books)
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            // Критически важная строка:
            ResultsListView.ItemsSource = books;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedBook = ResultsListView.SelectedItem as MediaItem;
            if (SelectedBook == null)
            {
                MessageBox.Show("Выберите книгу из списка!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            DialogResult = true;
            Close();
        }
    }
}