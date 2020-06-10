using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using WPFSurfacePlot3D;

namespace Bathymetry.Ui
{
    /// <summary>
    /// Interaction logic for Viewer.xaml
    /// </summary>
    public partial class Viewer : Window
    {

        private SurfacePlotModel viewModel;

        /// <summary>
        /// Initialize the main window (hence, this function runs on application start).
        /// You should initialize your SurfacePlotViewModel here, and set it as the
        /// DataContext for your SurfacePlotView (which is defined in MainWindow.xaml).
        /// </summary>
        public Viewer()
        {
            InitializeComponent();

            // Initialize surface plot objects
            viewModel = new SurfacePlotModel();
            propertyGrid.DataContext = viewModel;
            surfacePlotView.DataContext = viewModel;
        }
        public void LoadData(Point3D[,] points)
        {
            viewModel.PlotData(points);
        }

        private void FileExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
