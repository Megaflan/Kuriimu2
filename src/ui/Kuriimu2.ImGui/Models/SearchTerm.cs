using System;
using System.Threading.Tasks;
using ImGui.Forms.Controls.Text;

namespace Kuriimu2.ImGui.Models
{
    class SearchTerm
    {
        private const int ChangeTimer_ = 1000;

        private DateTime _changeTime;
        private Task _changeTask;
        private string _tempText;
        private string _text = string.Empty;

        private readonly TextBox _searchTextBox;

        public event EventHandler TextChanged;

        public SearchTerm(TextBox searchTextBox)
        {
            _searchTextBox = searchTextBox;

            searchTextBox.TextChanged += searchTextBox_TextChanged;
        }

        public string Get()
        {
            return string.IsNullOrEmpty(_text) ? "*" : _text;
        }

        public void Clear()
        {
            _searchTextBox.Text = string.Empty;
        }

        private void searchTextBox_TextChanged(object sender, EventArgs e)
        {
            var hasChanged = _text != _searchTextBox.Text;
            _text = _searchTextBox.Text;

            if (hasChanged)
                OnTextChanged();
        }

        private void OnTextChanged()
        {
            TextChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
