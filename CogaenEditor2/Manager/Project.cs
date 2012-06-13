using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using CogaenDataItems.Exporter;
using CogaenDataItems.Manager;

namespace CogaenEditor2.Manager
{
    public class Project : INotifyPropertyChanged
    {
        #region member
        private bool m_dirty = false;

        public bool Dirty
        {
            get { return m_dirty; }
            set { m_dirty = value; }
        }

        private ObservableCollection<IProjectElement> m_elements = new ObservableCollection<IProjectElement>();

        public ObservableCollection<IProjectElement> Elements
        {
            get
            {
                if (m_root != null)
                    return m_root.Entries;
                else
                    return null;
            }
            
        }

        private ProjectFilter m_root;

        public ProjectFilter Root
        {
            get { return m_root; }
            set
            {
                m_root = value;
                m_elements.Clear();
                m_elements.Add(m_root);
            }
        }

        public String Name
        {
            get { return Root.Name; }
            set
            {
                Root.Name = value;
                OnPropertyChanged("Name");
            }
        }

        public ObservableCollection<IProjectElement> RootElements
        {
            get { return m_elements; }
        }

        private String m_exportDirectory;

        public String ExportDirectory
        {
            get { return m_exportDirectory; }
            set
            {
                m_exportDirectory = value;
                Dirty = true;
                OnPropertyChanged("ExportDirectory");
            }
        }

        private String m_directory;

        public String ProjectDirectory
        {
            get { return m_directory; }
            set
            {
                m_directory = value;
                Dirty = true;
                OnPropertyChanged("ProjectDirectory");
            }
        }

        private String m_configFile;

        public String ConfigFile
        {
            get { return m_configFile; }
            set
            {
                m_configFile = value;
                Dirty = true;
                OnPropertyChanged("ConfigFile");
            }
        }


        private bool m_exportFilters = true;

        public bool ExportFilters
        {
            get { return m_exportFilters; }
            set
            {
                m_exportFilters = value;
                Dirty = true;
                OnPropertyChanged("ExportFilters");
            }
        }

        private IScriptExporter m_exporter = null;

        public IScriptExporter Exporter
        {
            get { return m_exporter; }
            set
            {
                m_exporter = value;
                Dirty = true;
                OnPropertyChanged("Exporter");
            }
        }

        #endregion

        #region CDtors
        public Project(String name)
        {
            m_root = new ProjectFilter(name);
            m_elements.Add(m_root);
        }
        #endregion

        #region methods
        #region validation
        /// <summary>
        /// Check if all files are here
        /// </summary>
        /// <param name="filenames">a list of files that couldn't be found or opened</param>
        /// <returns>true if all files are found</returns>
        public bool validate(List<String> filenames)
        {
            return validate(Root,filenames);
        }

        private bool validate(IProjectElement element, List<String> filenames)
        {
            bool valid = true;
            if (element is ProjectTemplateFile)
            {
                bool canOpen = tryOpen(element.Name);
                if (!canOpen)
                {
                    filenames.Add(element.Name);
                }
                valid = valid && canOpen;
            }
            else if (element is ProjectFilter)
            {
                ProjectFilter filter = element as ProjectFilter;
                foreach (IProjectElement subElement in filter.Entries)
                {
                    valid = validate(subElement, filenames) && valid;
                }
            }
            return valid;
        }

        private bool tryOpen(String filename)
        {
            bool valid = true;
            try
            {
                using (File.Open(filename, System.IO.FileMode.Open)) { }
            }
            catch(Exception)
            {
                valid = false;
            }

            return valid;
        }
        #endregion
        #region file management
        /// <summary>
        /// Adds a new Template to the Project
        /// </summary>
        /// <param name="name">filename without extension</param>
        public void addTemplateToProject(String name)
        {
            addTemplateToProject(name, Root);
        }

        /// <summary>
        /// Add a Template under a specific filter
        /// </summary>
        /// <param name="name">filename without extension</param>
        /// <param name="root">the filter</param>
        public void addTemplateToProject(String name, ProjectFilter root)
        {
            String filename = name + ".ctl";
            ProjectTemplateFile template = new ProjectTemplateFile(filename);
            template.Script = new ProjectScriptFile(name, template);

            root.addProjectElement(template);
            root.sort();
            Dirty = true;
        }

