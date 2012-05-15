using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace BirdFeed.ViewModels
{
    public class RandomValueSelectorViewModel : ViewModel
    {
        private readonly RandomValueSelector _selector;

        public event EventHandler ValueSelected;

        public ObservableCollection<String> AvailableValues { get; private set; }

        public RandomValueSelectorViewModel(RandomValueSelector selector)
        {
            this._selector = selector;
            this.AvailableValues = new ObservableCollection<String>(this._selector.AvailaleValues);
        }

        public void SelectValue()
        {
            var selectedValue = this._selector.DrawValue();
        }
    }
}
