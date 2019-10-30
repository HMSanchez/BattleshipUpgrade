using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Battleship
{
    public static class BSR
    {
        public static SolidColorBrush Hit = Brushes.Red;
        public static SolidColorBrush Miss = Brushes.LightGray;
        public static SolidColorBrush VisibleShip = Brushes.LightGreen;
        public static ImageBrush New = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Images/background.png")));
        public static SolidColorBrush QueuedShot = Brushes.Orange;
        public static SolidColorBrush Seaplane = Brushes.Yellow;        
    }
}
