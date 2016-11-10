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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BusyManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MapCalendar_OnSelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            MessageBox.Show("OnSelectedDatesChanged_OK");
        }

        private void AddObjectButton_Click(object sender, RoutedEventArgs e)
        {
            Rectangle newObject = new Rectangle();
            newObject.Name = "newObject";
            newObject.Height = 50;
            newObject.Width = 100;
            newObject.Margin = new Thickness(30, 30, 0, 0);
            newObject.RadiusY = 16;
            newObject.RadiusX = 16;
            newObject.HorizontalAlignment = HorizontalAlignment.Left;
            newObject.VerticalAlignment = VerticalAlignment.Top;
            newObject.Stroke = Brushes.Black;
            newObject.Fill = Brushes.LightGreen;


            MapGrid.Children.Add(newObject);
        }
    }
}
