using System;
using System.Collections.Generic;
using System.IO;
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

            string helpPath = Path.Combine(Path.GetDirectoryName(typeof(RenumberViewportsApp).Assembly.Location), "help\\RenumberViewports.pdf");
            string tabName = "Add-Ins";
            string panelName = "Views";
            if (RevitCommon.FileUtils.GetPluginSettings(typeof(RenumberViewportsApp).Assembly.GetName().Name, out Dictionary<string, string> settings))
            {
                // Settings retrieved, lets try to use them.
                if (settings.ContainsKey("help-path") && !string.IsNullOrWhiteSpace(settings["help-path"]))
                {
                    // Check to see if it's relative path
                    string hp = Path.Combine(Path.GetDirectoryName(typeof(RenumberViewportsApp).Assembly.Location), settings["help-path"]);
                    if (File.Exists(hp))
                        helpPath = hp;
                    else
                        helpPath = settings["help-path"];
                }
                if (settings.ContainsKey("tab-name") && !string.IsNullOrWhiteSpace(settings["tab-name"]))
                    tabName = settings["tab-name"];
                if (settings.ContainsKey("panel-name") && !string.IsNullOrWhiteSpace(settings["panel-name"]))
                    panelName = settings["panel-name"];
            }

            // Set the help file
            ContextualHelp help = null;
            if (File.Exists(helpPath))
                help = new ContextualHelp(ContextualHelpType.ChmFile, helpPath);
            else if (Uri.TryCreate(helpPath, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                help = new ContextualHelp(ContextualHelpType.Url, helpPath);
            if (help != null)
                renumberButtonData.SetContextualHelp(help);

            // Add to the ribbon
            RevitCommon.UI.AddToRibbon(application, tabName, panelName, renumberButtonData);

            return Result.Succeeded;
        }
    }
}
