using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Newtonsoft.Json;
using com.bodurov.NdSpace.Extensions;
using com.bodurov.NdSpace.Interface;
using com.bodurov.NdSpace.Model;

namespace com.bodurov.NdSpace.Visualizations
{
    public class TestConfig : ISpaceConfig
    {
        int ISpaceConfig.NumDimensions { get { return 1; } }
        double ISpaceConfig.DefaultEpsilon { get { return 5; } }
    }


    public partial class DimensionWindow : Window
    {

        private readonly IDimensionPainter _dimensionPainter;
        
        private readonly Space<int> _space;
        private readonly ISpaceManager _manager;
        private double _mouseDownX;



        public DimensionWindow()
        {
            InitializeComponent();

            TheCanvas.MouseMove += OnCanvasMouseMove;
            TheCanvas.MouseDown += OnCanvasMouseDown;
            TheCanvas.MouseUp += OnCanvasMouseUp;

            

            

            _manager = SpaceFactory.Current.DimensionManager;
            _space = _manager.CreateSpace<int>(new TestConfig());
            _dimensionPainter = new DimensionPainter(TheCanvas);

            _manager.PopulateSpace(_space, GetTestPoints(_space));

            DrawTree();

            //MessageBox.Show(sw.ElapsedMilliseconds + "|");
        }

        private static List<SpacePointSource<int>> GetTestPoints(Space<int> space)
        {
            var list = new List<SpacePointSource<int>>();
            for (var x = 50; x <= 950; x += 100)
            {
                list.Add(new SpacePointSource<int>(space, x, x));
            }
            return list;
        }

        class Dragged
        {
            public Rectangle Rect;
            public SpacePoint<int> Point;
        }

        private Dragged _dragged = null;
        private void OnSpacePointMouseDown(Rectangle rect, SpacePoint<int> point, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                _manager.RemovePoint(point);
                DrawTree();
            }
            else if (e.ChangedButton == MouseButton.Left)
            {
                _dragged = new Dragged { Point = point, Rect = rect };
                return;
            }
            _dragged = null;
        }

        private void OnSpacePointMouseUp(Rectangle rect, SpacePoint<int> point, MouseButtonEventArgs e)
        {
            
        }

        private void DrawTree()
        {
            var sw = Stopwatch.StartNew();
            _dimensionPainter.Draw(_space.Dimensions[0]);
            sw.Stop();


            InfoBox.Text = 
                JsonConvert.SerializeObject((object)new
                {
                    time = sw.ElapsedMilliseconds,
                    dim0_count = _space.Dimensions[0].Count
                }, Formatting.Indented);
        }

        private void OnCanvasMouseUp(object sender, MouseButtonEventArgs e)
        {
            _dragged = null;
            var mp = e.GetPosition(TheCanvas);
            var x = mp.X;
            if (Math.Abs(_mouseDownX - x) < 10)
            {
                if (e.ChangedButton == MouseButton.Right)
                {
                    _manager.AddPoint(_space, (int)(x * 100 + Rnd()), (float)x);
                    DrawTree();
                }
            }
            if (_chosenEllipses.Count > 0)
            {
                _chosenEllipses.ForEach(el => el.Fill = Brushes.Moccasin);
            }

            Rectangle rect;
            if (TryFindSpacePointRectangle(mp, out rect))
            {
                OnSpacePointMouseUp(rect, rect.Tag as SpacePoint<int>, e);
            }
        }

        private readonly HashSet<Ellipse> _chosenEllipses = new HashSet<Ellipse>();
        private void OnCanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            var mp = e.GetPosition(TheCanvas);
            _mouseDownX = mp.X;
            if (e.ChangedButton == MouseButton.Left)
            {
                DimensionPoint<int> left;
                DimensionPoint<int> right;
                var found = _manager.TryFindPoint(_space.Dimensions[0], (float)_mouseDownX, out left, out right);

                _chosenEllipses.Clear();
                foreach (FrameworkElement child in TheCanvas.Children)
                {
                    var point = child.Tag as DimensionPoint<int>;
                    if(point == null) continue;
                    if (point == left)
                    {
                        ((Ellipse) child).Fill = found ? Brushes.Red : Brushes.Orange;
                        _chosenEllipses.Add(((Ellipse) child));
                    }
                    if (point == right)
                    {
                        ((Ellipse) child).Fill = found ? Brushes.Red : Brushes.Orchid;
                        _chosenEllipses.Add(((Ellipse)child));
                    }
                    if(_chosenEllipses.Count >= 2) break;
                }
            }

            Rectangle rect;
            if (TryFindSpacePointRectangle(mp, out rect))
            {
                OnSpacePointMouseDown(rect, rect.Tag as SpacePoint<int>, e);
            }

        }

        private bool TryFindSpacePointRectangle(Point mp, out Rectangle found)
        {
            if (mp.Y > 200)
            {
                found = null;
                return false;
            }

            foreach (var rec in from rec in TheCanvas
                                    .Children
                                    .OfType<Rectangle>()
                                    .Where(rec => rec.Tag is SpacePoint<int>) 
                                let x1 = (double)rec.GetValue(Canvas.LeftProperty) 
                                let y1 = (double)rec.GetValue(Canvas.TopProperty) 
                                let x2 = rec.Width + x1 
                                let y2 = rec.Height + y1 
                                where mp.X > x1 && mp.X < x2 && mp.Y > y1 && mp.Y < y2 
                                select rec)
            {
                found = rec;
                return true;
            }
            found = null;
            return false;
        }

        private void OnCanvasMouseMove(object sender, MouseEventArgs e)
        {
            var mp = e.GetPosition(TheCanvas);
            _dimensionPainter.PointerLine.X1 = _dimensionPainter.PointerLine.X2 = mp.X;

            if (_dragged != null)
            {
                _manager.Reposition(_space.Dimensions[0], _dragged.Point, (float)mp.X);
                DrawTree();
            }
        }

        private static double Rnd()
        {
            return new Random(DateTime.UtcNow.Millisecond).Next(0, 100);
        }

        private void OnTestButtonClick(object sender, RoutedEventArgs e)
        {
            var dimension = _space.Dimensions[0];
            var head = dimension.Head;
            var tail = dimension.Tail;
        }
    }
}

