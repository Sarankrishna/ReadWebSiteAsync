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

            try
            {
                txtHTML.Text = string.Empty;
                //txtHTML.Text = await GetStringAsyc(txtWebURL.Text);
                txtHTML.Text = await GetStringTask(txtWebURL.Text);
                //txtHTML.Text = await GetStringAsycTaskCompletionSource(txtWebURL.Text);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void Button_Sync_Click(object sender, RoutedEventArgs e)
        {
            txtHTML.Text = string.Empty;
            txtHTML.Text = GetString(txtWebURL.Text);
        }

        private Task<string> GetStringTask(string url)
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

        private async Task<string> GetStringAsyc(string url)
        {
            var client = new WebClient();
            var htmlByte = await client.DownloadDataTaskAsync(new Uri(url));
            var task2 = await Task.Factory.StartNew(() =>
            {
                var str = Encoding.Default.GetString(htmlByte);
                Thread.Sleep(2000);
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

            try
            {
                getstring.BeginInvoke(url, (a) =>
                {
                    
                    var result = getstring.EndInvoke(a);
                    tcs.SetResult(result);
                }, null);


            }
            catch (Exception ex)
            {

                tcs.TrySetException(ex);
            }
           
            return tcs.Task;
        }

        private async Task OneTestAsync(int n)
        {
            await Task.Delay(n);
        }

        private Task AnotherTestAsync(int n)
        {
            return Task.Delay(n);
        }

        private void DoTestAsync(Func<int, Task> whatTest, int n)
        {
            Task task = null;
            try
            {
                // start the task
                task = whatTest(n);

                // do some other stuff, 
                // while the task is pending
                MessageBox.Show("Press enter to continue");
                task.Wait();
            }
            catch (Exception ex)
            {
                Console.Write("Error: " + ex.Message);
            }
        }

        private void Button_OneTextAsync(object sender, RoutedEventArgs e)
        {

                DoTestAsync(OneTestAsync, -2);

        }

        private void Button_AnotherTestAsync(object sender, RoutedEventArgs e)
        {

            DoTestAsync(AnotherTestAsync, -2);

        }
    }
}
