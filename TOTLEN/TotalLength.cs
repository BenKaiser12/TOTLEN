using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.AutoCAD;
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

            //Get Selection Set
            PromptSelectionResult acSelPrompt;
            acSelPrompt = ed.GetSelection();

            //Check if Pickfirst set was valid
            try
            {
                if (acSelPrompt.Status == PromptStatus.OK)
                {
                    //If it was valid, run the Sum methods below
                    SelectionSet acSelSet = acSelPrompt.Value;

                    double totalLengthPoly,
                        totalLengthLine,
                        totalLengthArc;

                    //Get the type of the selected object and convert it to a string
                    string sObjType = acSelSet.GetType().ToString();

                    //Check the object type, perform sum for given type
                    if (sObjType == "Polyline")
                    {
                        totalLengthPoly = SumEntityLength_Polyline(acSelSet);
                    }
                    else if (sObjType == "Line")
                    {
                        totalLengthLine = SumEntityLength_Line(acSelSet);
                    }
                    else if (sObjType == "Arc")
                    {
                        totalLengthArc = SumEntityLength_Arc(acSelSet);
                    }

                    double TotalLength = totalLengthLine + totalLengthArc + totalLengthPoly;

                    string sTotalLength = TotalLength.ToString();
                    String.Format("{0:0.00}", sTotalLength);
                    ed.WriteMessage("Total Length = {0}", sTotalLength);
                }
                else
                {
                    ed.WriteMessage("Sorry, There was an error.");
                }
            }
            catch
            {
                ed.WriteMessage("I didn't do what I was supposed to");
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
