// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TcpServerMultiPlugin.cs" company="">
//  2020 
// </copyright>
// <summary>
//   Multi TCP server class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using log4net;

namespace TcpServerPlugin
{
    /// <summary>
    /// Multi TCP server class.
    /// </summary>
    public class TcpServerMulti : IDisposable
    {
        /// <summary>
        /// Log4Net object.
        /// </summary>
        public static ILog Log = LogManager.GetLogger("MAIN");

        /// <summary>
        /// The _tcp listener.
        /// </summary>
        private static TcpListener _tcpListener;

        /// <summary>
        /// The _address.
        /// </summary>
        private readonly string _address;

        /// <summary>
        /// The _listener.
        /// </summary>
        private readonly Thread _listener;

        /// <summary>
        /// The _port.
        /// </summary>
        private readonly int _port = 9050;

        /// <summary>
        /// The _running.
        /// </summary>
        private readonly bool _running;

        /// <summary>
        /// The _client index.
        /// </summary>
        private int _clientIndex;

        /// <summary>
        /// The _encoding.
        /// </summary>
        private Encoding _encoding;

        /// <summary>
        /// The _inactive time.
        /// </summary>
        private int _inactiveTime = -1;

        /// <summary>
        /// The _use end code.
        /// </summary>
        private bool _useEndCode = false;

        private List<TcpClientData> _clients;

        private bool _isdisposing = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpServerMulti"/> class.
        /// </summary>
        /// <param name="port">
        /// TCP port.
        /// </param>
        public TcpServerMulti(int port)
        {
            _clients = new List<TcpClientData>();
            Encoding = Encoding.UTF8;
            if (port > 0) _port = port;

            _running = true;
            string sHostName = Dns.GetHostName();
            _address = string.Empty;
            IPAddress[] localAddress = Dns.GetHostAddresses(sHostName);

            _tcpListener = new TcpListener(IPAddress.Any,_port);
            _address = IPAddress.Any.ToString();// ipAddress.ToString();
            Log.InfoFormat("TCP Multi server starting with {0} on Port {1}", _address, _port);
            Log.InfoFormat("Enconding is: {0}",Encoding);


            if (_address == string.Empty)
            {
                Log.Fatal("No Ip Adress");
                return;
            }

            _tcpListener.Start();
            _listener = new Thread(Listen) { IsBackground = true };
            _listener.Start();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpServerMulti"/> class.
        /// </summary>
        /// <param name="address">
        /// IP Address that should be used (IP4/IP6).
        /// EMPTY-> use first IP4 Address
        /// </param>
        /// <param name="port">
        /// TCP port.
        /// </param>
        public TcpServerMulti(string address,int port=-1)
        {
            _clients = new List<TcpClientData>();
            Encoding = Encoding.UTF8;
            if (port > 0) _port = port;

            _running = true;
            string sHostName = Dns.GetHostName();
            _address = string.Empty;
            IPAddress[] localAddress = Dns.GetHostAddresses(sHostName);
            foreach (IPAddress ipAddress in localAddress)
            {
                if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    if (address == "" || address == ipAddress.ToString())
                    {
                        _tcpListener = new TcpListener(ipAddress, _port);
                        _address = ipAddress.ToString();
                        Log.InfoFormat("TCP Multi server starting with {0} on Port {1}", _address, _port);
                        break;
                    }
                    
                }
                if (ipAddress.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    if (address == ipAddress.ToString())
                    {
                        _tcpListener = new TcpListener(ipAddress, _port);
                        _address = ipAddress.ToString();
                        Log.InfoFormat("TCP Multi server starting with {0} on Port {1}", _address, _port);
                        break;
                    }
                    
                }
            }
            Log.InfoFormat("Enconding is: {0}", Encoding);

            if (_address == string.Empty)
            {
                Log.Fatal("No Ip Adress");
                return;
            }

            _tcpListener.Start();
            _listener = new Thread(Listen) { IsBackground = true };
            _listener.Start();
        }

