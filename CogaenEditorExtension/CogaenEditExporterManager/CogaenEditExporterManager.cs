using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CogaenDataItems.Exporter;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using CogaenDataItems.Manager;

namespace CogaenEditExporterManager
{
    public class CogaenEditExporterManager : IDisposable
    {
        private Dictionary<string, ExternalExporter> m_exporter = new Dictionary<string, ExternalExporter>();

        public Dictionary<string, ExternalExporter> Exporter
        {
            get { return m_exporter; }
            set { m_exporter = value; }
        }

        public Dictionary<string, ExternalExporter>.KeyCollection ExporterNames
        {
            get { return m_exporter.Keys; }
        }

        public void parseForExporter(string folder)
        {
            string[] files = Directory.GetFiles(folder);
            foreach (string file in files)
            {
                if (Path.GetExtension(file).CompareTo(".dll") == 0)
                {
                    try
                    {
                        loadExporterDll(file);
                    }
                    catch (Exception) { }
                }
            }
        }

        public ExternalExporter getFirstExporter()
        {
            return m_exporter.First().Value;
        }

        public string getFirstExporterName()
        {
            return m_exporter.First().Key;
        }

        public void loadExporterDll(string libraryName)
        {
            ExternalExporter exporter;
            if (CSharpExporter.TryCreateInstance(libraryName, Path.GetFullPath(libraryName), out exporter))
            {
                m_exporter.Add(Path.GetFileNameWithoutExtension(libraryName), exporter);
            }
            else if (CppExporter.TryCreateInstance(libraryName, Path.GetFullPath(libraryName), out exporter))
            {
                m_exporter.Add(Path.GetFileNameWithoutExtension(libraryName), exporter);
            }
        }

        public virtual void Dispose()
        {
            
        }
    }

    public static class NativMethods
    {
        [DllImport("Invoke", CharSet = CharSet.Unicode)]
        public extern static int InvokeFunc(IntPtr funcptr, IntPtr hModule,
                                            string message, string title, int flags);

        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string fileName);

        [DllImport("Kernel32.dll", CharSet = CharSet.Ansi)]
        public static extern int GetProcAddress(IntPtr hModule, string FunctionName);
        //public static extern System.IntPtr GetProcAddress(IntPtr hModule, string FunctionName);

        [DllImport("kernel32.dll")]
        public static extern bool FreeLibrary(IntPtr hModule);
    }

    public abstract class ExternalExporter : IDisposable
    {
        public abstract string ExportExtension { get; }
        public abstract bool ExportScript(IObjectBuilder script, out string scriptString);


        public abstract void Dispose();
    }

    public class CSharpExporter : ExternalExporter
    {
        #region c sharp
        private Assembly Library = null;
        private IScriptExporter m_exporter = null;

        public IScriptExporter Exporter
        {
            get { return m_exporter; }
        }
        #endregion

        public override string ExportExtension { get{ return "xmlScript";} }

        public static bool TryCreateInstance(string dllName, string fullPath, out ExternalExporter exporter)
        {
            exporter = null;
            try
            {
                Assembly Library = Assembly.LoadFrom(fullPath);
                // look for a IScriptExporter
                Type[] types = Library.GetTypes();
                foreach (Type type in types)
                {
                    if (type.GetInterface(typeof(IScriptExporter).ToString()) != null)
                    {
                        IScriptExporter scriptExporter = Library.CreateInstance(type.ToString()) as IScriptExporter;
                        CSharpExporter newExporter = new CSharpExporter();
                        newExporter.Library = Library;
                        newExporter.m_exporter = scriptExporter;

                        exporter = newExporter;
                        return true;
                    }
                }
            }
            catch(Exception)
            {
                return false;
            }

            return false;
        }

        public override void Dispose()
        {
            
        }

        public override bool ExportScript(IObjectBuilder script, out string scriptString)
        {
            scriptString = script.exportScript(this.Exporter);
            return scriptString.Length != 0;
        }
    }

    public class CppExporter : ExternalExporter
    {
        #region c++
        private IntPtr LibraryHandle = IntPtr.Zero;
        private IntPtr ExportScriptFunc = IntPtr.Zero;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate string ExportScriptDelegate(IntPtr template);

        private ExportScriptDelegate exportScript = null;
        #endregion

        public override string ExportExtension { get { return "csl"; } }

        public static bool TryCreateInstance(string dllName, string fullPath, out ExternalExporter exporter)
        {
            CppExporter cppExporter = new CppExporter();
            exporter = cppExporter;
            try
            {
                if (cppExporter.LibraryHandle == IntPtr.Zero)
                {
                    cppExporter.LibraryHandle = NativMethods.LoadLibrary(dllName);
                }

                if (cppExporter.LibraryHandle != IntPtr.Zero)
                {
                    cppExporter.ExportScriptFunc = new IntPtr(NativMethods.GetProcAddress(cppExporter.LibraryHandle, "ExportScript"));
                    if (cppExporter.ExportScriptFunc == IntPtr.Zero)
                        throw new Exception("");

                    cppExporter.exportScript = (ExportScriptDelegate)Marshal.GetDelegateForFunctionPointer(
                                                                                        cppExporter.ExportScriptFunc,
                                                                                        typeof(ExportScriptDelegate));

                    exporter = cppExporter;
                }
            }
            catch (Exception)
            {
                NativMethods.FreeLibrary(cppExporter.LibraryHandle);
                cppExporter = null;
                exporter = null;
                return false;
            }

            return cppExporter.LibraryHandle != IntPtr.Zero;
        }

        public override void Dispose()
        {
            if (LibraryHandle != IntPtr.Zero)
            {
                NativMethods.FreeLibrary(LibraryHandle);
            }
        }

        public override bool ExportScript(IObjectBuilder script, out string scriptString)
        {
            scriptString = "";
            //exportScript(Marshal.GetIUnknownForObject(script));
            return true;
        }
    }
}
