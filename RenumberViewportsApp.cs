using System;
using System.Windows.Media.Imaging;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using RevitCommon.Attributes;

namespace Elk
{
    [ExtApp(Name = "Renumber Viewports", Description = "Renumber viewports in a selected order",
        Guid = "3e105c1d-3c21-48e1-9ea7-cddeceac2bb5", Vendor = "LMNA", VendorDescription = "LMN Architects - Tech Studio, www.lmnarchitects.com/tech-studio",
        ForceEnabled = false, Commands = new[] { "Renumber Viewports" })]
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
                "Renumber Viewports", "Renumber\nViewports", path, typeof(RenumberViewportCmd).FullName)
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

            string panelName = Properties.Settings.Default.PanelName;
            int version = 0;
            if (int.TryParse(application.ControlledApplication.VersionNumber, out version))
            {
                if (version < 2017)
                    panelName = "Tools";
            }

            // Add to the ribbon
            RevitCommon.UI.AddToRibbon(application, Properties.Settings.Default.TabName, panelName, renumberButtonData);

            return Result.Succeeded;
        }
    }
}
