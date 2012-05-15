using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using BirdFeed.ViewModels;

namespace BirdFeed
{
    public class ContentViewSelector : DataTemplateSelector
    {
        public DataTemplate TwitterFeedTemplate { get; set; }
        public DataTemplate RandomValueSelectorTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is TwitterFeedViewModel)
                return this.TwitterFeedTemplate;
            else if (item is RandomValueSelectorViewModel)
                return this.RandomValueSelectorTemplate;


            return base.SelectTemplate(item, container);
        }
    }
}
