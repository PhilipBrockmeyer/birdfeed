using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BirdFeed.ViewModels;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace BirdFeed.Views
{
    /// <summary>
    /// Interaction logic for RandomNumberSelectorView.xaml
    /// </summary>
    public partial class RandomValueSelectorView : UserControl
    {
        private RandomValueSelectorViewModel _viewModel;
        private DispatcherTimer _timer;
        private DispatcherTimer _selectionTimer;
        private Int32 _hightlightedIndex;
        private Storyboard _selectionAnimation;
        private Storyboard _selectedAnimation;
        private Storyboard _selectionCompleteAnimation;
        private Int32 _increment = 1;
        private Int32 _initialRotations = 30;

        public RandomValueSelectorView()
        {
            InitializeComponent();

            this._selectionAnimation = (Storyboard)this.Resources["selectionAnimation"];
            this._selectedAnimation = (Storyboard)this.Resources["selectedAnimation"];
            this._selectionCompleteAnimation = (Storyboard)this.Resources["selectionCompleteAnimation"];

            this._selectedAnimation.Completed += new EventHandler(_selectedAnimation_Completed);
            this._selectionCompleteAnimation.Completed += new EventHandler(_selectionCompleteAnimation_Completed);
            this.DataContextChanged += new DependencyPropertyChangedEventHandler(RandomValueSelectorView_DataContextChanged);
            this._timer = new DispatcherTimer(TimeSpan.FromSeconds(1.0), DispatcherPriority.Normal, BeginAnimation, this.Dispatcher);
            this._selectionTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(50), DispatcherPriority.Normal, HighlightSelection, this.Dispatcher);

        }

        void RandomValueSelectorView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var oldViewModel = e.OldValue as RandomValueSelectorViewModel;

            if (oldViewModel != null)
                oldViewModel.PropertyChanged -= viewModel_PropertyChanged;

            var newViewModel = e.NewValue as RandomValueSelectorViewModel;
            this._viewModel = newViewModel;

            if (newViewModel == null)
                return;

            newViewModel.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(viewModel_PropertyChanged);
            this._timer.Start();
        }

        void viewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "SelectedValue")
                return;

            this._timer.Start();
        }

        private void BeginAnimation(Object sender, EventArgs e)
        {
            this._timer.Stop();
            AnimateSelection();
        }

        public void AnimateSelection()
        {
            if (!this._viewModel.AvailableValues.Contains(this._viewModel.SelectedValue))
                return;

            this._initialRotations = 30;
            this._increment = 1;
            this._selectionTimer.Interval = TimeSpan.FromMilliseconds(50);
            this._selectionTimer.Start();
        }

        private void HighlightSelection(Object sender, EventArgs e)
        {
            if (!this._viewModel.AvailableValues.Contains(this._viewModel.SelectedValue))
                return;

            var itemContainer = VisualTreeHelper.GetChild(items.ItemContainerGenerator.ContainerFromIndex(this._hightlightedIndex), 0);
                
            if ((this._selectionTimer.Interval > TimeSpan.FromMilliseconds(500.0)) && ((String)this._viewModel.SelectedValue == (String)items.Items[this._hightlightedIndex]))
            {
                Storyboard.SetTarget(this._selectedAnimation, itemContainer);
                this._selectedAnimation.Begin();
                this._selectionTimer.Stop();
            }
            else
            {
                Storyboard.SetTarget(this._selectionAnimation, itemContainer);

                this._selectionAnimation.Begin();

                this._hightlightedIndex = (this._hightlightedIndex + 1) % this._viewModel.AvailableValues.Count;

                if (this._initialRotations == 0)
                {
                    this._selectionTimer.Interval = this._selectionTimer.Interval.Add(TimeSpan.FromMilliseconds(this._increment));
                    this._increment += 1;
                }
                else
                {
                    this._initialRotations--;
                }
            }
        }

        void _selectedAnimation_Completed(object sender, EventArgs e)
        {
            this._selectionCompleteAnimation.Begin();
        }

        void _selectionCompleteAnimation_Completed(object sender, EventArgs e)
        {
            this._viewModel.ValueSelectionCompleted();         
        }
    }
}
