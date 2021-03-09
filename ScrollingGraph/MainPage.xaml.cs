using System;
using SkiaSharp;
using TouchTracking;
using Xamarin.Forms;

namespace ScrollingGraph
{
    public partial class MainPage : ContentPage
    {
        //Track Chart Properties
        float _height = 4000;
        float _width = 4000;

        //Track Chart Delta for Touch/Drag
        float _YDelta = 400;
        float _XDelta = 200;

        //Previous point sampled
        Point _previousTouchPoint;

        //Point[] testPoints = new Point[4];
        int[,] TestLineData = new int[,] { { 0, 0 }, { 20, 45 }, { 25, 75 }, { 30, 165 }, { 33, 45 }, { 52, -20 }, { 130, -85 }, { 230, 25 }, { 380, 225 }, { 420, 220 }, { 450, 42 }, { 480, -150 } };

        public MainPage()
        {
            InitializeComponent();

            //On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true); //Set safe areas for IOS
            DoRandomAdd();
        }

        void CanvasChartView_PaintSurface(System.Object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            e.Surface.Canvas.Clear(SKColors.White);

            //Setup SKPaint Object
            var _chartPaint = new SKPaint()
            {
                Color = SKColors.Red,
                TextSize = 26,
                Style = SKPaintStyle.Stroke,
                IsAntialias = true,
            };

            var _chartPaintThick = _chartPaint.Clone();
            _chartPaintThick.StrokeWidth = 5;

            var _chartDataPaint = new SKPaint()
            {
                Color = SKColors.Green,
                TextSize = 26,
                Style = SKPaintStyle.Stroke,
                IsAntialias = true,
                StrokeWidth = 3
            };

            //Draw X-Lines
            for (var i = 0 - _width; i < _width; i += 100)
            {
                e.Surface.Canvas.DrawLine(i + _XDelta, 0, i + _XDelta, _height, i == 0 ? _chartPaintThick : _chartPaint);
                e.Surface.Canvas.DrawText($"X:{i}", i + _XDelta, _chartPaint.FontSpacing, _chartPaint);
            }

            //Draw Y-Lines
            for (var i = 0 - _height; i < _height; i += 100)
            {
                e.Surface.Canvas.DrawLine(0, i + _YDelta, _width, i + _YDelta, i == 0 ? _chartPaintThick : _chartPaint);
                e.Surface.Canvas.DrawText($"Y:{i}", 0, i + _YDelta, _chartPaint);
            }

            //Draw Jonty Test Lines
            int iTestLen = TestLineData.GetLength(0);
            for (int i = 1; i < iTestLen; i++)
            {
                var VX = TestLineData[i - 1, 0];
                var VY = TestLineData[i - 1, 1];
                var VXc = TestLineData[i, 0];
                var VYc = TestLineData[i, 1];

                e.Surface.Canvas.DrawLine(VX + _XDelta, VY + _YDelta, VXc + _XDelta, VYc + _YDelta, _chartDataPaint);
            }
        }

        void TouchEffect_TouchAction(object sender, TouchActionEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("XXXX>> " + args.Type);

            //First touch, just mark the location
            if (args.Type == TouchActionType.Pressed)
            {
                _previousTouchPoint = args.Location;
                return;
            }

            //User is dragging/moving the chart
            if (args.Type == TouchActionType.Moved)
            {
                //Calculate Deltas in the drag
                var _touchYDelta = (float)Math.Round((_previousTouchPoint.Y - args.Location.Y) * 2, 0);
                var _touchXDelta = (float)Math.Round((_previousTouchPoint.X - args.Location.X) * 2, 0);

                //Re-Draw the Chart
                if (Math.Abs(_touchYDelta) >= 1 || Math.Abs(_touchXDelta) >= 1)
                {
                    _YDelta -= _touchYDelta;
                    _XDelta -= _touchXDelta;
                    _previousTouchPoint = args.Location;
                }
            }
            canvasChartView.InvalidateSurface();
        }

        #region Just Random Additional Drawing Testing

        int irnd = 0;
        void DoRandomAdd()
        {
            Device.StartTimer(new TimeSpan(0, 0, 2), () =>
            {
                Random rnd = new Random();
                int rndX = rnd.Next(200) + TestLineData[TestLineData.GetLength(0) - 1, 0];
                int rndY = rnd.Next(1000) - rnd.Next(1000);

                int[] val = new int[] { rndX, rndY };
                AddToMultiArray(val);

                irnd++;

                // do something every 60 seconds
                Device.BeginInvokeOnMainThread(() =>
                {
                    canvasChartView.InvalidateSurface();
                });

                return irnd < 20; // return true to repeat counting, false to stop timer
            });
        }

        void AddToMultiArray(int[] data)
        {
            var newData = new int[TestLineData.GetLength(0) + 1, 2];

            Array.Copy(TestLineData, newData, TestLineData.Length);
            var AddMe = new int[,] { { data[0], data[1] } };
            Array.Copy(AddMe, 0, newData, TestLineData.Length, data.Length);
            TestLineData = newData;
        }

        #endregion

    }
}
