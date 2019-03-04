using System;
using System.Collections.Generic;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace Elk
{
    [Transaction(TransactionMode.Manual)]
    public class RenumberViewportCmd : IExternalCommand
    {
        List<Viewport> selectedViewports;
        int startingNumber = 0;
        int stepSize = 0;
        int digits = 2;

        public int StartingNumber
        {
            get { return startingNumber; }
            set { startingNumber = value; }
        }

        public int StepSize
        {
            get { return stepSize; }
            set { stepSize = value; }
        }

        public int Digits
        {
            get { return digits; }
            set { digits = value; }
        }

        Result IExternalCommand.Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                UIDocument uiDoc = commandData.Application.ActiveUIDocument;
                Document doc = uiDoc.Document;
                int version = Convert.ToInt32(uiDoc.Application.Application.VersionNumber);
                try
                {
                    // Select the sheets
                    List<ElementId> elemList = new List<ElementId>();
                    ISelectionFilter selFilter = new ViewportSelectionFilter();

                    bool selectViewports = true;
                    while (selectViewports)
                    {
                        try
                        {
                            Reference r = uiDoc.Selection.PickObject(ObjectType.Element, selFilter, "Pick viewports in the desired order.");
                            elemList.Add(r.ElementId);
                            uiDoc.Selection.SetElementIds(elemList);
                        }
                        catch (Autodesk.Revit.Exceptions.OperationCanceledException)
                        {
                            selectViewports = false;
                        }
                    }

                    if (elemList != null && elemList.Count > 0)
                    {
                        // Get the Revit window handle
                        IntPtr handle = IntPtr.Zero;
                        if (version < 2019)
                            handle = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
                        else
                            handle = commandData.Application.GetType().GetProperty("MainWindowHandle") != null
                                ? (IntPtr)commandData.Application.GetType().GetProperty("MainWindowHandle").GetValue(commandData.Application)
                                : IntPtr.Zero;

                        // Construct form to specify the starting number
                        RenumberSettingsForm form = new RenumberSettingsForm(this);

                        // Assign the Revit window handle as the owner.
                        System.Windows.Interop.WindowInteropHelper wih = new System.Windows.Interop.WindowInteropHelper(form);
                        wih.Owner = handle;

                        form.ShowDialog();

                        // convert the digit value to a ToString modifier
                        string digStr = "D" + digits.ToString();

                        if (startingNumber > 0 && stepSize != 0)
                        {
                            Transaction t = new Transaction(doc, "Renumber views");
                            t.Start();
                            int currentNumber = startingNumber;
                            
                            // Renumber the viewports with an suffix to avoid duplicate numbering errors.
                            foreach (ElementId eid in elemList)
                            {
                                try
                                {
                                    Viewport vp = doc.GetElement(eid) as Viewport;
                                    Parameter param = vp.get_Parameter(BuiltInParameter.VIEWPORT_DETAIL_NUMBER);
                                    param.Set(param.AsString() + "temp");
                                }
                                catch (Exception ex)
                                {
                                    TaskDialog.Show("Error Temporary Parameter Setting", ex.Message);
                                }
                            }

                            // Actually renumber the viewports
                            foreach (ElementId eid in elemList)
                            {
                                try
                                {
                                    Viewport vp = doc.GetElement(eid) as Viewport;
                                    Parameter param = vp.get_Parameter(BuiltInParameter.VIEWPORT_DETAIL_NUMBER);
                                    param.Set(currentNumber.ToString(digStr));
                                }
                                catch (Exception ex)
                                {
                                    TaskDialog.Show("Error Setting Parameter", ex.Message);
                                }
                                currentNumber += stepSize;
                            }
                            t.Commit();
                        }
                    }

                    // Write to home
                    RevitCommon.FileUtils.WriteToHome("Renumber Viewports", commandData.Application.Application.VersionName, commandData.Application.Application.Username);

                    return Result.Succeeded;
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                    return Result.Failed;
                }

            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}
