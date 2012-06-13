using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace CogaenEditorConnect.Communication
{
    public delegate void Callback(String s, byte type, uint id, object data);
    
    public class CMessage
    {
        public static CMessage Halt = new CMessage();
        public static byte Done = byte.MaxValue;
        public static byte Connect = byte.MaxValue - 1;

        private String msg;

        public String Msg
        {
            get { return msg; }
            set { msg = value; }
        }

        private byte m_type;

        public byte Type
        {
            get { return m_type; }
            set { m_type = value; }
        }

        private Callback m_callback;

        public Callback Callback
        {
            get { return m_callback; }
            set { m_callback = value; }
        }
        private object m_data;

        public object Data
        {
            get { return m_data; }
            set { m_data = value; }
        }

        private uint m_id;

        public uint Id
        {
            get { return m_id; }
            set { m_id = value; }
        }

        #region CDtors
        public CMessage() { }

        public CMessage(String s, Callback callback)
        {
            msg = s;
            m_callback = callback;
        }

        public CMessage(String s, byte type, uint id, Callback callback, object data)
        {
            msg = s;
            m_type = type;
            m_id = id;
            m_callback = callback;
            m_data = data;
        }
        #endregion

        public virtual void execute(Dispatcher dispatcher)
        {
            if (dispatcher == null)
                throw new ArgumentNullException("I need a Dispatcher");
            else
                dispatcher.Invoke(m_callback, DispatcherPriority.DataBind, new object[] { msg, m_type,m_id, m_data });
            //App.Current.Dispatcher.Invoke((Callback)delegate
            //{
            //    m_callback(msg, m_data);
            //}, null);
            //App.Current.Dispatcher.Invoke(m_callback, DispatcherPriority.DataBind, new object[] { msg, m_type, m_data });
        }
    }
}
