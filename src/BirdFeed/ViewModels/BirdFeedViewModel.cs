using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace BirdFeed.ViewModels
{
    public class BirdFeedViewModel : ViewModel
    {
        private readonly Dictionary<Key, ICommand> _keyboardTriggers = new Dictionary<Key,ICommand>();

        private readonly TwitterFeedViewModel _twitterFeedViewModel = TwitterFeedViewModel.Instance;

        public TwitterFeedViewModel TwitterFeed
        {
            get { return _twitterFeedViewModel; }
        }

        public BirdFeedViewModel()
        {
            InitializeTriggers();
        }

        private void InitializeTriggers()
        {
            this._keyboardTriggers.Add(Key.A, new Command());
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
