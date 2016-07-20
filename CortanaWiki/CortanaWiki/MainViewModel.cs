using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CortanaWiki
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string themeGlyph = '\uE706'.ToString();

        public string ThemeGlyph
        {
            get { return themeGlyph; }
            set
            {
                themeGlyph = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ThemeGlyph)));
            }
        }
    }
}
