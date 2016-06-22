using System;
using System.Windows.Media.Imaging;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;

namespace Elk
{
    [Transaction(TransactionMode.Manual)]
    public class RenumberViewportsApp : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            string path = typeof(RenumberViewportsApp).Assembly.Location;
            
            // Create the pushbutton
            PushButtonData renumberButtonData = new PushButtonData(
                "Renumber Viewports", "Renumber\nViewports", path, "Elk.RenumberViewportCmd")
                {
                    LargeImage = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(Properties.Resources.ViewTitle_32x32.GetHbitmap(), IntPtr.Zero, System.Windows.Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()),
                    ToolTip = "Renumber viewports in the selected order"
                };

            // Set the help file
            System.IO.FileInfo fi = new System.IO.FileInfo(path);
            System.IO.DirectoryInfo directory = fi.Directory;
            string helpPath = directory.FullName + "\\help\\RenumberViewports.pdf";
            ContextualHelp help = new ContextualHelp(ContextualHelpType.ChmFile, helpPath);
            renumberButtonData.SetContextualHelp(help);

            // Add to the ribbon
            RevitCommon.UI.AddToRibbon(application, Properties.Settings.Default.TabName, Properties.Settings.Default.PanelName, renumberButtonData);

            return Result.Succeeded;
        }
    }
}
