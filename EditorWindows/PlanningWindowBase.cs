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
using System.Windows.Controls.Primitives;
using BeyondVisionEngine.EditorWindows;
using Microsoft.VisualBasic;
using Newtonsoft.Json;

namespace BeyondVisionEngine.EditorWindows
{
    /// <summary>
    /// Interaction logic for PlanningWindow.xaml
    /// </summary>
    public abstract class PlanningWindowBase : Window
    {
        protected bool _isMoving;
        protected Point? _buttonPosition;
        protected double _deltaX;
        protected double _deltaY;
        protected TranslateTransform _currentTT;
        protected Label _clickedLabel;
        protected List<Label> labels = new List<Label>();

        List<Tuple<Label, Label>> connections = new List<Tuple<Label, Label>>();
        List<Label> newConnection = new List<Label>();
        protected bool firstConnection = false;

        Point startPoint;
        Point endPoint;

        #region DragManagementMethods

        protected void TextBox_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            _clickedLabel = ((Label)sender);

            var mousePosition = Mouse.GetPosition(GetMainGrid());

            if (_buttonPosition == null)
                _buttonPosition = _clickedLabel.TransformToAncestor(GetMainGrid()).Transform(new Point(0, 0));

            if (Keyboard.IsKeyDown(Key.C))
            {
                if (!firstConnection)
                {
                    newConnection.Add(_clickedLabel);
                    firstConnection = true;

                    startPoint = FindAnchorPoint(mousePosition.X, mousePosition.Y, true);
                }
                else
                {
                    endPoint = FindAnchorPoint(mousePosition.X, mousePosition.Y);

                    newConnection.Add(_clickedLabel);
                    connections.Add(new Tuple<Label, Label>(newConnection[0], newConnection[1]));

                    AddNewConnection(newConnection[0], newConnection[1]);

                    var edge = new GraphEdge {Source = startPoint, Destination = endPoint};

                    AddLineToMainGrid(edge);

                    newConnection.Clear();
                    firstConnection = false;

                    ResetDraggingVariables();
                    return;
                }

            }

            if (e.ClickCount >= 2)
            {
                DblClkOnLabel();
                return;
            }

            _deltaX = mousePosition.X - _buttonPosition.Value.X;
            _deltaY = mousePosition.Y - _buttonPosition.Value.Y;
            _isMoving = true;

            _currentTT = _clickedLabel.RenderTransform as TranslateTransform;
        }

        private static Point diff;
        private static Point prev;

        protected void TextBox_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!_isMoving) return;

            var mousePoint = Mouse.GetPosition(GetMainGrid());

            var offsetX = (_currentTT == null ? _buttonPosition.Value.X : _buttonPosition.Value.X - _currentTT.X) + _deltaX - mousePoint.X;
            var offsetY = (_currentTT == null ? _buttonPosition.Value.Y : _buttonPosition.Value.Y - _currentTT.Y) + _deltaY - mousePoint.Y;

            if (prev.X == 0 && prev.Y == 0)
            {
                prev = new Point(offsetX, offsetY);
            }
            else
            {
                diff = new Point(prev.X - offsetX, prev.Y - offsetY);
                prev = new Point(offsetX,offsetY);
            }

            _clickedLabel.RenderTransform = new TranslateTransform(-offsetX, -offsetY);

            Trace.WriteLine($"{diff.X} : {diff.Y}");
            if (Math.Abs(diff.X) < 10 && Math.Abs(diff.Y) < 10)
                NotifyLabelMove(diff.X, diff.Y);
        }
        protected void TextBox_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            ResetDraggingVariables();
        }
        #endregion

        #region UtilityMethods

        protected void ResetDraggingVariables()
        {
            _isMoving = false;
            _buttonPosition = null;
        }


        #endregion

        #region AbstractMethods

        public abstract void AddNewConnection(Label label1, Label label2);

        public abstract void DblClkOnLabel();

        public abstract Grid GetMainGrid();

        public abstract void AddLineToMainGrid(GraphEdge line);

        public abstract void NotifyLabelMove(double offsetX, double offsetY);

        public abstract Point FindAnchorPoint(double mouseX, double mouseY, bool isStartingPoint = false);

        public abstract Point GetRelativeLabelPoint(Label label);

        #endregion

    }
}


