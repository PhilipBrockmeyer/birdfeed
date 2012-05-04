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
        private static Object lockObject = new Object();
        private static TwitterFeedViewModel _instance;
        public static TwitterFeedViewModel Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (_instance == null)
                        _instance = new TwitterFeedViewModel();
                }

                return _instance;
            }

        }

        private CircularQueue<TweetViewModel> _queue;
        private IEnumerator<TweetViewModel> _items;
        private TwitterService _service;
        private Int64 _lastId;
        private Dispatcher _dispatcher;
        private Boolean _isInitialized = false;

        private TweetViewModel _currentTweet;
        public TweetViewModel CurrentTweet
        {
            get { return this._currentTweet; }
            set { ChangeAndNotify(ref this._currentTweet, value, () => CurrentTweet); }
        }

        public String SearchTerm
        {
            get { return Settings.Default.SearchTerm; }
        }

        public TwitterFeedViewModel()
        {
            this._queue = new CircularQueue<TweetViewModel>(Settings.Default.HistorySize);
            this._items = this._queue.GetEnumerator();
            this._dispatcher = Dispatcher.CurrentDispatcher;
        }

        public void Initialize(Dispatcher dispatcher)
        {
            if (this._isInitialized)
                return;

            this._dispatcher = dispatcher;

            AnonymousAccess();

            AddInitialItems(Settings.Default.HistorySize);

            new DispatcherTimer(TimeSpan.FromSeconds(Settings.Default.TweetDuration), DispatcherPriority.Normal, Update, this._dispatcher);
            new DispatcherTimer(TimeSpan.FromSeconds(15.0), DispatcherPriority.Normal, Retrieve, this._dispatcher);

            this._isInitialized = true;
        }

        private void AddInitialItems(Int32 count)
        {
            var result = this._service.Search(Settings.Default.SearchTerm, count, TwitterSearchResultType.Recent, AppendTweetsAndDisplay);
        }

        private void Retrieve(Object sender, EventArgs e)
        {
            this._service.SearchSince(this._lastId, Settings.Default.SearchTerm, TwitterSearchResultType.Recent, AppendTweets);
        }

        private void AppendTweetsAndDisplay(TwitterSearchResult result, TwitterResponse response)
        {
            AppendTweets(result, response);
            DisplayNextTweet();
        }

        private void AppendTweets(TwitterSearchResult result, TwitterResponse response)
        {
            this._dispatcher.BeginInvoke((Action)(() =>
            {
                foreach (var status in result.Statuses)
                {
                    var tweet = new TweetViewModel();
                    tweet.SetData(status);
                    this._queue.Add(tweet);
                }

                this._lastId = result.MaxId;
            }));
        }

        private void AnonymousAccess()
        {
            this._service = new TwitterService();
            this._service.IncludeEntities = true;
        }

        private void Update(Object sender, EventArgs e)
        {
            DisplayNextTweet();
        }

        private void DisplayNextTweet()
        {
            if (!_items.MoveNext())
                return;

            this.CurrentTweet = this._items.Current;
        }
    }
}