        /// <summary>
        /// Removes or deletes a Element of the Project
        /// </summary>
        /// <param name="element"></param>
        /// <param name="deleteFile">Specifies if the file should be deleted or not</param>
        public void removeTemplateFromProject(IProjectElement element, bool deleteFile)
        {
            if (element is ProjectFilter)
            {
                ProjectFilter filter = element as ProjectFilter;
                if (filter.Parent != null)
                {
                    filter.Parent.removeProjectElement(filter);
                    foreach (IProjectElement subElement in filter.Entries)
                    {
                        removeTemplateFromProject(filter,deleteFile);
                    }
                }
            }
            else if (element is ProjectTemplateFile)
            {
                ProjectTemplateFile template = element as ProjectTemplateFile;
                if (template.Parent != null)
                {
                    template.Parent.removeProjectElement(template);
                    if (deleteFile)
                    {
                        try
                        {
                            File.Delete(template.Name);
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
            }
            Dirty = true;
        }

        /// <summary>
        /// Add a new Filter to the project
        /// </summary>
        /// <param name="name"></param>
        public void addFilterToProject(String name)
        {
            addFilterToProject(name, Root);
        }
        /// <summary>
        /// Add a new Filter to the project
        /// </summary>
        /// <param name="name"></param>
        /// <param name="root">the parent filter</param>
        public void addFilterToProject(String name, ProjectFilter root)
        {
            root.addProjectElement(new ProjectFilter(name));
            root.sort();
            Dirty = true;
        }

        /// <summary>
        /// Remove a filter from the Project
        /// </summary>
        /// <param name="filter"></param>
        public void removeFilterFromProject(ProjectFilter filter, bool deleteFiles)
        {
            removeTemplateFromProject(filter,deleteFiles);
        }
        #endregion
        #region export
        /// <summary>
        /// Export the whole project. This opens all templates and exports them.
        /// </summary>
        public void export()
        {
            export(Root);
        }

        public void export(ProjectFilter filter)
        {
            foreach (IProjectElement element in filter.Entries)
            {
                if (element is ProjectTemplateFile)
                {
                    export(element as ProjectTemplateFile);
                }
                else if (element is ProjectFilter)
                {
                    export(element as ProjectFilter);
                }
            }
        }

        public void export(ProjectTemplateFile template)
        {
            String full_path = ExportDirectory.TrimEnd('\\');
            if (m_exportFilters)
                full_path += setDirectory(template.Parent);
            if (!Directory.Exists(full_path))
            {
                Directory.CreateDirectory(full_path);
            }
            full_path += "\\" + template.Script.Name;
            App app = App.Current as App;
            ObjectBuilder ob = app.openTemplate(template.Name);
            app.exportTemplate(ob,full_path);
        }

        private String setDirectory(ProjectFilter filter)
        {
            if (filter.Parent != null)
                return setDirectory(filter.Parent) + "\\" + filter.Name;
            else
                return "";
        }
        #endregion
        #region (de)serialisation
        #region save
        /// <summary>
        /// Save the Project file as xml
        /// </summary>
        /// <param name="filename"></param>
        public void save(String filename)
        {
            XmlDocument xmlDoc = new XmlDocument();
            #region basic
            XmlElement projectNode = xmlDoc.CreateElement("Project");
            xmlDoc.AppendChild(projectNode);
            XmlAttribute projectName = xmlDoc.CreateAttribute("Name");
            projectName.Value = m_root.Name;
            projectNode.Attributes.Append(projectName);

            XmlAttribute exportPathNode = xmlDoc.CreateAttribute("ExportPath");
            exportPathNode.Value = ExportDirectory;
            projectNode.Attributes.Append(exportPathNode);

            XmlAttribute dirPathNode = xmlDoc.CreateAttribute("Path");
            dirPathNode.Value = ProjectDirectory;
            projectNode.Attributes.Append(dirPathNode);

            XmlAttribute exportFilterNode = xmlDoc.CreateAttribute("ExportFilter");
            exportFilterNode.Value = m_exportFilters.ToString();
            projectNode.Attributes.Append(exportFilterNode);

            XmlAttribute configFileNode = xmlDoc.CreateAttribute("ConfigFile");
            configFileNode.Value = m_configFile.ToString();
            projectNode.Attributes.Append(configFileNode);

            XmlAttribute scriptType = xmlDoc.CreateAttribute("ScriptType");
            scriptType.Value = m_exporter.Extension;
            projectNode.Attributes.Append(scriptType);

            #endregion
            #region files
            XmlElement files = xmlDoc.CreateElement("Files");
            foreach (IProjectElement file in m_root.Entries)
            {
                saveProjectElement(xmlDoc, files, file);
            }
            projectNode.AppendChild(files);
            #endregion

            #region settings

            #endregion

            xmlDoc.Save(filename);

            Dirty = false;
        }

        public void save()
        {
            save(Root.Name+".cep");
        }

        private void saveProjectElement(XmlDocument xmlDoc, XmlElement elementNode, IProjectElement element)
        {
            if (element is ProjectTemplateFile)
            {
                ProjectTemplateFile template = element as ProjectTemplateFile;
                saveProjectTemplate(xmlDoc, elementNode, template);
            }
            else if (element is ProjectFilter)
            {
                ProjectFilter filter = element as ProjectFilter;
                saveProjectFilter(xmlDoc, elementNode, filter);
            }
        }

        private void saveProjectFilter(XmlDocument xmlDoc, XmlElement elementNode, ProjectFilter filter)
        {
            XmlElement filterNode = xmlDoc.CreateElement("Filter");
            XmlAttribute filterName = xmlDoc.CreateAttribute("Name");
            filterName.Value = filter.Name;
            filterNode.Attributes.Append(filterName);
            XmlAttribute filterExport = xmlDoc.CreateAttribute("Export");
            filterExport.Value = filter.Export.ToString();
            filterNode.Attributes.Append(filterExport);

            foreach (IProjectElement subElement in filter.Entries)
            {
                saveProjectElement(xmlDoc, filterNode, subElement);
            }
            elementNode.AppendChild(filterNode);
        }

        private void saveProjectTemplate(XmlDocument xmlDoc, XmlElement elementNode, ProjectTemplateFile template)
        {
            XmlElement file = xmlDoc.CreateElement("File");
            XmlAttribute filename = xmlDoc.CreateAttribute("Filename");
            filename.Value = template.Name;
            file.Attributes.Append(filename);

            XmlAttribute scriptname = xmlDoc.CreateAttribute("Script");
            scriptname.Value = template.Script.Name;
            file.Attributes.Append(scriptname);

            elementNode.AppendChild(file);
        }
        #endregion

        #region load
        public static Project load(string filename)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filename);
            String name = "unnamed";
            String exportDir = ".";
            String projectDir = ".";
            String configFile = "";
            bool bExportFilter = true;
            IScriptExporter exporter = null;
            
            ProjectFilter root = new ProjectFilter("root");

            foreach (XmlElement element in xmlDoc.ChildNodes)
            {
                if (element.Name == "Project")
                {
                    foreach (XmlAttribute attrib in element.Attributes)
                    {
                        if (attrib.Name == "Name")
                        {
                            name = attrib.Value;
                            root.Name = name;
                        }
                        else if (attrib.Name == "ExportPath")
                        {
                            exportDir = attrib.Value;
                        }
                        else if (attrib.Name == "Path")
                        {
                            projectDir = attrib.Value;
                        }
                        else if (attrib.Name == "ExportFilter")
                        {
                             bool.TryParse(attrib.Value, out bExportFilter);
                        }
                        else if (attrib.Name == "ConfigFile")
                        {
                            configFile = attrib.Value;
                        }
                        else if (attrib.Name == "ScriptType")
                        {
                            App app = App.Current as App;
                            exporter = app.getExporter(attrib.Value);
                        }
                    }
                }
                loadSub(element, root);
            }


            Project newProject = new Project(name);
            newProject.Root = root;
            //newProject.m_root = root;
            //newProject.m_elements = newProject.m_root.Entries;
            newProject.ExportDirectory = exportDir;
            newProject.ProjectDirectory = projectDir;
            newProject.ExportFilters = bExportFilter;
            newProject.ConfigFile = configFile;
            newProject.Exporter = exporter;

            //Directory.SetCurrentDirectory(newProject.ProjectDirectory);

            List<String> files = new List<String>();
            bool valid = newProject.validate(files);
            if (valid)
                return newProject;
            else
            {
                throw new ProjectFilesNotFoundException("Couldn't load all files of the project", files);
            }
        }

        private static void loadSub(XmlElement node, ProjectFilter root)
        {
            foreach (XmlElement element in node.ChildNodes)
            {
                if (element.Name == "Files")
                {
                    loadFiles(element, root);
                }
            }
        }

        private static void loadFiles(XmlElement filesNode, ProjectFilter root)
        {
            foreach (XmlElement element in filesNode.ChildNodes)
            {
                if (element.Name == "Filter")
                {
                    String name = "";
                    bool export = true;

                    foreach (XmlAttribute attrib in element.Attributes)
                    {
                        if (attrib.Name == "Name")
                        {
                            name = attrib.Value;
                        }
                        else if (attrib.Name == "Export")
                        {
                            bool.TryParse(attrib.Value, out export);
                        }
                    }
                    ProjectFilter filter = new ProjectFilter(name);
                    filter.Export = export;
                    root.addProjectElement(filter);
                    loadFiles(element, filter);
                }
                else if (element.Name == "File")
                {
                    string filename = "";
                    string scriptname = "";
                    foreach (XmlAttribute attrib in element.Attributes)
                    {
                        if (attrib.Name == "Filename")
                        {
                            filename = attrib.Value;
                        }
                        else if (attrib.Name == "Script")
                        {
                            scriptname = attrib.Value;
                        }
                    }
                    ProjectTemplateFile template = new ProjectTemplateFile(filename);
                    template.Script = new ProjectScriptFile(scriptname, template);
                    root.addProjectElement(template);
                }
            }
        }
        #endregion
        #endregion
        #region sorting
        public void sort()
        {
            Root.sort();
        }
        #endregion
        #endregion

        #region drag 'n'drop
        public void PreviewDragOver(IProjectElement p, DragEventArgs e)
        {
            if (p != null)
            {
                e.Effects = DragDropEffects.Move;
            }
        }

        public void Drop(IProjectElement p, DragEventArgs e)
        {
            if (p != null)
            {
                if (e.OriginalSource is FrameworkElement)
                {
                    FrameworkElement tb = e.OriginalSource as FrameworkElement;
                    if (tb.DataContext is ProjectFilter)
                    {
                        ProjectFilter filter = tb.DataContext as ProjectFilter;
                        if (p == filter)
                        {
                            return;
                        }
                        p.Parent.removeProjectElement(p);
                        filter.addProjectElement(p);
                        filter.sort();
                        Dirty = true;
                    }
                    else if (tb.DataContext is ProjectTemplateFile)
                    {
                        ProjectTemplateFile template = tb.DataContext as ProjectTemplateFile;
                        if (p == template)
                        {
                            return;
                        }
                        if (p.Parent != null)
                            p.Parent.removeProjectElement(p);
                        Dirty = true;
                        if (template.Parent != null)
                        {
                            template.Parent.addProjectElement(p);
                            template.Parent.sort();
                        }
                        else
                        {
                            Root.addProjectElement(p);
                            Root.sort();
                        }
                    }
                }
            }
        }
        #endregion

        #region Property Changed
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
    }

