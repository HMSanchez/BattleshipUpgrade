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

namespace Battleship
{

    public partial class Setup : UserControl
    {
        public event EventHandler play;
        public string name;
        public Difficulty difficulty = Difficulty.Simple;
        public GameMode gamemode = GameMode.Default;

        public Setup()
        {
            InitializeComponent();
            cboxGameMode.ItemsSource = Enum.GetValues(typeof(GameMode));
            cboxGameMode.SelectedIndex = 0;
        }

        /// <summary>
        /// Start the ship placement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            name = txtboxName.Text;
            if (name == "")
            {
                MessageBox.Show("You must enter a name", "Error", MessageBoxButton.OK,MessageBoxImage.Error);
            }
            else
            {
                if ((bool)rbtnSimple.IsChecked)
                {
                    difficulty = Difficulty.Simple;
                }
                else
                {
                    difficulty = Difficulty.Intelligent;
                }
                gamemode = (GameMode)cboxGameMode.SelectedItem;
                play(this,e);
            }
        }

        /// <summary>
        /// Set difficulty to simple
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbtnSimple_Click(object sender, RoutedEventArgs e)
        {
            difficulty = Difficulty.Simple;
        }

        /// <summary>
        /// Set difficulty to intelligent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbtnIntelligent_Click(object sender, RoutedEventArgs e)
        {
            difficulty = Difficulty.Intelligent;
        }
    }
}