        public static IPAddress[] GetIpAddresses()
        {
            string sHostName = Dns.GetHostName();
            return Dns.GetHostAddresses(sHostName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpServerMulti"/> class with default port 9050.
        /// </summary>
        public TcpServerMulti()
            : this(-1)
        {
        }

        /// <summary>
        /// The connected event handler.
        /// </summary>
        /// <param name="sender">
        /// Sender.
        /// </param>
        /// <param name="msg">
        /// Message.
        /// </param>
        public delegate void ConnectedEventHandler(TcpClientData sender, string msg);

        /// <summary>
        /// The receive event handler.
        /// </summary>
        /// <param name="sender">
        /// Sender.
        /// </param>
        /// <param name="msg">
        /// Message.
        /// </param>
        public delegate void ReceiveEventHandler(TcpClientData sender, string msg);

        /// <summary>
        /// The terminated event handler.
        /// </summary>
        /// <param name="sender">
        /// Sender.
        /// </param>
        public delegate void TerminatedEventHandler(TcpClientData sender);

        /// <summary>
        /// The client connected.
        /// </summary>
        public event ConnectedEventHandler ClientConnected;

        /// <summary>
        /// Client terminated.
        /// </summary>
        public event TerminatedEventHandler ClientTerminated;

        /// <summary>
        /// Receive message.
        /// </summary>
        public event ReceiveEventHandler ReceiveMsg;

        /// <summary>
        /// Gets the address.
        /// </summary>
        public string Address
        {
            get
            {
                return _address;
            }
        }

        /// <summary>
        /// Gets or sets the encoder.
        /// </summary>
        public Encoding Encoding
        {
            get
            {
                return _encoding;
            }

            set
            {
                _encoding = value;
                Log.InfoFormat("Enconding is change to: {0}", Encoding);
            }
        }

        /// <summary>
        /// Gets or sets the inactive time in seconds.
        /// </summary>
        public int InactiveTime
        {
            get
            {
                return _inactiveTime;
            }

            set
            {
                _inactiveTime = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether use end code.
        /// </summary>
        public bool UseEndCode
        {
            get
            {
                return _useEndCode;
            }

            set
            {
                _useEndCode = value;
            }
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            _isdisposing = true;
            for (int i=_clients.Count-1;i>=0;i--)
            {
                _clients[i].StopClient();
            }
            if (_tcpListener != null)
            {
                _tcpListener.Stop();
                _tcpListener = null;
            }

            if (_listener != null)
            {
                //_listener.Abort();
                _listener.Interrupt();
            }
            
        }

        /// <summary>
        /// Receive message from client.
        /// </summary>
        /// <param name="sender">
        /// Sender.
        /// </param>
        /// <param name="msg">
        /// Message.
        /// </param>
        internal void ReceiveClient(TcpClientData sender, string msg)
        {
            if (ReceiveMsg != null) ReceiveMsg(sender, msg);
        }

        /// <summary>
        /// Terminate client.
        /// </summary>
        /// <param name="sender">
        /// Sender.
        /// </param>
        internal void TerminateClient(TcpClientData sender)
        {
            _clients.Remove(sender);
            if (ClientTerminated != null) ClientTerminated(sender);
        }

        /// <summary>
        /// Listen Thread.
        /// </summary>
        private void Listen()
        {
            try
            {
                while (_running)
                {
                    _clientIndex++;
                    if (_clientIndex > 30000) _clientIndex = 1;
                    TcpClientData cd = new TcpClientData(this, string.Empty + _clientIndex, _tcpListener.AcceptTcpClient());
                    cd.SetProtocol0(_useEndCode);
                    cd.StartClient();
                    if (ClientConnected != null) ClientConnected(cd, "Connected: " + _clientIndex);
                    _clients.Add(cd);
                }
            }
            catch (ThreadAbortException)
            {
                Log.Info("Listen Thread Exit");
            }
            catch (Exception exc)
            {
                if (!_isdisposing)
                {
                    Log.ErrorFormat("EXC Listen: {0}", exc.Message);
                    Log.ErrorFormat("Trace:{0}", exc);
                }
            }
            finally
            {
                if (_tcpListener != null)
                {
                    _tcpListener.Stop();
                    _tcpListener = null;
                }
            }
            Log.Info("Listen Thread Stopped");
        }
    }
}