    #region helper classes

    #region sorting
    public class ProjectElementComparer : IComparer<IProjectElement>
    {

        public int Compare(IProjectElement x, IProjectElement y)
        {
            return x.Name.CompareTo(y.Name);
        }
    }
    #endregion

    public interface IProjectElement
    {
        string Name
        {
            set;
            get;
        }

        ProjectFilter Parent
        {
            get;
            set;
        }

        bool Editable
        {
            get;
            set;
        }

        void sort();
    }

    public class ProjectFilter : IProjectElement, INotifyPropertyChanged
    {
        #region members
        private ObservableCollection<IProjectElement> m_entries = new ObservableCollection<IProjectElement>();

        public ObservableCollection<IProjectElement> Entries
        {
            get { return m_entries; }
            set { m_entries = value; }
        }

        private string m_name;

        public string Name
        {
            get { return m_name; }
            set { m_name = value; OnPropertyChanged("Name"); }
        }

        private ProjectFilter m_parent;

        public ProjectFilter Parent
        {
            get { return m_parent; }
            set { m_parent = value; OnPropertyChanged("Parent"); }
        }

        private bool m_editable = false;
        public bool Editable
        {
            get { return m_editable; }
            set { m_editable = value; OnPropertyChanged("Editable"); }
        }

        private bool m_export = true;

