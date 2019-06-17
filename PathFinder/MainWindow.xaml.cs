using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PathFinder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        Storage _storage;

        private void Program_Loaded(object sender, RoutedEventArgs e)
        {
            LoopForever();
        }

        private void LoopForever()
        {
            reset();
            startPathFinder();
        }

        private void reset()
        {
            txtNoSolution.Visibility = Visibility.Hidden;
            Canvas.SetZIndex(txtNoSolution, 100);
            Canvas.SetTop(txtNoSolution, this.ActualHeight / 2 - txtNoSolution.ActualHeight / 2);
            Canvas.SetLeft(txtNoSolution, this.ActualWidth / 2 - txtNoSolution.ActualWidth / 2);

            if (_storage != null)
            {
                _storage.Loop = false;
                Thread.Sleep(1010);
                _storage.reset();
            }
        }

        private void startPathFinder()
        {
            _storage = Storage.getInstance();
            _storage.Window = this;

            _storage.Grid = new Spot[_storage.Cols, _storage.Rows];
            _storage.WidthOfOneSpot = (int)Math.Floor(this.Width / _storage.Cols - 1);
            _storage.HeigthOfOneSpot = (int)Math.Floor(this.Height / _storage.Rows - 1);

            Random rand = new Random();
            for (int i = 0; i < _storage.Cols; i++)
            {
                for (int j = 0; j < _storage.Rows; j++)
                {
                    bool temp = false;
                    if (rand.Next(100) <= _storage.Difficulty) { temp = true; }
                    _storage.Grid[i, j] = new Spot(i, j, temp);
                }
            }

            for (int i = 0; i < _storage.Cols; i++)
            {
                for (int j = 0; j < _storage.Rows; j++)
                {
                    _storage.Grid[i, j].addNeighbors();
                }
            }

            _storage.Start = _storage.Grid[0, 0];
            _storage.Start.Wall = false;
            _storage.End = _storage.Grid[_storage.Cols - 1, _storage.Rows - 1];
            _storage.End.Wall = false;

            _storage.OpenSet.Add(_storage.Start);

            BackgroundWorker bw = new BackgroundWorker();
            bw.RunWorkerCompleted += Bw_RunWorkerCompleted;
            bw.DoWork += Bw_DoWork;
            bw.RunWorkerAsync();
        }

        private void Bw_DoWork(object sender, DoWorkEventArgs e)
        {
            while (_storage.Loop)
            {
                Thread.Sleep(1000 / _storage.Framerate);
                Application.Current.Dispatcher.Invoke((Action)(() =>
                {
                    Spot current = null;
                    if (_storage.OpenSet.Count > 0)
                    {
                        int winner = 0;
                        for (int i = 0; i < _storage.OpenSet.Count; i++)
                        {
                            if (_storage.OpenSet.ElementAt(i).F < _storage.OpenSet.ElementAt(winner).F)
                            {
                                winner = i;
                            }
                        }

                        current = _storage.OpenSet.ElementAt(winner);

                        if (current == _storage.End)
                        {
                            if (!_storage.Path.Contains(current)) { _storage.Path.Enqueue(current); }
                            _storage.Loop = false;
                            //MessageBox.Show("DONE!");
                        }

                        _storage.OpenSet.Remove(current);
                        _storage.ClosedSet.Add(current);

                        List<Spot> neighbors = current.Neighbors;
                        for (int i = 0; i < neighbors.Count; i++)
                        {
                            Spot neighbor = neighbors.ElementAt(i);
                            if (!_storage.ClosedSet.Contains(neighbor) && !neighbor.Wall)
                            {
                                int tempG = current.G + 1;

                                bool newPath = false;
                                if (_storage.OpenSet.Contains(neighbor))
                                {
                                    if (tempG < neighbor.G)
                                    {
                                        neighbor.G = tempG;
                                        newPath = true;
                                    }
                                }
                                else
                                {
                                    neighbor.G = tempG;
                                    newPath = true;
                                    _storage.OpenSet.Add(neighbor);
                                }

                                if (newPath)
                                {
                                    neighbor.H = heuritic(neighbor, _storage.End);
                                    neighbor.F = neighbor.G + neighbor.H;
                                    neighbor.Previous = current;
                                }
                            }
                        }

                    }
                    else
                    {
                        //MessageBox.Show("No solution!");
                        Console.WriteLine("No solution!");
                        txtNoSolution.Visibility = Visibility.Visible;
                        _storage.Loop = false;
                    }

                    for (int i = 0; i < _storage.Cols; i++)
                    {
                        for (int j = 0; j < _storage.Rows; j++)
                        {
                            _storage.Grid[i, j].show(Storage.WHITE);
                        }
                    }

                    for (int i = 0; i < _storage.ClosedSet.Count; i++)
                    {
                        _storage.ClosedSet.ElementAt(i).show(Storage.RED);
                    }

                    for (int i = 0; i < _storage.OpenSet.Count; i++)
                    {
                        _storage.OpenSet.ElementAt(i).show(Storage.GREEN);
                    }

                    Spot temp = current;
                    if (temp != null)
                    {
                        _storage.Path.Clear();
                        _storage.Path.Enqueue(temp);
                        while (temp.Previous != null)
                        {
                            temp.show(Storage.BLUE);
                            _storage.Path.Enqueue(temp.Previous);
                            temp = temp.Previous;
                        }
                    }

                    for (int i = 0; i < _storage.Path.Count; i++)
                    {
                        _storage.Path.ElementAt(i).show(Storage.TRANSPARENT);
                    }


                    for (int i = 0; i < _storage.Lines.Count; i++)
                    {
                        mainCanvas.Children.Remove(_storage.Lines.ElementAt(i));
                    }

                    for (int i = 0; i < mainCanvas.Children.Count; i++)
                    {
                        if (mainCanvas.Children[i] is Line) { mainCanvas.Children.RemoveAt(i); }
                    }

                    Thread.Sleep(5);
                    _storage.Lines.Clear();
                    for (int i = 0; i < _storage.Path.Count; i++)
                    {
                        if (i < _storage.Path.Count - 1)
                        {
                            Line line = new Line()
                            {
                                X1 = _storage.Path.ElementAt(i).I * _storage.WidthOfOneSpot + _storage.WidthOfOneSpot / 2,
                                Y1 = _storage.Path.ElementAt(i).J * _storage.HeigthOfOneSpot + _storage.HeigthOfOneSpot / 2,
                                X2 = _storage.Path.ElementAt(i + 1).I * _storage.WidthOfOneSpot + _storage.WidthOfOneSpot / 2,
                                Y2 = _storage.Path.ElementAt(i + 1).J * _storage.HeigthOfOneSpot + _storage.HeigthOfOneSpot / 2,
                                Stroke = Storage.PURPLE,
                                StrokeThickness = _storage.LineThickness,
                                StrokeStartLineCap = PenLineCap.Round,
                                StrokeEndLineCap = PenLineCap.Round
                            };
                            _storage.Lines.Add(line);
                            mainCanvas.Children.Add(line);
                            //Console.Write(_storage.Path.ElementAt(i).I + ", " + _storage.Path.ElementAt(i).J + "  ");
                            Console.WriteLine("i = " + i + " | " + line.X1 + ", " + line.Y1 + "   ->    " + line.X2 + ", " + line.Y2 + " | ");
                        }

                        //if (i == _storage.Path.Count - 1)
                        //{
                        //    Line line = new Line()
                        //    {
                        //        X1 = _storage.Path.ElementAt(i - 1).I * _storage.WidthOfOneSpot + _storage.WidthOfOneSpot / 2,
                        //        Y1 = _storage.Path.ElementAt(i - 1).J * _storage.HeigthOfOneSpot + _storage.HeigthOfOneSpot / 2,
                        //        X2 = _storage.Path.ElementAt(i).I * _storage.WidthOfOneSpot + _storage.WidthOfOneSpot / 2,
                        //        Y2 = _storage.Path.ElementAt(i).J * _storage.HeigthOfOneSpot + _storage.HeigthOfOneSpot / 2,
                        //        Stroke = Storage.PURPLE,
                        //        StrokeThickness = _storage.LineThickness,
                        //        StrokeStartLineCap = PenLineCap.Round,
                        //        StrokeEndLineCap = PenLineCap.Round
                        //    };
                        //    _storage.Lines.Add(line);
                        //    mainCanvas.Children.Add(line);
                        //}
                    }
                    Console.WriteLine(); Console.WriteLine();

                    //for (int i = _storage.Path.Count - 1; i > 0; i++)
                    //{
                    //    if (i > 0)
                    //    {
                    //        Line line = new Line()
                    //        {
                    //            X1 = _storage.Path.ElementAt(i).I * _storage.WidthOfOneSpot + _storage.WidthOfOneSpot / 2,
                    //            Y1 = _storage.Path.ElementAt(i).J * _storage.HeigthOfOneSpot + _storage.HeigthOfOneSpot / 2,
                    //            X2 = _storage.Path.ElementAt(i - 1).I * _storage.WidthOfOneSpot + _storage.WidthOfOneSpot / 2,
                    //            Y2 = _storage.Path.ElementAt(i - 1).J * _storage.HeigthOfOneSpot + _storage.HeigthOfOneSpot / 2,
                    //            Stroke = Storage.PURPLE,
                    //            StrokeThickness = _storage.LineThickness,
                    //            StrokeStartLineCap = PenLineCap.Round,
                    //            StrokeEndLineCap = PenLineCap.Round
                    //        };
                    //        //_storage.Lines.Add(line);
                    //        mainCanvas.Children.Add(line);
                    //        Console.WriteLine(_storage.Path.ElementAt(i).I + ", " + _storage.Path.ElementAt(i).J);
                    //    }

                    //    //if (i == _storage.Path.Count - 1)
                    //    //{
                    //    //    Line line = new Line()
                    //    //    {
                    //    //        X1 = _storage.Path.ElementAt(i - 1).I * _storage.WidthOfOneSpot + _storage.WidthOfOneSpot / 2,
                    //    //        Y1 = _storage.Path.ElementAt(i - 1).J * _storage.HeigthOfOneSpot + _storage.HeigthOfOneSpot / 2,
                    //    //        X2 = _storage.Path.ElementAt(i).I * _storage.WidthOfOneSpot + _storage.WidthOfOneSpot / 2,
                    //    //        Y2 = _storage.Path.ElementAt(i).J * _storage.HeigthOfOneSpot + _storage.HeigthOfOneSpot / 2,
                    //    //        Stroke = Storage.PURPLE,
                    //    //        StrokeThickness = _storage.LineThickness,
                    //    //        StrokeStartLineCap = PenLineCap.Round,
                    //    //        StrokeEndLineCap = PenLineCap.Round
                    //    //    };
                    //    //    _storage.Lines.Add(line);
                    //    //    mainCanvas.Children.Add(line);
                    //    //}
                    //}


                }));
            }
        }

        private void Bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Thread.Sleep(2000);
            LoopForever();
        }

        private int heuritic(Spot neighbor, Spot end)
        {
            return (int)Math.Sqrt(Math.Pow(Math.Abs(end.I - neighbor.I), 2) + Math.Pow(Math.Abs(end.J - neighbor.J), 2));
            //return Math.Abs(end.I - neighbor.I) + Math.Abs(end.J - neighbor.J);
        }

        private void Program_Closing(object sender, CancelEventArgs e)
        {
            _storage.Loop = false;
        }

        private void Program_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.R)
            {
                LoopForever();
            }
        }
    }
}
