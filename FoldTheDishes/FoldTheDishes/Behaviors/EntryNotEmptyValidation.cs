using System;
using System.Globalization;
using Xamarin.Forms;

namespace FoldTheDishes.Behaviors
{
    public class EntryNotEmptyValidation : Behavior<Entry>
    {
        protected override void OnAttachedTo(Entry entry)
        {
            entry.TextChanged += Entry_TextChanged;
            base.OnAttachedTo(entry);
        }

        private void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidationHelper.ValidateEntryAndSetPlaceholderColor((Entry)sender, e.NewTextValue);
        }
    }
}
