/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using Microsoft.VisualStudio.Project;
using Microsoft.Build.Evaluation;

namespace CogaenEditExtension
{

	/// <summary>
	/// All public properties on Nodeproperties or derived classes are assumed to be used by Automation by default.
	/// Set this attribute to false on Properties that should not be visible for Automation.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class AutomationBrowsableAttribute : System.Attribute
	{
		public AutomationBrowsableAttribute(bool browsable)
		{
			this.browsable = browsable;
		}

		public bool Browsable
		{
			get
			{
				return this.browsable;
			}
		}

		private bool browsable;
	}


	[CLSCompliant(false), ComVisible(true)]
    public class CEFileNodeProperties : SingleFileGeneratorNodeProperties
	{

        #region fields
        CEBuildAction ceBuildAction = CEBuildAction.Compile;
        #endregion

		#region properties
        //[Browsable(true)]
        //[CategoryAttribute("Advanced")]
        //[DisplayNameAttribute("Export")]
        //[DescriptionAttribute("Export this file as a script.")]
        //public bool Export
        //{
        //    get
        //    {
        //        switch (this.ceBuildAction)
        //        {
        //            case CEBuildAction.Compile:
        //                return true;
        //            default:
        //                return false;
        //        }
        //    }
        //    set
        //    {
        //        this.ceBuildAction = value?CEBuildAction.Compile:CEBuildAction.None;
        //    }
        //}

        [Browsable(true)]
        public override BuildAction BuildAction
        {
            get
            {
                return base.BuildAction;
            }
            set
            {
                base.BuildAction = value;
            }
        }

        [Browsable(false)]
        public override string CustomTool
        {
            get
            {
                return base.CustomTool;
            }
            set
            {
                base.CustomTool = value;
            }
        }

        [Browsable(false)]
        public override string CustomToolNamespace
        {
            get
            {
                return base.CustomToolNamespace;
            }
            set
            {
                base.CustomToolNamespace = value;
            }
        }

        ////[SRCategoryAttribute(SR.Advanced)]
        ////[LocDisplayName(SR.BuildAction)]
        ////[SRDescriptionAttribute(SR.BuildActionDescription)]
        //public virtual BuildAction BuildAction
        //{
        //    get
        //    {
        //        string value = null;//this.Node.ItemNode.ItemName;
        //        if(value == null || value.Length == 0)
        //        {
        //            return BuildAction.None;
        //        }
        //        return (BuildAction)Enum.Parse(typeof(BuildAction), value);
        //    }
        //    set
        //    {
        //        //this.Node.ItemNode.ItemName = value.ToString();
        //    }
        //}

		#region non-browsable properties - used for automation only
		[Browsable(false)]
		public string Extension
		{
			get
			{
				return Path.GetExtension(this.Node.Caption);
			}
		}
		#endregion

		#endregion

		#region ctors
        public CEFileNodeProperties(HierarchyNode node)
			: base(node)
		{
		}
		#endregion

		#region overridden methods
		public override string GetClassName()
		{
            return "";// SR.GetString(SR.FileProperties, CultureInfo.CurrentUICulture);
		}
		#endregion
	}


    #region converter
    [CLSCompliant(false), ComVisible(true)]
    public class ScriptExporterConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(
                           ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection
                     GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(CogaenEditExtensionPackage.ExporterManager.ExporterNames);
        }

