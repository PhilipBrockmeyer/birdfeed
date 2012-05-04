using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TweetSharp;
using System.Windows.Threading;
using BirdFeed.Properties;

namespace BirdFeed
{
    public class TwitterFeedViewModel : ViewModel
    {
        private CircularQueue<TweetViewModel> _queue;
        private IEnumerator<TweetViewModel> _items;
        private TwitterService _service;
        private Int64 _lastId;
        private Dispatcher _dispatcher;

        private TweetViewModel _currentTweet;
        public TweetViewModel CurrentTweet
        {
            get { return this._currentTweet; }
            set { ChangeAndNotify(ref this._currentTweet, value, () => CurrentTweet); }
        }

        public TwitterFeedViewModel()
        {
            this._queue = new CircularQueue<TweetViewModel>(10);
            this._items = this._queue.GetEnumerator();
            this._dispatcher = Dispatcher.CurrentDispatcher;
            AnonymousAccess();

            AddInitialItems(10);

            new DispatcherTimer(TimeSpan.FromSeconds(10.0), DispatcherPriority.Normal, Update, this._dispatcher);
            new DispatcherTimer(TimeSpan.FromSeconds(15.0), DispatcherPriority.Normal, Retrieve, this._dispatcher);
        }

        private void AddInitialItems(Int32 count)
        {
            this._service.Search(Settings.Default.SearchTerm, count, TwitterSearchResultType.Recent, AppendTweets);
        }

        private void Retrieve(Object sender, EventArgs e)
        {
            this._service.SearchSince(this._lastId, Settings.Default.SearchTerm, TwitterSearchResultType.Recent, AppendTweets);
        }

        private void AppendTweets(TwitterSearchResult result, TwitterResponse response)
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
        }

        private void AnonymousAccess()
        {
            this._service = new TwitterService();
            this._service.IncludeEntities = true;
        }

        private void Update(Object sender, EventArgs e)
        {

            if (!_items.MoveNext())
                return;

            this.CurrentTweet = this._items.Current;
        }
    }
}
