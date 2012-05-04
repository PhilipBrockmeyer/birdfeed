using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BirdFeed
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.DataContextChanged += new DependencyPropertyChangedEventHandler(MainWindow_DataContextChanged);

        }

        void MainWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ((TwitterFeedViewModel)e.NewValue).PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(MainWindow_PropertyChanged);
        }

        void MainWindow_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "CurrentTweet")
                return;

            var viewModel = this.DataContext as TwitterFeedViewModel;

            if (viewModel.CurrentTweet == null)
                return;

            var textBlock = new TextBlock() { TextWrapping = TextWrapping.Wrap };

            foreach (var segment in viewModel.CurrentTweet.Text)
            {
                textBlock.Inlines.Add(new Run 
                    { 
                        Text = segment.Text, 
                        Foreground = segment.IsHighlighted ? (Brush)this.Resources["HighlightBrush"] : (Brush)this.Resources["TextBrush"]
                    });
            }

            this.tweetContent.Content = textBlock;
        }
    }
}
