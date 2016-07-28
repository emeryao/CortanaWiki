using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

namespace CortanaWiki
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        #region Property
        public MainViewModel Vm { get; set; } = new MainViewModel();
        #endregion

        #region Field
        private BitmapImage titleImage = new BitmapImage(new Uri("ms-appx:///Assets/CortanaWikiIcon.png"));
        private BitmapImage titleImageBlue = new BitmapImage(new Uri("ms-appx:///Assets/CortanaWikiIcon-blue.png"));

        private bool isMobile = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile";

        private bool isWebviewLoading = true;
        #endregion

        public MainPage()
        {
            this.InitializeComponent();

            if (isMobile)
            {
                InputPane.GetForCurrentView().Showing += (sender, args) =>
                {
                    this.Vm.NoKeyboard = false;
                };

                InputPane.GetForCurrentView().Hiding += (sender, args) =>
                {
                    this.Vm.NoKeyboard = true;
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
                this.Vm.IsComplete = true;
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
                if (isMobile)
                {
                    TextBox tb = sender as TextBox;
                    tb.IsEnabled = false;
                    tb.IsEnabled = true;
                }
                await Query(txbKeyword.Text);
            }
        }

        private async Task Query(string keyword)
        {
            this.Vm.IsBusy = true;
            if (string.IsNullOrWhiteSpace(keyword))
            {
                this.Vm.Result = "打开Cortana (小娜) 对她说：小娜百科查询+你想查询的词语。就可以听到小娜说给你听的查询结果啦！";
                this.Vm.IsComplete = false;
                this.Vm.IsBusy = false;
                return;
            }

            var data = await QueryBaike.BaiduBaike.QueryByKeyword(keyword);
            if (!string.IsNullOrWhiteSpace(data.Url))
            {
                this.Vm.IsComplete = true;
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri(data.Url));
                requestMessage.Headers.Add("User-Agent", "Mozilla/5.0 (Windows Phone 10.0;  Android 4.2.1; Nokia; Lumia 520) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2486.0 Mobile Safari/537.36 Edge/13.10570");
                webView.NavigateWithHttpRequestMessage(requestMessage);
            }
            else
            {
                this.Vm.IsComplete = false;
                this.Vm.Result = data.Summary;
            }

            this.Vm.IsBusy = false;
        }

        private async void abbClearTemp_Click(object sender, RoutedEventArgs e)
        {
            txbKeyword.Text = "";
            this.Vm.Result = "打开Cortana (小娜) 对她说：小娜百科查询+你想查询的词语。就可以听到小娜说给你听的查询结果啦！";

            var files = await ApplicationData.Current.TemporaryFolder.GetFilesAsync();
            foreach (var file in files)
            {
                await file.DeleteAsync();
            }

            MessageDialog dlg = new MessageDialog("缓存清理完成！", "提示");
            await dlg.ShowAsync();
        }

        private async void abbToggleTheme_Click(object sender, RoutedEventArgs e)
        {
            this.RequestedTheme = this.RequestedTheme == ElementTheme.Light ? ElementTheme.Dark : ElementTheme.Light;
            ApplicationData.Current.LocalSettings.Values["Theme"] = this.RequestedTheme.ToString();
            this.AdaptImageSource();
            if (this.Vm.IsComplete && !this.isWebviewLoading)
            {
                string jsCommand = "document.styleSheets[0].deleteRule(0);document.styleSheets[0].deleteRule(0);";
                await this.webView.InvokeScriptAsync("eval", new string[] { jsCommand });
                this.webView_LoadCompleted(null, null);
            }
        }

        private async void StoreComment_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-windows-store://review/?ProductId=9NBLGGH5KJ1B"));
        }

        private void AdaptImageSource()
        {
            if (this.RequestedTheme == ElementTheme.Dark)
            {
                this.Vm.TitleImage = titleImage;
                this.Vm.ThemeGlyph = '\uE706'.ToString();

                if (this.isMobile && Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                {
                    StatusBar statusBar = StatusBar.GetForCurrentView();
                    statusBar.BackgroundColor = Colors.Black;
                    statusBar.ForegroundColor = Colors.White;
                    statusBar.BackgroundOpacity = 1;
                }

                if (!this.isMobile && Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView"))
                {
                    var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                    if (titleBar != null)
                    {
                        titleBar.ButtonBackgroundColor = Colors.Black;
                        titleBar.ButtonForegroundColor = Colors.White;
                        titleBar.BackgroundColor = Colors.Black;
                        titleBar.ForegroundColor = Colors.White;
                    }
                }

            }
            else
            {
                this.Vm.TitleImage = this.titleImageBlue;
                this.Vm.ThemeGlyph = '\uE708'.ToString();
                if (this.isMobile && Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                {
                    StatusBar statusBar = StatusBar.GetForCurrentView();
                    statusBar.BackgroundColor = Colors.White;
                    statusBar.ForegroundColor = Colors.Black;
                    statusBar.BackgroundOpacity = 1;
                }

                if (!this.isMobile && Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView"))
                {
                    var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                    if (titleBar != null)
                    {
                        titleBar.ButtonBackgroundColor = Colors.White;
                        titleBar.ButtonForegroundColor = Colors.Black;
                        titleBar.BackgroundColor = Colors.White;
                        titleBar.ForegroundColor = Colors.Black;
                    }
                }

            }
        }

        private async void webView_LoadCompleted(object sender, NavigationEventArgs e)
        {
            string jsCommand = "";
            switch (this.RequestedTheme)
            {
                case ElementTheme.Default:
                case ElementTheme.Light:
                    jsCommand = "document.styleSheets[0].insertRule('body{filter:invert(0%)}'); document.styleSheets[0].insertRule('img{filter:invert(0%)}'); ";
                    break;
                case ElementTheme.Dark:
                    jsCommand = "document.styleSheets[0].insertRule('body{filter:invert(100%)}'); document.styleSheets[0].insertRule('img{filter:invert(100%)}'); ";
                    break;
                default:
                    break;
            }
            await this.webView.InvokeScriptAsync("eval", new string[] { jsCommand });
            this.isWebviewLoading = false;
        }

        private void webView_ContentLoading(WebView sender, WebViewContentLoadingEventArgs args)
        {
            this.isWebviewLoading = true;
        }
    }
}
