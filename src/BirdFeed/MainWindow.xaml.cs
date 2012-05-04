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
using System.Windows.Media.Animation;
using System.Windows.Threading;
using BirdFeed.Properties;

namespace BirdFeed
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Storyboard _fadeOut;            
            

        public MainWindow()
        {
            InitializeComponent();

            this._fadeOut = (Storyboard)this.Resources["FadeOut"];
            this._fadeOut.Completed += new EventHandler(_fadeOut_Completed);

            new DispatcherTimer(TimeSpan.FromSeconds(Settings.Default.TweetDuration), DispatcherPriority.Normal, Update, this.Dispatcher);
        }


        private void Update(Object sender, EventArgs e)
        {
            this._fadeOut.Begin();
        }

        void _fadeOut_Completed(object sender, EventArgs e)
        {
            var viewModel = this.DataContext as TwitterFeedViewModel;

            viewModel.DisplayNextTweet();

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
            
            var sb = (Storyboard)this.Resources["FadeIn"];
            sb.Begin();

        }
    }
}
