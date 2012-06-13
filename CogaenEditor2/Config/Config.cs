using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.IO;
using System.ComponentModel;
using CogaenEditor2.GUI;
using CogaenDataItems.DataItems;

namespace CogaenEditor2.Configuration
{
    #region option types
    public abstract class IOption
    {
        public enum OptionType
        {
            BOOLEAN,
            STRING,
            STRINGSEMANTIC,
            FOLDER,
            LIST
        }
        protected String m_name = "";
        public String Name 
        {
            get { return m_name; }
            set { m_name = value; }
        }
        private String m_tooltip;

        protected String Tooltip
        {
            get { return m_tooltip; }
            set { m_tooltip = value; }
        }

        public IOption(String name, String toolTip)
        {
            m_name = name;
            m_tooltip = toolTip;
        }

        public abstract OptionType Type
        {
            get;
        }

        public virtual void serialize(System.IO.BinaryWriter bw)
        {
            bw.Write(Type.ToString());
            bw.Write(m_name);
            bw.Write(m_tooltip);
        }
        public virtual void deserialize(System.IO.BinaryReader br)
        {
            m_name = br.ReadString();
            m_tooltip = br.ReadString();
        }
    }

    public class BoolOption : IOption
    {
        public const OptionType m_type = OptionType.BOOLEAN;

        public override OptionType Type
        {
            get { return m_type; }
        }

        private bool m_value = false;

        public bool Value
        {
            get { return m_value; }
            set { m_value = value; }
        }

        public BoolOption(String name,String tooltip, bool value) : base(name, tooltip)
        {
            m_value = value;
        }

        #region de/serialzation
        public override void serialize(System.IO.BinaryWriter bw)
        {
            base.serialize(bw);
            bw.Write(m_value);
        }

        public override void deserialize(System.IO.BinaryReader br)
        {
            base.deserialize(br);
            m_value = br.ReadBoolean();
        }
        #endregion
    }

    public class StringOption : IOption
    {
        public const OptionType m_type = OptionType.STRING;

        public override OptionType Type
        {
            get { return m_type; }
        }

        private String m_value = "";

        public String Value
        {
            get { return m_value; }
            set { m_value = value; }
        }

        public StringOption(String name, String tooltip, String value) : base(name, tooltip)
        {
            m_value = value;
        }

        #region de/serialzation
        public override void serialize(System.IO.BinaryWriter bw)
        {
            base.serialize(bw);
            bw.Write(m_value);
        }

        public override void deserialize(System.IO.BinaryReader br)
        {
            base.deserialize(br);
            m_value = br.ReadString();
        }
        #endregion
    }

    public class StringSemanticOption : IOption
    {
        public const OptionType m_type = OptionType.STRINGSEMANTIC;

        public override OptionType Type
        {
            get { return m_type; }
        }

        private String m_value = "";

        public String Value
        {
            get { return m_value; }
            set { m_value = value; }
        }

        private ParameterSemantic m_semantic = ParameterSemantic.NONE;

        public ParameterSemantic Semantic
        {
            get { return m_semantic; }
            set { m_semantic = value; }
        }

        public StringSemanticOption(String name, String tooltip, String value, ParameterSemantic semantic)
            : base(name, tooltip)
        {
            m_value = value;
            m_semantic = semantic;
        }

        #region de/serialzation
        public override void serialize(System.IO.BinaryWriter bw)
        {
            base.serialize(bw);
            bw.Write(m_value);
            bw.Write(ParameterTypeName.SemanticToString(m_semantic));
        }

        public override void deserialize(System.IO.BinaryReader br)
        {
            base.deserialize(br);
            m_value = br.ReadString();
            m_semantic = ParameterTypeName.SemanticFromString(br.ReadString());
        }
        #endregion
    }

    public class ListOption : IOption, IHasEntries
    {
        public const OptionType m_type = OptionType.LIST;

        public override OptionType Type
        {
            get { return m_type; }
        }

        private int m_selectedItem = -1;

        private ObservableCollection<String> m_value = new ObservableCollection<String>();

        public ObservableCollection<String> Value
        {
            get { return m_value; }
            set { m_value = value; }
        }

        public int SelectedItem
        {
            get { return m_selectedItem; }
            set { m_selectedItem = value; }
        }

        public ListOption(String name, String tooltip)
            : base(name, tooltip)
        {
        }

        public void remove(int entry)
        {
            if (m_selectedItem >= 0)
            {
                m_value.RemoveAt(entry);
            }
        }

        #region de/serialzation
        public override void serialize(System.IO.BinaryWriter bw)
        {
            base.serialize(bw);
            bw.Write(m_value.Count);
            foreach (String option in m_value)
            {
                bw.Write(option);
            }
        }

