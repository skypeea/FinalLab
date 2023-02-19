using Autodesk.Revit.Attributes;
using Autodesk.Revit.Creation;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB.Architecture;

namespace FinalLab
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class MarkRooms : IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, ref string message, Autodesk.Revit.DB.ElementSet elements)
        {
            try
            {


                Autodesk.Revit.DB.Document doc = commandData.Application.ActiveUIDocument.Document;
                List<Level> levels = new FilteredElementCollector(doc)
                    .OfClass(typeof(Level))
                    .OfType<Level>()
                    .ToList();


                using (Transaction ts = new Transaction(doc, "Create rooms"))
                {
                    ts.Start();

                    foreach (Level level in levels)
                    {
                        int startNumber = 0;
                        if (level.Name == "Уровень 1")
                        {
                            startNumber = 101;
                        }
                        else
                        {
                            startNumber = 201;
                        }

                        PlanTopology planTopology = doc.get_PlanTopology(level);

                        foreach (PlanCircuit c in planTopology.Circuits)
                        {
                            if (null != c)
                            {
                                Room room = doc.Create.NewRoom(null, c); //создание комнаты
                                room.Number = "" + startNumber;         //и присвоение номера комнате
                                startNumber++;
                            }
                        }
                    }
                    ts.Commit();
                }
                return Result.Succeeded;
            }
            
            catch (Exception ex)
            {
                TaskDialog.Show("Info", ex.Message);
                return Result.Failed;
            }
        }
    }
}
