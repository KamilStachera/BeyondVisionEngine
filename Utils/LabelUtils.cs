using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace BeyondVisionEngine.Utils
{
    /// <summary>
    /// Class for managing labels
    /// </summary>
    public static class LabelUtils
    {
        /// <summary>
        /// Method creates new label
        /// </summary>
        /// <param name="name">When empty string is passed -> "" method generates it's own random string</param>
        /// <param name="content">Label description (as seen on the screen)</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="onMouseDown"></param>
        /// <param name="onMouseMove"></param>
        /// <param name="onMouseUp"></param>
        /// <param name="brush"></param>
        /// <param name="borderThickness"></param>
        /// <returns>Newly created label</returns>
        public static Label CreateNewLabel(string name,
                                           string content,
                                           int width,
                                           int height,
                                           MouseButtonEventHandler onMouseDown,
                                           MouseEventHandler onMouseMove,
                                           MouseButtonEventHandler onMouseUp,
                                           SolidColorBrush brush,
                                           int borderThickness = 0)
        {
            if (name == "")
                name = GeneralUtils.GenerateRandomName();

            var label = new Label
            {
                Name = name,
                Content = content,
                Height = height,
                Width = width
            };

            label.MouseDown += onMouseDown;
            label.MouseMove += onMouseMove;
            label.MouseUp += onMouseUp;
            label.BorderBrush = brush;
            label.BorderThickness = new Thickness(borderThickness);

            return label;
        }

        public static Point GetUpperLeftAnchorPoint(Label label, Grid grid)
        {
            return label.TransformToAncestor(grid).Transform(new Point(0, 0));
        }

        public static Point FindLabelMainAnchorPoint(Label label, Grid grid)
        {
            return new Point(GetUpperLeftAnchorPoint(label, grid).X + label.ActualWidth / 2, GetUpperLeftAnchorPoint(label, grid).Y);
        }

        public static Point FindLabelDownLeftAnchorPoint(Label label, Grid grid)
        {
            return new Point(GetUpperLeftAnchorPoint(label, grid).X, GetUpperLeftAnchorPoint(label, grid).Y + label.ActualHeight);
        }

        public static Point FindLabelDownRightAnchorPoint(Label label, Grid grid)
        {
            return new Point(GetUpperLeftAnchorPoint(label, grid).X + label.ActualWidth, GetUpperLeftAnchorPoint(label, grid).Y + label.ActualHeight);
        }

        public static Point FindLabelUpperRightAnchorPoint(Label label, Grid grid)
        {
            return new Point(GetUpperLeftAnchorPoint(label, grid).X + label.ActualWidth, GetUpperLeftAnchorPoint(label, grid).Y);

        }

    }
}
