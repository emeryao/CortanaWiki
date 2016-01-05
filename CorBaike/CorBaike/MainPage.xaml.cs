using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

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
            base.OnNavigatedTo(e);

            txbKeyword.Text = e.Parameter.ToString();

            string result = (await QueryBaike.BaiduBaike.QueryByKeyword(txbKeyword.Text)).Summary;

            if (!string.IsNullOrWhiteSpace(result))
                txbResult.Text = result;
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
                if ((image.Source as BitmapImage)?.UriSource?.AbsoluteUri != "ms-appx:///Assets/CorBaikeLogo-310.png")
                {
                    BitmapImage bitmap = new BitmapImage(new Uri("ms-appx:///Assets/CorBaikeLogo-310.png"));
                    image.Source = bitmap;
                }

                txbResult.Text = "打开小娜/Cortana对她说：小娜百科查询+你想查询的词语。就可以听到小娜说给你听的查询结果啦！";
                return;
            }

            var data = await QueryBaike.BaiduBaike.QueryByKeyword(keyword);

            if (!string.IsNullOrWhiteSpace(data.Summary))
                txbResult.Text = data.Summary;

            if (data.Image != null)
            {

                FileRandomAccessStream stream = (FileRandomAccessStream)await data.Image.OpenAsync(FileAccessMode.Read);
                BitmapImage bitmap = new BitmapImage();

                await bitmap.SetSourceAsync(stream);

                image.Source = bitmap;
            }
            else
            {
                BitmapImage bitmap = new BitmapImage(new Uri("ms-appx:///Assets/CorBaikeLogo-310.png"));
                image.Source = bitmap;
            }
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

            if ((image.Source as BitmapImage)?.UriSource?.AbsoluteUri != "ms-appx:///Assets/CorBaikeLogo-310.png")
            {
                BitmapImage bitmap = new BitmapImage(new Uri("ms-appx:///Assets/CorBaikeLogo-310.png"));
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
    }
}
