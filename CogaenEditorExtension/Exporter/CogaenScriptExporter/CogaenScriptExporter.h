// CogaenScriptExporter.h
#ifndef COGAEN_SCRIPT_EXPORTER_H_
#define COGAEN_SCRIPT_EXPORTER_H_

using namespace System;

#define DLLEXPORT __declspec(dllexport)

    extern "C" __declspec(dllexport)
    void ExportScript();


namespace CogaenScriptExporter {

	public class CogaenScriptExporter
	{
		// TODO: Add your methods for this class here.
	};
}
#endif