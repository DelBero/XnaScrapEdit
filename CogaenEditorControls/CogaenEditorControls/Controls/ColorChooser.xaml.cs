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
using System.ComponentModel;

namespace CogaenEditorControls.Controls
{
    /// <summary>
    /// Interaction logic for ColorChooser.xaml
    /// </summary>
    public partial class ColorChooser : UserControl, INotifyPropertyChanged
    {
        #region member

        public static readonly DependencyProperty ColorProperty =
        DependencyProperty.Register("Color", typeof(Color),
        typeof(ColorChooser), new FrameworkPropertyMetadata(Color.FromArgb(255,255,255,255), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnColorChanged));

        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set
            {
                SetValue(ColorProperty, value);
                if (DirectUpdate)
                    OnColorChanged();
                //OnPropertyChanged("Color");
            }
        }

        public static readonly DependencyProperty DirectUpdateProperty =
        DependencyProperty.Register("DirectUpdate", typeof(bool),
        typeof(ColorChooser), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnDirectUpdateChanged));

        public bool DirectUpdate
        {
            get { return (bool)GetValue(DirectUpdateProperty); }
            set
            {
                SetValue(DirectUpdateProperty, value);
                //OnPropertyChanged("Color");
            }
        }
        #endregion

        public ColorChooser()
        {
            InitializeComponent();
        }

        #region dependency property callbacks
        private static void OnColorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            if (sender is ColorChooser)
            {
                ColorChooser cc = sender as ColorChooser;
                cc.Color = (Color)eventArgs.NewValue;
            }
        }

        private static void OnDirectUpdateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            if (sender is ColorChooser)
            {
                ColorChooser cc = sender as ColorChooser;
                cc.DirectUpdate = (bool)eventArgs.NewValue;
            }
        }
        #endregion

        #region INotifyPropertyChanged
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


        #region events
        public class ColorChangedEventsArgs : EventArgs
        {
            private Color m_color;

            public Color Color
            {
                get { return m_color; }
            }
            public ColorChangedEventsArgs(Color c)
            {
                m_color = c;
            }
        }

        public delegate void ColorChangedEventHandler(object sender, ColorChangedEventsArgs e);
        public event ColorChangedEventHandler ColorChanged;

        protected void OnColorChanged()
        {
            ColorChangedEventHandler handler = ColorChanged;
            if (handler != null)
            {
                handler(this, new ColorChangedEventsArgs(Color));
            }
        }
        #endregion

        private void DefinerButton_Click(object sender, RoutedEventArgs e)
        {
            ColorDefinerPopup.IsOpen = true;
        }

        private void ColorDefiner_MouseLeave(object sender, MouseEventArgs e)
        {
            ColorDefinerPopup.IsOpen = false;
            OnColorChanged();
        }
    }
}
