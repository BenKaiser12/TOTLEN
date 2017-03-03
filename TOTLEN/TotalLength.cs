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
        [CommandMethod("TOTLEN", CommandFlags.UsePickSet)]
        public void TotalLengthOfSelected()
        {
            //Variables...
            string noValidLength = "";
            double totalPolyLength = 0,
                totalLineLength = 0,
                totalArcLength = 0,
                totalAllLength = 0;

            //get Document and Databse
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acDb = acDoc.Database;

            //define editor variable
            Editor ed = acDoc.Editor;

            //Get Selection Set
            PromptSelectionResult acSelRes = ed.SelectImplied();            

            //Check if Selection is Valid
            try
            {
                if (acSelRes.Status == PromptStatus.Error)
                {
                    PromptSelectionOptions acSelOpt = new PromptSelectionOptions();
                    acSelOpt.MessageForAdding = "\nPlease Make a Selection of Line, Arcs, and/or Polylines";
                    acSelRes = ed.GetSelection(acSelOpt);
                }
                else
                {
                    ed.SetImpliedSelection(new ObjectId[0]);
                }

                if (acSelRes.Status == PromptStatus.OK)
                {
                    SelectionSet acSelSet = acSelRes.Value;

                    //If it was valid, run the Sum methods below
                    using (Transaction acTrans = acDb.TransactionManager.StartTransaction())
                    {
                        try
                        {
                            //Step through Objects in Selection
                            foreach (SelectedObject acSelObj in acSelSet)
                            {

                                //If there is a valid object, 
                                if (acSelObj != null)
                                {
                                    Entity acEnt = acTrans.GetObject(acSelObj.ObjectId, OpenMode.ForRead) as Entity;
                                    Type acSelObjType = acEnt.GetType();

                                    if (acSelObjType == typeof(Polyline))
                                    {
                                        totalPolyLength += (acEnt as Polyline).Length;
                                        //ed.WriteMessage("\nSelected Polyline");
                                        
                                    }
                                    else if (acSelObjType == typeof(Line))
                                    {
                                        totalLineLength += (acEnt as Line).Length;
                                        //ed.WriteMessage("\nSelected Line");
                                    }
                                    else if (acSelObjType == typeof(Arc))
                                    {
                                        totalArcLength += (acEnt as Arc).Length;
                                        //ed.WriteMessage("\nSelected Arc");
                                    }
                                    else
                                    {
                                        noValidLength = "blah";
                                    }
                                }

                            }
                        }
                        catch
                        {
                            ed.WriteMessage("\nHe's Dead, Jim");
                        }
                    }
                }
                if (noValidLength != "blah")
                {
                    totalAllLength = totalPolyLength + totalLineLength + totalArcLength;


                    ed.WriteMessage("\nTotal Length = {0}", totalAllLength.ToString("F2"));

                }
                else
                {
                    ed.WriteMessage("\nSorry, I am not an accepted onject.");
                }
            }
            catch
            {
                ed.WriteMessage("\nI didn't do what I was supposed to");
            }     
        }        
    }
}
