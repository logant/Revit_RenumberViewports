using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace Elk
{
    /// <summary>
    /// Interaction logic for RenumberSettingsForm.xaml
    /// </summary>
    public partial class RenumberSettingsForm : Window
    {
        string startingNumber;
        string stepSize;
        string digits;
        RenumberViewportCmd command;

        public RenumberSettingsForm(RenumberViewportCmd cmd)
        {
            command = cmd;
            InitializeComponent();

            stepSizeTextBox.Text = "1";
            digitsTextBox.Text = Properties.Settings.Default.Digits.ToString();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void startingTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            startingNumber = startingTextBox.Text;
        }

        private void stepSizeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            stepSize = stepSizeTextBox.Text;
        }

        private void digitsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            digits = digitsTextBox.Text;
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

       private void okButton_Click(object sender, RoutedEventArgs e)
        {
            int startInt = 0;
            int stepInt = 1;
            int digitInt = 1;

            // Convert the textbox values to numbers
            int.TryParse(startingNumber, out startInt);
            int.TryParse(stepSize, out stepInt);
            int.TryParse(digits, out digitInt);

            // Save the digit setting
            if(digitInt != Properties.Settings.Default.Digits)
            {
                Properties.Settings.Default.Digits = digitInt;
                Properties.Settings.Default.Save();
            }

            // Pass the information to the RenumberViewportsCmd and close
            command.Digits = digitInt;
            command.StartingNumber = startInt;
            command.StepSize = stepInt;
            Close();
        }

        private void Border_KeyDown(object sender, KeyEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control) // Is Ctrl key pressed
            {
                if (Keyboard.IsKeyDown(Key.U))
                {
                    // Launch the UI form
                    System.Diagnostics.Process proc = System.Diagnostics.Process.GetCurrentProcess();
                    IntPtr handle = proc.MainWindowHandle;
                    RevitCommon.UILocation uiForm = new RevitCommon.UILocation("Renumber Viewports", Properties.Settings.Default.TabName, Properties.Settings.Default.PanelName);
                    System.Windows.Interop.WindowInteropHelper wih = new System.Windows.Interop.WindowInteropHelper(uiForm) {Owner = handle};
                    uiForm.ShowDialog();

                    string tab = uiForm.Tab;
                    string panel = uiForm.Panel;

                    if (tab != Properties.Settings.Default.TabName || panel != Properties.Settings.Default.PanelName)
                    {
                        Properties.Settings.Default.TabName = tab;
                        Properties.Settings.Default.PanelName = panel;
                        Properties.Settings.Default.Save();

                        Autodesk.Revit.UI.TaskDialog.Show("Warning", "Changes to the panel or tab this tool resides on will take place when Revit restarts.");
                    }
                }
            }
        }
    }
}