        public bool Export
        {
            get { return m_export; }
            set { m_export = value; OnPropertyChanged("Export"); }
        }

        #endregion

        #region CDtors
        public ProjectFilter(String name)
        {
            m_name = name;
        }
        #endregion

        public void addProjectElement(IProjectElement element)
        {
            element.Parent = this;
            m_entries.Add(element);
        }

        public void removeProjectElement(IProjectElement element)
        {
            element.Parent = null;
            m_entries.Remove(element);
        }

        public void sort()
        {
            List<IProjectElement> filters = new List<IProjectElement>();
            List<IProjectElement> files = new List<IProjectElement>();
            List<IProjectElement> others = new List<IProjectElement>();
            // separate filters, files and other
            foreach (IProjectElement element in m_entries)
            {
                element.sort();
                if (element is ProjectFilter)
                {
                    filters.Add(element);
                }
                else if (element is ProjectTemplateFile)
                    files.Add(element);
                else
                    others.Add(element);
            }
            // sort them
            ProjectElementComparer comparer = new ProjectElementComparer();
            filters.Sort(comparer);
            files.Sort(comparer);
            others.Sort(comparer);
            // reinsert sorted
            m_entries.Clear();
            foreach (IProjectElement element in filters)
            {
                m_entries.Add(element);
            }
            foreach (IProjectElement element in files)
            {
                m_entries.Add(element);
            }
            foreach (IProjectElement element in others)
            {
                m_entries.Add(element);
            }
        }

