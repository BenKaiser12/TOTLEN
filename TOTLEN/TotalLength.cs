using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sio = System.IO;
using sw = System.Windows;


using Autodesk.AutoCAD;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using AcAp = Autodesk.AutoCAD.ApplicationServices.Application;
using acw = Autodesk.AutoCAD.Windows;
using System.IO;

namespace TOTLEN
{
    public class Commands : IExtensionApplication
    {
        void IExtensionApplication.Initialize()
        {
            ContextMenu.Attach();
        }

        void IExtensionApplication.Terminate()
        {
            ContextMenu.Detach();
        }

        [CommandMethod("TOTLEN", CommandFlags.UsePickSet)]
        static public void TotalLength()
        {
            var acDoc = AcAp.DocumentManager.MdiActiveDocument;
            var acDb = acDoc.Database;
            var ed = acDoc.Editor;

            // build a selection filter to get only measurable entities
            var filter = new SelectionFilter(new[]
            {
                new TypedValue(-4, "<OR"),
                    new TypedValue(0, "ARC,CIRCLE,ELLIPSE,LINE,LWPOLYLINE,SPLINE"),
                    new TypedValue(-4, "<AND"),
                    new TypedValue(0, "POLYLINE"), // polylines 2d or 3d
                        new TypedValue(-4, "<NOT"), // but not meshes
                            new TypedValue(-4, "&"),
                            new TypedValue(70, 112),
                        new TypedValue(-4, "NOT>"),
                    new TypedValue(-4, "AND>"),
                new TypedValue(-4, "OR>")
                });

            var selection = ed.GetSelection(filter);
            if (selection.Status != PromptStatus.OK)
                return;

            using (var tr = acDb.TransactionManager.StartTransaction())
            {
                // use Linq queries to get lengths by type in a dictionary
                var lengths = selection.Value
                    .Cast<SelectedObject>()
                    .Select(selObj => (Curve)tr.GetObject(selObj.ObjectId, OpenMode.ForRead))
                    .ToLookup(curve => curve.GetType().Name,                         // <- key selector
                              curve => curve.GetDistanceAtParameter(curve.EndParam)) // <- element selector
                    .ToDictionary(group => group.Key,    // <- key selector
                                  group => group.Sum()); // <- element selector

                // print results
                foreach (var entry in lengths)
                {
                    ed.WriteMessage($"\n{entry.Key,-12} = {entry.Value,9:0.00}");
                }
                ed.WriteMessage($"\nTotal Length = {lengths.Values.Sum(),9:0.00}");

                tr.Commit();
            }
            AcAp.DisplayTextScreen = false;
        }
    }
}
