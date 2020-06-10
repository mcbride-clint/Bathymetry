using HelixToolkit.Wpf;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace WPFSurfacePlot3D
{
    public enum ColorCoding
    {
        /// <summary>
        /// No color coding, use coloured lights
        /// </summary>
        ByLights,

        /// <summary>
        /// Color code by gradient in y-direction using a gradient brush with white ambient light
        /// </summary>
        ByGradientY
    }

    class SurfacePlotModel : INotifyPropertyChanged
    {
        private int defaultFunctionSampleSize = 100;

        // So the overall goal of this section is to output the appropriate values to SurfacePlotVisual3D - namely,
        // - DataPoints as Point3D, plus xAxisTicks (and y, z) as double[]
        // - plus all the appropriate properties, which can be directly edited/bindable by the user

        public SurfacePlotModel()
        {
            Title = "New Surface Plot";
            XAxisLabel = "x-Axis";
            YAxisLabel = "y-Axis";
            ZAxisLabel = "z-Axis";

            ColorCoding = ColorCoding.ByLights;
        }

        #region === Public Methods ===


        public void PlotData(double[,] zData2DArray)
        {
            int n = zData2DArray.GetLength(0);
            int m = zData2DArray.GetLength(1);
            Point3D[,] newDataArray = new Point3D[n, m];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    Point3D point = new Point3D(i, j, zData2DArray[i, j]);
                    newDataArray[i, j] = point;
                }
            }
            DataPoints = newDataArray;
            RaisePropertyChanged("DataPoints");
        }

        public void PlotData(double[,] zData2DArray, double xMinimum, double xMaximum, double yMinimum, double yMaximum)
        {

        }

        public void PlotData(double[,] zData2DArray, double[] xArray, double[] yArray)
        {
            // Note - check that dimensions match!!


        }

        public void PlotData(Point3D[,] point3DArray)
        {
            // Directly plot from a Point3D array
        }
           

        #endregion

        #region === Private Methods ===

        private Point3D[,] CreateDataArrayFromFunction(Func<double, double, double> f, double[] xArray, double[] yArray)
        {
            Point3D[,] newDataArray = new Point3D[xArray.Length, yArray.Length];
            for (int i = 0; i < xArray.Length; i++)
            {
                double x = xArray[i];
                for (int j = 0; j < yArray.Length; j++)
                {
                    double y = yArray[j];
                    newDataArray[i, j] = new Point3D(x, y, f(x, y));
                }
            }
            return newDataArray;
        }

        private double[] CreateLinearlySpacedArray(double minValue, double maxValue, int numberOfPoints)
        {
            double[] array = new double[numberOfPoints];
            double intervalSize = (xMax - xMin) / (numberOfPoints - 1);
            for (int i = 0; i < numberOfPoints; i++)
            {
                array[i] = minValue + i * intervalSize;
            }
            return array;
        }

        /*
        private void SetTicksAutomatically()
        {
            xTickMin = xMin;
            xTickMax = xMax;
            xNumberOfTicks = 10;
            xTickInterval = (xTickMax - xTickMin) / (xNumberOfTicks - 1);
            for (int i = 0; i < xNumberOfTicks; i++)
            {
                //xTickMin
            }
        } */

        #endregion

        #region === Exposed Properties ===

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string property)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }

        public Point3D[,] DataPoints { get; set; }
        public double[] XAxisTicks { get; set; }
        public double[] YAxisTicks { get; set; }
        public double[] ZAxisTicks { get; set; }

        private string title;
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                RaisePropertyChanged("Title");
            }
        }

        private string xAxisLabel;
        public string XAxisLabel
        {
            get { return xAxisLabel; }
            set
            {
                xAxisLabel = value;
                RaisePropertyChanged("XAxisLabel");
            }
        }

        private string yAxisLabel;
        public string YAxisLabel
        {
            get { return yAxisLabel; }
            set
            {
                yAxisLabel = value;
                RaisePropertyChanged("YAxisLabel");
            }
        }

        private string zAxisLabel;
        public string ZAxisLabel
        {
            get { return zAxisLabel; }
            set
            {
                zAxisLabel = value;
                RaisePropertyChanged("ZAxisLabel");
            }
        }

        private bool showSurfaceMesh;
        public bool ShowSurfaceMesh
        {
            get { return showSurfaceMesh; }
            set
            {
                showSurfaceMesh = value;
                RaisePropertyChanged("ShowSurfaceMesh");
            }
        }

        private bool showContourLines;
        public bool ShowContourLines
        {
            get { return showContourLines; }
            set
            {
                showContourLines = value;
                RaisePropertyChanged("ShowContourLines");
            }
        }

        private bool showMiniCoordinates;
        public bool ShowMiniCoordinates
        {
            get { return showMiniCoordinates; }
            set
            {
                showMiniCoordinates = value;
                RaisePropertyChanged("ShowMiniCoordinates");
            }
        }

        #endregion

        private double xMin, xMax;

        public ColorCoding ColorCoding { get; set; }

    }
}
