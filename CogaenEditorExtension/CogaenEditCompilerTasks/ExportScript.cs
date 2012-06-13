using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.IO;
using CogaenEditExporterManager;
using CogaenDataItems.Manager;

namespace CogaenEditCompilerTasks
{
    public class ExportScript : Task
    {
        public override bool Execute()
        {
            // look for an exporter
            if (m_expManager.Exporter.TryGetValue(Path.GetFileNameWithoutExtension(Exporter), out m_exporter))
            {
                return export();
            }
            else
            {
                getExporter(Exporter);
                if (m_expManager.Exporter.TryGetValue(Path.GetFileNameWithoutExtension(Exporter), out m_exporter))
                {
                    return export();
                }
            }
            return false;
        }

        private bool export()
        {
            try
            {
                foreach (string file in scriptFiles)
                {
                    if (!exportFile(file))
                        throw new Exception();
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        private bool exportFile(string filename)
        {
            // load file into an ObjectBuilder 
            ObjectBuilder objectBuilder = new ObjectBuilder(null);
            objectBuilder.deserializeFromXml(filename);
            string script;
            if (m_exporter.ExportScript(objectBuilder, out script))
            {
                // construct output filename
                StringBuilder expoFileBuilder = new StringBuilder();
                expoFileBuilder.Append(OutputDirectory);
                if (!OutputDirectory.EndsWith("\\"))
                    expoFileBuilder.Append("\\");

                // cerate directory if it doesn't exist yet
                if (!Directory.Exists(expoFileBuilder.ToString()))
                {
                    Directory.CreateDirectory(expoFileBuilder.ToString());
                }

                expoFileBuilder.Append(Path.GetFileNameWithoutExtension(filename));
                if (!m_exporter.ExportExtension.StartsWith("."))
                {
                    expoFileBuilder.Append(".");
                }
                expoFileBuilder.Append(m_exporter.ExportExtension);

                
                // write file to disk
                using (FileStream fs = File.Create(expoFileBuilder.ToString()))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.Write(script);
                    }
                }
                return true;
            }
            return false;
        }

        private bool getExporter(string exporterName)
        {
            StringBuilder exporterFilenameBuilder = new StringBuilder();

            if (base.BuildEngine != null)
            {
                exporterFilenameBuilder.Append(Path.GetDirectoryName(base.BuildEngine.ProjectFileOfTaskNode));
                exporterFilenameBuilder.Append("\\");
            }
            exporterFilenameBuilder.Append(exporterName);

            if (Path.GetExtension(exporterName).CompareTo(".dll") == 0)
            {
            }
            else if (Path.GetExtension(exporterName).Length == 0)
            {
                exporterFilenameBuilder.Append(".dll");
            }
            else
                return false;

            m_expManager.loadExporterDll(exporterFilenameBuilder.ToString());

            return true;
        }

        private string[] scriptFiles;
        [Required]
        public string[] ScriptFiles
        {
            get { return scriptFiles; }
            set { scriptFiles = value; }
        }

        private string m_exporterName;
        [Required]
        public string Exporter
        {
            get { return m_exporterName; }
            set { m_exporterName = value; }
        }

        private string m_OutputDirectory;
        [Required]
        public string OutputDirectory
        {
            get { return m_OutputDirectory; }
            set { m_OutputDirectory = value; }
        }

        private ExternalExporter m_exporter = null;
        CogaenEditExporterManager.CogaenEditExporterManager m_expManager = new CogaenEditExporterManager.CogaenEditExporterManager();
    }
}
