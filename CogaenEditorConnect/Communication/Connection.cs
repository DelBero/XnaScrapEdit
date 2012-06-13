using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Xml;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace CogaenEditorConnect.Communication
{
    public class Connection : INotifyPropertyChanged, IDisposable
    {
        public enum CommandType
        {
            C_QUERY = 8,
            C_ACTION = 9,
            // REST
            C_GET = 12,
            C_PUT = 13,
            C_POST = 14,
            C_DELETE = 15
        }

        public class Header
        {
            //public char id;
            public uint type;
            public uint length;

            /// <summary>
            /// Write the header to a byte array.
            /// </summary>
            /// <param name="buffer"></param>
            /// <returns>The size of the Header</returns>
            public int writeTo(byte[] buffer)
            {
                int size = Size();
                byte[] h1 = System.BitConverter.GetBytes(type);
                byte[] h2 = System.BitConverter.GetBytes(length);

                Buffer.BlockCopy(h1, 0, buffer, 0, sizeof(uint));
                Buffer.BlockCopy(h2, 0, buffer, sizeof(uint), sizeof(uint));

                return size;
            }

            public int Size()
            {
                return sizeof(uint) * 2;
            }
        };

        #region static member
        private static uint m_IdCounter = 0;

        public static uint IdCounter
        {
            get { return Connection.m_IdCounter++; }
        }
        #endregion

        #region members
        private bool m_connected;
        private TcpClient m_client = null;
        private Thread m_thread = null;
        private Queue<CMessage> m_messageQueue = new Queue<CMessage>();
        private Mutex m_queueMutex = new Mutex();
        private Semaphore m_queueSema = new Semaphore(0, int.MaxValue);
        private Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

        private IMessageHandler m_messageHandler;

        public TcpClient Client
        {
            get { return m_client; }
            set 
            {
                m_client = value;
                // start thread
                if (m_client != null)
                {
                    if (m_thread == null)
                    {
                        ThreadStart start = new ThreadStart(run);
                        m_thread = new Thread(start);
                        m_thread.Name = "ConnectionThread";
                        m_thread.Start();
                    }
                }
                else
                {
                    Dispose();
                }
                OnPropertyChanged("Client");
            }
        }

        public bool Connected
        {
            get { return m_connected; }
            set
            {
                m_connected = value;
                OnPropertyChanged("Connected");
                OnPropertyChanged("CanConnect");
                OnPropertyChanged("ConnectedString");
            }
        }

        public bool CanConnect
        {
            get { return !m_connected; }
        }

        public String ConnectedString
        {
            get
            {
                if (m_connected)
                {
                    return "Visible";
                }
                else
                {
                    return "Hidden";
                }
            }
            set
            {
            }
        }

        #endregion

        #region delegates
        public delegate void StartDownload();
        public delegate void Download(int done, int total);
        public delegate void EndDownload();
        #endregion

        #region CDtors
        public Connection(IMessageHandler msgHandler)
        {
            m_messageHandler = msgHandler;
        }

        public void Dispose()
        {
            m_queueMutex.WaitOne();
            m_messageQueue.Enqueue(CMessage.Halt);
            m_queueSema.Release(1);
            m_queueMutex.ReleaseMutex();
        }
        #endregion

        public static Connection Clone(Connection rhs, IMessageHandler messageHandler)
        {
            Connection newConnection = new Connection(messageHandler);
            newConnection.Client = rhs.Client;
            newConnection.Connected = rhs.Connected;

            return newConnection;
        }

        private String decodeRestMsg(String restMsg)
        {
            restMsg = restMsg.Trim();
            // 8th line is the content
            int i = 0;
            int index = 0;
            while (i < 7 && index < restMsg.Length)
            {
                char c = restMsg[index++];
                if (c == '\n')
                {
                    ++i;
                }
            }
            if (i < 7)
            {
                // TODO error handling
                return restMsg;
            }
            // wrap with xml root
            //return "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" + restMsg.Substring(index);
            return restMsg.Substring(index);
        }

        /// <summary>
        /// Reads a REST header and returns the length
        /// </summary>
        /// <param name="msg"></param>
        /// <returns>Content Length</returns>
        private int getDataLength(String msg)
        {
            msg = msg.Trim();
            int i = 0;
            while (i < 2)
            {
                int index = msg.IndexOf("\n");
                if (index > 0)
                {
                    msg = msg.Substring(index + 1);
                    ++i;
                }
                else
                {
                    i = 2;
                }
            }
            if (i == 2)
            {
                int lenIndex = msg.IndexOf(":");
                int endIndex = msg.IndexOf("\n");
                if (lenIndex > 0)
                {
                    String length = msg.Substring(lenIndex + 1, endIndex - lenIndex - 1);
                    return int.Parse(length);
                }
            }
            return 0;
        }

        /// <summary>
        /// process a message in REST format
        /// </summary>
        /// <param name="s"></param>
        /// <param name="type"></param>
        /// <param name="callback"></param>
        /// <param name="data"></param>
        private void send_internREST(String s, byte type, uint id, Callback callback, object data)
        {
            if (Client == null || !Client.Connected)
                return;
            try
            {
                s = s + "\nAccept: text/xml\n";
                NetworkStream nws = Client.GetStream();

                // REST cannot use Header
                //Header header = new Header();
                //header.type = type;
                //header.length = (uint)s.Length;
                //int size = header.Size();            
                byte[] buffer = new byte[s.Length];
                //header.writeTo(buffer);

                for (int i = 0; i < +s.Length; ++i)
                {
                    buffer[i] = (byte)s[i];
                }

                if (nws.CanWrite)
                    Client.Client.Send(buffer, s.Length, SocketFlags.None);
                else
                    throw new SocketException();
                // wait for response
                byte[] response = new byte[1024];
                StringBuilder asw = new StringBuilder();
                int recvd;
                int totalRecvd = 0;
                int totalLength = 0;

                // type
                int startIndex = 1;
                byte returnType = 0;
                bool first = true;

                //StartDownload sd = (App.Current as App).MessageHandler.startDownload;
                //Download dl = (App.Current as App).MessageHandler.downloading;
                //EndDownload ed = (App.Current as App).MessageHandler.downloadDone;

                //App.Current.Dispatcher.Invoke(sd, DispatcherPriority.DataBind);


                do
                {
                    recvd = Client.Client.Receive(response);


                    if (first)
                    {
                        returnType = response[0];
                        first = false;
                    }
                    else
                    {
                        startIndex = 0;
                    }
                    for (int i = startIndex; i < recvd; ++i)
                    {
                        asw.Append((char)response[i]);
                    }
                    // get total length
                    if (totalRecvd == 0)
                    {
                        totalLength = getDataLength(asw.ToString());
                    }
                    totalRecvd += recvd;

                    //App.Current.Dispatcher.Invoke(dl, DispatcherPriority.DataBind, new object[] { totalRecvd, totalLength });
                }
                while (Client.Available > 0);

                //App.Current.Dispatcher.Invoke(ed, DispatcherPriority.DataBind);


                String answer = decodeRestMsg(asw.ToString());

                ConnectionLog.LogAnswer(id, answer);

                if (callback != null)
                {
                    m_messageHandler.answer(answer, returnType, id, callback, data);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Connection Error", MessageBoxButtons.OK);
                //Client.Close();
                m_messageHandler.disconnect();
            }

        }

        /// <summary>
        /// Connect to a client
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        private bool connect_intern(String ip, String port, Callback callback)
        {
            if (Client == null)
            {
                Client = new TcpClient();
            }
            try
            {
                int p = Int32.Parse(port);
                Client.Connect(IPAddress.Parse(ip), p);
                Connected = true;
                if (callback != null)
                {
                    m_messageHandler.answer("", 0,0, callback, true);
                }
            }
            catch (System.Net.Sockets.SocketException sockEx)
            {
                Client = null;
                Connected = false;
                if (callback != null)
                {
                    m_messageHandler.answer(sockEx.Message, 0,0, callback, false);
                }
            }
            return Connected;
        }

        /// <summary>
        /// Dispatch a message
        /// </summary>
        /// <param name="msg"></param>
        private void dispatch(CMessage msg)
        {
            if (msg.Type == CMessage.Done)
            {
                return;
            }
            else if (msg.Type == CMessage.Connect)
            {
                String[] data = msg.Msg.Split(new char[] { ',' });
                if (data.Length != 2)
                {
                    throw new Exception("Error, Invalid connection information");
                }
                else
                {
                    connect_intern(data[0], data[1], msg.Callback);
                }
            }
            else
            {

                send_internREST(msg.Msg, msg.Type, msg.Id, msg.Callback, msg.Data);
            }
        }

        /// <summary>
        /// This pops messages from the queue (blocking)
        /// </summary>
        private void run()
        {
            CMessage nextMsg;
            do
            {
                // wait until msgs arrive
                m_queueSema.WaitOne();
                m_queueMutex.WaitOne();
                nextMsg = m_messageQueue.Dequeue();
                m_queueMutex.ReleaseMutex();

                //send_intern(nextMsg.Msg, nextMsg.Type, nextMsg.Callback);
                dispatch(nextMsg);
            } while (nextMsg.Type != CMessage.Done);
            System.Console.WriteLine("Connection done!");
        }

        /// <summary>
        /// Entrypoint for using the ActiveObject
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns>The Id of the message</returns>
        public uint send(String s, byte t, Callback callback, object data)
        {
            if (!Connected)
                return 0;
            m_queueMutex.WaitOne();
            uint id = Connection.IdCounter;

            ConnectionLog.LogMessage(id, s);

            m_messageQueue.Enqueue(new CMessage(s, t, id, callback, data));
            m_queueSema.Release(1);
            m_queueMutex.ReleaseMutex();
            return id;
        }

        /// <summary>
        /// Connect to a client
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public void connect(String ip, String port, Callback callback)
        {
            Client = new TcpClient();
            m_queueMutex.WaitOne();
            m_messageQueue.Enqueue(new CMessage(ip+","+port, CMessage.Connect, 0 , callback, null));
            m_queueSema.Release(1);
            m_queueMutex.ReleaseMutex();
        }

        public void disconnect()
        {
            if (Client != null)
            {
                Client.Close();
                Client = null;
            }
            Connected = false;
        }

        #region notify
        // Declare the event
        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                if (dispatcher.Thread == Thread.CurrentThread)
                {
                    handler(this, new PropertyChangedEventArgs(name));
                }
                else
                {
                    dispatcher.BeginInvoke(DispatcherPriority.DataBind,
                               (ThreadStart)delegate()
                               {
                                   handler(this, new PropertyChangedEventArgs(name));
                               });
                }
            }

        }
        #endregion


        
    }
}
