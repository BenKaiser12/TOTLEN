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
            PromptSelectionResult acSelPrompt;
            acSelPrompt = ed.GetSelection();

            //Check if Selection is Valid
            try
            {
                if (acSelPrompt.Status == PromptStatus.OK)
                {
                    SelectionSet acSelSet = acSelPrompt.Value;
                    
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
                                        ed.WriteMessage("\nSelected Polyline");
                                        //ed.WriteMessage("Selected Polyline. Total Length = {0}", totalPolyLength);
                                    }
                                    else if (acSelObjType == typeof(Line))
                                    {
                                        totalLineLength += (acEnt as Line).Length;
                                        ed.WriteMessage("\nSelected Line");
                                    }
                                    else if (acSelObjType == typeof(Arc))
                                    {
                                        totalArcLength += (acEnt as Arc).Length;
                                        ed.WriteMessage("\nSelected Arc");
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

                    if(noValidLength != "blah")
                    {
                        totalAllLength = totalPolyLength + totalLineLength + totalArcLength;

                        string sTotalLength = totalAllLength.ToString();
                        String.Format("{0:0.00}", sTotalLength);
                        ed.WriteMessage("\nTotal Length = {0}", sTotalLength);
                    }
                    else
                    {
                        ed.WriteMessage("\nI am not an accepted onject.  Sorry, you lose. :(");
                    }

                    
                }
                else
                {
                    ed.WriteMessage("\nSorry, There was an error.");
                }
            }
            catch
            {
                ed.WriteMessage("I didn't do what I was supposed to");
            }     
        }        
    }
}
