using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using TweetSharp;
using System.Net;

namespace BirdFeed.ViewModels
{
    public class TweetViewModel : ViewModel
    {
        private TwitterSearchStatus _status;

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

        private Boolean _hasImage;
        public Boolean HasImage
        {
            get { return _hasImage; }
            set { ChangeAndNotify(ref _hasImage, value, () => HasImage); }
        }

        private readonly ObservableCollection<TextSegment> _text = new ObservableCollection<TextSegment>();
        public ObservableCollection<TextSegment> Text { get { return _text; } }
  
        public void SetData(TwitterSearchStatus status)
        {
            this._status = status;

            this.ScreenName = status.Author.ScreenName;
            this.Name = status.FromUserName;
            this.HasImage = false;

            SetUserImage(status);
            SetPicture(status);
            SetText(status);
            UpdateTime();                        
        }

        private void SetText(TwitterSearchStatus status)
        {
            Text.Clear();
            Int32 entityIndex = 0;

            if (status.Entities == null)
            {
                Text.Add(new TextSegment { Text = status.Text });
                return;
            }

            if (status.Entities.Any())
            {
                var entities = status.Entities.OrderBy(x => x.StartIndex);
                Text.Add(new TextSegment() { Text = WebUtility.HtmlDecode(status.Text.Substring(0, entities.First().StartIndex)) });

                foreach (var entity in entities)
                {
                    Text.Add(new TextSegment { Text = WebUtility.HtmlDecode(status.Text.Substring(entity.StartIndex, entity.EndIndex - entity.StartIndex)), IsHighlighted = true });
                    entityIndex++;

                    if (entities.Count() > entityIndex)
                    {
                        var nextEntity = entities.ElementAt(entityIndex);
                        var distance = nextEntity.StartIndex - entity.EndIndex;
                        Text.Add(new TextSegment() { Text = WebUtility.HtmlDecode(status.Text.Substring(entity.EndIndex, distance)) });
                    }
                    else
                    {
                        Text.Add(new TextSegment() { Text = WebUtility.HtmlDecode(status.Text.Substring(entity.EndIndex, status.Text.Length - entity.EndIndex)) });
                    }
                }
            }
            else
            {
                Text.Add(new TextSegment { Text = WebUtility.HtmlDecode(status.Text) });
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
            this.HasImage = false;

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
                this.HasImage = true;
            }
        }


        public void UpdateTime()
        {
            this.Time = DateTime.UtcNow.Subtract(this._status.CreatedDate).ToFriendlyString();            
        }
    }
}
