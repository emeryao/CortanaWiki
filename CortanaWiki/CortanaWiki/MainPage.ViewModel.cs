using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

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

        public string Version { get; } = $"当前版本：{Windows.ApplicationModel.Package.Current.Id.Version.Major}.{Windows.ApplicationModel.Package.Current.Id.Version.Minor}.{Windows.ApplicationModel.Package.Current.Id.Version.Build}";

        private BitmapImage titleImage = new BitmapImage(new Uri("ms-appx:///Assets/CortanaWikiIcon.png"));

        public BitmapImage TitleImage
        {
            get { return titleImage; }
            set
            {
                titleImage = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.TitleImage)));
            }
        }

        private bool isBusy = false;

        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                isBusy = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.IsBusy)));
            }
        }


        private bool noKeyboard = true;

        public bool NoKeyboard
        {
            get { return noKeyboard; }
            set
            {
                noKeyboard = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.NoKeyboard)));
            }
        }

        private string result = "打开Cortana (小娜) 对她说：小娜百科查询+关键词。就可以听到小娜说给你听的查询结果啦！";

        public string Result
        {
            get { return result; }
            set
            {
                result = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Result)));
            }
        }

        private bool isComplete = false;

        public bool IsComplete
        {
            get { return isComplete; }
            set
            {
                isComplete = value;
                if (!value)
                {
                    this.Result = this.result;
                }
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.IsComplete)));
            }
        }

    }
}
