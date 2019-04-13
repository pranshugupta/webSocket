using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ServerApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Load Grid variables
        int _OddEven = 0;
        string _Line = string.Empty;
        private DataSource _DataSource = new DataSource();
        CancellationTokenSource _Cts;
        CancellationToken _Token;
        #endregion

        #region Connection Variables
        Socket _ServerSocket;
        Socket _Handler;
        byte[] _ByteData = new byte[4096];

        #endregion
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = _DataSource;

            StartConnection();
        }

        #region Loading Screen Data

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                _DataSource.FileName = filename;
            }
        }

        async private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (FileStream fs = File.Open(_DataSource.FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (BufferedStream bs = new BufferedStream(fs))
                using (StreamReader sr = new StreamReader(bs))
                {
                    _Cts = new CancellationTokenSource();
                    _Token = _Cts.Token;

                    while ((_Line = sr.ReadLine()) != null && _Line.CompareTo("+---------+---------------+----------+") == 0)
                    {
                        await Task.Factory.StartNew(() => AddRecord(sr), _Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void AddRecord(StreamReader sr)
        {
            if (_Token.IsCancellationRequested)
                return;

            string transferData = string.Empty;
            FileData fileData = new FileData();
            _OddEven = _OddEven + 1;


            string[] firstLine = sr.ReadLine().Split(',');
            {
                fileData.Time = firstLine[0];
                fileData.Note1 = firstLine[1];
                fileData.Note2 = firstLine[2].Split(' ')[0].Trim();
                fileData.Name = firstLine[2].Split(' ')[3].Trim();
            }

            string[] secondLine = sr.ReadLine().Split('|');
            {
                for (int i = 0; i < secondLine.Length; i++)
                {
                    string args = secondLine[i].Trim().ToUpper();
                    if (!string.IsNullOrWhiteSpace(args))
                    {
                        int value = Convert.ToInt32(args, 16);
                        fileData.Value = fileData.Value + value;
                    }
                }
            }

            if (_OddEven % 2 == 1)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,
                     new Action(() => _DataSource.OddDataSource.Add(fileData)));
                transferData = string.Format("ODD~{0}~{1}~{2}~{3}~{4}", fileData.Time, fileData.Note1, fileData.Note2, fileData.Name, fileData.Value);
            }
            else
            {
                Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,
                     new Action(() => _DataSource.EvenDataSource.Add(fileData)));

                transferData = string.Format("EVEN~{0}~{1}~{2}~{3}~{4}", fileData.Time, fileData.Note1, fileData.Note2, fileData.Name, fileData.Value);
            }

            Send(_Handler, transferData);

            _Line = sr.ReadLine();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            _Cts.Cancel();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            _Cts.Cancel();
            _DataSource.FileName = string.Empty;
            Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,
                     new Action(() => {
                         _DataSource.EvenDataSource.Clear();
                         _DataSource.OddDataSource.Clear();
                     }));
        }

        #endregion

        #region Socket Communication

        private void StartConnection()
        {
            try
            {
                // We are using TCP sockets
                _ServerSocket = new Socket(
                addressFamily: AddressFamily.InterNetwork,
                socketType: SocketType.Stream,
                protocolType: ProtocolType.Tcp);

                // Assign the any IP of the hardware and listen on port number which the hardware is sending(here it's 5656)
                var oIPEndPoint = new IPEndPoint(address: IPAddress.Any, port: 8888);

                // Bind and listen on the given address
                _ServerSocket.Bind(localEP: oIPEndPoint);
                _ServerSocket.Listen(backlog: 1);

                MessageBox.Show("Launch Application Client Application");

                // Program is suspended while waiting for an incoming connection.
                _Handler = _ServerSocket.Accept();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Send(Socket handler, String data)
        {
            try
            {
                // Convert the string data to byte data using ASCII encoding.
                byte[] byteData = Encoding.ASCII.GetBytes(data);

                _Handler.Send(byteData);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        #endregion
    }
}
