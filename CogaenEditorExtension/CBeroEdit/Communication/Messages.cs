using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Windows;
using CogaenDataItems.DataItems;
using CogaenDataItems.Exporter;
using CogaenDataItems.Manager;
using CogaenEditorConnect.Communication;
using CogaenEditConnect.Communication.Default;
using System.Windows.Media;

namespace CBeroEdit.Communication
{

    public partial class CBeroEditMessageHandler : MessageHandler
    {
        #region BackgroundColor
        public void getBackgroundColor()
        {
            send("GET /Services/" + m_serviceName + "/BackgroundColor/ HTTP/1.1", (byte)Connection.CommandType.C_GET, backgroundColorCallback, null);
        }

        public void setBackgroundColor()
        {
            send("POST /Services/" + m_serviceName + "/BackgroundColor/ " + m_editor.BackgroundColorString + " HTTP/1.1", (byte)Connection.CommandType.C_POST, null);
        }
        #endregion

        #region AmbientColor
        public void getAmbientColor()
        {
            send("GET /Services/" + m_serviceName + "/AmbientColor/ HTTP/1.1", (byte)Connection.CommandType.C_GET, ambientColorCallback, null);
        }

        public void setAmbientColor()
        {
            send("POST /Services/" + m_serviceName + "/AmbientColor/ " + m_editor.AmbientColorString + " HTTP/1.1", (byte)Connection.CommandType.C_POST, null);
        }
        #endregion
    }
}
