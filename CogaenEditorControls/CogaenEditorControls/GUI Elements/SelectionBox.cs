using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CogaenEditorControls.GUI_Elements
{
    public class SelectionBox
    {
        private Rectangle m_selectionBox = new Rectangle();
        private Point m_start;
        private Point m_TopLeft;
        private Canvas m_canvas;
        private bool m_selecting = false;

        public bool Selecting
        {
            get { return m_selecting; }
            set { m_selecting = value; }
        }

        public SelectionBox()
        {
            m_selectionBox.Visibility = Visibility.Hidden;
        }

        public void startSelection(Point where, Canvas into)
        {
            if (m_selectionBox == null)
            {
                m_selectionBox = new Rectangle();
            }
            if (m_canvas != null)
            {
                m_canvas.Children.Remove(this.m_selectionBox);
            }
            m_start = where;
            m_TopLeft = m_start;
            m_canvas = into;
            Canvas.SetTop(m_selectionBox, m_start.Y);
            Canvas.SetLeft(m_selectionBox, m_start.X);
            m_selectionBox.Width = 0;
            m_selectionBox.Height = 0;
            //m_selectionBox.StrokeThickness = 1;
            m_selectionBox.Stroke = new SolidColorBrush(Color.FromArgb(0xff, 0x00, 0x00, 0xff));
            m_selectionBox.Fill = new SolidColorBrush(Color.FromArgb(0x44, 0x00, 0x00, 0x48));
            m_canvas.Children.Add(this.m_selectionBox);
            m_selectionBox.Visibility = Visibility.Visible;
            m_selecting = true;
        }

        public void extendSelection(Point where)
        {
            double x = where.X - m_start.X;
            double y = where.Y - m_start.Y;
            if (x < 0)
            {
                x = x * -1;
                Canvas.SetLeft(m_selectionBox, where.X);
                m_TopLeft.X = where.X;
            }
            else
            {
                m_TopLeft.X = m_start.X;
            }
            if (y < 0)
            {
                y = y * -1;
                Canvas.SetTop(m_selectionBox, where.Y);
                m_TopLeft.Y = where.Y;
            }
            else
            {
                m_TopLeft.Y = m_start.Y;
            }
            m_selectionBox.Width = Math.Max(0, x);
            m_selectionBox.Height = Math.Max(0, y);
        }

        /// <summary>
        /// Finish the selection.
        /// </summary>
        /// <returns>The selected area.</returns>
        public Rect endSelection()
        {
            Point p2 = Point.Add(m_TopLeft, new Vector(m_selectionBox.Width, m_selectionBox.Height));
            Rect ret = new Rect(m_TopLeft, p2);
            if (m_canvas != null)
            {
                m_canvas.Children.Remove(this.m_selectionBox);
            }
            m_selectionBox.Visibility = Visibility.Hidden;
            m_selecting = false;
            return ret;
        }
    }
}
