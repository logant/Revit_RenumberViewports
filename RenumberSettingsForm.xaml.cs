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
        static LinearGradientBrush enterBrush = null;
        static SolidColorBrush leaveBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
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

        private void closeButton_MouseEnter(object sender, MouseEventArgs e)
        {
            if (enterBrush == null)
                enterBrush = EnterBrush();
            closeButtonRect.Fill = enterBrush;
        }

        private void closeButton_MouseLeave(object sender, MouseEventArgs e)
        {
            closeButtonRect.Fill = leaveBrush;
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

        private void okButton_MouseEnter(object sender, MouseEventArgs e)
        {
            if (enterBrush == null)
                enterBrush = EnterBrush();
            okButtonRect.Fill = enterBrush;
        }

        private void okButton_MouseLeave(object sender, MouseEventArgs e)
        {
            okButtonRect.Fill = leaveBrush;
        }

        private LinearGradientBrush EnterBrush()
        {
            LinearGradientBrush b = new LinearGradientBrush();
            b.StartPoint = new Point(0, 0);
            b.EndPoint = new Point(0, 1);
            b.GradientStops.Add(new GradientStop(Color.FromArgb(255, 245, 245, 245), 0.0));
            b.GradientStops.Add(new GradientStop(Color.FromArgb(255, 195, 195, 195), 1.0));

            return b;
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
                    System.Windows.Interop.WindowInteropHelper wih = new System.Windows.Interop.WindowInteropHelper(uiForm);
                    wih.Owner = handle;
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
