using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Acumatica.Core.Windows.Controls
{
    public sealed class DatePicker : Control
    {
        public static readonly DependencyProperty SelectedDateProperty = DependencyProperty.Register("SelectedDate", typeof(DateTime?), typeof(DatePicker), new PropertyMetadata(default(DateTime), OnSelectedDateChanged));

        private bool _initialized = false;
        private bool _syncing = false;

        public DatePicker()
        {
            DefaultStyleKey = typeof(DatePicker);
            Loaded += OnLoaded;
        }

        private static void OnSelectedDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var picker = (DatePicker)d;
            picker.SyncControlsWithSelectedDate();
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            for (int i = 1; i <= 31; i++)
            {
                DayOptions.Items.Add(i.ToString());
            }
            
            for (int i = 1; i <= 12; i++)
            {
                DateTime monthStart = new DateTime(DateTime.Now.Year, i, 1);
                MonthOptions.Items.Add(monthStart.ToString("MMMM"));
            }
            
            DayOptions.SelectionChanged += DayOptionsOnSelectionChanged;
            MonthOptions.SelectionChanged += MonthOptionsOnSelectionChanged;
            YearOptions.SelectionChanged += YearOptionsOnSelectionChanged;

            _initialized = true;

            SyncControlsWithSelectedDate();
        }

        internal void SyncControlsWithSelectedDate()
        {
            if (!_initialized) return;

            _syncing = true;
            if (SelectedDate == null || SelectedDate.Value == DateTime.MinValue)
            {
                YearOptions.Items.Clear();
                for (int i = DateTime.Today.Year - 5; i < DateTime.Today.Year + 5; i++)
                {
                    YearOptions.Items.Add(i.ToString());
                }

                DayOptions.SelectedValue = null;
                MonthOptions.SelectedValue = null;
                YearOptions.SelectedValue = null;
            }
            else
            {
                YearOptions.Items.Clear();
                for (int i = SelectedDate.Value.Year - 5; i < SelectedDate.Value.Year + 5; i++)
                {
                    YearOptions.Items.Add(i.ToString());
                }

                DayOptions.SelectedIndex = SelectedDate.Value.Day - 1;
                MonthOptions.SelectedIndex = SelectedDate.Value.Month - 1;
                YearOptions.SelectedValue = SelectedDate.Value.Year.ToString();
            }
            _syncing = false;
        }

        internal void UpdateSelectedDate()
        {
            if (!_initialized) return;

            int year = int.Parse((string) YearOptions.SelectedValue);
            int month = MonthOptions.SelectedIndex + 1;
            int day = DayOptions.SelectedIndex + 1;

            int maxDaysInMonth = DateTime.DaysInMonth(year, month);
            if (day > maxDaysInMonth)
            {
                day = maxDaysInMonth;
                DayOptions.SelectedIndex = maxDaysInMonth - 1;
            }

            if (month == 0)
                month = 1;

            if (day == 0)
                day = 1;

            var newDate = new DateTime(year, month, day);
            if (newDate != SelectedDate)
            {
                SelectedDate = newDate;
            }
        }

        private void DayOptionsOnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            if (!_syncing) UpdateSelectedDate();
        }

        private void MonthOptionsOnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            if (!_syncing) UpdateSelectedDate();
        }

        private void YearOptionsOnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            if (!_syncing) UpdateSelectedDate();
        }

        public DateTime? SelectedDate
        {
            get { return (DateTime?)GetValue(SelectedDateProperty); }
            set { SetValue(SelectedDateProperty, value); }
        }

        private ComboBox DayOptions
        {
            get { return (ComboBox)GetTemplateChild("DayOptions"); }
        }

        private ComboBox MonthOptions
        {
            get { return (ComboBox)GetTemplateChild("MonthOptions"); }
        }

        private ComboBox YearOptions
        {
            get { return (ComboBox)GetTemplateChild("YearOptions"); }
        }
    }
}