        public override void deserialize(System.IO.BinaryReader br)
        {
            base.deserialize(br);
            int num = br.ReadInt32();
            for (int i = 0; i < num; ++i)
            {
                m_value.Add(br.ReadString());
            }
        }
        #endregion
    }

    public class FolderOption : IOption, INotifyPropertyChanged
    {
        public const OptionType m_type = OptionType.FOLDER;

        public override OptionType Type
        {
            get { return m_type; }
        }

        private String m_value = "";

        public String Value
        {
            get { return m_value; }
            set 
            { 
                m_value = value;
                OnPropertyChanged("Value");
            }
        }

        public FolderOption(String name, String tooltip, String value)
            : base(name, tooltip)
        {
            m_value = value;
        }

        #region de/serialzation
        public override void serialize(System.IO.BinaryWriter bw)
        {
            base.serialize(bw);
            bw.Write(m_value);
        }

        public override void deserialize(System.IO.BinaryReader br)
        {
            base.deserialize(br);
            m_value = br.ReadString();
        }
        #endregion

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

    #endregion


    public class OptionGroup
    {
        private String m_name;
        private ObservableCollection<IOption> m_options = new ObservableCollection<IOption>();
        public String Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        public ObservableCollection<IOption> Options
        {
            get { return m_options; }
            set { m_options = value; }
        }

        public OptionGroup() { }

        public OptionGroup(String name)
        {
            m_name = name;
        }

        #region de/serialzation
        public void serialize(System.IO.BinaryWriter bw)
        {
            bw.Write(m_name);
            bw.Write(m_options.Count);
            foreach (IOption option in m_options)
            {
                option.serialize(bw);
            }
        }

        public void deserialize(System.IO.BinaryReader br)
        {
            m_name = br.ReadString();
            int numOptions = br.ReadInt32();
            for (int i = 0; i < numOptions; ++i)
            {
                IOption option = null;
                String type = br.ReadString();
                if (type == IOption.OptionType.BOOLEAN.ToString())
                {
                    option = new BoolOption("", "", false);
                }
                else if (type == IOption.OptionType.STRING.ToString())
                {
                    option = new StringOption("", "", "");
                }
                else if (type == IOption.OptionType.STRINGSEMANTIC.ToString())
                {
                    option = new StringSemanticOption("", "", "",ParameterSemantic.NONE);
                }
                else if (type == IOption.OptionType.LIST.ToString())
                {
                    option = new ListOption("","");
                }
                else if (type == IOption.OptionType.FOLDER.ToString())
                {
                    option = new FolderOption("", "","");
                }
                if (option != null)
                {
                    option.deserialize(br);
                    m_options.Add(option);
                }
            }
        }
        #endregion
    }

    public class Config
    {

        #region member
        private static String m_defaultName = "config.cfg";

        public static String DefaultName
        {
            get { return Config.m_defaultName; }
            set { Config.m_defaultName = value; }
        }

        private static Config m_instance = null;

        private ObservableCollection<OptionGroup> m_options = new ObservableCollection<OptionGroup>();

        public ObservableCollection<OptionGroup> Options
        {
            get { return m_options; }
            set { m_options = value; }
        }
        #endregion

        #region CDtors
        private Config()
        {

        }
        #endregion

