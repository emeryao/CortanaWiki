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
        public MainPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("Theme"))
                this.RequestedTheme = (ElementTheme)Enum.Parse(typeof(ElementTheme), ApplicationData.Current.LocalSettings.Values["Theme"].ToString());

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
                if ((image.Source as BitmapImage)?.UriSource?.AbsoluteUri != "ms-appx:///Assets/CorBaikeIcon.png")
                {
                    BitmapImage bitmap = new BitmapImage(new Uri("ms-appx:///Assets/CorBaikeIcon.png"));
                    image.Source = bitmap;
                }

                txbResult.Text = "打开小娜/Cortana对她说：小娜百科查询+你想查询的词语。就可以听到小娜说给你听的查询结果啦！";
                txbResult.Visibility = Visibility.Visible;
                webView.Visibility = Visibility.Collapsed;

                return;
            }
            this.prograssRing.IsActive = true;

            var data = await QueryBaike.BaiduBaike.QueryByKeyword(keyword);
            if (!string.IsNullOrWhiteSpace(data.Url))
            {
                txbResult.Visibility = Visibility.Collapsed;
                webView.Visibility = Visibility.Visible;

                var requestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri(data.Url));
                requestMessage.Headers.Add("User-Agent", "Mozilla/5.0 (Windows Phone 10.0;  Android 4.2.1; Nokia; Lumia 520) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2486.0 Mobile Safari/537.36 Edge/13.10570");
                webView.NavigateWithHttpRequestMessage(requestMessage);
            }

            //if (!string.IsNullOrWhiteSpace(data.Summary))
            //    txbResult.Text = data.Summary;

            //if (data.Image != null)
            //{

            //    FileRandomAccessStream stream = (FileRandomAccessStream)await data.Image.OpenAsync(FileAccessMode.Read);
            //    BitmapImage bitmap = new BitmapImage();

            //    await bitmap.SetSourceAsync(stream);

            //    image.Source = bitmap;
            //}
            //else
            //{
            //    BitmapImage bitmap = new BitmapImage(new Uri("ms-appx:///Assets/CorBaikeIcon.png"));
            //    image.Source = bitmap;
            //}
            this.prograssRing.IsActive = false;
        }

        private void ThemeToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            this.RequestedTheme = ElementTheme.Dark;
        }

        private void ThemeToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            this.RequestedTheme = ElementTheme.Light;
        }

        private async void abbClearTemp_Click(object sender, RoutedEventArgs e)
        {
            txbKeyword.Text = "";
            txbResult.Text = "";

            if ((image.Source as BitmapImage)?.UriSource?.AbsoluteUri != "ms-appx:///Assets/CorBaikeIcon.png")
            {
                BitmapImage bitmap = new BitmapImage(new Uri("ms-appx:///Assets/CorBaikeIcon.png"));
                image.Source = bitmap;
            }

            var files = await ApplicationData.Current.TemporaryFolder.GetFilesAsync();
            foreach (var file in files)
            {
                await file.DeleteAsync();
            }

            MessageDialog dlg = new MessageDialog("缓存清理完成！", "提示");
            await dlg.ShowAsync();
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            this.RequestedTheme = this.RequestedTheme == ElementTheme.Light ? ElementTheme.Dark : ElementTheme.Light;
            ApplicationData.Current.LocalSettings.Values["Theme"] = this.RequestedTheme.ToString();
        }
    }
}
