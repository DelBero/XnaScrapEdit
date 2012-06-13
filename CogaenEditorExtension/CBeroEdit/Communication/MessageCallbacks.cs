using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Windows;
using CogaenDataItems.DataItems;
using System.Xml;
using CogaenDataItems.Helper;
using System.Collections.ObjectModel;
using CogaenDataItems.Manager;
//using System.Windows.Forms;
//using CogaenEditor2.Helper;
using CogaenEditorConnect.Communication;
using System.Windows.Forms;
using CogaenEditConnect.Communication.Default;
using System.Windows.Media;

namespace CBeroEdit.Communication
{
    public partial class CBeroEditMessageHandler : MessageHandler
    {
        public void backgroundColorCallback(String s, byte type, uint id, object data)
        {
            XmlNodeList color = getRestData(s, id);
            if (color != null)
            {
                foreach (XmlNode node in color)
                {
                    foreach (XmlAttribute attrib in node.Attributes)
                    {
                        if (attrib.Name == "Value")
                        {
                            m_editor.BackgroundColorString = attrib.Value;
                        }
                    }
                }
            }
        }

        public void ambientColorCallback(String s, byte type, uint id, object data)
        {
            XmlNodeList color = getRestData(s, id);
            if (color != null)
            {
                foreach (XmlNode node in color)
                {
                    foreach (XmlAttribute attrib in node.Attributes)
                    {
                        if (attrib.Name == "Value")
                        {
                            m_editor.AmbientColorString = attrib.Value;
                        }
                    }
                }
            }
        }
    }
}
