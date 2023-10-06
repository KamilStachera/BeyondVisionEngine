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
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Controls.Primitives;
using BeyondVisionEngine.Components;
using BeyondVisionEngine.EditorWindows;
using BeyondVisionEngine.Utils;
using Microsoft.VisualBasic;
using Newtonsoft.Json;

namespace BeyondVisionEngine
{
    /// <summary>
    /// Interaction logic for EditWindow.xaml
    /// </summary>
    public partial class EditGameWindow
    {
        List<MajorLocation> _locationList = new List<MajorLocation>();

        public EditGameWindow()
        {
            InitializeComponent();
        }

        private void AddLocationButton_Click(object sender, RoutedEventArgs e)
        {
            string locationName = Interaction.InputBox("Location name:", "TitLe", "NAME HERE", 200, 200);


            var newMajorLocationLabel = LabelUtils.CreateNewLabel("", locationName, 100, 100, TextBox_OnMouseDown,
                                                                   TextBox_OnMouseMove, TextBox_OnMouseUp, Brushes.Green);

            // width and height as place holders -> 20,20
            var newMajorLocation = new MajorLocation(newMajorLocationLabel.Name, newMajorLocationLabel, 10, 10);

            _locationList.Add(newMajorLocation);

            GetMainGrid().Children.Add(newMajorLocationLabel);
            labels.Add(newMajorLocationLabel);
        }

        private void SaveAndUpload_Click(object sender, RoutedEventArgs e)
        {
            var uploadManager = new UploadManager();
            var gameId = GeneralUtils.GenerateRandomName();

            MessageBox.Show("Uploading files..");

            foreach (var majorLocation in _locationList)
            {
                var locationId = majorLocation.LocationName;
                foreach (var dialogGraph in majorLocation.DialogsList)
                {
                    foreach (var dialogGraphDialogGameEvent in dialogGraph.dialogGameEvents)
                    {
                        foreach (var singleDialog in dialogGraphDialogGameEvent.Dialogs)
                        {
                            var dialogId = GeneralUtils.GenerateRandomName();
                            uploadManager.GenerateAndUploadDialogFile(gameId, locationId, dialogId, Convert.ToInt32(singleDialog.Voice), singleDialog.Text);
                            singleDialog.Voice = dialogId;
                        }
                    }
                }
            }

            var json = JsonConvert.SerializeObject(_locationList);
            File.Create($"{gameId}.txt").Dispose();
            File.WriteAllText($"{gameId}.txt", json);
            uploadManager.UploadJsonToS3(json, gameId);

            while (!uploadManager.CheckIfAllFilesWereUploaded())
            {
                Thread.Sleep(200);
            }

            MessageBox.Show("Files uploaded!");
        }

        private MajorLocation GetLocationByLabel(Label label)
        {
            return _locationList.First(x => x.Label.Name == label.Name);
        }


        public override void AddNewConnection(Label label1, Label label2)
        {
            var doorConnection = GetLocationByLabel(label1).DoorConnections.First(x => x.Destination == "tmp");
            doorConnection.Destination = label2.Name;

            doorConnection = GetLocationByLabel(label2).DoorConnections.First(x => x.Source == "tmp");
            doorConnection.Source = label1.Name;
        }

        public override void DblClkOnLabel()
        {
            EditMajorLocation editWindow = new EditMajorLocation(GetLocationByLabel(_clickedLabel));
            editWindow.ShowDialog();
        }

        public override Grid GetMainGrid()
        {
            return MainGrid;
        }


        public override void NotifyLabelMove(double offsetX, double offsetY)
        {
            var location = GetLocationByLabel(_clickedLabel);

            for (var i = 0; i < location.DoorsLocationOnGrid.Count; i++)
            {
                var doors = location.DoorsLocationOnGrid[i];

                for (int j = 0; j < GetMainGrid().Children.Count; j++)
                {
                    if (!(GetMainGrid().Children[j] is GraphEdge graphEdge))
                        continue;

                    if (IsInRange(graphEdge.Destination, doors))
                        graphEdge.Destination = new Point(graphEdge.Destination.X + offsetX, graphEdge.Destination.Y + offsetY);

                    if (IsInRange(graphEdge.Source, doors))
                        graphEdge.Source = new Point(graphEdge.Source.X + offsetX, graphEdge.Source.Y + offsetY);
                }

                location.DoorsLocationOnGrid[i] = new Point(doors.X + offsetX, doors.Y + offsetY);
            }
        }

        private bool IsInRange(Point p1, Point p2)
        {
            if (Math.Abs(p1.X - p2.X) < 1 && Math.Abs(p1.Y - p2.Y) < 1)
                return true;
            return false;
        }

        public override Point FindAnchorPoint(double mouseX, double mouseY, bool isStartingPoint = false)
        {
            var location = _locationList.First(x => x.Label.Name == _clickedLabel.Name);

            if (location.DoorsLocationList == new List<Point>())
            {
                MessageBox.Show("There is no possible doors to connect to");
                throw new Exception("Error while connecting two locations");
            }

            var closest = new Point(0, 0);
            var distance = double.MaxValue;
            var tmpPoint = new Point(0, 0);

            foreach (var point in location.DoorsLocationList)
            {
                var xDoorLocation = GetRelativeLabelPoint(location.Label).X + (point.X + 0.5) * (location.Label.ActualWidth / location.Width);
                var yDoorLocation = GetRelativeLabelPoint(location.Label).Y + (point.Y + 0.5) * (location.Label.ActualHeight / location.Height);

                var newDistance = Math.Sqrt(Math.Pow(xDoorLocation - mouseX, 2) + Math.Pow(yDoorLocation - mouseY, 2));
                if (newDistance < distance)
                {
                    tmpPoint = point;
                    distance = newDistance;
                    closest = new Point(xDoorLocation, yDoorLocation);
                }
            }

            if (isStartingPoint)
                location.DoorConnections.Add(new DoorConnection { Destination = "tmp", IsSource = true, location = new Tuple<int, int>((int)tmpPoint.X, (int)tmpPoint.Y) });
            else
                location.DoorConnections.Add(new DoorConnection { Source = "tmp", IsSource = false, location = new Tuple<int, int>((int)tmpPoint.X, (int)tmpPoint.Y) });

            location.DoorsLocationOnGrid.Add(closest);
            return closest;
        }

        public override Point GetRelativeLabelPoint(Label label)
        {
            return label.TransformToAncestor(GetMainGrid()).Transform(new Point(0, 0));
        }

        public override void AddLineToMainGrid(GraphEdge line)
        {
            GetMainGrid().Children.Add(line);
        }
    }
}
