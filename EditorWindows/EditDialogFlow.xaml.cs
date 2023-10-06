using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using BeyondVisionEngine.Components;
using BeyondVisionEngine.Utils;

namespace BeyondVisionEngine.EditorWindows
{
    /// <summary>
    /// Interaction logic for EditDialogFlow.xaml
    /// </summary>
    public partial class EditDialogFlow
    {
        private DialogGraph _dialogGraph;
        private string _recentlyConnected;

        public EditDialogFlow(DialogGraph dialogGraph)
        {
            _dialogGraph = dialogGraph;
            InitializeComponent();
        }
        private void EditDialogFlow_OnLoaded(object sender, RoutedEventArgs e)
        {
            foreach (var dialogGraphDialogGameEvent in _dialogGraph.dialogGameEvents)
            {
                dialogGraphDialogGameEvent.Label.MouseDown += TextBox_OnMouseDown;
                dialogGraphDialogGameEvent.Label.MouseMove += TextBox_OnMouseMove;
                dialogGraphDialogGameEvent.Label.MouseUp += TextBox_OnMouseUp;

                GetMainGrid().Children.Add(dialogGraphDialogGameEvent.Label);
            }
        }

        private DialogGameEvent GetDialogByLabel(Label label)
        {
            return _dialogGraph.dialogGameEvents.First(x => x.Label.Name == label.Name);
        }

        public override void AddNewConnection(Label label1, Label label2)
        {
            if (_recentlyConnected == "left")
                GetDialogByLabel(label1).LeftOutputDialog = GetDialogByLabel(label2).Name;
            else
                GetDialogByLabel(label1).RightOutputDialog = GetDialogByLabel(label2).Name;

            GetDialogByLabel(label2).InputDialog = GetDialogByLabel(label1).Name;
        }

        public override void DblClkOnLabel()
        {
            EditDialogEvent editDialog = new EditDialogEvent(GetDialogByLabel(_clickedLabel));
            editDialog.ShowDialog();
        }

        public override Grid GetMainGrid()
        {
            return EditDialogFlowMainGrid;
        }

        public override void AddLineToMainGrid(GraphEdge line)
        {
            GetMainGrid().Children.Add(line);
        }

        public override void NotifyLabelMove(double offsetX, double offsetY)
        {
            for (int i = 0; i < GetMainGrid().Children.Count; i++)
            {
                if (!(GetMainGrid().Children[i] is GraphEdge graphEdge))
                    continue;

                if (IsInLabelRange(graphEdge.Destination, _clickedLabel))
                    graphEdge.Destination = new Point(graphEdge.Destination.X + offsetX, graphEdge.Destination.Y + offsetY);

                if (IsInLabelRange(graphEdge.Source, _clickedLabel))
                    graphEdge.Source = new Point(graphEdge.Source.X + offsetX, graphEdge.Source.Y + offsetY);
            }
        }

        public bool IsInLabelRange(Point p1, Label label)
        {
            if (Math.Abs(LabelUtils.FindLabelMainAnchorPoint(_clickedLabel, GetMainGrid()).X - p1.X) < 3 && Math.Abs(LabelUtils.FindLabelMainAnchorPoint(_clickedLabel, GetMainGrid()).Y - p1.Y) < 3)
                return true;
            if (Math.Abs(LabelUtils.FindLabelDownLeftAnchorPoint(_clickedLabel, GetMainGrid()).X - p1.X) < 3 && Math.Abs(LabelUtils.FindLabelDownLeftAnchorPoint(_clickedLabel, GetMainGrid()).Y - p1.Y) < 3)
                return true;
            if (Math.Abs(LabelUtils.FindLabelDownRightAnchorPoint(_clickedLabel, GetMainGrid()).X - p1.X) < 3 && Math.Abs(LabelUtils.FindLabelDownRightAnchorPoint(_clickedLabel, GetMainGrid()).Y - p1.Y) < 3)
                return true;

            return false;
        }

