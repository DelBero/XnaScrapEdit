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
    /// Interaction logic for ColorDefiner.xaml
    /// </summary>
    public partial class ColorDefiner : UserControl, INotifyPropertyChanged
    {
        #region member
        public static readonly DependencyProperty ColorProperty =
        DependencyProperty.Register("Color", typeof(Color),
        typeof(ColorDefiner), new FrameworkPropertyMetadata(Color.FromArgb(255, 255, 255, 255), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnColorChanged));

        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set
            {
                SetValue(ColorProperty, value);
                //OnPropertyChanged("Color");
            }
        }
        #endregion

        public ColorDefiner()
        {
            InitializeComponent();
        }

        #region dependency property callbacks
        private static void OnColorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            if (sender is ColorDefiner)
            {
                ColorDefiner cc = sender as ColorDefiner;
                cc.Color = (Color)eventArgs.NewValue;
            }
        }
        #endregion

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider s = sender as Slider;
            if (s == SliderA)
            {
                Color oldColor = Color;
                Color = Color.FromArgb((byte)s.Value
                                    , oldColor.R
                                    , oldColor.G
                                    , oldColor.B);
            }
            else if (s == SliderR)
            {
                Color oldColor = Color;
                Color = Color.FromArgb(oldColor.A
                                    , (byte)s.Value
                                    , oldColor.G
                                    , oldColor.B);
            }
            else if (s == SliderG)
            {
                Color oldColor = Color;
                Color = Color.FromArgb(oldColor.A
                                    , oldColor.R
                                    ,(byte)s.Value
                                    , oldColor.B);
            }
            else if (s == SliderB)
            {
                Color oldColor = Color;
                Color = Color.FromArgb(oldColor.A
                                    , oldColor.R
                                    , oldColor.G
                                    , (byte)s.Value);
            }
        }

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
    }
}
