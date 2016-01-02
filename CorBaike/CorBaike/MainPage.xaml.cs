﻿using Windows.UI.Xaml;
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

            txbResult.Text = await QueryBaike.BaiduBaike.QueryByKeyword(txbKeyword.Text);
        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            txbResult.Text = await QueryBaike.BaiduBaike.QueryByKeyword(txbKeyword.Text);
        }
    }
}
