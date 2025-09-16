using System;
using System.Text;


using log4net;

namespace TCPClientPlugin
{
    using System.IO;
    using System.Net.Sockets;
    using System.Threading;

    public class TCPClientPlugin
    {
        /// <summary>
        /// Log4Net object.
        /// </summary>
        public static ILog Log = LogManager.GetLogger("MAIN");


        /// <summary>
        /// The _data.
        /// </summary>
        private readonly byte[] _data;
        
        /// <summary>
        /// The _olddata.
        /// </summary>
        private readonly byte[] _olddata;

        private TcpClient _tcpChannel = new TcpClient();

        /// <summary>
        /// The _ccendcode.
        /// </summary>
        private int _ccendcode = -1;

        /// <summary>
        /// The _ccstartcode.
        /// </summary>
        private int _ccstartcode = -1;

        /// <summary>
        /// The _dataoffset.
        /// </summary>
        private int _dataoffset;

        /// <summary>
        /// The _endcode.
        /// </summary>
        private int _endcode = -1;

        /// <summary>
        /// The _network stream.
        /// </summary>
        private NetworkStream _networkStream;

        /// <summary>
        /// The _received.
        /// </summary>
        private int _received;

        /// <summary>
        /// The _worker.
        /// </summary>
        private Thread _worker;

        /// <summary>
        /// The _wrunning.
        /// </summary>
        private bool _wrunning;

        private Encoding _encoding;
        

        private readonly object _syncRoot = new object();

        private bool _autoZero = true;


        private bool _active;

        private bool _logging = false;

        private string _remoteMachine = "127.0.0.1";

        private int _remotePort = 21000;

        /// <summary>
        /// Use client as receiver.
        /// </summary>
        private bool _useAsReceiver = false;

        /// <summary>
        /// The receive event handler.
        /// </summary>
        /// <param name="msg">
        /// Message.
        /// </param>
        public delegate void ReceiveEventHandler(string msg);

        /// <summary>
        /// Receive message.
        /// </summary>
        public event ReceiveEventHandler ReceiveMsg;

