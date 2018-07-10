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

            if (!RevitCommon.FileUtils.GetPluginSettings(typeof(RenumberViewportsApp).Assembly.GetName().Name, out string helpPath, out string tabName, out string panelName))
            {
                // Set the help file path
                System.IO.FileInfo fi = new System.IO.FileInfo(typeof(RenumberViewportsApp).Assembly.Location);
                System.IO.DirectoryInfo directory = fi.Directory;
                helpPath = directory.FullName + "\\help\\RenumberViewports.pdf";

                // Set the tab name
                tabName = Properties.Settings.Default.TabName;
                panelName = Properties.Settings.Default.PanelName;
            }
            else
            {
                // Check for nulls  in the returned settings
                if (helpPath == null)
                {
                    // Set the help file path
                    System.IO.FileInfo fi = new System.IO.FileInfo(typeof(RenumberViewportsApp).Assembly.Location);
                    System.IO.DirectoryInfo directory = fi.Directory;
                    helpPath = directory.FullName + "\\help\\RenumberViewports.pdf";
                }

                if (string.IsNullOrEmpty(tabName))
                    tabName = Properties.Settings.Default.TabName;
                if (string.IsNullOrEmpty(panelName))
                    panelName = Properties.Settings.Default.PanelName;
            }

            // Set the help file
            if (System.IO.File.Exists(helpPath))
            {
                ContextualHelp help = new ContextualHelp(ContextualHelpType.ChmFile, helpPath);
                renumberButtonData.SetContextualHelp(help);
            }

            //int version = 0;
            //if (int.TryParse(application.ControlledApplication.VersionNumber, out version))
            //{
            //    if (version < 2017)
            //        panelName = "Tools";
            //}

            // Add to the ribbon
            RevitCommon.UI.AddToRibbon(application, Properties.Settings.Default.TabName, panelName, renumberButtonData);

            return Result.Succeeded;
        }
    }
}
