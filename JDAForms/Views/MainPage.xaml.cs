using System;
using System.Threading.Tasks;
using Xamarin.Forms;

using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Diagnostics;
using Xamarin.Forms.Xaml;
using System.Collections.Generic;
using JDAForms.Model;

namespace JDAForms.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        SKPaintSurfaceEventArgs args;
        ProgressUtils progressUtils = new ProgressUtils();
        int leaves = 25;
        int totalleaves = 40;
        string input = "LEAVES AVAILABLE";
        string input2 = "OUT OF 40 ANNUAL LEAVES";

        public MainPage()
        {
            InitializeComponent();

            // Drawing the Radial Gauge
            initiateProgressUpdate();
            //Load Data
            LoadData();

        }

        /// <summary>
        /// Load Data in ItemSource
        /// </summary>
        void LoadData()
        {
            List<DataModel> data = new List<DataModel>()
            {
                new DataModel(new DateTime(2019, 12, 5),new DateTime(2019, 12, 9),
                "4 days next week","Hey i need to take of my personal works going for leave next 4 days"),
                new DataModel(new DateTime(2019, 11, 11),new DateTime(2019, 11, 16),
                "5 days next week","Hey i am out for vaction for next 5 days i will not be avalaible"),
                new DataModel(new DateTime(2019, 10, 17),new DateTime(2019, 10, 24),
                "7 days next week","Hey i need to take of my personal works going for leave next 4 days"),
            };
            listView.ItemsSource = data;
        }

        // Initializing the canvas & drawing over it
        async void OnCanvasViewPaintSurfaceAsync(object sender, SKPaintSurfaceEventArgs args1)

        {
            args = args1;
            await drawGaugeAsync();

        }

        void sliderValueChanged(object sender, ValueChangedEventArgs args)
        {
            if (canvas != null)
            {
                // Invalidating surface due to data change
                canvas.InvalidateSurface();
            }
        }


        //// Animating the Progress of Radial Gauge
        async void animateProgress(int progress)
        {
            sweepAngleSlider.Value = 1;

            // Looping at data interval of 5
            for (int i = 0; i < progress; i = i + 5)
            {
                sweepAngleSlider.Value = i;
                await Task.Delay(3);
            }

            sweepAngleSlider.Value = progress;
          

        }

        void initiateProgressUpdate()
        {
                animateProgress(progressUtils.getSweepAngle(totalleaves, leaves));
        }

        public async Task drawGaugeAsync()
        {
           // Radial Gauge Constants
           int uPadding = 150;
           int side = 350;
           int radialGaugeWidth = 20;

           // Line TextSize inside Radial Gauge
           int lineSize1 = 300;
           int lineSize2 = 40;
           int lineSize3 = 40;

           // Line Y Coordinate inside Radial Gauge
           int lineHeight1 = 200;
           int lineHeight2 = 350;
           int lineHeight3 = 400;

           // Start & End Angle for Radial Gauge
           float startAngle = -220;
           float sweepAngle = 260;

            try
            {
                
                // Getting Canvas Info 
                SKImageInfo info = args.Info;
                SKSurface surface = args.Surface;
                SKCanvas canvas = surface.Canvas;
                progressUtils.setDevice(info.Height, info.Width);
                canvas.Clear();

     
                // Top Padding for Radial Gauge
                float upperPading = progressUtils.getFactoredHeight(uPadding); 

                // Xc & Yc are center of the Circle
                int Xc = info.Width / 2;
                float Yc = progressUtils.getFactoredHeight(side);

                // X1 Y1 are lefttop cordiates of rectange
                int X1 = (int)(Xc - Yc); 
                int Y1 = (int)(Yc - Yc + upperPading);

                // X2 Y2 are rightbottom cordiates of rectange
                int X2 = (int)(Xc + Yc);
                int Y2 = (int)(Yc + Yc + upperPading);

                //Loggig Screen Specific Calculated Values
                 Debug.WriteLine("INFO " + info.Width + " - " + info.Height);
                 Debug.WriteLine(" C : " + upperPading + "  " + info.Height);
                 Debug.WriteLine(" C : " + Xc + "  " + Yc);
                 Debug.WriteLine("XY : " + X1 + "  " + Y1);
                 Debug.WriteLine("XY : " + X2 + "  " + Y2);   

                //  Empty Gauge Styling
                SKPaint paint1 = new SKPaint
                {
                    Style = SKPaintStyle.Stroke,
                    Color = Color.FromHex("#FEFEFD").ToSKColor(),                   // Colour of Radial Gauge
                    StrokeWidth = progressUtils.getFactoredWidth(radialGaugeWidth), // Width of Radial Gauge
                    StrokeCap = SKStrokeCap.Round                                   // Round Corners for Radial Gauge
                };

                // Filled Gauge Styling
                SKPaint paint2 = new SKPaint 
                {
                    Style = SKPaintStyle.Stroke,
                    Color = Color.FromHex("#F4BF2B").ToSKColor(),                   // Overlay Colour of Radial Gauge
                    StrokeWidth = progressUtils.getFactoredWidth(radialGaugeWidth), // Overlay Width of Radial Gauge
                    StrokeCap = SKStrokeCap.Round                                   // Round Corners for Radial Gauge
                };

                // Defining boundaries for Gauge
                SKRect rect = new SKRect(X1, Y1, X2, Y2);


                //canvas.DrawRect(rect, paint1);
                //canvas.DrawOval(rect, paint1);

                // Rendering Empty Gauge
                SKPath path1 = new SKPath();
                path1.AddArc(rect, startAngle, sweepAngle);
                canvas.DrawPath(path1, paint1);

                // Rendering Filled Gauge
                SKPath path2 = new SKPath();
                path2.AddArc(rect, startAngle, (float)sweepAngleSlider.Value);
                canvas.DrawPath(path2, paint2);

                //---------------- Drawing Text Over Gauge ---------------------------

                // LEAVES 
                using (SKPaint skPaint = new SKPaint())
                {
                    skPaint.Style = SKPaintStyle.Fill;
                    skPaint.IsAntialias = true;
                    skPaint.Color = SKColor.Parse("#FEFEFD");
                    skPaint.TextAlign = SKTextAlign.Center;
                    skPaint.TextSize = progressUtils.getFactoredHeight(lineSize1); 
                    skPaint.Typeface = SKTypeface.FromFamilyName(
                                        "Arial",
                                        SKFontStyleWeight.Bold,
                                        SKFontStyleWidth.Normal,
                                        SKFontStyleSlant.Upright);

                    // Drawing LEAVES Over Radial Gauge
                    canvas.DrawText(leaves+"", Xc, Yc + progressUtils.getFactoredHeight(lineHeight1), skPaint); 
                }

                // Text Styling
                using (SKPaint skPaint = new SKPaint())
                {
                    skPaint.Style = SKPaintStyle.Fill;
                    skPaint.IsAntialias = true;
                    skPaint.Color = SKColor.Parse("#FEFEFD");
                    skPaint.TextAlign = SKTextAlign.Center;
                    skPaint.TextSize = progressUtils.getFactoredHeight(lineSize2);
                    canvas.DrawText(input, Xc, Yc + progressUtils.getFactoredHeight(lineHeight2), skPaint);
                }

                // Input Text Styling
                using (SKPaint skPaint = new SKPaint())
                {
                    skPaint.Style = SKPaintStyle.Fill;
                    skPaint.IsAntialias = true;
                    skPaint.Color = SKColor.Parse("#FEFEFD");
                    skPaint.TextAlign = SKTextAlign.Center;
                    skPaint.TextSize = progressUtils.getFactoredHeight(lineSize3); 

                    // Drawing Text Over Radial Gauge
                        canvas.DrawText(input2, Xc, Yc + progressUtils.getFactoredHeight(lineHeight3), skPaint); 
                  
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
            }
        }

    }
}