        #region Property Changed
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
    }

    public class ProjectTemplateFile : IProjectElement, INotifyPropertyChanged
    {
        #region members
        private ProjectScriptFile m_script = null;

        public ProjectScriptFile Script
        {
            get { return m_script; }
            set { m_script = value; OnPropertyChanged("Script"); }
        }

        private string m_name;

        public string Name
        {
            get { return m_name; }
            set { rename(m_name, value); m_name = value; OnPropertyChanged("Name"); }
        }

        private ProjectFilter m_parent;

        public ProjectFilter Parent
        {
            get { return m_parent; }
            set { m_parent = value; OnPropertyChanged("Parent"); }
        }

        private bool m_editable = false;
        public bool Editable
        {
            get { return m_editable; }
            set { m_editable = value; OnPropertyChanged("Editable"); }
        }
        #endregion

        #region CDtors
        public ProjectTemplateFile(String name)
        {
            m_name = name;
        }
        #endregion

        public void sort()
        {

        }

        private void rename(String oldName, String newName)
        {
            if (File.Exists(newName))
            {
                MessageBoxResult result = MessageBox.Show("Overwrite?", "File Exists", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    File.Copy(oldName, newName, true);
                    File.Delete(oldName);
                }
            }
            else
            {
                File.Copy(oldName, newName, true);
                File.Delete(oldName);
            }
            
        }

        #region Property Changed
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
    }

    public class ProjectScriptFile : IProjectElement, INotifyPropertyChanged
    {
        #region members
        private ProjectTemplateFile m_template;

        public ProjectTemplateFile Template
        {
            get { return m_template; }
        }

        private string m_name;

        public string Name
        {
            get { return m_name; }
            set { m_name = value; OnPropertyChanged("Name"); }
        }

        private ProjectFilter m_parent;

        public ProjectFilter Parent
        {
            get { return m_parent; }
            set { m_parent = value; OnPropertyChanged("Parent"); }
        }

        private bool m_editable = false;
        public bool Editable
        {
            get { return m_editable; }
            set { m_editable = value; OnPropertyChanged("Editable"); }
        }
        #endregion

        #region CDtors
        public ProjectScriptFile(String name, ProjectTemplateFile template)
        {
            m_name = name;
            m_template = template;
        }
        #endregion

        public void sort()
        {

        }

        #region Property Changed
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
    }

    public class ProjectFilesNotFoundException : FileNotFoundException
    {
        private List<String> m_filenames;

        public List<String> Filenames
        {
            get { return m_filenames; }
        }
        public ProjectFilesNotFoundException(string message, List<String> filenames)
            : base(message)
        {
            m_filenames = filenames;
        }
    }

    #endregion
}
