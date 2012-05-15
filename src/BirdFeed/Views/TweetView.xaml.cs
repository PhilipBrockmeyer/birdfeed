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
using System.Windows.Threading;
using System.Windows.Media.Animation;
using BirdFeed.Properties;
<<<<<<< HEAD
using BirdFeed.ViewModels;
=======
>>>>>>> 6f0c4c9252c5ea1f6271a7f369f3e5f0d46eac89

namespace BirdFeed.Views
{
    /// <summary>
    /// Interaction logic for TweetView.xaml
    /// </summary>
    public partial class TweetView : UserControl
    {
        Storyboard _fadeOut;        

        public TweetView()
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

            if (viewModel.CurrentTweet.HasImage)
            {
                textBlock.FontSize = 20;
            }
            else
            {
                textBlock.FontSize = 36;
            }

            this.tweetContent.Content = textBlock;
            
            var sb = (Storyboard)this.Resources["FadeIn"];
            sb.Begin();
        }
    }
}