        //public override bool GetStandardValuesExclusive(
        //                   ITypeDescriptorContext context)
        //{
        //    return true;
        //}
    }

    [CLSCompliant(false), ComVisible(true)]
    public class ContentFoldersConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(
                           ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection
                     GetStandardValues(ITypeDescriptorContext context)
        {
            XnaContentReferencesProperties props = context.Instance as XnaContentReferencesProperties;
            if (props != null)
            {
                XnaContentReferenceNode node = props.Node as XnaContentReferenceNode;
                if (node != null)
                    return new StandardValuesCollection(node.BaseFolders);
            }
            return null;
        }

        //public override bool GetStandardValuesExclusive(
        //                   ITypeDescriptorContext context)
        //{
        //    return true;
        //}
    }
    #endregion

    #region special types
    [TypeConverterAttribute(typeof(SpellingOptionsConverter)),
    DescriptionAttribute("Erweitern, um die Rechtschreiboptionen für die Anwendung anzuzeigen.")]
    public class SpellingOptions
    {
        private bool spellCheckWhileTyping = true;
        private bool spellCheckCAPS = false;
        private bool suggestCorrections = true;
        [DefaultValueAttribute(true)]
        public bool SpellCheckWhileTyping
        {
            get { return spellCheckWhileTyping; }
            set { spellCheckWhileTyping = value; }
        }
        [DefaultValueAttribute(false)]
        public bool SpellCheckCAPS
        {
            get { return spellCheckCAPS; }
            set { spellCheckCAPS = value; }
        }
        [DefaultValueAttribute(true)]
        public bool SuggestCorrections
        {
            get { return suggestCorrections; }
            set { suggestCorrections = value; }
        }
    }

    public class SpellingOptionsConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context,
                                            System.Type destinationType)
        {
            if (destinationType == typeof(SpellingOptions))
                return true;
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context,
                                            CultureInfo culture,
                                            object value,
                                            System.Type destinationType)
        {
            if (destinationType == typeof(System.String) &&
            value is SpellingOptions)
            {
                SpellingOptions so = (SpellingOptions)value;
                return "Überprüfung während der Eingabe:" + so.SpellCheckWhileTyping +
                ", GROSSBUCHSTABEN überprüfen: " + so.SpellCheckCAPS +
                ", Korrekturen vorschlagen: " + so.SuggestCorrections;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context,
                                                System.Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context,
                              CultureInfo culture, object value)
        {
            if (value is string)
            {
                try
                {
                    string s = (string)value;
                    int colon = s.IndexOf(':');
                    int comma = s.IndexOf(',');
                    if (colon != -1 && comma != -1)
                    {
                        string checkWhileTyping = s.Substring(colon + 1,
                                                        (comma - colon - 1));
                        colon = s.IndexOf(':', comma + 1);
                        comma = s.IndexOf(',', comma + 1);
                        string checkCaps = s.Substring(colon + 1,
                                                        (comma - colon - 1));
                        colon = s.IndexOf(':', comma + 1);
                        string suggCorr = s.Substring(colon + 1);
                        SpellingOptions so = new SpellingOptions();
                        so.SpellCheckWhileTyping = Boolean.Parse(checkWhileTyping);
                        so.SpellCheckCAPS = Boolean.Parse(checkCaps);
                        so.SuggestCorrections = Boolean.Parse(suggCorr);
                        return so;
                    }
                }
                catch
                {
                    throw new ArgumentException(
                        " '" + (string)value + "' kann nicht in Typ SpellingOptions konvertiert werden");
                }
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
    #endregion

	[CLSCompliant(false), ComVisible(true)]
    public class CEProjectNodeProperties : ProjectNodeProperties
    {
        #region properties
        [Browsable(true),
        Microsoft.VisualStudio.Project.PropertyPageTypeConverterAttribute(typeof(ScriptExporterConverter)),
        CategoryAttribute("Output"),
        DisplayNameAttribute("Exporter"),
        DescriptionAttribute("Exporter to use with this project.")]
        public string Exporter
        {
            get
            {
                string exporterString = this.Node.ProjectMgr.GetProjectProperty("Exporter");
                if (exporterString.Length == 0 && CogaenEditExtensionPackage.ExporterManager.ExporterNames.Count > 0)
                {
                    // choose the first available exporter
                    exporterString = CogaenEditExtensionPackage.ExporterManager.getFirstExporterName();
                    this.Node.ProjectMgr.SetProjectProperty("Exporter", exporterString);
                }
                return exporterString;
            }
            set
            {
                this.Node.ProjectMgr.SetProjectProperty("Exporter", value);
            }
        }
		#endregion

		#region ctors
        public CEProjectNodeProperties(ProjectNode node)
			: base(node)
		{
		}
		#endregion

		#region overridden methods
        public override string GetClassName()
        {
            return "";// SR.GetString(SR.ProjectProperties, CultureInfo.CurrentUICulture);
        }

		/// <summary>
		/// ICustomTypeDescriptor.GetEditor
		/// To enable the "Property Pages" button on the properties browser
		/// the browse object (project properties) need to be unmanaged
		/// or it needs to provide an editor of type ComponentEditor.
		/// </summary>
		/// <param name="editorBaseType">Type of the editor</param>
		/// <returns>Editor</returns>
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", 
        //    Justification="The service provider is used by the PropertiesEditorLauncher")]
        //public override object GetEditor(Type editorBaseType)
        //{
        //    //// Override the scenario where we are asked for a ComponentEditor
        //    //// as this is how the Properties Browser calls us
        //    //if(editorBaseType == typeof(ComponentEditor))
        //    //{
        //    //    IOleServiceProvider sp;
        //    //    ErrorHandler.ThrowOnFailure(this.Node.GetSite(out sp));
        //    //    return new PropertiesEditorLauncher(new ServiceProvider(sp));
        //    //}

        //    return base.GetEditor(editorBaseType);
        //}

        //public override TypeConverter GetConverter()
        //{
        //    return new ScriptExporterConverter();
        //    //return base.GetConverter();
        //}

        public override int GetCfgProvider(out IVsCfgProvider p)
        {
            //if(this.Node != null && this.Node.ProjectManager != null)
            //{
            //    return this.Node.ProjectManager.GetCfgProvider(out p);
            //}

            return base.GetCfgProvider(out p);
        }
		#endregion
	}


    [ComVisible(true)]
    public class XnaContentReferencesProperties : ReferenceNodeProperties
    {
        #region ctors
        public XnaContentReferencesProperties(XnaContentReferenceNode node)
            : base(node)
        {
        }
        #endregion

        #region overriden methods
        [Browsable(false)]
        public override string FullPath
        {
            get
            {
                return ((XnaContentReferenceNode)Node).ReferencedProjectOutputPath;
            }
        }

        [Browsable(true),
        CategoryAttribute("Output"),
        DisplayNameAttribute("Content Project"),
        DescriptionAttribute("The Xna content project where exported scripts should be placed.")]
        public string ContentProject
        {
            get
            {
                return ((XnaContentReferenceNode)Node).ContentProject;
            }
        }

        [Browsable(true),
        Microsoft.VisualStudio.Project.PropertyPageTypeConverterAttribute(typeof(ContentFoldersConverter)),
        CategoryAttribute("Output"),
        DisplayNameAttribute("ScriptFolder"),
        DescriptionAttribute("The folder where to place exported scripts.")]
        public string ScriptFolder
        {
            get
            {
                //string[] folders = ((XnaContentReferenceNode)Node).BaseFolders;
                //return ((XnaContentReferenceNode)Node).ContentOutputFolder;
                return this.Node.ItemNode.GetMetadata("ScriptFolder");
            }
            set
            {
                ((XnaContentReferenceNode)Node).ContentOutputFolder = value;
                this.Node.ItemNode.SetMetadata("ScriptFolder", value);
            }
        }
        #endregion
    }


    public enum CEBuildAction { None, Compile };
}
