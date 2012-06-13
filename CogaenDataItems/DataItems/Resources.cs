using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml;
using System.Windows.Media.Media3D;
using System.Windows;
using System.Globalization;
using CogaenDataItems.Manager;

namespace CogaenDataItems.DataItems
{
    public class ResourceFolder : AbstractResource
    {
        #region member
        private ObservableCollection<AbstractResource> m_resources = new ObservableCollection<AbstractResource>();

        public ObservableCollection<AbstractResource> Resources
        {
            get { return m_resources; }
        }
        #endregion

        #region CDtor
        public ResourceFolder(String name) : base(name)
        {
        }
        #endregion

        public ResourceFolder addSubFolder(String name)
        {
            ResourceFolder newOne = new ResourceFolder(name);
            m_resources.Add(newOne);
            return newOne;
        }

        public void addResource(AbstractResource resource)
        {
            m_resources.Add(resource);
        }

        public ResourceFolder getFolder(String name)
        {
            foreach(AbstractResource folder in m_resources)
            {
                if (folder is ResourceFolder && folder.Name == name)
                {
                    return folder as ResourceFolder;
                }
            }
            return null;
        }

        public override void fromXml(XmlNode resource, IResourceManager resMan)
        {

        }
    }

    public abstract class AbstractResource
    {
        public enum ResourceType
        {
            MESH,
            TEXTURE,
            MATERIAL,
            SCRIPT,
            MACRO,
            FOLDER
        }

        #region member
        protected ResourceType m_type;

        public ResourceType Type
        {
            get { return m_type; }
        }

        private String m_name;

        public String Name
        {
            get { return m_name; }
        }
        #endregion

        #region CDtors
        public AbstractResource(String name)
        {
            m_name = name;
        }
        #endregion

        #region methods
        public abstract void fromXml(XmlNode resource, IResourceManager resMan);

        public static String ResourceTypeToString(ResourceType type)
        {
            switch (type)
            {
                case ResourceType.MESH:
                    {
                        return "Meshes";
                    }
                case ResourceType.FOLDER:
                    {
                        return "Folder";
                    }
                case ResourceType.MACRO:
                    {
                        return "Macros";
                    }
                case ResourceType.MATERIAL:
                    {
                        return "Materials";
                    }
                case ResourceType.SCRIPT:
                    {
                        return "Scripts";
                    }
                case ResourceType.TEXTURE:
                    {
                        return "Textures";
                    }
                default:
                    return "NONE";
            }
        }
        #endregion
    }

    public class MeshResource : AbstractResource
    {
        #region CDtors
        public MeshResource(String name)
            : base(name)
        {
            m_type = AbstractResource.ResourceType.MESH;
        }
        #endregion

        #region methods
        public override void fromXml(XmlNode resource, IResourceManager resMan)
        {
            /**
             *<Mesh Name="Filename">
             * <MeshPart Name="Mesh01">
             *  <Vertices>
             *   <Vertex X="1" Y="2" Z="1"/>
             *   <Vertex X="-1" Y="0" Z="3"/>
             *  </Vertices>
             *  <Normals>
             *   <Normal X="1" Y="0" Z="0"/>
             *  </Normals>
             *  <TexCoords>
             *   <TexCoord U="0" V="0.1"/>
             *  </TexCoords>
             *  <Indices>
             *   <Index Value="0"/>
             *   <Index Value="1"/>
             *  </Indices>
             * </Mesh>
             *</Meshes>
             */
            String fileName = "";
            foreach(XmlAttribute attrib in resource.Attributes)
                {
                    if (attrib.Name == "Name")
                    {
                        fileName = attrib.Value;
                    }
                }
            CultureInfo culture = new CultureInfo("en-US");
            ObservableCollection<Pair<String,MeshGeometry3D>> meshes = new ObservableCollection<Pair<String,MeshGeometry3D>>();
            foreach(XmlNode mesh in resource.ChildNodes)
            {
                MeshGeometry3D model = new MeshGeometry3D();
                String meshName = "";
                foreach(XmlAttribute attrib in mesh.Attributes)
                {
                    if (attrib.Name == "Name")
                    {
                        meshName = attrib.Value;
                    }
                }
                foreach (XmlNode child in mesh.ChildNodes)
                {
                    if (child.Name == "Vertices")
                    {
                        XmlNode vertices = child;
                        foreach(XmlNode vertex in vertices.ChildNodes)
                        {
                            model.Positions.Add(new Point3D(double.Parse(vertex.Attributes[0].Value, culture), double.Parse(vertex.Attributes[1].Value, culture), double.Parse(vertex.Attributes[2].Value, culture)));
                        }
                    }
                    else if (child.Name == "Normals")
                    {
                        XmlNode normals = child;
                        foreach (XmlNode normal in normals.ChildNodes)
                        {
                            model.Normals.Add(new Vector3D(double.Parse(normal.Attributes[0].Value, culture), double.Parse(normal.Attributes[1].Value, culture), double.Parse(normal.Attributes[2].Value, culture)));
                        }
                    }
                    else if (child.Name == "TexCoords")
                    {
                        XmlNode texcoords = child;
                        foreach (XmlNode texcoord in texcoords.ChildNodes)
                        {
                            model.TextureCoordinates.Add(new Point(double.Parse(texcoord.Attributes[0].Value, culture), double.Parse(texcoord.Attributes[1].Value, culture)));
                        }
                    }
                    else if (child.Name == "Indices")
                    {
                        XmlNode indices = child;
                        foreach (XmlNode index in indices.ChildNodes)
                        {
                            model.TriangleIndices.Add(int.Parse(index.Attributes[0].Value, culture));
                        }
                    }
                }
                model.Freeze();
                meshes.Add(new Pair<String,MeshGeometry3D>(meshName,model));
            }
            
            resMan.Meshes.Add(new Pair<String, ObservableCollection<Pair<String,MeshGeometry3D>>>(fileName,meshes));
        }

        public override string ToString()
        {
            return Name;
        }
        #endregion
    }

    public class TextureResource : AbstractResource
    {
        public TextureResource(String name)
            : base(name)
        {
            m_type = AbstractResource.ResourceType.TEXTURE;
        }

        #region methods
        public override void fromXml(XmlNode resource, IResourceManager resMan)
        {

        }

        public override string ToString()
        {
            return Name;
        }
        #endregion
    }

    public class ScriptResource : AbstractResource
    {
        public ScriptResource(String name)
            : base(name)
        {
            m_type = AbstractResource.ResourceType.SCRIPT;
        }

        #region methods
        public override void fromXml(XmlNode resource, IResourceManager resMan)
        {
            
        }

        public override string ToString()
        {
            return Name;
        }
        #endregion
    }

    public class MacroResource : AbstractResource
    {
        public MacroResource(String name)
            : base(name)
        {
            m_type = AbstractResource.ResourceType.MACRO;
        }

        #region methods
        public override void fromXml(XmlNode resource, IResourceManager resMan)
        {

        }

        public override string ToString()
        {
            return Name;
        }
        #endregion
    }

    public class MaterialResource : AbstractResource
    {
        public MaterialResource(String name)
            : base(name)
        {
            m_type = AbstractResource.ResourceType.MATERIAL;
        }

        #region methods
        public override void fromXml(XmlNode resource, IResourceManager resMan)
        {

        }

        public override string ToString()
        {
            return Name;
        }
        #endregion
    }
}
