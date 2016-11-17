using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
            MapComboBox.SelectedItem = MapComboBox.Items[0];
        }

        //MapTab
        List<TargetObject> TargetObjects;
        private bool TargetOnMove = false;
        private bool ChangeAllow = false;
        private double deltaX;
        private double deltaY;
        Label MovedLab;
        Rectangle MovedObj;
        private List<object> hitResultsList =new List<object>();

        private void MapCalendar_OnSelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            MessageBox.Show("OnSelectedDatesChanged_OK");
        }

        private void MapTab_OnInitialized(object sender, EventArgs e)
        {
            DrawingObjects();
        }

        private void DrawingObjects()
        {

        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if ((string)((ComboBoxItem)MapComboBox.SelectedItem).Content == "New Object")
                CreateObject();
        }

        private void CreateObject()
        {
            Rectangle newObject = DuplicatePrefab(ObjectPrefabBody);
            newObject.Name = (ObjectNameBox.Text+"_body");
            newObject.Visibility = Visibility.Visible;

            Label newLabel = DuplicatePrefab(ObjectPrefabLabel);
            newLabel.Name = (ObjectNameBox.Text + "_label");
            newLabel.Content = (ObjectNameBox.Text + "(free)(new)");
            newLabel.Visibility = Visibility.Visible;

            MapGrid.Children.Add(newObject);
            MapGrid.Children.Add(newLabel);
        }
        public static T DuplicatePrefab<T>(T from)
        {
            string genString = System.Windows.Markup.XamlWriter.Save(from);
            StringReader stringReader = new StringReader(genString);
            System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(stringReader);
            return (T)System.Windows.Markup.XamlReader.Load(xmlReader);
        }

        private void TargetObject_MoveStart(object sender, MouseButtonEventArgs e)
        {

            if (ChangeAllow)
            {
                // Retrieve the coordinate of the mouse position.
                Point pt = e.GetPosition((UIElement)sender);

                // Clear the contents of the list used for hit test results.
                hitResultsList.Clear();

                // Set up a callback to receive the hit test result enumeration.
                System.Windows.Media.VisualTreeHelper.HitTest(MapGrid, null,
                    new System.Windows.Media.HitTestResultCallback(MyHitTestResult),
                    new System.Windows.Media.PointHitTestParameters(pt));

                // Perform actions on the hit test results list.
                if (hitResultsList.Count > 0)
                {
                    if (hitResultsList[0].ToString() == "System.Windows.Controls.TextBlock")
                    {
                        MovedLab = (Label)(((Border)hitResultsList[1]).TemplatedParent);
                        MovedObj = (Rectangle)(hitResultsList[2]);
                    }
                    else
                    {
                        MovedLab = (Label)(((Border)hitResultsList[0]).TemplatedParent);
                        MovedObj = (Rectangle)(hitResultsList[1]);
                    }
                    deltaX = e.GetPosition(this.MapGrid).X - MovedLab.Margin.Left;
                    deltaY = e.GetPosition(this.MapGrid).Y - MovedLab.Margin.Top;
                    this.Cursor = Cursors.Hand;
                    TargetOnMove = true;
                }

            }
        }
        public System.Windows.Media.HitTestResultBehavior MyHitTestResult(System.Windows.Media.HitTestResult result)
        {
            // Add the hit test result to the list that will be processed after the enumeration.
            hitResultsList.Add(result.VisualHit);

            // Set the behavior to return visuals at all z-order levels.
            return System.Windows.Media.HitTestResultBehavior.Continue;
        }

        private void TargetObject_Move(object sender, MouseEventArgs e)
        {
            if (ChangeAllow&&TargetOnMove)
            {
                MovedObj.Margin = new Thickness(e.GetPosition(this.MapGrid).X - deltaX, e.GetPosition(this.MapGrid).Y - deltaY, 0, 0);
                MovedLab.Margin = new Thickness(e.GetPosition(this.MapGrid).X - deltaX, e.GetPosition(this.MapGrid).Y - deltaY, 0, 0);
            }
        }

        private void TargetObject_MoveEnd(object sender, MouseButtonEventArgs e)
        {
            if (ChangeAllow)
            {
                this.Cursor = Cursors.Arrow;
                //Save changes
                //Rectangle rect = (Rectangle)sender;
                //deltaX = e.GetPosition(this.MapGrid).X - rect.Margin.Left;
                //deltaY = e.GetPosition(this.MapGrid).Y - rect.Margin.Top;
                TargetOnMove = false;
            }
        }

        private void ChangeObjectButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ChangeAllow)
            {
                ChangeAllow = true;
                MapToolBox.Visibility = Visibility.Visible;
                ChangeObjectButton.Content = "End Change";
            }
            else
            {
                ChangeAllow = false;
                MapToolBox.Visibility = Visibility.Hidden;
                ChangeObjectButton.Content = "Change Objects";
            }
        }
    }
}
