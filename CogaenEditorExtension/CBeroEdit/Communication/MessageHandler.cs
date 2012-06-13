using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using System.Windows;
//using CogaenDataItems.DataItems;
using CogaenEditorConnect.Communication;
using System.Windows.Threading;
using CogaenDataItems.DataItems;
using System.Windows.Forms;
using CogaenEditConnect.Communication.Default;

namespace CBeroEdit.Communication
{
    public partial class CBeroEditMessageHandler: MessageHandler
    {
        #region member
        string m_serviceName;
        RenderManagerEditor m_editor;
        #endregion
        #region CDtors
        public CBeroEditMessageHandler(Dispatcher dispatcher, string serviceName, RenderManagerEditor editor)
            : base(dispatcher)
        {
            m_serviceName = serviceName;
            m_editor = editor;
        }
        #endregion
    }
}
