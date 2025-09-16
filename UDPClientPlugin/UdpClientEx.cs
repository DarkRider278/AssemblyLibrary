using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using log4net;

namespace UDPClientPlugin
{
    public class UdpClientEx
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
        private Encoding _encoder= Encoding.ASCII;


        private UdpClient _listener;

        private IPEndPoint _msgEP;


        /// <summary>
        /// Initializes a new instance of the <see cref="UdpClientEx"/> class.
        /// </summary>
        /// <param name="port">
        /// Udp port.
        /// </param>
        public UdpClientEx(string adress,int port)
        {
            if (port > 0) _port = port;
            _ipAdress = adress;
            _data = new byte[1024];
            _wrunning = true;
            _msgEP = new IPEndPoint(IPAddress.Parse(_ipAdress), _port);
            _listener = new UdpClient();
            _listener.Connect(_ipAdress,_port);
            _worker = new Thread(Worker) { IsBackground = true };
            _worker.Start();
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
        public delegate void ReceiveEventHandler(object sender, string msg, string clientadress);

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
                    if (_listener.Available > 0)
                    {
                        byte[] data = _listener.Receive(ref _msgEP);
                        string s = ByteArrayToString(_data);
                        ReceiveMsg(this, s.Substring(0, data.Length), _msgEP.Address.ToString());

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

        public void WriteData(string msg)
        {
            if (!_wrunning)
                return;
            byte[] b = StringToByteArray(msg);
            
            _listener.Send(b, b.Length);

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
