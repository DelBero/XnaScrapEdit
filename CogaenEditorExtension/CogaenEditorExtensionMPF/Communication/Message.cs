using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using CogaenEditorConnect.Communication;

namespace CogaenEditExtension.Communication
{
    public class CMessageEditor : CMessage
    {
        #region CDtors
        public CMessageEditor() { }

        public CMessageEditor(String s, Callback callback)
            : base(s, callback)
        {
        }

        public CMessageEditor(String s, byte type, uint id, Callback callback, object data)
            : base(s, type, id, callback, data)
        {
        }
        #endregion

        public override void execute(Dispatcher dispatcher)
        {
            //App.Current.Dispatcher.Invoke((Callback)delegate
            //{
            //    m_callback(msg, m_data);
            //}, null);
            //dispatcher.Invoke(Callback, DispatcherPriority.DataBind, new object[] { Msg, Type, Data });
        }
    }
}
