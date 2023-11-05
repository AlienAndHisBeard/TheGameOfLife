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

        public MainWindow()
        {
            InitializeComponent();
            AddParamsConverters();

            generationViewModel = new GenerationViewModel();

            DataContext = generationViewModel;
        }

        private void AddParamsConverters()
        {
            Underpop.SetBinding(TextBox.TextProperty, TextBinding("MinPop", 2));

            Overpop.SetBinding(TextBox.TextProperty, TextBinding("MaxPop", 3));

            Reproducepop.SetBinding(TextBox.TextProperty, TextBinding("ReproducePop", 3));

            SizeX.SetBinding(TextBox.TextProperty, TextBinding("SizeX", 10));

            SizeY.SetBinding(TextBox.TextProperty, TextBinding("SizeY", 10));
        }

        private static Binding TextBinding(string param, uint defaultValue)
        {
            return new Binding()
            {
                Path = new PropertyPath(param),
                Mode = BindingMode.TwoWay,
                Converter = new ParamsConverter(defaultValue),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
        }
    }
}