        public static Config defaultConfig()
        {
            Config conf = new Config();

            #region 3DEditor
            OptionGroup _3dOptions = new OptionGroup("3D Editor Options");

            _3dOptions.Options.Add(new ListOption("Model Component", "The Id of the Component that should be used for mesh data."));
            _3dOptions.Options.Add(new StringSemanticOption("Model Parameter", "The name of the parameter that should be used for mesh data. Separate multiple entries with ','", "model,mesh",ParameterSemantic.MESH));
            _3dOptions.Options.Add(new StringSemanticOption("Material Parameter", "The name of the parameter that should be used for material data. Separate multiple entries with ','", "material", ParameterSemantic.MATERIAL));
            _3dOptions.Options.Add(new ListOption("Position Component", "The Id of the Component that should be used for position data."));
            _3dOptions.Options.Add(new StringSemanticOption("Position Parameter", "The name of the parameter that should be used for position data. Separate multiple entries with ','", "position", ParameterSemantic.POSITION3D));
            _3dOptions.Options.Add(new ListOption("Orientation Component", "The Id of the Component that should be used for orientation data."));
            _3dOptions.Options.Add(new StringSemanticOption("Orientation Parameter", "The name of the parameter that should be used for orientation data. Separate multiple entries with ','", "orientation", ParameterSemantic.ORIENTATION3D));
            _3dOptions.Options.Add(new ListOption("Scale Component", "The Id of the Component that should be used for scaling data."));
            _3dOptions.Options.Add(new StringSemanticOption("Scale Parameter", "The name of the parameter that should be used for scaling data. Separate multiple entries with ','", "scale", ParameterSemantic.DIMENSION3D));
            _3dOptions.Options.Add(new ListOption("Camera Component", "The Id of the Component that should be used for Camera data."));

            conf.Options.Add(_3dOptions);
            #endregion

            #region StateMachine
            OptionGroup _StateMachines = new OptionGroup("StateMachines");
            _StateMachines.Options.Add(new ListOption("StateMachine Component", "The Id of the Componenet that can be used as a state machine."));
            _StateMachines.Options.Add(new StringSemanticOption("InitState Parameter", "The name of the parameter that specifies the initial state of the state machine. Separate multiple entries with ','", "initState", ParameterSemantic.INITSTATE));
            _StateMachines.Options.Add(new StringSemanticOption("Transitions Parameter", "The name of the parameter that holds all the transition data. Separate multiple entries with ','", "transitions", ParameterSemantic.TRANSITIONS));
            _StateMachines.Options.Add(new StringSemanticOption("Transition Parameter", "The name of the parameter that should be used for a ingle transition.", "transition", ParameterSemantic.TRANSITION));
            _StateMachines.Options.Add(new StringSemanticOption("Message Parameter", "The name of the parameter that holds the message id for a transition.", "msg", ParameterSemantic.MSG));
            _StateMachines.Options.Add(new StringSemanticOption("From Parameter", "The name of the parameter that specifies the 'From' state.", "transition", ParameterSemantic.FROM));
            _StateMachines.Options.Add(new StringSemanticOption("To Parameter", "The name of the parameter that specifies the 'To' state.", "transition", ParameterSemantic.TO));
            _StateMachines.Options.Add(new ListOption("State Component", "The Id of the Componenet that can be used as a state. Separate multiple entries with ','"));

            conf.Options.Add(_StateMachines);
            #endregion

            #region Paths
            OptionGroup _PathOptions = new OptionGroup("Paths");
            _PathOptions.Options.Add(new FolderOption("Export Path", "Default path for exporting.", Environment.CurrentDirectory + "\\Templates\\"));
            _PathOptions.Options.Add(new FolderOption("Save Path", "Default path for saving.", Environment.CurrentDirectory + "\\Scripts\\"));

            conf.Options.Add(_PathOptions);
            #endregion

            FileStream fs = File.Open(m_defaultName, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            conf.serialize(bw);
            bw.Flush();
            fs.Close();
            return conf;
        }

        /// <summary>
        /// Loads the config from a file or generates a new one
        /// </summary>
        /// <returns></returns>
        public static Config getConfig()
        {
            if (m_instance != null)
                return m_instance;

            m_instance = new Config();
            if (File.Exists(m_defaultName))
            {
                FileStream fs = File.Open(m_defaultName, FileMode.Open);
                if (fs != null)
                {
                    BinaryReader br = new BinaryReader(fs);
                    m_instance.deserialize(br);
                    fs.Close();
                }
                return m_instance;
            }
            else
            {
                m_instance = defaultConfig();
                return m_instance;
            }
        }

        /// <summary>
        /// Returns the option with the specified name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IOption getOption(String name)
        {
            foreach (OptionGroup optionGroup in m_options)
            {
                foreach (IOption option in optionGroup.Options)
                {
                    if (option.Name == name)
                    {
                        return option;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Return a specific option.
        /// </summary>
        /// <param name="name">Name of the option</param>
        /// <param name="group">Name of the option group to look in.</param>
        /// <returns></returns>
        public IOption getOption(String name, String group)
        {
            foreach (OptionGroup optionGroup in m_options)
            {
                if (optionGroup.Name == group)
                {
                    foreach (IOption option in optionGroup.Options)
                    {
                        if (option.Name == name)
                        {
                            return option;
                        }
                    }
                }
            }
            return null;
        }

        public OptionGroup GetOptionGroup(String name)
        {
            foreach (OptionGroup optionGroup in m_options)
            {
                if (optionGroup.Name == name)
                {
                    return optionGroup;
                }
            }
            return null;
        }

        public void save()
        {
            FileStream fs = File.Open(m_defaultName, FileMode.Create);
            serialize(new BinaryWriter(fs));
            fs.Close();
        }

        public void saveAs(String filename)
        {
            FileStream fs = File.Open(filename, FileMode.Create);
            serialize(new BinaryWriter(fs));
            fs.Close();
        }


        #region de/serialzation
        public void serialize(System.IO.BinaryWriter bw)
        {
            bw.Write(m_options.Count);
            foreach (OptionGroup option in m_options) 
            {
                option.serialize(bw);
            }
            bw.Flush();
        }

        public void deserialize(System.IO.BinaryReader br)
        {
            int numOptions = br.ReadInt32();
            for (int i = 0; i < numOptions; ++i)
            {
                OptionGroup newGroup = new OptionGroup();
                newGroup.deserialize(br);
                m_options.Add(newGroup);
            }
        }
        #endregion
    }
}
