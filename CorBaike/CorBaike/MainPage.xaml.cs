using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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

            string result = await QueryBaike.BaiduBaike.QueryByKeyword(txbKeyword.Text);

            if (!string.IsNullOrWhiteSpace(result))
                txbResult.Text = result;
        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            txbResult.Text = await QueryBaike.BaiduBaike.QueryByKeyword(txbKeyword.Text);
        }

        private async void txbKeyword_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                string result = await QueryBaike.BaiduBaike.QueryByKeyword(txbKeyword.Text);

                if (!string.IsNullOrWhiteSpace(result))
                    txbResult.Text = result;
            }
        }
    }
}
