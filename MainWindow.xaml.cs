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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BeyondVisionEngine
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            var imiona = new List<string> { "Asia", "Kasia", "Basia", "Zenek" };

            for (var i = imiona.Count - 1; i >= 0; i--)
            {
                if (!imiona[i].EndsWith("a"))
                    imiona.RemoveAt(i);
            }

            imiona = imiona.Where(imie => imie.EndsWith("a")).ToList();


            imiona = (from imie in imiona
                      where imie.EndsWith("a")
                      select imie).ToList();

            var editWindow = new EditGameWindow();
            editWindow.ShowDialog();
        }
    }
}
