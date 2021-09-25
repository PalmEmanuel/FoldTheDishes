using FoldTheDishes.Services;
using System.ComponentModel;
using Xamarin.Forms;

namespace FoldTheDishes.Behaviors
{
    class DateValidationBehavior : Behavior<DatePicker>
    {
        public Label DateLabel { get; set; }
        public Label TimeLabel { get; set; }
        public TimePicker TimePicker { get; set; }

        protected override void OnAttachedTo(DatePicker datepicker)
        {
            datepicker.DateSelected += DatePicker_DateSelected;
            base.OnAttachedTo(datepicker);
        }

        private void DatePicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            ValidationHelper.ValidateDateTimeAndSetLabelColor(e.NewDate, TimePicker.Time, DateLabel, TimeLabel);
        }

        protected override void OnDetachingFrom(DatePicker datePicker)
        {
            datePicker.DateSelected -= DatePicker_DateSelected;
            base.OnDetachingFrom(datePicker);
        }
    }

    class TimeValidationBehavior : Behavior<TimePicker>
    {
        public Label DateLabel { get; set; }
        public Label TimeLabel { get; set; }
        public DatePicker DatePicker { get; set; }

        protected override void OnAttachedTo(TimePicker timepicker)
        {
            timepicker.PropertyChanged += TimePicker_TimeSelected;
            base.OnAttachedTo(timepicker);
        }

        private void TimePicker_TimeSelected(object sender, PropertyChangedEventArgs e)
        {
            ValidationHelper.ValidateDateTimeAndSetLabelColor(DatePicker.Date, ((TimePicker)sender).Time, DateLabel, TimeLabel);
        }

        protected override void OnDetachingFrom(TimePicker timePicker)
        {
            timePicker.PropertyChanged -= TimePicker_TimeSelected;
            base.OnDetachingFrom(timePicker);
        }
    }
}
