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

                SumEntityLength_Polyline();

            }
            else
            {
                //Request For Selection
                PromptSelectionResult acSelPrompt = acDoc.Editor.GetSelection();

                if (acSelprompt.Status == PromptStatus.OK)
                {
                    SelectionSet acSelSet = acSelprompt.Value;

                    SumEntityLength_Polyline();
                }
            }


            //Sum all valid object types
            double totalLengthAll = totalPolyLength + totalLineLength + totalArcLength;


        }

        private void SumEntityLength_Polyline()
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
            }

        }

        private void SumEntityLength_Line()
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
            }

        }

        private void SumEntityLength_Arc()
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
            }

        }
    }
}
