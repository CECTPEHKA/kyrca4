using System;
using System.Windows;
using MyMediaLibrary.Data;

namespace MyMediaLibrary
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Гарантированное создание БД
            using (var db = new MediaLibraryContext())
            {
                db.Database.EnsureCreated();
            }

            base.OnStartup(e);
        }
    }
}