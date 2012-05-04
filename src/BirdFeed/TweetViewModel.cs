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

            Int32 entityIndex = 0;
            TextSource = status.Text;

            Text.Clear();

            var img = new BitmapImage();
            img.BeginInit();

            img.UriSource =

            new Uri(status.Author.ProfileImageUrl, UriKind.Absolute);
            img.EndInit();

            this.UserImage = img;
            if (status.Entities == null)
            {

                Text.Add(

                new TextSegment { Text = status.Text });
                return;
            }

            if (status.Entities.Any())
            {

                var entities = status.Entities.OrderBy(x => x.StartIndex);
                Text.Add(

                new TextSegment() { Text = status.Text.Substring(0, entities.First().StartIndex) });
                foreach (var entity in entities)
                {

                    Text.Add(

                    new TextSegment { Text = status.Text.Substring(entity.StartIndex, entity.EndIndex - entity.StartIndex), IsHighlighted = true });
                    entityIndex++;

                    if (entities.Count() > entityIndex)
                    {

                        var nextEntity = entities.ElementAt(entityIndex);
                        var
                        distance = nextEntity.StartIndex - entity.EndIndex;
                        Text.Add(

                        new TextSegment() { Text = status.Text.Substring(entity.EndIndex, distance) });
                    }

                    else
                    {

                        Text.Add(

                        new TextSegment() { Text = status.Text.Substring(entity.EndIndex, status.Text.Length - entity.EndIndex) });
                    }

                }

            }

            else
            {

                Text.Add(

                new TextSegment { Text = status.Text });
            }

        }
    }
}
