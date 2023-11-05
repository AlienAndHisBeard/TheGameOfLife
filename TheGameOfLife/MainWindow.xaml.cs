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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TheGameOfLife.Models;
using TheGameOfLife.Utils;
using TheGameOfLife.ViewModels;
using static TheGameOfLife.Models.Structs.Structs;

namespace TheGameOfLife
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private GenerationViewModel generationViewModel;

        /// <summary>
        /// Initializes main window and bindings, creates view model
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            AddParamsConverters();

            generationViewModel = new GenerationViewModel();

            DataContext = generationViewModel;
        }

        /// <summary>
        /// Setups data bindings for the appropriate elements in the MainWindow
        /// </summary>
        private void AddParamsConverters()
        {
            Underpop.SetBinding(TextBox.TextProperty, TextBinding("MinPop", 2, 0, 20));

            Overpop.SetBinding(TextBox.TextProperty, TextBinding("MaxPop", 3, 1, 25));

            Reproducepop.SetBinding(TextBox.TextProperty, TextBinding("ReproducePop", 3, 1, 25));

            SizeX.SetBinding(TextBox.TextProperty, TextBinding("SizeX", 10, 5, 100));

            SizeY.SetBinding(TextBox.TextProperty, TextBinding("SizeY", 10, 5, 100));
        }

        /// <summary>
        /// Creates binding for given param. Values given to this bindings are restricted by the converter.
        /// </summary>
        /// <param name="param">Binding param name</param>
        /// <param name="defaultValue">Default value given to the param</param>
        /// <param name="minValue">Max value of the param</param>
        /// <param name="maxValue">Min value of the param</param>
        /// <returns></returns>
        private static Binding TextBinding(string param, uint defaultValue, int minValue = 0, int maxValue = 100)
        {
            return new Binding()
            {
                Path = new PropertyPath(param),
                Mode = BindingMode.TwoWay,
                Converter = new ParamsConverter(defaultValue, minValue, maxValue),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
        }
    }
}
