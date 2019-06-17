using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PathFinder
{
    class Storage
    {
        private static Storage _instance;

        public static Storage getInstance()
        {
            if (_instance == null)
            {
                _instance = new Storage();
            }
            return _instance;
        }

        int _cols;
        int _rows;
        Spot[,] _grid;
        List<Spot> _openSet;
        List<Spot> _closedSet;
        Spot _start;
        Spot _end;
        int _framerate;
        int _difficulty;
        int _widthOfOneSpot;
        int _heigthOfOneSpot;
        bool _loop = true;
        Queue<Spot> _path;
        List<Line> _lines;
        int _lineThickness;
        MainWindow _window;

        public static readonly SolidColorBrush BLACK = new SolidColorBrush(Colors.Black);
        public static readonly SolidColorBrush WHITE = new SolidColorBrush(Colors.White);
        public static readonly SolidColorBrush RED = new SolidColorBrush(Colors.Red);
        public static readonly SolidColorBrush GREEN = new SolidColorBrush(Colors.Lime);
        public static readonly SolidColorBrush BLUE = new SolidColorBrush(Colors.Blue);
        public static readonly SolidColorBrush PURPLE = new SolidColorBrush(Colors.Purple);
        public static readonly SolidColorBrush TRANSPARENT = new SolidColorBrush(Colors.Transparent);

        public void reset()
        {
            for (int i = 0; i < Cols; i++)
            {
                for (int j = 0; j < Rows; j++)
                {
                    Grid[i, j].reset();
                }
            }
            Grid = null;
            if (OpenSet != null) { OpenSet.Clear(); }
            if (ClosedSet != null) { ClosedSet.Clear(); }
            Start = null;
            End = null;
            Loop = true;
            if (Path != null) { Path.Clear(); }
            if (Lines != null) { Lines.Clear(); }
            Window = null;
            _instance = null;
        }


        public Storage()
        {
            _cols = 20;
            _rows = 20;
            _framerate = 5;
            _lineThickness = 6;
            _difficulty = 50;
            _openSet = new List<Spot>();
            _closedSet = new List<Spot>();
            _path = new Queue<Spot>();
            _lines = new List<Line>();
        }

        public int Rows
        {
            get
            {
                return _rows;
            }

            set
            {
                _rows = value;
            }
        }

        public int Cols
        {
            get
            {
                return _cols;
            }

            set
            {
                _cols = value;
            }
        }

        public Spot[,] Grid
        {
            get
            {
                return _grid;
            }

            set
            {
                _grid = value;
            }
        }

        public List<Spot> OpenSet
        {
            get
            {
                return _openSet;
            }
        }

        public List<Spot> ClosedSet
        {
            get
            {
                return _closedSet;
            }
        }

        public Spot Start
        {
            get
            {
                return _start;
            }

            set
            {
                _start = value;
            }
        }

        public Spot End
        {
            get
            {
                return _end;
            }

            set
            {
                _end = value;
            }
        }

        public int Framerate
        {
            get
            {
                return _framerate;
            }

            set
            {
                _framerate = value;
            }
        }

        public int WidthOfOneSpot
        {
            get
            {
                return _widthOfOneSpot;
            }

            set
            {
                _widthOfOneSpot = value;
            }
        }

        public int HeigthOfOneSpot
        {
            get
            {
                return _heigthOfOneSpot;
            }

            set
            {
                _heigthOfOneSpot = value;
            }
        }

        public bool Loop
        {
            get
            {
                return _loop;
            }

            set
            {
                _loop = value;
            }
        }

        public Queue<Spot> Path
        {
            get
            {
                return _path;
            }
        }

        public MainWindow Window
        {
            get
            {
                return _window;
            }

            set
            {
                _window = value;
            }
        }

        public int LineThickness
        {
            get
            {
                return _lineThickness;
            }

            set
            {
                _lineThickness = value;
            }
        }

        public List<Line> Lines
        {
            get
            {
                return _lines;
            }
        }

        public int Difficulty
        {
            get
            {
                return _difficulty;
            }

            set
            {
                _difficulty = value;
            }
        }
    }
}
