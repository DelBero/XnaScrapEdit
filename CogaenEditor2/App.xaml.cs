using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.ComponentModel;
using CogaenEditor2.Communication;
using CogaenEditor2.GUI.Menu;
using CogaenDataItems.DataItems;
using CogaenEditor2.Manager;
using System.IO;
using System.Collections.ObjectModel;
using CogaenDataItems.Exporter;
using CogaenEditor2.Exporter;
using CogaenEditor2.GUI.Windows;
using CogaenEditor2.Configuration;
using CogaenEditor2.Helper;
using System.Text;
using System.Windows.Forms;
using CogaenEditor2.GUI.DragDrop;
using System.Windows.Input;
using CogaenDataItems.Manager;
using CogaenEditorConnect.Communication;

namespace CogaenEditor2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application, INotifyPropertyChanged, IDisposable
    {
        #region static members
        public static char separator = '*';
        #endregion

        #region member
        MessageHandler m_MessageHandler = null;

        public MessageHandler MessageHandler
        {
            get { return m_MessageHandler; }
        }

        private CogaenData m_data = null;

        public CogaenData Data
        {
            get { return m_data; }
        }

        private Connection m_connection;

        private MenuManager m_menu = MenuManager.getDefaultMenu();

        public MenuManager Menu
        {
            get { return m_menu; }
        }

        private ResourceManager m_resourceManager = new ResourceManager();

        public ResourceManager ResourceManager
        {
            get { return m_resourceManager; }
            set { m_resourceManager = value; }
        }

        private Config m_config = Config.getConfig();

        public Config Config
        {
            get { return m_config; }
        }

        private DragDropHandler m_DragDropHandler = new DragDropHandler();

        public DragDropHandler DragDropHandler
        {
            get { return m_DragDropHandler; }
        }

        private String m_statusText = "Ok";

        public String StatusText
        {
            get { return m_statusText; }
            set
            {
                m_statusText = value;
                OnPropertyChanged("StatusText");
            }
        }

        #region statusbar
        private int m_statusValue = 0;

        public int StatusValue
        {
            get { return m_statusValue; }
            set
            {
                m_statusValue = value;
                OnPropertyChanged("StatusValue");
            }
        }

        private int m_statusMaxValue = 100;

        public int StatusMaxValue
        {
            get { return m_statusMaxValue; }
            set
            {
                m_statusMaxValue = value;
                OnPropertyChanged("StatusMaxValue");
            }
        }
        #endregion

        #region recent files
        private RecentFiles m_recent = new RecentFiles();

        public RecentFiles Recent
        {
            get { return m_recent; }
            set { m_recent = value; }
        }
        #endregion

        #region windows
        private StateMachineWindow m_stateMachineWin = new StateMachineWindow();

        public StateMachineWindow StateMachineWin
        {
            get { return m_stateMachineWin; }
        }
        #endregion

        #region GameObject creation and Templates
        private Project m_currentProject = null;

        public Project CurrentProject
        {
            get { return m_currentProject; }
            set { m_currentProject = value; OnPropertyChanged("CurrentProject"); }
        }

        private ObjectBuilder m_activeObjectBuilder = null;
        private TemplateManager m_templates = null;

        public TemplateManager Templates
        {
            get { return m_templates; }
            set { m_templates = value; }
        }

        public ObjectBuilder ObjectBuilder
        {
            get
            {
                return m_activeObjectBuilder;
            }
        }

        private List<IScriptObject> m_clipboardObject = null;

        public List<IScriptObject> ClipboardObject
        {
            get { return m_clipboardObject; }
            private set
            {
                m_clipboardObject = value;
                OnPropertyChanged("ClipboardObject");
            }
        }
        #endregion

        #region exporting
        private ObservableCollection<IScriptExporter> m_exporter = new ObservableCollection<IScriptExporter>();

        public ObservableCollection<IScriptExporter> Exporter
        {
            get { return m_exporter; }
        }

        #endregion

        #endregion

        #region events
        public event PropertyChangedEventHandler ProjectChanged;
        #endregion

        #region CDtors
        public App()
        {
            m_data = new CogaenData(new ObjectBuilder("Live", null));

            m_templates = new TemplateManager(Data);

            m_MessageHandler = new MessageHandler(this, m_data);
            m_connection = new Connection(m_MessageHandler);
            m_MessageHandler.Connection = m_connection;

            m_exporter.Add(new CogaenScriptingExporter());
            m_exporter.Add(new XnaXmlScriptExporter());
        }
        #endregion

        #region methods

        #region init
        public void mainWindowDone()
        {
            //(MainWindow as MainWindow).m_2dLiveEditor.DataContext = m_2dEditor;
            (MainWindow as MainWindow).DataContext = this;
        }
        #endregion

        #region CogaenData
        public void updateData()
        {
            MessageHandler.updateSubsystemData();

            MessageHandler.updateMacroData();

            MessageHandler.updateResources();
            //MessageHandler.Data.getAllComponents(m_elements);
        }
        #endregion

        #region projects
        public bool isLoaded(ProjectTemplateFile projectTemplate, out ObjectBuilder selected)
        {
            String name = "";
            int index = projectTemplate.Name.LastIndexOf('.');
            if (index > 0)
                name = projectTemplate.Name.Substring(0, index);
            foreach (ObjectBuilder template in Templates.Templates)
            {
                if (template.Name == name)
                {
                    selected = template;
                    return true;
                }
            }
            selected = null;
            return false;
        }

        public void newProject(NewProjectData data)
        {
            CurrentProject = new Project(data.Name);
            CurrentProject.ProjectDirectory = data.Directory;
            CurrentProject.ExportDirectory = data.ExportDirectory;
            CurrentProject.Exporter = data.Exporter;
            CurrentProject.ConfigFile = data.ConfigFile;
            Config.saveAs(CurrentProject.ConfigFile);
            Directory.SetCurrentDirectory(data.Directory);
            saveCurrentProject();
            ProjectChanged(this, new PropertyChangedEventArgs("CurrentProject"));
        }

        public void saveCurrentProject()
        {
            if (CurrentProject != null)
            {
                CurrentProject.save();
                if (CurrentProject.ConfigFile != "")
                    Config.saveAs(CurrentProject.ConfigFile);
            }
            saveOpenTemplates();
        }

        public void loadProject(String filename)
        {
            try
            {
                CurrentProject = Project.load(filename);
                Recent.newRecentFile(filename, filename);
                Directory.SetCurrentDirectory(CurrentProject.ProjectDirectory);
                // load config
                if (CurrentProject.ConfigFile != "")
                {
                    Config.deserialize(new BinaryReader(new FileStream(CurrentProject.ConfigFile, FileMode.OpenOrCreate)));
                }
                if (ProjectChanged != null)
                    ProjectChanged(this, new PropertyChangedEventArgs("CurrentProject"));
            }
            catch (ProjectFilesNotFoundException e)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Couldn't load the following files: \n");
                foreach (String file in e.Filenames)
                {
                    sb.Append(file);
                    sb.Append("\n");
                }
                System.Windows.MessageBox.Show(sb.ToString(), "Error", MessageBoxButton.OK);
            }

        }

        public void closeProject(bool save)
        {
            if (save)
            {
                saveCurrentProject();
            }
            while (Templates.Template != null)
            {
                closeTemplate();
            }
            CurrentProject = null;
        }

        public void deleteProjectItem(IProjectElement element)
        {
            if (CurrentProject != null && element != null)
            {
                MessageBoxResult result = System.Windows.MessageBox.Show("Do you want to also delete the file(s) from your disk", "Delete Files?", MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes)
                {
                    CurrentProject.removeTemplateFromProject(element, true);
                }
                else if (result == MessageBoxResult.No)
                {
                    CurrentProject.removeTemplateFromProject(element, false);
                }
            }
        }

        public void projectProperties()
        {
            ProjectPropertiesWindow propWin = ProjectPropertiesWindow.GetInstance();
            propWin.DataContext = CurrentProject;
            propWin.Show();
        }
        #endregion

        #region templates
        public void selectObjectBuilder(int index)
        {
            if (index >= 0)
            {
                m_activeObjectBuilder = m_templates.Templates[index];
                m_templates.Selected = index;
            }
        }

        public void selectLiveEditor()
        {
            m_activeObjectBuilder = Data.LiveGameObjects as ObjectBuilder;
        }

        public void selectTemplateEditor()
        {
            m_activeObjectBuilder = Templates.Template;
        }

        private void newTemplate(ProjectFilter parent, String name)
        {
            ObjectBuilder newTemplate = Templates.newTemplate(name);
            if (CurrentProject != null)
            {
                CurrentProject.addTemplateToProject(name, parent);
                saveTemplate(newTemplate, true);
            }
            MainWindow mw = MainWindow as MainWindow;
            if (mw != null)
            {
                mw.TemplateTab.SelectedIndex = m_templates.Templates.Count - 1;
                selectObjectBuilder(mw.TemplateTab.SelectedIndex);
            }
        }

        public void newTemplate(ProjectFilter parent)
        {
            StringQueryItem qry = new StringQueryItem("Enter new template name", "New Template", "New");
            StringQuery q = new StringQuery();
            q.DataContext = qry;
            bool? result = q.ShowDialog();
            if (result.Value)
            {
                newTemplate(parent, qry.Text);
            }
        }

        public void newTemplate()
        {
            if (CurrentProject != null)
                newTemplate(CurrentProject.Root);
            else
                newTemplate(null);
        }

        public void renameTemplate()
        {
            renameTemplate(Templates.Template);
        }

        public void renameTemplate(ObjectBuilder template)
        {
            StringQueryItem qry = new StringQueryItem("Enter new name", "New Name", "NewName");
            StringQuery q = new StringQuery();
            q.DataContext = qry;
            bool? res = q.ShowDialog();
            if (res.Value)
            {
                template.Name = qry.Text;
            }
        }

        public void loadTemplate(ProjectTemplateFile projectFile)
        {
            ObjectBuilder selected;
            if (isLoaded(projectFile, out selected))
            {
                Templates.Template = selected;
                return;
            }
            using (System.IO.Stream file = File.Open(projectFile.Name, FileMode.Open))
            {
                if (file != null)
                {
                    file.Position = 0;
                    System.IO.BinaryReader br = new System.IO.BinaryReader(file);
                    Version v = new Version();
                    v.deserialize(br);
                    ObjectBuilder newOb = m_templates.newTemplate("");
                    newOb.deserialize(br);
                    MainWindow mw = MainWindow as MainWindow;
                    mw.TemplateTab.SelectedIndex = m_templates.Templates.Count - 1;
                    selectObjectBuilder(mw.TemplateTab.SelectedIndex);

                    file.Close();
                }
            }
        }

        public void loadTemplate()
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Title = "Load Template";
            openDialog.Filter = "Template files (*.ctl)|*.ctl";
            FolderOption path = Config.getOption("Save Path") as FolderOption;
            openDialog.InitialDirectory = path.Value;
            DialogResult result = openDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                openTemplate(openDialog.FileName);
                //System.IO.Stream file = openDialog.OpenFile();
                //if (file != null)
                //{
                //    file.Position = 0;
                //    System.IO.BinaryReader br = new System.IO.BinaryReader(file);
                //    Version v = new Version();
                //    v.deserialize(br);
                //    ObjectBuilder newOb = m_templates.newTemplate("");
                //    newOb.deserialize(br);
                //    MainWindow mw = MainWindow as MainWindow;
                //    mw.TemplateTab.SelectedIndex = m_templates.Templates.Count - 1;
                //    selectObjectBuilder(mw.TemplateTab.SelectedIndex);
                //}
            }
        }

        public ObjectBuilder openTemplate(String filename)
        {
            Recent.newRecentFile(filename, filename);
            ObjectBuilder newOb = null;
            using (System.IO.Stream file = File.Open(filename, FileMode.Open))
            {
                using (System.IO.BinaryReader br = new System.IO.BinaryReader(file))
                {
                    Version v = new Version();
                    v.deserialize(br);
                    newOb = new ObjectBuilder(Data);
                    newOb.deserialize(br);
                }
            }
            return newOb;
        }

        public void saveOpenTemplates()
        {
            foreach (ObjectBuilder ob in Templates.Templates)
            {
                saveTemplate(ob, true);
            }
        }

        public void saveTemplate(bool force)
        {
            saveTemplate(ObjectBuilder, force);
        }

        public void saveTemplate(ObjectBuilder objectBuilder, bool force)
        {
            DialogResult result;
            String filename = "";
            if (!force)
            {
                SaveFileDialog saveDialog;
                saveDialog = new SaveFileDialog();
                saveDialog.Title = "Save Template";
                saveDialog.Filter = "Template files (*.ctl)|*.ctl";
                FolderOption path = Config.getOption("Save Path") as FolderOption;
                saveDialog.InitialDirectory = path.Value;
                result = saveDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    filename = saveDialog.FileName;
                }
            }
            else
            {
                String path = "";
                if (CurrentProject != null)
                {
                    path = CurrentProject.ProjectDirectory.TrimEnd('\\');
                    path += "\\";
                    filename = path + objectBuilder.Name + ".ctl";
                }
            }
            if (filename != "")
            {
                using (System.IO.Stream file = new FileStream(filename, FileMode.OpenOrCreate))
                {
                    using (System.IO.BinaryWriter bw = new System.IO.BinaryWriter(file))
                    {
                        Version v = new Version();
                        v.serialize(bw);
                        objectBuilder.serialize(bw);
                        objectBuilder.Dirty = false;
                    }
                }
            }
        }

        public void exportTemplate(ObjectBuilder template, String filename)
        {
            if (CurrentProject.Exporter != null)
            {
                String script = template.exportScript(CurrentProject.Exporter);

                using (FileStream fs = System.IO.File.Open(filename + CurrentProject.Exporter.Extension, FileMode.OpenOrCreate))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.Write(script);
                        sw.Flush();
                    }
                    fs.Close();
                }
            }
        }

        public void exportTemplate()
        {
            ExportWindow expWin = new ExportWindow();
            bool? ret = expWin.ShowDialog();
            if (ret.HasValue && ret.Value)
            {
                // export
                String script = Templates.Template.exportScript(expWin.Exporter);

                using (FileStream fs = System.IO.File.Open(expWin.SaveFile.Text, FileMode.Create))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.Write(script);
                        sw.Flush();
                    }
                    fs.Close();
                }
            }
        }

        public void registerTemplate()
        {
            if (Templates.Template.IsMacro)
            {
                MessageHandler.registerMacro(Templates.Template.Name, Templates.Template);
            }
            else
            {
                MessageHandler.registerScript(Templates.Template.Name, Templates.Template);
            }
        }

        public void runTemplate()
        {
            //CogaenScriptingExporter exporter = new CogaenScriptingExporter();
            if (CurrentProject == null)
            {
                System.Windows.MessageBox.Show("No project set.", "Error while exporting", MessageBoxButton.OK);
                return;
            }
            IScriptExporter exporter = CurrentProject.Exporter;
            if (exporter == null)
            {
                System.Windows.MessageBox.Show("Current project has no exporter set.", "Error while exporting", MessageBoxButton.OK);
                return;
            }

            // check if game objects already exist
            ObjectBuilder active = Templates.Template;
            if (active.ScriptObjects.Count <= 0)
            {
                System.Windows.MessageBox.Show("Template is empty.", "Error while exporting", MessageBoxButton.OK);
                return;
            }
            String names = "";
            foreach (GameObject go in active.ScriptObjects)
            {
                if (!go.AutoId)
                {
                    names += go.Name + separator;
                }
            }
            names = names.TrimEnd(separator);
            if (names.Length >= 1)
            {
                //updateLiveGameobjectData();
                MessageHandler.runScriptOnce(Templates.Template.Name, Templates.Template);
            }
            else
            {
                //registerScript(Templates.Template.Name, Templates.Template);
                MessageHandler.runScriptOnce(Templates.Template.Name, Templates.Template);
            }
        }

        public void closeTemplate()
        {
            if (Templates.Template.Dirty)
            {
                MessageBoxResult result = System.Windows.MessageBox.Show("Do you want to save this Template before closing", "Save?", MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes)
                {
                    saveTemplate(CurrentProject != null);
                    Templates.remove(Templates.Template);
                    MainWindow mw = this.MainWindow as MainWindow;
                    mw.sortMacros();
                }
                else if (result == MessageBoxResult.No)
                {
                    foreach (IScriptObject pSo in Templates.Template.ScriptObjects)
                    {
                        if (pSo is GameObject)
                        {
                            GameObject pGo = pSo as GameObject;
                        }
                        else if (pSo is MacroRegistration)
                        {
                            MacroRegistration mr = pSo as MacroRegistration;
                            mr.Script.IsRegistered = false;
                        }
                    }
                    Templates.Templates.Remove(Templates.Template);
                    MainWindow mw = this.MainWindow as MainWindow;
                    mw.sortMacros();
                }
            }
            else
            {
                foreach (IScriptObject pSo in Templates.Template.ScriptObjects)
                {
                    if (pSo is GameObject)
                    {
                        GameObject pGo = pSo as GameObject;
                    }
                    else if (pSo is MacroRegistration)
                    {
                        MacroRegistration mr = pSo as MacroRegistration;
                        mr.Script.IsRegistered = false;
                    }
                }
                Templates.Templates.Remove(Templates.Template);
                MainWindow mw = this.MainWindow as MainWindow;
                mw.sortMacros();
            }
        }

        public void copy()
        {
            m_clipboardObject = ObjectBuilder.ActiveObject;
        }

        public void paste()
        {
            ObjectBuilder.addScriptObjectCopy(m_clipboardObject, Templates.Template);
        }

        public void delete()
        {
            ObjectBuilder.deleteScriptObject(ObjectBuilder.ActiveObject);
        }
        #endregion

        #region gameobjects
        public GameObject newGameObject()
        {
            StringQueryItem qry = new StringQueryItem("Enter new GameObject name", "New GameObject", "New");
            StringQuery q = new StringQuery();
            q.DataContext = qry;
            bool? result = q.ShowDialog();
            if (result.Value)
            {
                GameObject go = ObjectBuilder.newGameObject(qry.Text);
                go.ParentObjectBuilder = ObjectBuilder;
                ObjectBuilder.Dirty = true;
                return go;
            }
            return null;
        }
        #endregion

        #region macros
        public void convertScriptToMacro()
        {
            ObjectBuilder template = ObjectBuilder;
            if (template != null && !template.IsMacro)
            {
                Templates.Templates.Remove(template);
                template.IsMacro = true;
                Templates.Templates.Add(template);
                MainWindow mw = MainWindow as MainWindow;
                if (mw != null)
                {
                    mw.TemplateTab.SelectedItem = template;
                }
            }
        }

        public void convertMacroToScript()
        {
            ObjectBuilder macro = ObjectBuilder;
            if (macro != null && macro.IsMacro)
            {
                Templates.Templates.Remove(macro);
                macro.IsMacro = false;
                Templates.Templates.Add(macro);
                MainWindow mw = MainWindow as MainWindow;
                if (mw != null)
                {
                    mw.TemplateTab.SelectedItem = macro;
                }
            }
        }

        public void registerMacroInScript(ObjectBuilder macro)
        {
            if (macro.IsRegistered)
            {
                System.Windows.MessageBox.Show("Macro is already registered!", "Error");
            }
            else if (macro == ObjectBuilder)
            {
                System.Windows.MessageBox.Show("Cannot register macro within itself!", "Error");
            }
            else
            {
                MacroRegistration macroReg = new MacroRegistration();
                StringQueryItem qry = new StringQueryItem("Enter new macro name", "New Macro", "Macro");
                StringQuery q = new StringQuery();
                q.DataContext = qry;
                bool? result = q.ShowDialog();
                if (result.Value)
                {
                    macro.IsRegistered = true;
                    macro.RegisteredName = qry.Text;
                    macroReg.Name = qry.Text;
                    macroReg.Script = macro;
                    ObjectBuilder.ScriptObjects.Add(macroReg);
                }
            }
        }

        public void addParameterToMacro()
        {
            Parameter p = new Parameter();
            ParameterWindow paramWin = new ParameterWindow();
            paramWin.DataContext = p;
            bool? ret = paramWin.ShowDialog();
            if (ret.HasValue && ret.Value)
            {
                ObjectBuilder.Parameters.Add(p);

                // TODO
                // check all macrocalls and update parameters
            }
        }
        #endregion

        #region exporting
        public IScriptExporter getExporter(string extension)
        {
            XnaXmlScriptExporter xml = new XnaXmlScriptExporter();
            if (extension == xml.Extension)
            {
                return xml;
            }

            CogaenScriptingExporter cogaen = new CogaenScriptingExporter();
            if (extension == cogaen.Extension)
            {
                return cogaen;
            }

            return null;
        }
        #endregion

        #region closing
        public bool canClose()
        {
            bool close = true;
            //if (CurrentProject != null && CurrentProject.Dirty)
            //{
            //    MessageBoxResult result = MessageBox.Show("Do you want to save this Project before closing", "Save?", MessageBoxButton.YesNoCancel);
            //    if (result == MessageBoxResult.Yes)
            //    {
            //        saveCurrentProject();
            //    }
            //    else if (result == MessageBoxResult.Cancel)
            //    {
            //        close = false;
            //    }
            //}

            //foreach (ObjectBuilder obj in Templates.Templates)
            //{
            //    if (obj.Dirty)
            //    {
            //        MessageBoxResult result = MessageBox.Show("Do you want to save this Template before closing", "Save?", MessageBoxButton.YesNoCancel);
            //        if (result == MessageBoxResult.Yes)
            //        {
            //            saveTemplate(CurrentProject != null);
            //        }
            //        else if (result == MessageBoxResult.Cancel)
            //        {
            //            close = false;
            //        }
            //    }
            //}
            return close;
        }
        #endregion
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            m_MessageHandler.stop();
            m_connection.send("DONE", CMessage.Done, null, null);
        }

        #endregion

        #region INotifyPropertyChanged Members

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

        #region version
        public class Version
        {
            private int m_major = 1;
            private int m_minor = 0;
            private int m_rev = 0;

            public Version()
            {
            }

            public Version(int major, int minor, int rev)
            {
                m_major = major;
                m_minor = minor;
                m_rev = rev;
            }

            public void serialize(System.IO.BinaryWriter bw)
            {
                bw.Write(m_major);
                bw.Write(m_minor);
                bw.Write(m_rev);
            }

            public void deserialize(System.IO.BinaryReader br)
            {
                m_major = br.ReadInt32();
                m_minor = br.ReadInt32();
                m_rev = br.ReadInt32();
            }
        }
        #endregion

        #region recent files
        public class RecentFile : INotifyPropertyChanged
        {
            #region member
            String filename;

            public String Filename
            {
                get { return filename; }
                set { filename = value; OnPropertyChanged("Filename"); }
            }
            String fullFilename;

            public String FullFilename
            {
                get { return fullFilename; }
                set { fullFilename = value; OnPropertyChanged("FullFilename"); }
            }
            #endregion

            #region CDtors
            public RecentFile(String file, String path)
            {
                Filename = file;
                FullFilename = path;
            }
            #endregion

            public override string ToString()
            {
                return Filename;
            }

            #region INotifyPropertyChanged Members
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
        public class RecentFiles : INotifyPropertyChanged
        {
            #region memeber
            List<RecentFile> m_recentFiles = new List<RecentFile>(10);

            public List<RecentFile> Recent
            {
                get { return m_recentFiles; }
                set
                {
                    m_recentFiles = value;
                    OnPropertyChanged("Recent");
                }
            }

            private string defaultfile = "./recent.txt";
            #endregion

            #region CDtors
            public RecentFiles()
            {
                try
                {
                    using (FileStream recentFilesFile = File.Open(defaultfile, FileMode.Open))
                    {
                        using (BinaryReader br = new BinaryReader(recentFilesFile))
                        {
                            String file;
                            String path;
                            short index = 0;
                            try
                            {
                                do
                                {
                                    file = br.ReadString();
                                    path = br.ReadString();
                                    m_recentFiles.Add(new RecentFile(file,path));
                                    ++index;
                                    if (index == 10)
                                        break;
                                }
                                while (file != null);
                            }
                            catch (Exception)
                            {
                            }
                        }
                        recentFilesFile.Close();
                    }
                }
                catch (Exception)
                {
                }
            }

            ~RecentFiles()
            {
                save(defaultfile);
            }
            #endregion

            #region methods
            public void newRecentFile(String filename, String path)
            {
                RecentFile file = new RecentFile(filename, path);
                if (m_recentFiles.Contains(file))
                {
                    int index = m_recentFiles.IndexOf(file);
                    m_recentFiles.Remove(file);
                }
                m_recentFiles.Insert(0, file);

                OnPropertyChanged("Recent");

                save(defaultfile);
            }

            private void save(string name)
            {
                using (Stream recentFilesFile = File.Open(name, FileMode.Create))
                {
                    using (BinaryWriter bw = new BinaryWriter(recentFilesFile))
                    {
                        for (short i = 0; i < 10; ++i)
                        {
                            if (m_recentFiles.Count > i && m_recentFiles[i] != null)
                            {
                                bw.Write(m_recentFiles[i].Filename);
                                bw.Write(m_recentFiles[i].FullFilename);
                            }
                            else
                                break;
                        }
                    }
                }
            }
            #endregion

            #region INotifyPropertyChanged Members

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
        #endregion
    }
}
