using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using BirdFeed.ViewModels;

namespace BirdFeed.Commands
{
    public class RandomValueSelectorCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private BirdFeedViewModel _parentViewModel;
        private readonly RandomValueSelectorViewModel _viewModel;
        private readonly RandomValueSelector _selector;

        public RandomValueSelectorCommand()
        {
            this._selector = new RandomValueSelector();            
            this._viewModel = new RandomValueSelectorViewModel(this._selector);

            this._viewModel.ValueSelected += new EventHandler(_viewModel_ValueSelected);
        }

        void _viewModel_ValueSelected(object sender, EventArgs e)
        {
            _parentViewModel.CurrentViewModel = _parentViewModel.TwitterFeed;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var birdFeedViewModel = parameter as BirdFeedViewModel;
            this._parentViewModel = birdFeedViewModel;

            if (birdFeedViewModel == null)
                return;

            _parentViewModel.CurrentViewModel = this._viewModel;
            this._viewModel.SelectValue();
        }
    }
}
