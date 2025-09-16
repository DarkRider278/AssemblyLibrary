// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TcpClientData.cs" company="">
//   2020
// </copyright>
// <summary>
//   Defines the TcpClientData type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using log4net;

namespace TcpServerPlugin
{
    /// <summary>
    /// The tcp client data class.
    /// </summary>
    public class TcpClientData : IDisposable
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
        /// The _id.
        /// </summary>
        private readonly string _id;

        private string _name;

        /// <summary>
        /// The _master.
        /// </summary>
        private readonly TcpServerMulti _master;

        /// <summary>
        /// The _olddata.
        /// </summary>
        private readonly byte[] _olddata;

        /// <summary>
        /// The _tcp client.
        /// </summary>
        private readonly TcpClient _tcpClient;

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
        /// The _hb.
        /// </summary>
        private Thread _hb;

        /// <summary>
        /// The _last time.
        /// </summary>
        private DateTime _lastTime;

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

        private bool _isTimeout = false;

        private Encoding _encoding;

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpClientData"/> class.
        /// </summary>
        /// <param name="master">
        /// The master handle.
        /// </param>
        /// <param name="id">
        /// ID of the client.
        /// </param>
        /// <param name="client">
        /// The socket.
        /// </param>
        public TcpClientData(TcpServerMulti master, string id, TcpClient client)
        {
            _data = new byte[1024];
            _olddata = new byte[8192];
            _master = master;
            _id = id;
            _tcpClient = client;
            _dataoffset = 0;
            _name = "";
            Encoding = _master.Encoding;
        }

        /// <summary>
        /// Gets the client id.
        /// </summary>
        public string ID
        {
            get
            {
                return _id;
            }
        }

        public string IDName
        {
            get
            {
                return _id + (_name!="" ? " - "+_name:"");
            }
        }

        public bool IsTimeout
        {
            get
            {
                return this._isTimeout;
            }
        }

        public Encoding Encoding
        {
            get => _encoding;
            set => _encoding = value;
        }

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            _wrunning = false;
            //_worker.Abort();
            /*_networkStream.Close();
            _tcpClient.Client.Shutdown(SocketShutdown.Both);
            _tcpClient.Close();*/
            _master.TerminateClient(this);
        }

        /// <summary>
        /// Set CC protocol.
        /// </summary>
        /// <param name="startcode">
        /// Startcode.
        /// </param>
        /// <param name="endcode">
        /// Endcode.
        /// </param>
        public void SetCCProtocol(int startcode = -1, int endcode = -1)
        {
            _ccstartcode = startcode;
            _ccendcode = endcode;
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
        /// Start client receiving.
        /// </summary>
        public void StartClient()
        {
            _networkStream = _tcpClient.GetStream();
            _networkStream.ReadTimeout = 500;
            _wrunning = true;
            _worker = new Thread(Worker) { IsBackground = true };
            _worker.Start();
            if (_master.InactiveTime > 0)
            {
                _lastTime = DateTime.Now;
                _hb = new Thread(HB) { IsBackground = true };
                _hb.Start();
            }
        }

        /// <summary>
        /// Stop client receiving.
        /// </summary>
        public void StopClient()
        {
            Dispose();
            Thread.Sleep(40);
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
                    byte[] code = new byte[] { (byte)_ccstartcode };
                    _networkStream.Write(code, 0, 1);
                    _networkStream.Write(b, 0, b.Length);
                    code[0] = (byte)_ccendcode;
                    _networkStream.Write(code, 0, 1);
                }
                else _networkStream.Write(b, 0, b.Length);

                _networkStream.Flush();
            }
            catch (SocketException e)
            {
                if (e.SocketErrorCode == SocketError.ConnectionAborted
                    || e.SocketErrorCode == SocketError.Disconnecting)
                {
                    StopClient();
                    Log.InfoFormat("Client({0}) terminated", _id);
                }
            }
            catch (IOException)
            {
                StopClient();
                Log.InfoFormat("Client({0}) terminated", _id);
            }
            catch (Exception)
            {
                // ignored
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
            /*UTF8Encoding enc = new UTF8Encoding();
            return enc.GetString(arr);*/
            //return _master.Encoding.GetString(arr);
            return _encoding.GetString(arr);
        }

        /// <summary>
        /// The heartbeat.
        /// </summary>
        private void HB()
        {
            try
            {
                while (_wrunning)
                {
                    Thread.Sleep(1000);
                    TimeSpan d = DateTime.Now.Subtract(_lastTime);
                    if (d.TotalSeconds > (_master.InactiveTime + 1))
                    {
                        Log.WarnFormat("Client({0}) terminated Timeout", _id);
                        _isTimeout = true;
                        StopClient();
                        break;
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        /// <summary>
        /// Convert string to byte array.
        /// </summary>
        /// <param name="data">
        /// String data.
        /// </param>
        /// <returns>
        /// Result.
        /// </returns>
        private byte[] StringToByteArray(string data)
        {
            /*UTF8Encoding enc = new UTF8Encoding();
            return enc.GetBytes(data);*/
            //return _master.Encoding.GetBytes(data);
            return _encoding.GetBytes(data);
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
                                    _master.ReceiveClient(this, s.Substring(0, _dataoffset + count));
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
                            _master.ReceiveClient(this, s.Substring(0, _received));
                        }

                        _lastTime = DateTime.Now;
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
                Log.ErrorFormat("EXC ClientWorker({0}): {1}", _id, exc.Message);
                Log.ErrorFormat("ClientWorker({0}) Trace:{1}", _id, exc.ToString());
            }
            finally
            {
                _networkStream.Close();
                _tcpClient.Close();
            }

            Log.InfoFormat("Client({0}) exit", _id);
        }
    }
}