        public override Point FindAnchorPoint(double mouseX, double mouseY, bool isStartingPoint = false)
        {
            var dialogEvent = GetDialogByLabel(_clickedLabel);

            if (isStartingPoint && dialogEvent.DecisionCount < 2)
            {
                MessageBox.Show("Not enough decisions in dialog");
                throw new Exception("Error while trying to connect two dialogs");
            }

            if (!isStartingPoint)
                return LabelUtils.FindLabelMainAnchorPoint(_clickedLabel, GetMainGrid());

            var leftPoint = LabelUtils.FindLabelDownLeftAnchorPoint(_clickedLabel, GetMainGrid());
            var rightPoint = LabelUtils.FindLabelDownRightAnchorPoint(_clickedLabel, GetMainGrid());

            var leftDecisionDistance = Math.Sqrt(Math.Pow(leftPoint.X - mouseX, 2) + Math.Pow(leftPoint.Y - mouseY, 2));
            var rightDecisionDistance =
                Math.Sqrt(Math.Pow(rightPoint.X - mouseX, 2) + Math.Pow(rightPoint.Y - mouseY, 2));

            if (leftDecisionDistance < rightDecisionDistance)
            {
                _recentlyConnected = "left";
                return leftPoint;
            }

            _recentlyConnected = "right";
            return rightPoint;
        }

        public override Point GetRelativeLabelPoint(Label label)
        {
            return label.TransformToAncestor(GetMainGrid()).Transform(new Point(0, 0));
        }

        private void AddDialogButton_OnClick(object sender, RoutedEventArgs e)
        {

            var labelContent = $"Dialog";
            var newEventLabel = LabelUtils.CreateNewLabel("", labelContent, 100, 50, TextBox_OnMouseDown,
                                                              TextBox_OnMouseMove, TextBox_OnMouseUp, Brushes.Green, 5);

            GetMainGrid().Children.Add(newEventLabel);

            var newDialog = new DialogGameEvent(newEventLabel);
            _dialogGraph.dialogGameEvents.Add(newDialog);
        }

        private void EditDialogFlow_OnClosed(object sender, EventArgs e)
        {

            foreach (var child in GetMainGrid().Children)
            {
                if (!(child is Label label)) continue;
                label.MouseDown -= TextBox_OnMouseDown;
                label.MouseMove -= TextBox_OnMouseMove;
                label.MouseUp -= TextBox_OnMouseUp;
            }

            GetMainGrid().Children.Clear();
            Close();
        }
    }
}


//private void LoadEventLabels()
//{
//    int count = 1;
//    foreach (var locationEvent in _location.EventsList)
//    {
//        if (locationEvent is DialogGameEvent)
//        {
//            EditMajorLocationGrid.Children.Add(locationEvent.Label);
//        }
//        count += 1;
//    }
//}

//private void AddDialogEventButton_Click(object sender, RoutedEventArgs e)
//{
//    var labelContent = $"Dialog - {DialogCount}";
//    var newEventLabel = LabelUtils.CreateNewLabel("", labelContent, 100, 50, TextBox_OnMouseDown,
//                                                      TextBox_OnMouseMove, TextBox_OnMouseUp, Brushes.Green);

//    EditMajorLocationGrid.Children.Add(newEventLabel);

//    var newDialog = new DialogGameEvent(newEventLabel);
//    _location.EventsList.Add(newDialog);
//    DialogCount += 1;
//}

//private void ClearAllEventsButton_Click(object sender, RoutedEventArgs e)
//{
//    _location.EventsList.Clear();

//    for (int i = EditMajorLocationGrid.Children.Count - 1; i >= 0; i--)
//    {
//        var child = EditMajorLocationGrid.Children[i];
//        if (child is Label)
//        {
//            EditMajorLocationGrid.Children.RemoveAt(i);
//        }
//    }
//}

//private GameEvent GetEventByLabel(Label label)
//{
//    return _location.EventsList.First(x => x.Label.Name == label.Name);
//}

//public override void SetFocusOnClickedLabel(Label clickedLabel)
//{
//    _location.EventsList.Where(x => x.Label.Name == _clickedLabel.Name).ToList().ForEach(x => x.Label.BorderBrush = Brushes.Red);
//    _location.EventsList.Where(x => x.Label.Name != _clickedLabel.Name).ToList().ForEach(x => x.Label.BorderBrush = Brushes.Green);
//}

//public override void AddNewConnection(Label label1, Label label2)
//{
//    GetEventByLabel(label1).Connections.Add(GetEventByLabel(label2));
//    GetEventByLabel(label2).Connections.Add(GetEventByLabel(label1));
//}

//public override void DblClkOnLabel()
//{
//    EditDialogEvent editWindow = new EditDialogEvent((DialogGameEvent)_location.EventsList.First(x => x.Label == _clickedLabel));
//    editWindow.Show();
//}

//public override Grid GetMainGrid()
//{
//    return EditMajorLocationGrid;
//}

//public override void UpdateView()
//{
//}

//public override void AddLineToMainGrid(GraphEdge line)
//{
//    GetMainGrid().Children.Add(line);
//}