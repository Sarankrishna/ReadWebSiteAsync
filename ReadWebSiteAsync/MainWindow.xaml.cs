using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ReadWebSiteAsync
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_Async_Click(object sender, RoutedEventArgs e)
        {
            txtHTML.Text = string.Empty;
            //txtHTML.Text = await GetStringAsyc(txtWebURL.Text);
            txtHTML.Text = await GetStringAsycTaskCompletionSource(txtWebURL.Text);
        }

        private void Button_Sync_Click(object sender, RoutedEventArgs e)
        {
            txtHTML.Text = string.Empty;
            txtHTML.Text = GetString(txtWebURL.Text);
        }

        private Task<string> GetStringAsyc(string url)
        {
            var client = new WebClient();
            var task = client.DownloadDataTaskAsync(new Uri(url));

            var task2 = task.ContinueWith<string>(task1 =>
            {
                var str = Encoding.Default.GetString(task1.Result);
                Thread.Sleep(5000);
                return str;
            });
            return task2;
        }

        private string GetString(string url)
        {
            var client = new WebClient();
            var data = client.DownloadData(new Uri(url));
            var str = Encoding.Default.GetString(data);
            Thread.Sleep(5000);
            return str;
        }

        private Task<string> GetStringAsycTaskCompletionSource(string url)
        {

            var tcs= new TaskCompletionSource<string>();
            Func<string,string> getstring = GetString;
            
            getstring.BeginInvoke(url, (a) =>
            {
                var result = getstring.EndInvoke(a);
                tcs.SetResult(result);
            }, null);

           
            return tcs.Task;
        }


        

    }
}
