using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BeyondVisionEngine
{
    public sealed class GraphEdge : UserControl
    {
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(Point), typeof(GraphEdge), new FrameworkPropertyMetadata(default(Point)));
        public Point Source
        {
            get => (Point)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        public static readonly DependencyProperty DestinationProperty = DependencyProperty.Register("Destination", typeof(Point), typeof(GraphEdge), new FrameworkPropertyMetadata(default(Point)));
        public Point Destination
        {
            get => (Point)GetValue(DestinationProperty);
            set => SetValue(DestinationProperty, value);
        }

        public GraphEdge()
        {
            var segment = new LineSegment(default, true);
            var figure = new PathFigure(default, new[] { segment }, false);
            var geometry = new PathGeometry(new[] { figure });
            var sourceBinding = new Binding { Source = this, Path = new PropertyPath(SourceProperty) };
            var destinationBinding = new Binding { Source = this, Path = new PropertyPath(DestinationProperty) };
            BindingOperations.SetBinding(figure, PathFigure.StartPointProperty, sourceBinding);
            BindingOperations.SetBinding(segment, LineSegment.PointProperty, destinationBinding);
            BindingOperations.SetBinding(segment, LineSegment.PointProperty, destinationBinding);
            Content = new Path
            {
                Data = geometry,
                StrokeThickness = 1,
                Stroke = Brushes.Red,
                MinWidth = 2,
                MinHeight = 2
            };
        }
    }
}
