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
    public class CSVExport
    {
        [CommandMethod("CSVExport", CommandFlags.UsePickSet)]
        public void ExportToCSVFile()
        {
            var acDoc = AcAp.DocumentManager.MdiActiveDocument;
            var acDb = acDoc.Database;
            var ed = acDoc.Editor;
            

            var filter = new SelectionFilter(new[]
            {
                new TypedValue(-4,"OR"),
                    new TypedValue(0,"ARC,CIRCLE,ELLIPSE,LINE,LWPOLYLINE,SPLINE"),
                    new TypedValue(-4, "AND"),
                        new TypedValue(0,"POLYLINE"),
                        new TypedValue(-4,"NOT"),
                            new TypedValue(-4,"&"),
                            new TypedValue(70, 112),
                        new TypedValue(-4,"NOT>"),
                    new TypedValue(-4,"AND>"),
                new TypedValue(-4,"OR>"),
            });

            PromptSelectionResult selection = ed.GetSelection(filter);
            if (selection.Status == PromptStatus.OK)
                return;

            sw.Forms.SaveFileDialog sfd = new sw.Forms.SaveFileDialog();
            sfd.Filter = "*.csv Files|*.csv";

            try
            {
                using (Transaction acTrans = acDb.TransactionManager.StartTransaction())
                {
                    sfd.ShowDialog();
                    string filename = sfd.FileName;

                    if (filename != "")
                    {
                        StreamWriter sWriter;                        
                        sWriter = GetWriterForFile(filename);

                        var lengths = selection.Value
                                    .Cast<SelectedObject>()
                                    .Select(selObj => (Curve)acTrans.GetObject(selObj.ObjectId, OpenMode.ForRead))
                                    .ToLookup(curve => curve.GetType().Name,                         // <- key selector
                                              curve => curve.GetDistanceAtParameter(curve.EndParam)) // <- element selector
                                    .ToDictionary(group => group.Key,    // <- key selector
                                                  group => group.Sum()); // <- element selector

                        // print results
                        foreach (var entry in lengths)
                        {
                            sWriter.WriteLine($"\n{entry.Key,-12} , {entry.Value,9:0.00}");
                        }

                    }
                    else
                    {
                        ed.WriteMessage("\nHa, you're joking, right?");
                    }
                }
            }
            catch
            {
                ed.WriteMessage("\nI don't know how to write....");

            }

        }

        private static StreamWriter GetWriterForFile(string filename)
        {
            StreamWriter sWriter;

            FileStream fs = File.Open(filename,
                                      FileMode.CreateNew,
                                      FileAccess.Write);

            sWriter = new StreamWriter(fs, System.Text.Encoding.UTF8);
            return sWriter;
        }
    }
}
