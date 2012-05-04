using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BirdFeed
{
    public class TwitterFeedViewModel : ViewModel
    {

        private CircularQueue<TweetViewModel> _queue;
        private IEnumerator<TweetViewModel> _items;
        private TwitterService _service;
        private Int64 _lastId;
        private Dispatcher _dispatcher;
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

        public class TextSegment
        {

            public String Text { get; set; }
            public Boolean IsHighlighted { get; set; }
        }

        private TweetViewModel _currentTweet;
        public TweetViewModel CurrentTweet
        {

            get { return this._currentTweet; }
            set { ChangeAndNotify(ref this._currentTweet, value, () => CurrentTweet); }
        }

        public TViewModel(Dispatcher dispatcher)
        {

            this._queue = new CircularQueue<TweetViewModel>(10);
            this._items = this._queue.GetEnumerator();
            this._dispatcher = dispatcher;
            AnonymousAccess();

            AddInitialItems(10);

            new DispatcherTimer(TimeSpan.FromSeconds(10.0), DispatcherPriority.Normal, Update, dispatcher);
            new DispatcherTimer(TimeSpan.FromSeconds(15.0), DispatcherPriority.Normal, Retrieve, dispatcher);
        }

        private void AddInitialItems(Int32 count)
        {

            var results = this._service.Search("#CFLDraft", count, TwitterSearchResultType.Recent, (result, response) =>
            {

                foreach (var status in result.Statuses)
                {

                    this._dispatcher.BeginInvoke((Action)(() =>
                    {

                        var tweet = new TweetViewModel();
                        tweet.SetData(status);

                        this._queue.Add(tweet);
                    }));

                }

                this._lastId = result.MaxId;
            });

        }

        private void AnonymousAccess()
        {

            this._service = new TwitterService();
            this._service.IncludeEntities = true;
        }

        private void Retrieve(Object sender, EventArgs e)
        {

            var results = this._service.SearchSince(this._lastId, "#CFLDraft", TwitterSearchResultType.Recent, (result, response) =>
            {

                foreach (var status in result.Statuses)
                {

                    this._dispatcher.BeginInvoke((Action)(() =>
                    {

                        var tweet = new TweetViewModel();
                        tweet.SetData(status);

                        this._queue.Add(tweet);
                    }));

                }

                this._lastId = result.MaxId;
            });

        }

        private void Update(Object sender, EventArgs e)
        {

            if (!_items.MoveNext())
                return;
            this.CurrentTweet = this._items.Current;
        }
    }
}
