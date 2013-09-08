using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using com.bodurov.NdSpace.Extensions;
using com.bodurov.NdSpace.Model;

namespace com.bodurov.NdSpace.Visualizations
{
    public delegate void SpacePointEventHandler(SpacePoint<int> point, MouseButtonEventArgs e);
    public interface IDimensionPainter
    {
        void Draw(Dimension<int> dimension);
        Line PointerLine { get; }
    }
    public class DimensionPainter : IDimensionPainter
    {
        private const double ypad = 50;
        private const double height = 600;

        private readonly Canvas _canvas;

        private readonly Line _pointerLine;

        public DimensionPainter(Canvas canvas)
        {
            _canvas = canvas;

            _canvas.Children.Add(_pointerLine = new Line
            {
                X1 = 0,
                Y1 = 0,
                X2 = 0,
                Y2 = height,
                Stroke = new SolidColorBrush(Color.FromArgb(0x77, 0xFF, 0, 0)),
                StrokeThickness = 1
            });
        }

        Line IDimensionPainter.PointerLine { get { return _pointerLine; } }

        void IDimensionPainter.Draw(Dimension<int> dimension)
        {
            ClearCanvas();


            if (dimension.Head == null) return;

            DrawHeadPointer(dimension.Head.Position, ypad * 3, 7);
            DrawTailPointer(dimension.Tail.Position, ypad * 3, 7);

            var dpl0 = dimension.Head.Head;
            while (dpl0 != null)
            {

                DrawVerticalLines(dpl0);

                dpl0 = dpl0.Next;
            }

            _canvas.Children.Add(_pointerLine);
        }

        private void ClearCanvas()
        {
            _canvas.Children.Clear();
        }

        private void DrawHeadPointer(double x, double y, int pad)
        {
            var p = new Polygon
            {
                Stroke = Brushes.Green,
                Fill = Brushes.Green,
                StrokeThickness = 0.5,
                Points = new PointCollection
                            {
                                new Point(x - pad, y - pad), 
                                new Point(x - pad*2, y - pad),
                                new Point(x - pad, y - pad*2), 
                            }
            };
            _canvas.Children.Add(p);
        }

        private void DrawTailPointer(double x, double y, int pad)
        {
            var p = new Polygon
            {
                Stroke = Brushes.OrangeRed,
                Fill = Brushes.OrangeRed,
                StrokeThickness = 0.5,
                Points = new PointCollection
                            {
                                new Point(x + pad, y + pad), 
                                new Point(x + pad*2, y + pad),
                                new Point(x + pad, y + pad*2), 
                            }
            };
            _canvas.Children.Add(p);
        }

        private void DrawVerticalLines(DimensionLink<int> dpl0)
        {
            const double ySpacePoints = ypad * 2;
            const double yLevel0 = ypad * 2 + ypad * 2;
            const int linePad = 10;

            var x = (double)dpl0.Point.Position;
            _canvas.Children.Add(new Line { X1 = x, Y1 = ySpacePoints + linePad, X2 = x, Y2 = ySpacePoints + ypad - linePad, Stroke = new SolidColorBrush(Colors.DarkBlue) });
            _canvas.Children.Add(new Line { X1 = x, Y1 = ySpacePoints + ypad + linePad, X2 = x, Y2 = yLevel0 - linePad, Stroke = new SolidColorBrush(Colors.DarkBlue) });

            DrawHeadPointer(x, yLevel0, linePad / 2);
            DrawTailPointer(x, yLevel0 + dpl0.Point.Tail.Level * ypad, linePad / 2);

            DrawLink(dpl0);

            if (dpl0.Next != null)
            {
                var nextX = dpl0.Next.Point.Position;
                _canvas.Children.Add(new Line { X1 = x + linePad, Y1 = yLevel0, X2 = nextX - linePad, Y2 = yLevel0, Stroke = new SolidColorBrush(Colors.DarkGreen) });
            }

            var upper = dpl0.Upper;
            var prevYLevel = yLevel0;
            while (upper != null)
            {
                DrawLink(upper);

                var yLevel = prevYLevel + ypad;
                _canvas.Children.Add(new Line { X1 = x, Y1 = prevYLevel + linePad, X2 = x, Y2 = yLevel - linePad, Stroke = new SolidColorBrush(Colors.DarkBlue) });
                prevYLevel = yLevel;

                if (upper.Next != null)
                {
                    var nextX = upper.Next.Point.Position;
                    _canvas.Children.Add(new Line { X1 = x + linePad, Y1 = yLevel, X2 = nextX - linePad, Y2 = yLevel, Stroke = new SolidColorBrush(Colors.DarkGreen) });
                }

                upper = upper.Upper;
            }
        }

        private void DrawLink(DimensionLink<int> link)
        {
            var x = (double)link.Point.Position;
            var y = link.Level * ypad + ypad * 4;
            const int containerPad = 7;
            var p = new Polygon
            {
                Stroke = Brushes.Black,
                Fill = Brushes.LightBlue,
                StrokeThickness = 0.5,
                Points = new PointCollection { new Point(x, y - containerPad), new Point(x + containerPad, y), new Point(x, y + containerPad), new Point(x - containerPad, y) }
            };
            _canvas.Children.Add(p);

            var tb = Text(link.Level.ToString(CultureInfo.InvariantCulture), x, y);
            _canvas.Children.Add(tb);

            if (link.Level == 0)
            {
                var el = new Ellipse
                {
                    Tag = link.Point,
                    Width = containerPad * 2,
                    Height = containerPad * 2,
                    Fill = Brushes.Moccasin,
                    Stroke = Brushes.Gray
                };
                el.SetValue(Canvas.LeftProperty, x - containerPad);
                el.SetValue(Canvas.TopProperty, y - ypad - containerPad);
                _canvas.Children.Add(el);
                y = ypad * 2;

                foreach (var sp in link.Point.Points)
                {
                    _canvas.Children.Add(new Rectangle
                        {
                            Width = containerPad * 5,
                            Height = containerPad * 2,
                            Fill = Brushes.LightSkyBlue,
                            Stroke = Brushes.Black,
                            StrokeThickness = 0.5,
                            Tag = sp
                        }
                        .Do(r => r.SetValue(Canvas.LeftProperty, x - containerPad * 2.5))
                        .Do(r => r.SetValue(Canvas.TopProperty, y - containerPad)));
                    

                    tb = Text(sp.Value.ToString(CultureInfo.InvariantCulture), x, y);
                    _canvas.Children.Add(tb);
                    

                    y -= containerPad * 2;
                }

            }
        }


        

        private static TextBlock Text(string text, double x, double y)
        {
            var tb = new TextBlock { Text = text, Foreground = Brushes.Black, FontSize = 11 };
            tb.Arrange(new Rect(0, 0, 200, 200));
            tb.SetValue(Canvas.LeftProperty, x - tb.DesiredSize.Width / 2.0);
            tb.SetValue(Canvas.TopProperty, y - tb.DesiredSize.Height / 2.0);
            tb.LayoutTransform = new ScaleTransform(1, -1);
            return tb;
        }


    }
}