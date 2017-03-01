using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;


namespace TOTLEN
{
    public class TotalLength
    {
        [CommandMethod("TOTLEN")]
        public void TotalLengthOfSelected()
        {
            //get Document and Databse
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acDb = acDoc.Database;

            //define editor variable
            Editor ed = acDoc.Editor;

            PromptSelectionResult acSelprompt;
            acSelprompt = ed.SelectImplied();

            if (acSelprompt.Status == PromptStatus.OK)
            {
                SelectionSet acSelSet = acSelprompt.Value;

                double totalLengthPoly = SumEntityLength_Polyline(acSelSet);
                double totalLengthLine = SumEntityLength_Line(acSelSet);
                double totalLengthArc = SumEntityLength_Arc(acSelSet);

                double TotalLength = totalLengthLine + totalLengthArc + totalLengthPoly;

                string sTotalLength = TotalLength.ToString();
                String.Format("{0:0.00}", sTotalLength);
                ed.WriteMessage("Total Length = {0}", sTotalLength);

            }
            else
            {
                //Request For Selection
                PromptSelectionResult acSelPrompt = acDoc.Editor.GetSelection();

                if (acSelprompt.Status == PromptStatus.OK)
                {
                    SelectionSet acSelSet = acSelprompt.Value;

                    double totalLengthPoly = SumEntityLength_Polyline(acSelSet);
                    double totalLengthLine = SumEntityLength_Line(acSelSet);
                    double totalLengthArc = SumEntityLength_Arc(acSelSet);

                    double TotalLength = totalLengthLine + totalLengthArc + totalLengthPoly;

                    string sTotalLength = TotalLength.ToString();
                    String.Format("{0:0.00}", sTotalLength);
                    ed.WriteMessage("Total Length = {0}", sTotalLength);
                }
            }

            
        }

        private double SumEntityLength_Polyline(SelectionSet acSelSet)
        {
            //get Document and Databse
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acDb = acDoc.Database;

            //define editor variable
            Editor ed = acDoc.Editor;

            //Set Total Length Variable before beginning the sumation of selected lengths
            Double totalPolyLength = 0;

            using (Transaction acTrans = acDb.TransactionManager.StartTransaction())
            {
                try
                {
                    //Step through Objects in Selection
                    foreach (SelectedObject acSelObj in acSelSet)
                    {
                        //If there is a valid object, add it's length to the current total
                        if (acSelObj != null)
                        {
                            Entity acEnt = acTrans.GetObject(acSelObj.ObjectId, OpenMode.ForRead) as Entity;
                            totalPolyLength += (acEnt as Polyline).Length;
                            
                        }
                    }
                }
                catch
                {
                    ed.WriteMessage("Sorry, there was an error.");
                }

                return totalPolyLength;

            }

        }

        private double SumEntityLength_Line(SelectionSet acSelSet)
        {
            //get Document and Databse
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acDb = acDoc.Database;

            //define editor variable
            Editor ed = acDoc.Editor;

            //Set Total Length Variable before beginning the sumation of selected lengths
            Double totalLineLength = 0;

            using (Transaction acTrans = acDb.TransactionManager.StartTransaction())
            {
                try
                {
                    //Step through Objects in Selection
                    foreach (SelectedObject acSelObj in acSelSet)
                    {
                        //If there is a valid object, add it's length to the current total
                        if (acSelObj != null)
                        {
                            Entity acEnt = acTrans.GetObject(acSelObj.ObjectId, OpenMode.ForRead) as Entity;
                            totalLineLength += (acEnt as Line).Length;
                        }
                    }
                }
                catch
                {
                    ed.WriteMessage("Sorry, there was an error.");
                }

                return totalLineLength;

            }

        }

        private double SumEntityLength_Arc(SelectionSet acSelSet)
        {
            //get Document and Databse
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acDb = acDoc.Database;

            //define editor variable
            Editor ed = acDoc.Editor;

            //Set Total Length Variable before beginning the sumation of selected lengths
            Double totalArcLength = 0;

            using (Transaction acTrans = acDb.TransactionManager.StartTransaction())
            {
                try
                {
                    //Step through Objects in Selection
                    foreach (SelectedObject acSelObj in acSelSet)
                    {
                        //If there is a valid object, add it's length to the current total
                        if (acSelObj != null)
                        {
                            Entity acEnt = acTrans.GetObject(acSelObj.ObjectId, OpenMode.ForRead) as Entity;
                            totalArcLength += (acEnt as Arc).Length;
                        }
                    }
                }
                catch
                {
                    ed.WriteMessage("Sorry, there was an error.");
                }

                return totalArcLength;

            }

        }
    }
}