        /// <summary>
        /// Initializes a new instance of the <see cref="TCPClientPlugin"/> class.
        /// </summary>
        public TCPClientPlugin()
        {
            _tcpChannel.SendTimeout = 10000;
            _data = new byte[1024];
            _olddata = new byte[8192];
            _dataoffset = 0;
            Encoding = Encoding.UTF8;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TCPClientPlugin"/> class.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <param name="port">
        /// The port.
        /// </param>
        public TCPClientPlugin(string address, int port)
        {

            _remoteMachine = address;
            _remotePort = port;
            _tcpChannel.SendTimeout = 10000;
            _data = new byte[1024];
            _olddata = new byte[8192];
            _dataoffset = 0;
            Encoding = Encoding.UTF8;
        }

        public Encoding Encoding
        {
            get => _encoding;
            set => _encoding = value;
        }

        public bool Active
        {
            get
            {
                return _active;
            }

            set
            {
                if (value != _active)
                {
                    if (value)
                    {
                        Connect();
                    }
                    else
                    {
                        Disconnect();
                    }
                }
            }
        }

        public bool Logging
        {
            get
            {
                return _logging;
            }

            set
            {
                _logging = value;
            }
        }

        /// <summary>
        /// Gets or sets the ip address.
        /// </summary>
        public string RemoteMachine
        {
            get
            {
                return _remoteMachine;
            }

            set
            {
                _remoteMachine = value;
            }
        }

        /// <summary>
        /// Gets or sets the Remote Port
        /// </summary>
        public int RemotePort
        {
            get
            {
                return _remotePort;
            }

            set
            {
                _remotePort = value;
            }
        }

        public void SetCCProtocol(int startcode = -1, int endcode = -1)
        {
            _ccstartcode = startcode;
            _ccendcode = endcode;
        }


        /// <summary>
        /// Use client as receiver.
        /// </summary>
        public bool UseAsReceiver
        {
            get
            {
                return _useAsReceiver;
            }
            set
            {
                _useAsReceiver = value;
            }
        }

        public bool AutoZero
        {
            get => _autoZero;
            set => _autoZero = value;
        }

        /// <summary>
        /// Set protocol use encode.
        /// </summary>
        /// <param name="use">
        /// Use.
        /// </param>
        public void SetProtocol0(bool use)
        {
            _endcode = use ? 0 : -1;
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            _wrunning = false;
            Active = false;

            if (_tcpChannel != null) _tcpChannel.Dispose();
        }

        /// <summary>
        /// Get Alive/Active State
        /// </summary>
        /// <returns>
        /// True or False>.
        /// </returns>
        public bool IsAlive()
        {
            {
                //_tcpChannel.WriteBytes(null, false);
                if (Active && !_tcpChannel.Connected)
                {
                    Disconnect();
                }
            }
            return Active;
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
            return _encoding.GetString(arr);
        }

        /// <summary>
        /// Convert string to byte array.
        /// </summary>
        /// <param name="str">
        /// String.
        /// </param>
        /// <returns>
        /// Result.
        /// </returns>
        private byte[] StringToByteArray(string str)
        {
            return _encoding.GetBytes(str);
        }

        /// <summary>
        /// Send data.
        /// </summary>
        /// <param name="msg">
        /// Data.
        /// </param>
        public void WriteData(string msg)
        {
            if (!_wrunning)
                return;
            byte[] b = StringToByteArray(msg);
            try
            {
                if (_ccstartcode > -1)
                {
                    byte[] code = [(byte)_ccstartcode];
                    _networkStream.Write(code, 0, 1);
                    _networkStream.Write(b, 0, b.Length);
                    code[0] = (byte)_ccendcode;
                    _networkStream.Write(code, 0, 1);
                }
                else _networkStream.Write(b, 0, b.Length);

                if (AutoZero)
                {
                    byte[] code = new byte[] { 0 };
                    _networkStream.Write(code, 0, 1);
                }

                _networkStream.Flush();
            }
            catch (SocketException e)
            {
                if (e.SocketErrorCode == SocketError.ConnectionAborted
                    || e.SocketErrorCode == SocketError.Disconnecting)
                {
                    Dispose();
                    Log.InfoFormat("Client terminated");
                }
            }
            catch (IOException)
            {
                Dispose();
                Log.InfoFormat("Client terminated");
            }
            catch (Exception)
            {
                // ignored
            }
        }

        /// <summary>
        /// Receiver Thread.
        /// </summary>
        private void Worker()
        {
            try
            {
                int count;
                while (_wrunning)
                {
                    if (_networkStream.DataAvailable)
                    {
                        _received = _networkStream.Read(_data, 0, _data.Length);
                        count = 0;
                        if (_endcode == 0)
                        {
                            for (int i = 0; i < _received; i++)
                            {
                                if (_data[i] == 0)
                                {
                                    string s = ByteArrayToString(_olddata);
                                    if (ReceiveMsg != null)
                                        ReceiveMsg(s.Substring(0, _dataoffset + count));
                                    _dataoffset = 0;
                                    count = 0;
                                }
                                else
                                {
                                    _olddata[_dataoffset + count] = _data[i];
                                    count++;
                                }
                            }

                            _dataoffset = count;
                        }
                        else
                        {
                            string s = ByteArrayToString(_data);
                            if (ReceiveMsg != null)
                                ReceiveMsg(s.Substring(0, _received));
                        }
                    }
                    else
                    {
                        Thread.Sleep(20);
                    }
                }

                //_networkStream.Close();
                //_tcpClient.Close();
            }
            catch (ThreadAbortException)
            {
                Log.Info("Client Worker Thread Exit");
            }
            catch (Exception exc)
            {
                Log.ErrorFormat("EXC ClientWorker: {0}",  exc.Message);
                Log.ErrorFormat("ClientWorker Trace:{0}",  exc);
            }
            finally
            {
                _networkStream.Close();
                _tcpChannel.Close();
            }

            Log.InfoFormat("TCP Client exit");
        }

        /// <summary>
        /// Connect.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Connect Error
        /// </exception>
        private void Connect()
        {
            Disconnect();
            if (_remoteMachine == string.Empty)
            {
                Log.Error("No Remote Maschine");
                throw new InvalidOperationException("No Remote Maschine");
            }

            if (_remoteMachine != string.Empty)
            {
                if (_remotePort < 0)
                {
                    Log.Error("Invalid Remote Port");
                    throw new InvalidOperationException("Invalid Remote Port");
                }

                lock (_syncRoot)
                {
                    _tcpChannel = new TcpClient();
                    _tcpChannel.Connect(_remoteMachine,_remotePort);
                    if (_tcpChannel.Connected)
                    {
                        _active = true;
                        _networkStream = _tcpChannel.GetStream();
                        if (!_active)
                        {
                            Disconnect();
                        }
                        else
                        {
                            if (UseAsReceiver)
                            {
                                _wrunning = true;
                                _worker = new Thread(Worker) { IsBackground = true };
                                _worker.Start();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Disconnect.
        /// </summary>
        private void Disconnect()
        {
            lock (_syncRoot)
            {
                _active = false;

                _tcpChannel.Close();
            }
        }
    }
}
