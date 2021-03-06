﻿using System;
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
    /// Interaction logic for Player.xaml
    /// </summary>
    public partial class Player
    {
        public Player()
        {
            InitializeComponent();
            this.ShowCloseButton = false;   
        }

        private void MetroWindow_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
        public bool IsClosed { get; private set; }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            IsClosed = true;
        }
    }
}
