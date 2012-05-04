using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using TweetSharp;

namespace BirdFeed
{
    public class TweetViewModel : ViewModel
    {
        private BitmapImage _userImage;
        public BitmapImage UserImage
        {
            get { return _userImage; }
            set { ChangeAndNotify(ref _userImage, value, () => UserImage); }
        }

        private BitmapImage _picture;
        public BitmapImage Picture
        {
            get { return _picture; }
            set { ChangeAndNotify(ref _picture, value, () => Picture); }
        }

        private String _screenName;
        public String ScreenName
        {
            get { return _screenName; }
            set { ChangeAndNotify(ref _screenName, value, () => ScreenName); }
        }

        private String _name;
        public String Name
        {
            get { return _name; }
            set { ChangeAndNotify(ref _name, value, () => Name); }
        }

        private String _time;
        public String Time 
        {
            get { return _time; }
            set { ChangeAndNotify(ref _time, value, () => Time); }
        }

        private readonly ObservableCollection<TextSegment> _text = new ObservableCollection<TextSegment>();
        public ObservableCollection<TextSegment> Text { get { return _text; } }
        private String _textSource;
        public String TextSource
        {

            get { return _textSource; }
            set { ChangeAndNotify(ref _textSource, value, () => TextSource); }
        }

        public void SetData(TwitterSearchStatus status)
        {
            this.ScreenName = status.Author.ScreenName;
            this.Name = status.FromUserName;

            SetUserImage(status);
            SetPicture(status);
            SetText(status);

            this.Time = GetFriendlyTime(status.CreatedDate);
        }

        private void SetText(TwitterSearchStatus status)
        {
            Text.Clear();
            Int32 entityIndex = 0;
            TextSource = status.Text;

            if (status.Entities == null)
            {
                Text.Add(new TextSegment { Text = status.Text });
                return;
            }

            if (status.Entities.Any())
            {
                var entities = status.Entities.OrderBy(x => x.StartIndex);
                Text.Add(

                new TextSegment() { Text = status.Text.Substring(0, entities.First().StartIndex) });
                foreach (var entity in entities)
                {
                    Text.Add(new TextSegment { Text = status.Text.Substring(entity.StartIndex, entity.EndIndex - entity.StartIndex), IsHighlighted = true });
                    entityIndex++;

                    if (entities.Count() > entityIndex)
                    {
                        var nextEntity = entities.ElementAt(entityIndex);
                        var distance = nextEntity.StartIndex - entity.EndIndex;
                        Text.Add(new TextSegment() { Text = status.Text.Substring(entity.EndIndex, distance) });
                    }
                    else
                    {
                        Text.Add(new TextSegment() { Text = status.Text.Substring(entity.EndIndex, status.Text.Length - entity.EndIndex) });
                    }
                }
            }
            else
            {
                Text.Add(new TextSegment { Text = status.Text });
            }
        }

        private void SetUserImage(TwitterSearchStatus status)
        {
            var img = new BitmapImage();
            img.BeginInit();
            img.CreateOptions |= BitmapCreateOptions.IgnoreColorProfile;
            img.UriSource = new Uri(status.Author.ProfileImageUrl, UriKind.Absolute);
            img.EndInit();
            this.UserImage = img;
        }

        private void SetPicture(TwitterSearchStatus status)
        {
            if (status.Entities == null)
                return;

            if (status.Entities.Media.Any())
            {
                var img = new BitmapImage();
                img.BeginInit();
                img.CreateOptions |= BitmapCreateOptions.IgnoreColorProfile;
                img.UriSource = new Uri(status.Entities.Media.First().MediaUrl, UriKind.Absolute);
                img.EndInit();
                this.Picture = img;
            }
        }

        private String GetFriendlyTime(DateTime dt)
        {
            var ts = new TimeSpan(DateTime.UtcNow.Ticks - dt.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 60)
            {
                return ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";
            }
            if (delta < 120)
            {
                return "a minute ago";
            }
            if (delta < 2700) // 45 * 60
            {
                return ts.Minutes + " minutes ago";
            }
            if (delta < 5400) // 90 * 60
            {
                return "an hour ago";
            }
            if (delta < 86400) // 24 * 60 * 60
            {
                return ts.Hours + " hours ago";
            }
            if (delta < 172800) // 48 * 60 * 60
            {
                return "yesterday";
            }
            if (delta < 2592000) // 30 * 24 * 60 * 60
            {
                return ts.Days + " days ago";
            }
            if (delta < 31104000) // 12 * 30 * 24 * 60 * 60
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "one month ago" : months + " months ago";
            }
            int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
            return years <= 1 ? "one year ago" : years + " years ago";
        }
    }
}
