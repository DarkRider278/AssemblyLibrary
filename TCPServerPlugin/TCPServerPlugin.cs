// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TcpServerPlugin.cs" company="">
//   2020
// </copyright>
// <summary>
//   TCP server class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TcpServerPlugin
{
    /// <summary>
    /// TCP server class.
    /// </summary>
    public class TcpServer : IDisposable
    {
        /// <summary>
        /// The _data.
        /// </summary>
        private static byte[] _data;

        /// <summary>
        /// The _network stream.
        /// </summary>
        private static NetworkStream _networkStream;

        /// <summary>
        /// The _tcp client.
        /// </summary>
        private static TcpClient _tcpClient;

        /// <summary>
        /// The _tcp listener.
        /// </summary>
        private static TcpListener _tcpListener;

        /// <summary>
        /// The _received.
        /// </summary>
        private static int _received;

        /// <summary>
        /// The _listener.
        /// </summary>
        private readonly Thread _listener;

        /// <summary>
        /// The _running.
        /// </summary>
        private readonly bool _running;

        /// <summary>
        /// The _ip adress.
        /// </summary>
        private string _ipAdress = string.Empty;

        /// <summary>
        /// The _port.
        /// </summary>
        private int _port = 9050;

        /// <summary>
        /// The _worker.
        /// </summary>
        private Thread _worker;

        /// <summary>
        /// The _wrunning.
        /// </summary>
        private bool _wrunning;

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpServer"/> class.
        /// </summary>
        /// <param name="port">
        /// TCP port.
        /// </param>
        public TcpServer(int port)
        {
            if (port > 0) _port = port;
            _data = new byte[1024];
            _running = true;
            string sHostName = Dns.GetHostName();
            IPAdress = string.Empty;
            IPAddress[] localAddress = Dns.GetHostAddresses(sHostName);
            foreach (IPAddress ipAddress in localAddress)
            {
                if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    _tcpListener = new TcpListener(ipAddress, _port);
                    IPAdress = ipAddress.ToString();
                    break;
                }
            }

            if (IPAdress == string.Empty)
            {
                // MessageBox.Show("TCP Error");
                return;
            }

            _tcpListener.Start();
            _listener = new Thread(Listen) { IsBackground = true };
            _listener.Start();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpServer"/> class with default port 9050.
        /// </summary>
        public TcpServer()
            : this(-1)
        {
        }

        /// <summary>
        /// The receive event handler.
        /// </summary>
        /// <param name="sender">
        /// Sender.
        /// </param>
        /// <param name="msg">
        /// Message.
        /// </param>
        public delegate void ReceiveEventHandler(object sender, string msg);

        /// <summary>
        /// Receive message.
        /// </summary>
        public event ReceiveEventHandler ReceiveMsg;

        /// <summary>
        /// Gets or sets the ip adress.
        /// </summary>
        public string IPAdress
        {
            get
            {
                return _ipAdress;
            }

            set
            {
                _ipAdress = value;
            }
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            _listener.Abort();
            _worker.Abort();
            _networkStream.Close();
            _tcpClient.Close();
            _tcpListener.Stop();
        }

        /// <summary>
        /// Listen thread.
        /// </summary>
        public void Listen()
        {
            try
            {
                while (_running)
                {
                    _tcpClient = _tcpListener.AcceptTcpClient();
                    _networkStream = _tcpClient.GetStream();
                    _networkStream.Flush();
                    _networkStream.ReadTimeout = 200;
                    _wrunning = true;
                    _worker = new Thread(Worker) { IsBackground = true };
                    _worker.Start();
                }

                _tcpListener.Stop();
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Stop client.
        /// </summary>
        public void StopClient()
        {
            _wrunning = false;
            Thread.Sleep(700);
        }

        /// <summary>
        /// Receiver Thread.
        /// </summary>
        public void Worker()
        {
            try
            {
                while (_wrunning)
                {
                    if (_networkStream.DataAvailable)
                    {
                        _received = _networkStream.Read(_data, 0, _data.Length);
                        string s = ByteArrayToString(_data);
                        ReceiveMsg(this, s.Substring(0, _received));
                        _networkStream.Flush();
                    }
                    else
                    {
                        Thread.Sleep(500);
                    }
                }

                _networkStream.Close();
                _tcpClient.Close();
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Convert byte array to string.
        /// </summary>
        /// <param name="arr">
        /// Byte array.
        /// </param>
        /// <returns>
        /// Result.
        /// </returns>
        private string ByteArrayToString(byte[] arr)
        {
            ASCIIEncoding enc = new ASCIIEncoding();
            return enc.GetString(arr);
        }
    }
}