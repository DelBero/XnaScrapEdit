using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CogaenDataItems.Manager.Interfacese;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using CBeroEdit.Communication;
using CogaenEditorConnect.Communication;
using System.ComponentModel;
using CogaenEditorControls.Controls;

namespace CBeroEdit
{
    /// <summary>
    /// Interaction logic for RenderManagerEditor.xaml
    /// </summary>
    [Guid(EditorGuids.CBeroEditorGuidString)]
    public partial class RenderManagerEditor : UserControl, IEditorGui, INotifyPropertyChanged
    {
        #region member
        Connection m_connection;
        CBeroEditMessageHandler m_messageHandler;
        private string m_serviceName;

        #region
        private Color m_backgroundColor = Color.FromRgb(255,255,255);
        public Color BackgroundColor
        {
            get { return m_backgroundColor; }
            set
            {
                m_backgroundColor = value;
                OnPropertyChanged("BackgroundColor");
            }
        }
        public string BackgroundColorString
        {
            get { return printColor(m_backgroundColor); }
            set
            {
                BackgroundColor = parseColor(value);
            }
        }

        private Color m_ambientColor = Color.FromRgb(255, 255, 255);
        public Color AmbientColor
        {
            get { return m_ambientColor; }
            set
            {
                m_ambientColor = value;
                OnPropertyChanged("AmbientColor");
            }
        }
        public string AmbientColorString
        {
            get { return printColor(m_ambientColor); }
            set
            {
                AmbientColor = parseColor(value);
            }
        }

        #endregion

        public Guid GetGuid()
        {
            return EditorGuids.CBeroGuidEditor;
        }
        #endregion

        #region CDtors
        public RenderManagerEditor()
        {
            InitializeComponent();
            DataContext = this;
        }

        //public RenderManagerEditor(string serviceName, Connection connection)
        //{
        //    m_connection = connection;
        //    m_serviceName = serviceName;
        //    InitializeComponent();
        //    DataContext = this;
        //    updateAllData();
        //}
        #endregion

        #region IEditorGui
        public void Reinit(string serviceName, object connection)
        {
            m_serviceName = serviceName;
            m_connection = connection as Connection;
            updateAllData();
        }

        public UserControl GetControl()
        {
            return this;
        }
        #endregion

        private void updateAllData()
        {
            checkConnection();
            //if (m_connection.Connected)
            {
                m_messageHandler.getBackgroundColor();
            }
        }

        private void checkConnection()
        {
            if (m_messageHandler == null)
            {
                m_messageHandler = new CBeroEditMessageHandler(this.Dispatcher, m_serviceName, this);
                m_messageHandler.Connection = Connection.Clone(m_connection, m_messageHandler);
            }
            if (!m_messageHandler.Connection.Connected)
            {
                m_messageHandler.Connection = Connection.Clone(m_connection, m_messageHandler);
            }
            
        }

        private Color parseColor(string data)
        {
            data = data.Trim(new char[] { '{', '}' });
            string[] values = data.Split(' ');
            if (values.Length < 3 || values.Length > 4)
                return Color.FromRgb(255,0,0);

            byte[] iValues = new byte[values.Length];
            bool ok = true;
            for (int i = 0; i < values.Length; ++i)
                ok |= byte.TryParse(values[i].Substring(2), out iValues[i]);

            if (values.Length == 3)
                return Color.FromRgb(iValues[0], iValues[1], iValues[2]);
            else if (values.Length == 4)
                return Color.FromArgb(iValues[3], iValues[0], iValues[1], iValues[2]);
            else
                return Color.FromRgb(255, 0, 0);
        }

        private string printColor(Color color)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{R:");
            sb.Append(color.R.ToString());
            sb.Append(" G:");
            sb.Append(color.G.ToString());
            sb.Append(" B:");
            sb.Append(color.B.ToString());
            sb.Append(" A:");
            sb.Append(color.A.ToString());
            sb.Append("}");

            return sb.ToString();
        }

        #region property changed
        // Declare the event
        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion

        private void colorChooser1_ColorChanged(object sender, ColorChooser.ColorChangedEventsArgs e)
        {
            m_messageHandler.setBackgroundColor();
        }

        private void colorChooser2_ColorChanged(object sender, ColorChooser.ColorChangedEventsArgs e)
        {
            m_messageHandler.setAmbientColor();
        }
    }

    public static class EditorGuids
    {
        [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
        public const string CBeroEditorGuidString = "D3F71433-1503-42C7-B838-D1FFD6F4C27F";
        public static Guid CBeroGuidEditor = new Guid(CBeroEditorGuidString);
    }
}
