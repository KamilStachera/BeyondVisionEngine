using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BeyondVisionEngine.Components;
using BeyondVisionEngine.EditorWindows;
using BeyondVisionEngine.Utils;
using BeyondVisionEngine.Visuals;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using Point = System.Drawing.Point;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace BeyondVisionEngine
{
    /// <summary>
    /// Interaction logic for EditMajorLocation.xaml
    /// </summary>
    public partial class EditMajorLocation
    {
        private MajorLocation _location;
        private Tuple<int, int> _mousePosition;

        public EditMajorLocation(MajorLocation location)
        {
            _location = location;
            InitializeComponent();
            Title = $"EDITING - {location.LocationName}";
            EditMajorLocationGrid.MouseDown += EditMajorLocationGridOnMouseDown;
        }

        private void EditMajorLocationGridOnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var mousePositionX = e.GetPosition(EditMajorLocationGrid).X;
            var mousePositionY = e.GetPosition(EditMajorLocationGrid).Y;

            var cellX = (int)(mousePositionX / _location.LocationCells[0][0].Width);
            var cellY = (int)(mousePositionY / _location.LocationCells[0][0].Height);

            if (Mouse.LeftButton == MouseButtonState.Pressed)
                HandleLMBClickOnGrid(cellX, cellY);
            if (Mouse.RightButton == MouseButtonState.Pressed)
                HandleRMBClickOnGrid();

            UpdateView();
        }

        private void UpdateView()
        {
            canvas.Children.Clear();

            for (var i = 0; i < _location.LocationCells.Length; i++)
            {
                var t = _location.LocationCells[i];
                for (var j = 0; j < t.Length; j++)
                {
                    var t1 = t[j];
                    var rectW = t1.Width;
                    var rectH = t1.Height;

                    var rect = new Rectangle
                    {
                        Width = rectW,
                        Height = rectH,
                        Stroke = t1.HasDialog ? Brushes.Blue : Brushes.Gray,
                        StrokeThickness = 4,
                        Fill = t1.IsCollision ? Brushes.Black : Brushes.White,
                    };

                    if (t1.IsDoors)
                        ChangeIcon(rect, "doors");
                    else if (t1.IsKey)
                        ChangeIcon(rect, "key");
                    else if (t1.IsEnemy)
                        ChangeIcon(rect, "enemy");
                    else if (_location.StartPoint.X == j && _location.StartPoint.Y == i)
                        ChangeIcon(rect, "start");
                    else if (_location.EndPoint.X == j && _location.EndPoint.Y == i)
                        ChangeIcon(rect, "end");


                    Canvas.SetTop(rect, t1.StartHeight);
                    Canvas.SetLeft(rect, t1.StartWidth);
                    canvas.Children.Add(rect);
                }
            }
        }

        private void ChangeIcon(Rectangle rectangle, string icon)
        {
            var pathToIcon = "";

            switch (icon)
            {
                case "key":
                    pathToIcon = "key.png";
                    break;
                case "enemy":
                    pathToIcon = "czaszka.gif";
                    break;
                case "doors":
                    pathToIcon = "doors.png";
                    break;
                case "start":
                    pathToIcon = "start.png";
                    break;
                case "end":
                    pathToIcon = "end.png";
                    break;
            }

            rectangle.Fill = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(pathToIcon, UriKind.Relative))
            };
        }


        private void InitializeCells()
        {
            var cellsCountX = _location.Width;
            var cellsCountY = _location.Height;

            var gridHeight = EditMajorLocationGrid.ActualHeight;
            var gridWidth = EditMajorLocationGrid.ActualWidth;

            var cellWidth = gridWidth / cellsCountX;
            var cellHeight = gridHeight / cellsCountY;

            _location.LocationCells = new LocationCell[cellsCountY][];
            for (var i = 0; i < _location.LocationCells.Length; i++)
            {
                _location.LocationCells[i] = new LocationCell[cellsCountX];
                for (var j = 0; j < _location.LocationCells[i].Length; j++)
                {
                    _location.LocationCells[i][j] = new LocationCell(cellWidth * j, cellHeight * i, cellWidth, cellHeight);
                    if (i == 0 || j == 0 || i == _location.LocationCells.Length - 1 || j == _location.LocationCells.Length - 1)
                        _location.LocationCells[i][j].IsCollision = true;
                }
            }
        }

        private void HandleLMBClickOnGrid(int cellX, int cellY)
        {
            var cell = _location.LocationCells[cellY][cellX];

            if (cell.IsFilled())
            {
                if (cell.IsDoors)
                    _location.DoorsLocationList.Remove(_location.DoorsLocationList.First(x => (int)x.X == cellX && (int)x.Y == cellY));
                if (cell.IsEnemy)
                    _location.Enemies.Remove(_location.Enemies.First(x => x.Position.Item1 == cellX && x.Position.Item2 == cellY));
                if (cell.HasDialog)
                    _location.DialogsList.Remove(_location.DialogsList.First(x => x.positionInMajorLocation.Item1 == cellX && x.positionInMajorLocation.Item2 == cellY));

                cell.ClearCell();
            }
            else
                cell.IsCollision = true;
        }

        private void HandleRMBClickOnGrid()
        {
            _mousePosition = GetMousePosition();
        }


        private Tuple<int, int> GetMousePosition()
        {
            var mousePositionX = Mouse.GetPosition(EditMajorLocationGrid).X;
            var mousePositionY = Mouse.GetPosition(EditMajorLocationGrid).Y;

            var cellX = (int)(mousePositionX / _location.LocationCells[0][0].Width);
            var cellY = (int)(mousePositionY / _location.LocationCells[0][0].Height);

            return new Tuple<int, int>(cellX, cellY);
        }

        private void EditMajorLocation_OnClosing(object sender, CancelEventArgs e)
        {
            if (!System.IO.Directory.Exists("maps"))
                System.IO.Directory.CreateDirectory("maps");

            var bounds = this.RestoreBounds;
            using (Bitmap bitmap = new Bitmap((int)bounds.Width - 14, (int)bounds.Height - 38))
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.CopyFromScreen(new Point((int)bounds.Left + 7, (int)bounds.Top + 31), Point.Empty, new System.Drawing.Size((int)bounds.Size.Width - 14, (int)bounds.Size.Height - 38));
                }

                bitmap.Save($@"maps/map - {_location.Label.Name} - {_location.ImageCount}.jpg", ImageFormat.Jpeg);
            }

            _location.Label.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri($@"maps/map - {_location.Label.Name} - {_location.ImageCount}.jpg", UriKind.Relative)) };
            _location.Label.Content = "";
            _location.ImageCount += 1;
        }

        private void EditMajorLocation_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (_location.LocationCells == null)
                InitializeCells();

            UpdateView();
        }

        private void Dialog_OnClick(object sender, RoutedEventArgs e)
        {
            if (_location.LocationCells[_mousePosition.Item2][_mousePosition.Item1].HasDialog)
            {
                EditDialogFlow dialogFlowWindow0 = new EditDialogFlow(_location.DialogsList
                    .First(x => x.positionInMajorLocation.Item1 == _mousePosition.Item1 && x.positionInMajorLocation.Item2 == _mousePosition.Item2));
                dialogFlowWindow0.ShowDialog();
                return;
            }

            _location.LocationCells[_mousePosition.Item2][_mousePosition.Item1].HasDialog = true;
            UpdateView();

            _location.DialogsList.Add(new DialogGraph(new Tuple<int, int>(_mousePosition.Item1, _mousePosition.Item2)));

            EditDialogFlow dialogFlowWindow = new EditDialogFlow(_location.DialogsList
                .First(x => x.positionInMajorLocation.Item1 == _mousePosition.Item1 && x.positionInMajorLocation.Item2 == _mousePosition.Item2));
            dialogFlowWindow.ShowDialog();
        }

        private void Doors_OnClick(object sender, RoutedEventArgs e)
        {
            _location.LocationCells[_mousePosition.Item2][_mousePosition.Item1].IsDoors = true;
            _location.DoorsLocationList.Add(new System.Windows.Point(_mousePosition.Item1, _mousePosition.Item2));
            UpdateView();
        }

        private void Key_OnClick(object sender, RoutedEventArgs e)
        {
            _location.LocationCells[_mousePosition.Item2][_mousePosition.Item1].IsKey = true;
            UpdateView();
        }

        private void Enemy_OnClick(object sender, RoutedEventArgs e)
        {
            if (_location.LocationCells[_mousePosition.Item2][_mousePosition.Item1].IsEnemy)
            {
                var enemyWindow0 = new EnemyWindow(_location.Enemies
                    .First(x => x.Position.Item1 == _mousePosition.Item1 && x.Position.Item2 == _mousePosition.Item2));

                enemyWindow0.ShowDialog();
                return;
            }

            _location.LocationCells[_mousePosition.Item2][_mousePosition.Item1].IsEnemy = true;

            var enemy = new Enemy();
            var enemyWindow = new EnemyWindow(enemy);
            enemyWindow.ShowDialog();
            enemy.Position = new Tuple<int, int>(_mousePosition.Item1, _mousePosition.Item2);
            _location.Enemies.Add(enemy);

            UpdateView();
        }

        private void GameStart_OnClick(object sender, RoutedEventArgs e)
        {
            _location.StartPoint = new System.Windows.Point(_mousePosition.Item1, _mousePosition.Item2);
            UpdateView();
        }

        private void GameEnd_OnClick(object sender, RoutedEventArgs e)
        {
            _location.EndPoint = new System.Windows.Point(_mousePosition.Item1, _mousePosition.Item2);
            UpdateView();
        }
    }
}