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
using System.Windows.Media.Media3D;
using System.ComponentModel;
//using FbxSdkWrapper;
using Microsoft.Win32;

namespace CogaenEditorControls.Controls
{
    class Topology
    {
        private List<Vector3D> m_v;
        private List<int> m_i;
        private List<Edge> m_edges = new List<Edge>();
        private List<Triangle> m_triangles = new List<Triangle>();

        public Topology(List<Vector3D> v, List<int> i)
        {
            m_v = v;
            m_i = i;

            for (int j = 0; j < m_i.Count; j += 3)
            {
                Edge e1 = new Edge(m_v[m_i[j]], m_v[m_i[j + 1]]);
                Edge e2 = new Edge(m_v[m_i[j + 1]], m_v[m_i[j + 2]]);
                Edge e3 = new Edge(m_v[m_i[j + 2]], m_v[m_i[j]]);
                e1 = addEdge(e1);
                e2 = addEdge(e2);
                e3 = addEdge(e3);
                addTriangle(e1, e2, e3);
            }
        }

        private Edge addEdge(Edge edge)
        {
            foreach (Edge e in m_edges)
            {
                if (e.Equals(edge))
                {
                    return e;
                }
            }
            m_edges.Add(edge);
            return edge;
        }

        private void addTriangle(Edge e1, Edge e2, Edge e3)
        {
            Triangle t = new Triangle(e1,e2,e3);
            e1.asignToTriangle(t);
            e2.asignToTriangle(t);
            e3.asignToTriangle(t);
            m_triangles.Add(t);
        }
    }
    class Triangle
    {
        private Edge m_e1;

        public Edge Edge1
        {
            get { return m_e1; }
            set { m_e1 = value; }
        }

        private Edge m_e2;

        public Edge Edge2
        {
            get { return m_e2; }
            set { m_e2 = value; }
        }

        private Edge m_e3;

        public Edge Edge3
        {
            get { return m_e3; }
            set { m_e3 = value; }
        }
        public Triangle(Edge e1, Edge e2, Edge e3)
        {
            m_e1 = e1;
            m_e2 = e2;
            m_e3 = e3;
        }
    }

    class Edge : IEquatable<Edge>
    {
        
        private Triangle m_triangle1 = null;

        internal Triangle Triangle1
        {
            get { return m_triangle1; }
            set { m_triangle1 = value; }
        }
        private Triangle m_triangle2 = null;

        internal Triangle Triangle2
        {
            get { return m_triangle2; }
            set { m_triangle2 = value; }
        }

        private Vector3D m_v1;

        internal Vector3D V1
        {
            get { return m_v1; }
            set { m_v1 = value; }
        }

        private Vector3D m_v2;

        internal Vector3D V2
        {
            get { return m_v2; }
            set { m_v2 = value; }
        }

        public Edge(Vector3D v1, Vector3D v2)
        {
            m_v1 = v1;
            m_v2 = v2;
        }

        #region IEquatable<Edge> Members

        public bool Equals(Edge other)
        {
            return (other.m_v1 == m_v1 && other.m_v2 == m_v2) || ((other.m_v2 == m_v1 && other.m_v1 == m_v2));
        }

        #endregion

