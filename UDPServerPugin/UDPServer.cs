// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UdpServerPlugin.cs" company="">
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
using log4net;

namespace UDPServerPugin
{
    public class UdpServer : IDisposable
    {
        /// <summary>
        /// Log4Net object.
        /// </summary>
        public static ILog Log = LogManager.GetLogger("MAIN");

        /// <summary>
        /// The _data.
        /// </summary>
        private static byte[] _data;

        /// <summary>
        /// The _received.
        /// </summary>
        private static int _received;


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
        /// The _encoder.
        /// </summary>
        private Encoding _encoder = Encoding.ASCII;


        private UdpClient _listener;

        private IPEndPoint _msgEP;


        /// <summary>
        /// Initializes a new instance of the <see cref="UdpServer"/> class.
        /// </summary>
        /// <param name="port">
        /// Udp port.
        /// </param>
        public UdpServer(int port)
        {
            if (port > 0) _port = port;
            _data = new byte[1024];
            _wrunning = true;
            _msgEP = new IPEndPoint(IPAddress.Any, _port);
            _listener = new UdpClient(_msgEP);
            
            _worker = new Thread(Worker) { IsBackground = true };
            _worker.Start();
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="UdpServer"/> class with default port 9050.
        /// </summary>
        public UdpServer()
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
        public delegate void ReceiveEventHandler(object sender, string msg,string clientadress);

        /// <summary>
        /// Receive message.
        /// </summary>
        public event ReceiveEventHandler ReceiveMsg;


        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            _worker.Abort();
            _listener.Close();
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
                    if (_listener.Available>0)
                    {
                        byte[] data = _listener.Receive(ref _msgEP);
                        string s = ByteArrayToString(data);
                        ReceiveMsg(this, s.Substring(0, data.Length),_msgEP.Address.ToString());

                    }
                    else
                    {
                        Thread.Sleep(500);
                    }
                }

                _listener.Close();
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

        public void WriteData(string msg,string adress="")
        {
            if (!_wrunning)
                return;
            byte[] b = StringToByteArray(msg);
            if (adress == "")
            {
                _listener.Send(b, b.Length);
            }
            else
            {
                _listener.Send(b, b.Length, adress, _port);
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
            return _encoder.GetBytes(data);
        }
    }
}
