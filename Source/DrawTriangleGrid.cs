using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Wallpaperomatic
{
    public class TriangleGrid
    {
        private readonly Random _rand;
        private readonly int _rows;
        private readonly int _cols;
        private readonly int _jiggleX;
        private readonly int _jiggleY;
        private readonly int _width;
        private readonly int _height;

        private List<Color> _colours = new List<Color>();
        private List<BitmapImage> _textures = new List<BitmapImage>();
        private Point[][] _points;

        public TriangleGrid(string texturePath, string colours, int rows, int cols, int jiggleX, int jiggleY, int width, int height )
        { 
            _rows = rows;
            _cols = cols;
            _jiggleX = jiggleX;
            _jiggleY = jiggleY;
            _width = width;
            _height = height;

            _rand = new Random();
            CalculateGrid();
            GetTextures(texturePath);

            if (_textures == null || _textures.Count == 0)  GetColours(colours);
        }

        private void GetTextures(string texturePath)
        {
            if (!Directory.Exists(texturePath))
            {
                _textures = new List<BitmapImage>();
                return;
            }

            _textures =  Directory.GetFiles(texturePath, "*.jpg").Select(file => new BitmapImage(new Uri(file))).ToList();
        }

        private void GetColours(string colours)
        {
            _colours = colours.Split(',').Select(colour => (Color)ColorConverter.ConvertFromString(colour)).ToList();
        }


        public void Draw(string fileName)
        {
            CalculateGrid();
            JiggleGrid();

            var drawingVisual = new DrawingVisual();
            using (var drawingContext = drawingVisual.RenderOpen())
            {
                PaintGrid(drawingContext);
            }

            SaveAsJpeg(drawingVisual, fileName);
        }

        private void SaveAsJpeg(Visual drawingVisual, string fileName)
        {
            var rbt = new RenderTargetBitmap(_width, _height, 96, 96, PixelFormats.Pbgra32);

            rbt.Render(drawingVisual);
            rbt.Freeze();

            var jpeg = new JpegBitmapEncoder();

            jpeg.Frames.Add(BitmapFrame.Create(rbt));
            using (Stream stm = File.Create(fileName))
            {
                jpeg.Save(stm);
            }
        }

        private void CalculateGrid()
        {
            var triangleWidth = (double)_width / (_cols-1);
            var triangleHeight = (double)_height / (_rows-1);
            _points = new Point[_rows][];

            for (var rowIdx = 0; rowIdx < _rows; rowIdx++)
            {
                var row = new List<Point>();


                if ((rowIdx % 2) != 0)
                {
                    row.Add(new Point(0, rowIdx * triangleHeight));
                }

                for (var colIdx = 0; colIdx < _cols; colIdx++)
                {
                    var x = colIdx * triangleWidth;
                    var y = rowIdx * triangleHeight;

                    if ((rowIdx % 2) != 0 && colIdx < _cols - 1)
                    {
                        x = x + (triangleWidth / 2);
                    }

                    row.Add(new Point(x, y));
                }

                _points[rowIdx] = row.ToArray();
            }
        }

        public void JiggleGrid()
        {
            for (var rowIdx = 0; rowIdx < _points.Length; rowIdx++)
            {
                for (var colIdx = 0; colIdx < _points[rowIdx].Length; colIdx++)
                {
                    var offsetX = (colIdx == 0 || colIdx == _points[rowIdx].Length - 1) ? 0 : _rand.Next(_jiggleX * -1, _jiggleX);
                    var offsetY = (rowIdx == 0 || rowIdx == _points.Length - 1) ? 0 : _rand.Next(_jiggleY * -1, _jiggleY);

                    _points[rowIdx][colIdx].Offset(offsetX, offsetY);
                }
            }
        }

        public void PaintGrid(DrawingContext drawingContext)
        {
            for (var rowIdx = 1; rowIdx < _points.Length; rowIdx++)
            {
                var topRow = _points[rowIdx - 1];
                var bottomRow = _points[rowIdx];

                DrawRow(drawingContext, topRow, bottomRow);
                DrawRow(drawingContext, bottomRow, topRow);
            }
        }

        private void DrawRow(DrawingContext drawingContext, Point[] topRow, Point[] bottomRow)
        {
            var bottomPointIdx = 0;
            if (topRow.Length < bottomRow.Length) bottomPointIdx++;

            for (var topPointIdx = 1; topPointIdx < topRow.Length; topPointIdx++)
            {
                var points = new List<Point> { topRow[topPointIdx - 1], bottomRow[bottomPointIdx], topRow[topPointIdx] };

                DrawPolygon(drawingContext, points);
                bottomPointIdx++;
            }
        }

        private Brush GetBrush()
        {
            if (_textures.Count > 0)
            {
                var image = _textures[_rand.Next(0, _textures.Count)];
                return new ImageBrush(image) { TileMode = TileMode.Tile };
            }
            else
            {
                var colour = _colours[_rand.Next(0, _colours.Count)];
                return new SolidColorBrush(colour);
            }
        }

        private void DrawPolygon(DrawingContext drawingContext, IList<Point> polygonPoints)
        {
            if (polygonPoints.Count < 2) return; // not a polygon

            var brush = GetBrush();

            var streamGeometry = new StreamGeometry();
            using (var geometryContext = streamGeometry.Open())
            {
                geometryContext.BeginFigure(polygonPoints[0], true, true);
                polygonPoints.RemoveAt(0); //craptarded framework (ok ok I'm probably doing it wrong)!
                geometryContext.PolyLineTo(polygonPoints, true, true);
            }
            streamGeometry.Freeze();

            drawingContext.DrawGeometry(brush, new Pen(brush, 0), streamGeometry);
        }
    }
}