        public void asignToTriangle(Triangle t)
        {
            if (Triangle1 == null)
                Triangle1 = t;
            else if (Triangle2 == null)
                Triangle2 = t;
        }
    }
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class NavigationMeshControl : UserControl, INotifyPropertyChanged
    {
        #region member
        private PerspectiveCamera m_camera = new PerspectiveCamera();

        public PerspectiveCamera Camera
        {
            get { return m_camera; }
            set
            {
                m_camera = value;
                OnPropertyChanged("Camera");
            }
        }

        ModelVisual3D m_mesh = new ModelVisual3D();

        public ModelVisual3D Mesh
        {
            get { return m_mesh; }
            set
            {
                m_mesh = value;
                OnPropertyChanged("Mesh");
            }
        }

        private double m_rotX = 0.0;
        private double m_rotY = 0.0;
        private double m_rotZ = 0.0;

        //private Point3D m_position = new Point3D();
        private Vector3D m_scale = new Vector3D(1,1,1);

        #region mouse
        private double m_mouseX = 0.0;
        private double m_mouseY = 0.0;
        #endregion

        #region transform
        ScaleTransform3D m_scaleTransform = new ScaleTransform3D(new Vector3D(1,1,1));
        RotateTransform3D m_rotateTransform = new RotateTransform3D();
        TranslateTransform3D m_translateTransform = new TranslateTransform3D();
        #endregion
        #endregion
        public NavigationMeshControl()
        {
            InitializeComponent();
            DataContext = this;

            Camera.Position = new Point3D(0,5,10);
            Camera.LookDirection = new Vector3D(0,-0.5,-1);
            Camera.NearPlaneDistance = 1;
            Camera.FarPlaneDistance = 1000;
            Camera.UpDirection =  new Vector3D(0,1,0);
        }

        #region PropertyChanged
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

        #region menu
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileOpen = new OpenFileDialog();
            fileOpen.Filter = "Fbx|*.fbx";
            bool? ret = fileOpen.ShowDialog();
            if (ret.HasValue && ret.Value)
            {
                loadMesh(fileOpen.FileName);
            }
            //
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion

        private void loadMesh(String filename)
        {
            //FbxSdk sdk = new FbxSdk();
            //sdk.loadMesh(filename);
            //if (sdk.hasMoreMeshes())
            //{
            //    List<Vector3D> v = new List<Vector3D>();
            //    List<Vector3D> n = new List<Vector3D>();
            //    List<Vector3D> t = new List<Vector3D>();
            //    List<int> indices = new List<int>();
            //    Mesh mesh = new Mesh(v, n, t, indices);
            //    sdk.getNextMesh(mesh);

            //    MeshGeometry3D newMesh = new MeshGeometry3D();
            //    newMesh.Positions = new Point3DCollection(v.Count);
            //    foreach (Vector3D vector in v)
            //    {
            //        newMesh.Positions.Add(new Point3D(vector.X, vector.Y, vector.Z));
            //    }

            //    newMesh.Normals = new Vector3DCollection(n);

            //    newMesh.TriangleIndices = new Int32Collection(indices);
            //    GeometryModel3D geom = new GeometryModel3D();
            //    geom.Geometry = newMesh;
            //    ModelVisual3D NavigationMesh = new ModelVisual3D();
            //    NavigationMesh.Content = geom;
            //    MeshNode.Children.Clear();
            //    MeshNode.Children.Add(NavigationMesh);

            //    geom.Material = new DiffuseMaterial(new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 0, 255)));

            //    fitMeshIntoView(geom);

            //    generateTopology(v,indices);
            //}
        }

        private void generateTopology(List<Vector3D> v, List<int> indices)
        {
            Topology top = new Topology(v,indices);
        }
        /// <summary>
        /// Alter the camera position so that a mesh can be seen as a whole
        /// </summary>
        private void fitMeshIntoView(GeometryModel3D mesh)
        {
            double x = mesh.Bounds.SizeX;
            double y = mesh.Bounds.SizeY;
            double z = mesh.Bounds.SizeZ;
            double dScale = 1.0/ Math.Max(x,Math.Max(y,z));
            m_scale = new Vector3D(dScale * 10, dScale * 10, dScale * 10);
            scale();
        }

        #region mesh transform
        private void rotate(double x, double y, double z)
        {
            RotateTransform3D rotationGroup = new RotateTransform3D();
            
            AxisAngleRotation3D rotY = new AxisAngleRotation3D(new Vector3D(0,1,0), y);
            Rotation3D final = rotY;
            m_rotateTransform.Rotation = final;
            UpdateTransform();
        }

        private void scale()
        {
            m_scaleTransform = new ScaleTransform3D(m_scale);
            UpdateTransform();
        }

        private void UpdateTransform()
        {
            Transform3DGroup group = new Transform3DGroup();
            group.Children.Add(m_scaleTransform);
            group.Children.Add(m_rotateTransform);
            group.Children.Add(m_translateTransform);
            MeshNode.Transform = group;
        }

        #endregion

        private void hitTest(Point p)
        {

        }

        #region mouse
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.RightButton == MouseButtonState.Released)
            {
                hitTest(e.GetPosition(m_Viewport));
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                Point p = e.GetPosition(m_Viewport);
                m_mouseX = p.X;
                m_mouseY = p.Y;
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                Point p = e.GetPosition(m_Viewport);
                m_rotY += (p.X - m_mouseX) / 50;
                m_mouseX = p.X;
                m_mouseY = p.Y;
                rotate(m_rotX,m_rotY,m_rotZ);
            }
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double newScale = m_scale.X * (e.Delta / 100);
            //m_scale = new Vector3D(newScale, newScale, newScale);
            //scale();
        }
        #endregion
    }
}
