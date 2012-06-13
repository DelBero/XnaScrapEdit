// Guids.cs
// MUST match guids.h
using System;

namespace BeroInc.CBeroEditors
{
    static class GuidList
    {
        public const string guidCBeroEditorsPkgString = "4b790098-555b-4c80-9200-5da93cdf2070";
        public const string guidCBeroEditorsCmdSetString = "22df3c35-b2a9-4331-83d7-d08ff6f018dc";
        public const string guidCBeroEditorsEditorFactoryString = "36fc57f8-2110-43f3-91ac-03ce7473008d";

        public static readonly Guid guidCBeroEditorsCmdSet = new Guid(guidCBeroEditorsCmdSetString);
        public static readonly Guid guidCBeroEditorsEditorFactory = new Guid(guidCBeroEditorsEditorFactoryString);
    };
}