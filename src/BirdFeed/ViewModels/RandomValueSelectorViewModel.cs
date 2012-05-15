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

        private String _selectedValue;
        public String SelectedValue
        {
            get { return _selectedValue; }
            set { this.ChangeAndNotify(ref _selectedValue, value, () => SelectedValue); }
        }

        public RandomValueSelectorViewModel(RandomValueSelector selector)
        {
            this._selector = selector;
            this.AvailableValues = new ObservableCollection<String>(this._selector.AvailaleValues);
        }

        public void SelectValue()
        {
            this.SelectedValue = this._selector.DrawValue();
        }

        public void ValueSelectionCompleted()
        {
            this.AvailableValues.Remove(SelectedValue);

            if (this.ValueSelected != null)
                this.ValueSelected(this, new EventArgs());
        }
    }
}
