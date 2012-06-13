using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Media.Media3D;
using System.Windows;
using FbxSdkWrapper;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CogaenDataItems.Manager;
using CogaenDataItems.DataItems;


namespace CogaenEditor2.Manager
{
    public class ResourceManager: IResourceManager
    {
        #region member
        private FbxSdk m_fbx = new FbxSdk();
        private String m_mediaRoot;

        private ObservableCollection<Pair<String, ObservableCollection<Pair<String, MeshGeometry3D>>>> m_meshes = new ObservableCollection<Pair<String, ObservableCollection<Pair<String, MeshGeometry3D>>>>();
        private ObservableCollection<Pair<String, MeshGeometry3D>> m_meshList = new ObservableCollection<Pair<String, MeshGeometry3D>>();
        private ObservableCollection<Pair<String, Drawing>> m_textures = new ObservableCollection<Pair<String, Drawing>>();
        private ObservableCollection<Pair<String, CMaterial>> m_materials = new ObservableCollection<Pair<String, CogaenDataItems.DataItems.CMaterial>>();

        public ObservableCollection<Pair<String, CMaterial>> Materials
        {
            get { return m_materials; }
            set { m_materials = value; }
        }

        public ObservableCollection<Pair<String, ObservableCollection<Pair<String, MeshGeometry3D>>>> Meshes
        {
            get { return m_meshes; }
        }

        public ObservableCollection<Pair<String, Drawing>> Textures
        {
            get { return m_textures; }
        }

        #endregion

        public ResourceManager()
        {
            m_mediaRoot = Directory.GetCurrentDirectory()+"\\media";
            loadDefaultFolder();
        }

        public void loadDefaultFolder()
        {
            loadFolder(m_mediaRoot);
        }

        public void loadFolder(String path)
        {
            foreach (String f in Directory.GetFiles(path))
            {
                if (isMesh(f))
                {
                    loadMesh(f, path);
                }
                else if (isTexture(f))
                {
                    loadTexture(f, path);
                }
            }
        }

        public void loadFile(String filename)
        {
            if (isMesh(filename))
            {
                loadMesh(filename, "");
            }
            else if (isTexture(filename))
            {
                loadTexture(filename, "");
            }
        }

        #region loading

        public void loadMesh(String filename, String relativePath)
        {
            int c = m_fbx.loadMesh(filename);
            ObservableCollection<Pair<String, MeshGeometry3D>> meshList = new ObservableCollection<Pair<string, MeshGeometry3D>>();
            while (m_fbx.hasMoreMeshes())
            {
                Mesh m = new Mesh(new List<Vector3D>(), new List<Vector3D>(), new List<Vector3D>(), new List<int>());
                m_fbx.getNextMesh(m);

                MeshGeometry3D model = new MeshGeometry3D();
                for (int i = 0; i < m.vertices.Count; ++i)
                {
                    model.Positions.Add(new Point3D(m.vertices[i].X, m.vertices[i].Y, m.vertices[i].Z));
                    model.Normals.Add(m.normals[i]);
                    model.TextureCoordinates.Add(new Point(m.texcoords[i].X, m.texcoords[i].Y));
                }

                for (int i = 0; i < m.indices.Count; ++i)
                {
                    model.TriangleIndices.Add(m.indices[i]);

                }
                model.Freeze();
                
                String name = m.name;

                meshList.Add(new Pair<String, MeshGeometry3D>(name, model));
            }
            if (c > 0)
            {
                String name = filename.Substring(relativePath.Length, filename.Length - relativePath.Length).Trim('\\');
                m_meshes.Add(new Pair<String, ObservableCollection<Pair<String, MeshGeometry3D>>>(name, meshList));
            }
        }

        public void loadTexture(String filename, String relativePath)
        {
            Drawing drawing = new ImageDrawing();
            ImageSource img = new BitmapImage(new Uri(filename));
            String name = filename.Substring(relativePath.Length, filename.Length - relativePath.Length).Trim('\\');
            m_textures.Add(new Pair<String, Drawing>(name, drawing));
        }

        #endregion

        #region getting

        public MeshGeometry3D getMesh(String name)
        {
            foreach (Pair<String, ObservableCollection<Pair<String, MeshGeometry3D>>> m in m_meshes)
            {
                foreach (Pair<String, MeshGeometry3D> p in m.Value)
                {
                    if (p.Key == name)
                    {
                        return p.Value;
                    }
                }
            }
            return null;
        }

        public Drawing getTexture(String name)
        {
            foreach (Pair<String, Drawing> pair in m_textures)
            {
                if (pair.Key == name)
                {
                    return pair.Value;
                }
            }
            return null;
        }


        public CogaenDataItems.DataItems.CMaterial getMaterial(String name)
        {
            foreach (Pair<String, CogaenDataItems.DataItems.CMaterial> pair in m_materials)
            {
                if (pair.Key == name)
                {
                    return pair.Value;
                }
            }
            return null;
        }
        #endregion

        #region check extensions
        private static String[] MeshExtensions = new String[] { "fbx" };
        private static String[] TextureExtensions = new String[] { "png", "bmp", "tif", "tiff", "gif", "jpg", "jpeg" };

        private static bool isMesh(String file) 
        {
            foreach (String extension in MeshExtensions)
            {
                if (file.ToLower().EndsWith(extension))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool isTexture(String file)
        {
            foreach (String extension in TextureExtensions)
            {
                if (file.ToLower().EndsWith(extension))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

    }
}
