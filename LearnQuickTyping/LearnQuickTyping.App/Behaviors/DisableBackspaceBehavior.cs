using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnQuickTyping.App.Behaviors
{
    public class DisableBackspaceBehavior : Behavior<Entry>
    {
        private string _previousText = string.Empty;

        protected override void OnAttachedTo(Entry entry)
        {
            base.OnAttachedTo(entry);
            entry.TextChanged += OnTextChanged;
            _previousText = entry.Text ?? string.Empty;
        }

        protected override void OnDetachingFrom(Entry entry)
        {
            base.OnDetachingFrom(entry);
            entry.TextChanged -= OnTextChanged;
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = sender as Entry;
            if (!string.IsNullOrEmpty(e.OldTextValue) &&
                (string.IsNullOrEmpty(e.NewTextValue) || e.NewTextValue.Length < e.OldTextValue.Length))
            {
                // Restore previous text
                entry.Text = _previousText;
                // Move cursor to end
                entry.CursorPosition = entry.Text.Length;
            }
            else
            {
                // Update previous text for valid changes
                _previousText = e.NewTextValue ?? string.Empty;
            }
        }
    }
}
