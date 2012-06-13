using System;
using CogaenDataItems.Manager;
using CogaenDataItems.DataItems;
using System.Collections.ObjectModel;
using System.Windows.Media.Media3D;
using System.Windows.Media;

namespace CogaenDataItems.Manager
{
    public interface IResourceManager
    {
        CMaterial getMaterial(String name);
        MeshGeometry3D getMesh(String name);
        Drawing getTexture(String name);
        void loadDefaultFolder();
        void loadFile(String filename);
        void loadFolder(String path);
        void loadMesh(String filename, String relativePath);
        void loadTexture(String filename, String relativePath);
        ObservableCollection<Pair<String, CMaterial>> Materials { get; set; }
        ObservableCollection<Pair<String, ObservableCollection<Pair<String, MeshGeometry3D>>>> Meshes { get; }
        ObservableCollection<Pair<String, Drawing>> Textures { get; }
    }
}
