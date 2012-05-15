using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using BirdFeed.Commands;

namespace BirdFeed.ViewModels
{
    public class BirdFeedViewModel : ViewModel
    {
        private static Object lockObject = new Object();
        private static BirdFeedViewModel _instance;
        public static BirdFeedViewModel Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (_instance == null)
                        _instance = new BirdFeedViewModel();
                }

                return _instance;
            }
        }

        private readonly Dictionary<Key, ICommand> _keyboardTriggers = new Dictionary<Key,ICommand>();

        private readonly TwitterFeedViewModel _twitterFeedViewModel = TwitterFeedViewModel.Instance;
        public TwitterFeedViewModel TwitterFeed
        {
            get { return _twitterFeedViewModel; }
        }
        
        private ViewModel _currentViewModel;
        public ViewModel CurrentViewModel
        {
            get { return _currentViewModel; }
            set { base.ChangeAndNotify(ref _currentViewModel, value, () => CurrentViewModel); }
        }

        public BirdFeedViewModel()
        {
            InitializeTriggers();
            CurrentViewModel = TwitterFeed;
        }

        private void InitializeTriggers()
        {
            this._keyboardTriggers.Add(Key.R, new RandomValueSelectorCommand());
        }

        public void ExecuteKeyboardTrigger(Key key)
        {
            if (!this._keyboardTriggers.ContainsKey(key))
                return;

            var command = this._keyboardTriggers[key];

            command.Execute(this);
        }
    }
}
