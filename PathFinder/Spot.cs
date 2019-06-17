using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PathFinder
{
    public class Spot
    {
        int _i;
        int _j;
        int _f;
        int _g;
        int _h;
        bool _wall = false;
        Shape _shape;
        List<Spot> _neighbors;
        Spot _previous;

        Storage _storage;


        public void reset()
        {
            _storage.Window.mainCanvas.Children.Remove(Shape);
            if (Neighbors != null) { Neighbors.Clear(); }
            Previous = null;
        }

        public Spot(int i, int j, bool wall)
        {
            _storage = Storage.getInstance();

            _wall = wall;
            _i = i;
            _j = j;
            Shape = new Rectangle()
            {
                Width = _storage.WidthOfOneSpot,
                Height = _storage.HeigthOfOneSpot,
                Stroke = Storage.BLACK,
                StrokeThickness = 1,
            };
            Canvas.SetLeft(Shape, I * _storage.WidthOfOneSpot);
            Canvas.SetTop(Shape, J * _storage.HeigthOfOneSpot);

            if (Wall) { Shape.Fill = Storage.BLACK; }
            else { Shape.Fill = Storage.WHITE; }

            _storage.Window.mainCanvas.Children.Add(Shape);

            _neighbors = new List<Spot>();
        }

        public void addNeighbors()
        {
            //if not on the left edge
            if (I > 0)
            {
                Neighbors.Add(_storage.Grid[I - 1, J]);
            }

            //if not on the right edge
            if (I < _storage.Cols - 1)
            {
                Neighbors.Add(_storage.Grid[I + 1, J]);
            }


            //if not on the bottom edge
            if (J < _storage.Rows - 1)
            {
                Neighbors.Add(_storage.Grid[I, J + 1]);
            }

            //if not on the top edge
            if (J > 0)
            {
                this.Neighbors.Add(_storage.Grid[I, J - 1]);
            }



            //if not on the top left corner
            if (I > 0 && J > 0)
            {
                Neighbors.Add(_storage.Grid[I - 1, J - 1]);
            }

            //if not on the top right corner
            if (I < _storage.Cols - 1 && J > 0)
            {
                Neighbors.Add(_storage.Grid[I + 1, J - 1]);
            }

            //if not on the left bottom corner
            if (I > 0 && J < _storage.Rows - 1)
            {
                Neighbors.Add(_storage.Grid[I - 1, J + 1]);
            }

            //if not on the right bottom corner
            if (I < _storage.Cols - 1 && J < _storage.Rows - 1)
            {
                Neighbors.Add(_storage.Grid[I + 1, J + 1]);
            }
        }

        public void show(SolidColorBrush col)
        {
            if (!Wall)
            {
                this.Shape.Fill = col;
            }
        }

        public int F
        {
            get
            {
                return _f;
            }

            set
            {
                _f = value;
            }
        }

        public int G
        {
            get
            {
                return _g;
            }

            set
            {
                _g = value;
            }
        }

        public int H
        {
            get
            {
                return _h;
            }

            set
            {
                _h = value;
            }
        }

        public int I
        {
            get
            {
                return _i;
            }

            set
            {
                _i = value;
            }
        }

        public int J
        {
            get
            {
                return _j;
            }

            set
            {
                _j = value;
            }
        }

        public List<Spot> Neighbors
        {
            get
            {
                return _neighbors;
            }
        }

        public Spot Previous
        {
            get
            {
                return _previous;
            }

            set
            {
                _previous = value;
            }
        }

        public bool Wall
        {
            get
            {
                return _wall;
            }

            set
            {
                _wall = value;
            }
        }

        public Shape Shape
        {
            get
            {
                return _shape;
            }

            set
            {
                _shape = value;
            }
        }
    }
}
