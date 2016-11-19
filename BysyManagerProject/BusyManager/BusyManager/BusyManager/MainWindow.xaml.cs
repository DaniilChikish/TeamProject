﻿using System;
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
            try
            {
                TargetObjects = Cryptor.LoadData(FilePath).ReturnContent();
            }
            catch (Exception)
            {
                MessageBox.Show("FileOpenError");
                TargetObjects = new List<TargetObject>();
            }

            MapComboBoxUpload();
        }

        //main
        List<TargetObject> TargetObjects;
        public string DirPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\BusyManager";
        public string FilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\BusyManager\\crypt.dat";
        //MapTab
        private bool TargetOnMove = false;
        private bool ChangeAllow = false;
        private double deltaX;
        private double deltaY;
        Label MovedLab;
        Rectangle MovedObj;
        private List<object> hitResultsList = new List<object>();

        private void MapDatePicker_OnSelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            DrawingObjects((DateTime)MapDatePicker.SelectedDate);
        }

        private void DrawingObjects(DateTime date)
        {
                foreach (TargetObject item in TargetObjects)
                {
                    CreateObjectOnMap(item.IDName, item.GetState(date), item.Margin);
                }
        }

        private void SaveObjectsButton_Click(object sender, RoutedEventArgs e)
        {
            if ((string)((ComboBoxItem)MapComboBox.SelectedItem).Content == "New Object")
            {
                //interface
                CreateObjectOnMap(ObjectNameBox.Text, TargetObjectState.Available, new Thickness(30, 30, 0, 0));
                //data
                TargetObjects.Add(new TargetObject(ObjectNameBox.Text, ObjectPropertiesBox.Text, new Thickness(30, 30, 0, 0)));
            }
            else
            {

            }
        }

        private void CreateObjectOnMap(string name, TargetObjectState state, Thickness margin)
        {

            Rectangle newObject = DuplicatePrefab(ObjectPrefabBody);
            newObject.Name = (name);
            newObject.Margin = margin;
            newObject.Visibility = Visibility.Visible;

            Label newLabel = DuplicatePrefab(ObjectPrefabLabel);
            newLabel.Name = (name + "_label");
            switch (state)
            {
                case TargetObjectState.Available:
                    newLabel.Content = (name + "(Available)");
                    break;
                case TargetObjectState.Busy:
                    newLabel.Content = (name + "(Bysy)");
                    break;
                case TargetObjectState.Free:
                    newLabel.Content = (name + "(Free)");
                    break;
                case TargetObjectState.Maintenance:
                    newLabel.Content = (name + "(Maintanence)");
                    break;
                case TargetObjectState.notAvailable:
                    newLabel.Content = (name + "(notAvailable)");
                    break;
            }
            newLabel.Margin = margin;
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
            if (ChangeAllow && TargetOnMove)
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
                TargetOnMove = false;
                //Save changes
                TargetObjects.Find(x => x.IDName == MovedObj.Name).Margin = MovedObj.Margin;
                MapComboBoxUpload();
            }
        }

        private void MapComboBoxUpload()
        {
            MapComboBox.Items.Clear();
            ComboBoxItem newObj = new ComboBoxItem();
            newObj.Content = "New Object";
            MapComboBox.Items.Add(newObj);
                foreach (TargetObject item in TargetObjects)
                {
                    newObj = new ComboBoxItem();
                    newObj.Content = item.IDName;
                    MapComboBox.Items.Add(newObj);
                }
            MapComboBox.SelectedItem = MapComboBox.Items[0];
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

        private void SaveDataButton_Click(object sender, RoutedEventArgs e)
        {
            Cryptor.SaveData(new TargetObjectsContainer<TargetObject>(TargetObjects), "default", DirPath, FilePath);
        }

        private void MapTab_Loaded(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("MapTab_Loaded");
        }
    }
}
