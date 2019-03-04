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

        // Removing the Border_KeyDown function as the RevitCommon.UILocation stuff has been purged from RevitCommon
        // The RevitCommon.Config file manages everything that this function did and more.
    }
}
