using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace BirdFeed
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            var viewModel = new TwitterFeedViewModel();
            viewModel.Initialize(this.MainWindow.Dispatcher);

            this.MainWindow.DataContext = viewModel;
        }
    }
}
