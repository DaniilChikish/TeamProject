using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BusyManager
{
    /// <summary>
    /// Interaction logic for ChangeStateDialog.xaml
    /// </summary>
    public partial class ChangeStateDialog : Window
    {
        private string iDName;
        private MainWindow mainWindow;

        public ChangeStateDialog()
        {
            InitializeComponent();
        }

        public ChangeStateDialog(MainWindow mainWindow, string iDName)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            this.iDName = iDName;
            this.Title += iDName;
            BeginPicker.SelectedDate = DateTime.Now;
            EndPicker.SelectedDate = DateTime.Now + new TimeSpan(1, 0, 0, 0, 0);
        }

        private void DaysBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DaysBox.Text = new String(DaysBox.Text.Where(c => Char.IsDigit(c)).ToArray());
            DaysBox.SelectionStart = DaysBox.Text.Length;
            if (BeginPicker.SelectedDate != null && EndPicker.SelectedDate != null && DaysBox.Text != "")
                EndPicker.SelectedDate = BeginPicker.SelectedDate + new TimeSpan(Convert.ToInt16(DaysBox.Text), 0, 0, 0, 0);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            //TargetObjectState change;
            DateTime begin;
            //if (BeginPicker.SelectedDate != null)
                begin = (DateTime)BeginPicker.SelectedDate;
            //else begin = DateTime.Now;
            DateTime end;
            //if (EndPicker.SelectedDate != null)
                end = (DateTime)EndPicker.SelectedDate;
            //else end = DateTime.Now + new TimeSpan(Convert.ToInt16(DaysBox.Text), 0, 0, 0, 0);
            TargetObjectState state = TargetObjectState.Available;
            switch ((string)(((ComboBoxItem)StateChoiseBox.SelectedItem).Content))
            {
                case "Free":
                    state = TargetObjectState.Free;
                    break;
                case "Busy":
                    state = TargetObjectState.Busy;
                    break;
                case "Maintenance":
                    state = TargetObjectState.Maintenance;
                    break;
                case "Available":
                    state = TargetObjectState.Available;
                    break;
                case "notAvailable":
                    state = TargetObjectState.notAvailable;
                    break;
            }
            mainWindow.AddChange(this.iDName, new TargetTimeState(state, CustomerBox.Text, begin, end));
            this.Close();
        }
        private void Picker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BeginPicker.SelectedDate != null && EndPicker.SelectedDate != null)
            {
                DaysBox.Text = (BeginPicker.SelectedDate - EndPicker.SelectedDate).Value.Days.ToString();
                if (BeginPicker.SelectedDate > EndPicker.SelectedDate)
                    EndPicker.SelectedDate = BeginPicker.SelectedDate + new TimeSpan(Convert.ToInt16(DaysBox.Text), 0, 0, 0, 0);
            }
        }
        //private void EndPicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (BeginPicker.SelectedDate != null && EndPicker.SelectedDate != null)
        //        DaysBox.Text = (BeginPicker.SelectedDate - EndPicker.SelectedDate).Value.Days.ToString();
        //}
    }
}
