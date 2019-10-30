using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace Battleship
{
    public static class Extensions
    {
        public static void Log(this StackPanel panel, string message)
        {
            TextBlock l = new TextBlock();
            l.TextWrapping = System.Windows.TextWrapping.Wrap;
            l.Text = message;
            l.FontFamily = new System.Windows.Media.FontFamily("Stencil");
            l.Margin = new System.Windows.Thickness(5, 1, 5, 0);
            l.Width = 260;
            l.Background = new SolidColorBrush(Color.FromArgb(0xff,0xee,0xee,0xee));
            l.Padding = new System.Windows.Thickness(5,2,5,2);
            l.FontSize = 16;
            panel.Children.Add(l);
        }
    }
}
