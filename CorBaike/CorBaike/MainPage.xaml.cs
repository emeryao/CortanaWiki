using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

namespace CorBaike
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        #region Dependency Property
        public string ThemeGlyph
        {
            get { return (string)GetValue(ThemeGlyphProperty); }
            set { SetValue(ThemeGlyphProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Glyph.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ThemeGlyphProperty =
            DependencyProperty.Register("Glyph", typeof(string), typeof(MainPage), new PropertyMetadata('\uE706'.ToString()));

        public string Version
        {
            get { return (string)GetValue(VersionProperty); }
            set { SetValue(VersionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Version.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VersionProperty =
            DependencyProperty.Register("Version", typeof(string), typeof(MainPage), new PropertyMetadata("当前版本：1.0.0"));


        public BitmapImage TitleImage
        {
            get { return (BitmapImage)GetValue(TitleImageProperty); }
            set { SetValue(TitleImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TitleImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleImageProperty =
            DependencyProperty.Register("TitleImage", typeof(BitmapImage), typeof(MainPage), new PropertyMetadata(titleImage));

        public bool IsBusy
        {
            get { return (bool)GetValue(IsBusyProperty); }
            set { SetValue(IsBusyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsBusy.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsBusyProperty =
            DependencyProperty.Register("IsBusy", typeof(bool), typeof(MainPage), new PropertyMetadata(false));

        public string Keyword
        {
            get { return (string)GetValue(KeywordProperty); }
            set { SetValue(KeywordProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Keyword.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty KeywordProperty =
            DependencyProperty.Register("Keyword", typeof(string), typeof(MainPage), new PropertyMetadata(""));

        public bool NoKeyboard
        {
            get { return (bool)GetValue(NoKeyboardProperty); }
            set { SetValue(NoKeyboardProperty, value); }
        }

        // Using a DependencyProperty as the backing store for KeyboardOpened.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NoKeyboardProperty =
            DependencyProperty.Register("NoKeyboard", typeof(bool), typeof(MainPage), new PropertyMetadata(true));
        #endregion

        #region Private Property
        private static BitmapImage titleImage = new BitmapImage(new Uri("ms-appx:///Assets/CorBaikeIcon.png"));
        private BitmapImage titleImageBlue = new BitmapImage(new Uri("ms-appx:///Assets/CorBaikeIcon-blue.png"));

        #endregion

        public MainPage()
        {
            //Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily==Windows.Desktop?Windows.Mobile;
            this.InitializeComponent();

            this.Version = $"当前版本：{Windows.ApplicationModel.Package.Current.Id.Version.Major}.{Windows.ApplicationModel.Package.Current.Id.Version.Minor}.{Windows.ApplicationModel.Package.Current.Id.Version.Build}";

            if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
            {

                Windows.UI.ViewManagement.InputPane.GetForCurrentView().Showing += (sender, args) =>
                {
                    this.NoKeyboard = false;
                };

                Windows.UI.ViewManagement.InputPane.GetForCurrentView().Hiding += (sender, args) =>
                {
                    this.NoKeyboard = true;
                };
            }
        }


        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("Theme"))
            {
                this.RequestedTheme = (ElementTheme)Enum.Parse(typeof(ElementTheme), ApplicationData.Current.LocalSettings.Values["Theme"].ToString());
                this.AdaptImageSource();
            }

            base.OnNavigatedTo(e);

            txbKeyword.Text = e.Parameter.ToString();

            var result = (await QueryBaike.BaiduBaike.QueryByKeyword(txbKeyword.Text));

            if (!string.IsNullOrWhiteSpace(result.Url))
            {
                txbResult.Visibility = Visibility.Collapsed;
                webView.Visibility = Visibility.Visible;

                var requestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri(result.Url));
                requestMessage.Headers.Add("User-Agent", "Mozilla/5.0 (Windows Phone 10.0;  Android 4.2.1; Nokia; Lumia 520) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2486.0 Mobile Safari/537.36 Edge/13.10570");
                webView.NavigateWithHttpRequestMessage(requestMessage);
            }

        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            await Query(txbKeyword.Text);
        }

        private async void txbKeyword_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                await Query(txbKeyword.Text);
            }
        }

        private async Task Query(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                txbResult.Text = "打开Cortana (小娜) 对她说：小娜百科查询+你想查询的词语。就可以听到小娜说给你听的查询结果啦！";
                txbResult.Visibility = Visibility.Visible;
                webView.Visibility = Visibility.Collapsed;

                return;
            }
            this.IsBusy = true;

            var data = await QueryBaike.BaiduBaike.QueryByKeyword(keyword);
            if (!string.IsNullOrWhiteSpace(data.Url))
            {
                txbResult.Visibility = Visibility.Collapsed;
                webView.Visibility = Visibility.Visible;

                var requestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri(data.Url));
                requestMessage.Headers.Add("User-Agent", "Mozilla/5.0 (Windows Phone 10.0;  Android 4.2.1; Nokia; Lumia 520) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2486.0 Mobile Safari/537.36 Edge/13.10570");
                webView.NavigateWithHttpRequestMessage(requestMessage);
            }

            this.IsBusy = false;
        }

        private async void abbClearTemp_Click(object sender, RoutedEventArgs e)
        {
            txbKeyword.Text = "";
            txbResult.Text = "";

            var files = await ApplicationData.Current.TemporaryFolder.GetFilesAsync();
            foreach (var file in files)
            {
                await file.DeleteAsync();
            }

            MessageDialog dlg = new MessageDialog("缓存清理完成！", "提示");
            await dlg.ShowAsync();
        }

        private void abbToggleTheme_Click(object sender, RoutedEventArgs e)
        {
            this.RequestedTheme = this.RequestedTheme == ElementTheme.Light ? ElementTheme.Dark : ElementTheme.Light;
            ApplicationData.Current.LocalSettings.Values["Theme"] = this.RequestedTheme.ToString();
            this.AdaptImageSource();
        }

        private void AdaptImageSource()
        {
            if (this.RequestedTheme == ElementTheme.Dark)
            {
                this.TitleImage = titleImage;
                this.ThemeGlyph = '\uE706'.ToString();
            }
            else
            {
                this.TitleImage = this.titleImageBlue;
                this.ThemeGlyph = '\uE708'.ToString();
            }
        }

    }
}
