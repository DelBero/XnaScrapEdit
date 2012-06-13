using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using CogaenEditorConnect.Communication;

namespace CogaenEditor2.Communication
{
    public class CMessageEditor : CMessage
    {
        #region CDtors
        public CMessageEditor() { }

        public CMessageEditor(String s, Callback callback)
            : base(s, callback)
        {
        }

        public CMessageEditor(String s, byte type, Callback callback, object data)
            : base(s, type, callback, data)
        {
        }
        #endregion

        public override void execute()
        {
            //App.Current.Dispatcher.Invoke((Callback)delegate
            //{
            //    m_callback(msg, m_data);
            //}, null);
            App.Current.Dispatcher.Invoke(Callback, DispatcherPriority.DataBind, new object[] { Msg, Type, Data });
        }
    }
}
