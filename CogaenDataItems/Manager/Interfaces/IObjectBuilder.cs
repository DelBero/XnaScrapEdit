using System;
using CogaenDataItems.DataItems;
using System.Collections.Generic;
using System.Windows;
using CogaenDataItems.Exporter;
namespace CogaenDataItems.Manager
{
    public interface IObjectBuilder
    {
        bool Dirty { get; set; }
        bool IsLive { get; set; }
        bool IsMacro { get; set; }
        bool IsRegistered { get; set; }
        Point Offset { get; set; }
        float Scaling { get; set; }
        List<IScriptObject> ActiveObject { get; }
        System.Collections.ObjectModel.ObservableCollection<CogaenDataItems.DataItems.Parameter> Parameters { get; set; }
        string RegisteredName { get; set; }
        System.Collections.ObjectModel.ObservableCollection<CogaenDataItems.DataItems.IScriptObject> ScriptObjects { get; set; }
        void clear();
        void updateUi();
        void RecomputeZOrder();

        void AddParameter(Parameter p);
        void RemoveParameter(Parameter p);

        /// <summary>
        /// picking
        /// </summary>
        /// <param name="p"></param>
        /// <param name="extendSelection"></param>
        /// <param name="forceClear"></param>
        /// <returns></returns>
        IScriptObject click(System.Windows.Point p, bool extendSelection, bool forceClear);

        /// <summary>
        /// Selects all GameObjects withan the specified region.
        /// </summary>
        /// <param name="selection">Region to select.</param>
        /// <returns>All GameObject in the region</returns>
        List<IScriptObject> select(Rect selection);

        /// <summary>
        /// Sort the Objects so that they dont overlap
        /// </summary>
        void sort();

        /// <summary>
        /// Sort from top to bottom
        /// </summary>
        void sortTopDown();

        /// <summary>
        /// Export the objectbuilder using the given ScriptExporter
        /// </summary>
        /// <param name="exporter"></param>
        /// <returns></returns>
        String exportScript(IScriptExporter exporter);

        GameObject newGameObject(String name);
        void addScriptObjectCopy(List<IScriptObject> scriptObjects);
        void deleteScriptObject(List<IScriptObject> scriptObjects);
        void load(string filename);
        void serializeToXml(string filename);

    }
}
