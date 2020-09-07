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
using System.Windows.Shapes;

namespace PokemonBWOverlay
{
    /// <summary>
    /// Interaction logic for Enemy.xaml
    /// </summary>
    public partial class Enemy
    {
        public Enemy()
        {
            InitializeComponent();
            this.ShowCloseButton = false;

        }

        private void Grid_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
    }
}
