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

            //Variables...
           double totalPolyLength = 0,
                totalLineLength = 0,
                totalArcLength = 0,
                totalAllLength = 0;


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
                                Type acSelObjType = acSelObj.GetType();
                                
                                //If there is a valid object, check its type and depending
                                //on that, add it's length to the current total
                                if (acSelObjType == typeof(Polyline))
                                {
                                    Entity acEnt = acTrans.GetObject(acSelObj.ObjectId, OpenMode.ForRead) as Entity;
                                    totalPolyLength += (acEnt as Polyline).Length;
                                    ed.WriteMessage("Selected Polyline");
                                }
                                else if (acSelObjType == typeof(Line))
                                {
                                    Entity acEnt = acTrans.GetObject(acSelObj.ObjectId, OpenMode.ForRead) as Entity;
                                    totalLineLength += (acEnt as Line).Length;
                                    ed.WriteMessage("Selected Line");
                                }
                                else if (acSelObjType == typeof(Arc))
                                {
                                    Entity acEnt = acTrans.GetObject(acSelObj.ObjectId, OpenMode.ForRead) as Entity;
                                    totalArcLength += (acEnt as Arc).Length;
                                    ed.WriteMessage("Selected Arc");
                                }
                            }
                        }
                        catch
                        {
                            ed.WriteMessage("Sorry, there was an error.");
                        }

                        totalAllLength = totalArcLength + totalLineLength + totalPolyLength;                    

                    }
                    
                    string sTotalLength = totalAllLength.ToString();
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
    }
}
