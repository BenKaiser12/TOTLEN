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
    class ContextMenu
    {
        private static acw.ContextMenuExtension conMenuExt;

        public static void Attach()
        {
            conMenuExt = new acw.ContextMenuExtension();
            acw.MenuItem mi = new acw.MenuItem("Total Length");
            mi.Click += new EventHandler(OnTotalLength);
            conMenuExt.MenuItems.Add(mi);
            RXClass rxc = Entity.GetClass(typeof(Entity));
            AcAp.AddObjectContextMenuExtension(rxc, conMenuExt);
        }

        public static void Detach()
        {
            RXClass rxc = Entity.GetClass(typeof(Entity));
            AcAp.RemoveObjectContextMenuExtension(rxc, conMenuExt);
        }

        private static void OnTotalLength(Object o, EventArgs e)
        {
            Document acDoc = AcAp.DocumentManager.MdiActiveDocument;
            acDoc.SendStringToExecute("_.TOTLEN ", true, false, false);
        }
    }
}
