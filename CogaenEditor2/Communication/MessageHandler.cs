using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using System.Windows;
using CogaenDataItems.DataItems;
using CogaenEditorConnect.Communication;

namespace CogaenEditor2.Communication
{
    public partial class MessageHandler: IDisposable, CogaenEditorConnect.Communication.IMessageHandler
    {
        #region member
        private App m_app;

        public App App
        {
            get { return m_app; }
        }
        private Connection m_connection;

        public Connection Connection
        {
            get { return m_connection; }
            set { m_connection = value; }
        }

        CogaenData m_data;

        public CogaenData Data
        {
            get { return m_data; }
            set { m_data = value; }
        }

        #region threading
        private Thread m_mainLoopThread;
        private Queue<CMessage> m_messageQueue = new Queue<CMessage>();
        private Mutex m_queueMutex = new Mutex();
        private Semaphore m_queueSema = new Semaphore(0, int.MaxValue);
        private bool m_running = true;
        public bool Running
        {
            get { return m_running; }
            set { m_running = value; }
        }
        #endregion
        #endregion

        #region CDtors
        public MessageHandler(App app, CogaenData data)
        {
            m_app = app;
            m_data = data;

            ThreadStart start = new ThreadStart(run);
            m_mainLoopThread = new Thread(start);
            m_mainLoopThread.Name = "MessageHandlerThread";
            m_mainLoopThread.Start();
        }
        #endregion

        #region threading
        private void run()
        {
            while (m_running)
            {
                m_queueSema.WaitOne();
                m_queueMutex.WaitOne();
                CMessage msg = m_messageQueue.Dequeue();
                if (msg == CMessage.Halt)
                {
                    m_running = false;
                }
                else
                {
                    msg.execute();
                }
                m_queueMutex.ReleaseMutex();
            }
            System.Console.WriteLine("MessageHandler done!");
        }

        public void stop()
        {
            m_queueMutex.WaitOne();
            m_messageQueue.Enqueue(CMessage.Halt);
            m_queueSema.Release(1);
            m_queueMutex.ReleaseMutex();
        }
        #endregion

        #region communication
        /// <summary>
        /// Send a command to the engine
        /// </summary>
        /// <param name="s">The command</param>
        /// <param name="type">The type of the command</param>
        /// <param name="callback">The callback function to invoke on completion</param>
        /// <param name="data">Optional data</param>
        public void send(String s, byte type, Callback callback, object data = null)
        {
            Connection.send(s, type, callback, data);
        }

        /// <summary>
        /// Process the answer received from the engine
        /// </summary>
        /// <param name="answer">The String containing the answer</param>
        /// <param name="type">The type of the answer (unused)</param>
        /// <param name="callback">The delegate to call</param>
        /// <param name="data">CogaenEdit data that is linked to this answer (e.g. a gameobject)</param>
        public void answer(String answer, byte type, Callback callback, object data)
        {
            m_queueMutex.WaitOne();
            m_messageQueue.Enqueue(new CMessage(answer, type, callback, data));
            m_queueSema.Release(1);
            m_queueMutex.ReleaseMutex();
        }
        #endregion

        #region helper
        private XmlNodeList getRestData(String xml)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                //XmlNodeList body = doc.GetElementsByTagName("Body");
                //// first one is the name of the list
                //XmlNode node = body.Item(0);
                XmlNode node = doc.FirstChild;
                if (node == null)
                {
                    return null;
                }
                return node.ChildNodes;
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK);
                return null;
                //throw;
            }
        }

        private AbstractResource getResourceTypeFromName(String type, String name)
        {
            if (type == "Mesh")
            {
                return new MeshResource(name);
            }
            else if (type == "Texture")
            {
                return new TextureResource(name);
            }
            else if (type == "Script")
            {
                return new ScriptResource(name);
            }
            else if (type == "Macro")
            {
                return new MacroResource(name);
            }
            else if (type == "Material")
            {
                return new MaterialResource(name);
            }
            else
            {
                return new ResourceFolder(name);
            }
        }
        #endregion

        #region connection
        public void connect(String ip, String port)
        {
            Connection.connect(ip, port, connectCallback);
        }

        public void disconnect()
        {
            Connection.disconnect();
            //(App.Current as App).Data.Clear();
        }
        #endregion

        #region Statusbar
        public void startDownload()
        {
            //if (m_informDownload)
            //{
            //    m_downloadCounter = 0;
            //    StatusText = "Loading";
            //    (MainWindow as MainWindow).m_statusProgressbar.IsEnabled = true;
            //}
        }

        public void downloading(int done, int total)
        {
            //if (m_informDownload)
            //{
            //    ++m_downloadCounter;
            //    if (m_downloadCounter > 3)
            //    {
            //        m_downloadCounter = 0;
            //    }
            //    StatusText = "Loading";
            //    for (int i = 0; i < m_downloadCounter; ++i)
            //    {
            //        StatusText += ".";
            //    }

            //    // progressbar
            //    StatusValue = done;
            //    StatusMaxValue = total;
            //}
        }

        public void downloadDone()
        {
            //if (m_informDownload)
            //{
            //    StatusText = "Ok";
            //    StatusValue = 0;
            //    StatusMaxValue = 100;
            //    (MainWindow as MainWindow).m_statusProgressbar.IsEnabled = false;
            //}
        }
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            m_running = false;
        }

        #endregion
    }
}
