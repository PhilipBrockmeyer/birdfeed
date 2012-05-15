using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BirdFeed.Properties;

namespace BirdFeed
{
    public class RandomValueSelector
    {
        private static readonly Random R = new Random();
        private readonly List<String> _availableValues;

        public IEnumerable<String> AvailaleValues { get { return _availableValues; } }

        public RandomValueSelector()
        {
            this._availableValues = new List<String>();
            InitializeAvailableValues();
        }

        private void InitializeAvailableValues()
        {
            var separator = Settings.Default.RandomValueSeparator;
            var values = Settings.Default.RandomValues;
            this._availableValues.AddRange(values.Split(separator).Select(x => x.Trim()));
        }

        public String DrawValue()
        {
            if (_availableValues.Count == 0)
                return null;

            var selectedIndex = R.Next(0, _availableValues.Count);
            var selectedValue = this._availableValues[selectedIndex];
            this._availableValues.RemoveAt(selectedIndex);
            return selectedValue;
        }
    }
}
