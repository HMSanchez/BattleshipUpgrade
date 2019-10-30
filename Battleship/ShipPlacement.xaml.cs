using System;
using System.Collections.Generic;
using System.IO;
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
    /// <summary>
    /// Interaction logic for ShipPlacement.xaml
    /// </summary>
    public partial class ShipPlacement : UserControl
    {
        public event EventHandler play;

        enum Orientation { VERTICAL, HORIZONTAL };
        Orientation orientation = Orientation.HORIZONTAL;
        SolidColorBrush unselected = new SolidColorBrush(Colors.Black);
        SolidColorBrush selected = new SolidColorBrush(Colors.Green);
        String ship = "";
        int size;
        int numShipsPlaced;
        System.Windows.Shapes.Path lastShip;
        System.Windows.Shapes.Path[] ships;
        Polygon lastArrow;
        public Grid[] playerGrid;
        string[] alphabet = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J" };
        string[] shipNames = new string[] { "destroyer", "cruiser", "submarine", "battleship", "carrier" };
        ImageBrush[] destroyerImages = new ImageBrush[2];
        ImageBrush[] cruiserImages = new ImageBrush[3];
        ImageBrush[] submarineImages = new ImageBrush[3];
        ImageBrush[] battleshipImages = new ImageBrush[4];
        ImageBrush[] carrierImages = new ImageBrush[5];
        int desCount = 0;
        int cruCount = 0;
        int subCount = 0;
        int bsCount = 0;
        int acCount = 0;

        SolidColorBrush[] shipColors = new SolidColorBrush[] {(SolidColorBrush)(new BrushConverter().ConvertFrom("#88cc00")), (SolidColorBrush)(new BrushConverter().ConvertFrom("#33cc33")),
                                                                  (SolidColorBrush)(new BrushConverter().ConvertFrom("#00e64d")),(SolidColorBrush)(new BrushConverter().ConvertFrom("#00cc00")),
                                                                  (SolidColorBrush)(new BrushConverter().ConvertFrom("#00e600"))};

        public ShipPlacement()
        {

            InitializeComponent();
            shipImages();
            InitializeGrid();
            SetShips();
            AddLabels();
            reset();

        }


        /// <summary>
        /// Reset the setDown grid.
        /// Tags: 
        ///     0. water
        ///     1. destroyer
        ///     2. cruiser
        ///     3. submarine
        ///     4. battleship
        ///     5. carrier
        /// </summary>
        private void reset()
        {
            shipImages();
            desCount = 0;
            cruCount = 0;
            subCount = 0;
            bsCount = 0;
            acCount = 0;
            if (lastArrow != null)
            {
                lastArrow.Stroke = unselected;
            }
            lastArrow = rightPoly;
            rightPoly.Stroke = selected;

            foreach (var element in playerGrid)
            {
                element.Tag = "water";
                element.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Images/background.png")));
            }

            foreach (var element in ships)
            {
                element.IsEnabled = true;
                element.Opacity = 100;
                if (element.Stroke != unselected)
                {
                    element.Stroke = unselected;
                }
            }
            numShipsPlaced = 0;
            lastShip = null;
        }

        /// <summary>
        /// When the ship format is clicked, make it show and
        /// put it to global variable ship.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ship_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Shapes.Path shipPath = (System.Windows.Shapes.Path)sender;
            if (!shipPath.IsEnabled)
            {
                return;
            }
            if (lastShip != null)
            {
                lastShip.Stroke = unselected;
            }

            lastShip = shipPath;
            ship = shipPath.Name;
            shipPath.Stroke = selected;

            switch (ship)
            {
                case "carrier":
                    size = 5;
                    break;
                case "battleship":
                    size = 4;
                    break;
                case "submarine":
                case "cruiser":
                    size = 3;
                    break;
                case "destroyer":
                    size = 2;
                    break;
            }
        }
        /// <summary>
        /// When the orientation arrow (left,right,Down,down) is selected
        /// make it show and change the orientation enum.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void orientationMouseDown(object sender, MouseButtonEventArgs e)
        {
            Polygon arrow = (Polygon)sender;

            lastArrow.Stroke = unselected;
            lastArrow = arrow;
            arrow.Stroke = selected;

            if (arrow.Name.Equals("rightPoly") || arrow.Name.Equals("leftPoly"))
            {
                orientation = Orientation.HORIZONTAL;
            }
            else
            {
                orientation = Orientation.VERTICAL;
            }



        }

        /// <summary>
        /// When grid square is clicked, determine if a ship should
        /// be placed there and if yes, place it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridMouseDown(object sender, MouseButtonEventArgs e)
        {
            shipImages();
            Grid square = (Grid)sender;
            int index = -1;
            int temp;
            int counter = 1;
            string shipName = square.Tag.ToString();
            if (!square.Tag.Equals("water"))
            {

                return;
            }

            //Check if square has a ship already in place
            if (!square.Tag.Equals("water"))
            {
                MessageBox.Show("Not valid", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //Check if ship has been selected
            if (lastShip == null)
            {
                MessageBox.Show("You must choose a ship", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //Find chosen square. Index should never be -1.
            index = Array.IndexOf(playerGrid, square);

            //Check if there is enough space for the ship
            if (orientation.Equals(Orientation.HORIZONTAL))
            {
                try
                {
                    counter = 1;
                    for (int i = 0; i < size; i++)
                    {
                        //This sees if the index is within the grid going ---->
                        //START HERE
                        //
                        //
                        //
                        //
                        if (index + i <= 99)
                        {
                            if (!playerGrid[index + i].Tag.Equals("water"))
                            {
                                throw new IndexOutOfRangeException("Invalid ship placement, not enough space!");
                            }
                        }
                        //Goes <---- to see if there is space
                        else
                        {
                            //if ((index / 10) + (size * 10) > 100)
                            //{
                            //    throw new IndexOutOfRangeException("Invalid ship placement, not enough space!");
                            //}
                            //if (!playerGrid[index - counter].Tag.Equals("water"))
                            //{
                            //    throw new IndexOutOfRangeException("Invalid ship placement");
                            //}
                            counter++;
                            throw new IndexOutOfRangeException("Invalid ship placement");
                        }

                    }
                }
                catch (IndexOutOfRangeException iore)
                {
                    MessageBox.Show(iore.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

            }
            else //for orientation down
            {
                try
                {
                    counter = 10;
                    for (int i = 0; i < size * 10; i += 10)
                    {
                        if (index + i <= 99)
                        {
                            if (!playerGrid[index + i].Tag.Equals("water"))
                            {
                                throw new IndexOutOfRangeException("Invalid ship placement!");
                            }
                        }
                        else
                        {
                            if (!playerGrid[index - counter].Tag.Equals("water"))
                            {
                                throw new IndexOutOfRangeException("Invalid ship placement! Wrong counter.");
                            }
                            counter += 10;
                        }
                    }
                    if ((index / 10) + (size * 10) > 100)
                    {
                        throw new IndexOutOfRangeException("Invalid ship placement, not enough space!");
                    }
                }
                catch (IndexOutOfRangeException iore)
                {
                    MessageBox.Show(iore.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            //Set the ship to grid
            if (orientation.Equals(Orientation.HORIZONTAL))
            {
                if (index + size > index /10*10+9)
                {
                    index = index / 10 * 10 + (10 - size);
                }
                //If two rows
                if ((index + size - 1) % 10 < size - 1)
                {
                    counter = 0;
                    temp = 1;

                    while ((index + counter) % 10 > 1)
                    {
                        playerGrid[index + counter].Background = selectColor();
                        playerGrid[index + counter].Tag = ship;
                        playerGrid[index + counter].MouseRightButtonDown += shipRemovalMouseDown;
                        counter++;
                    }

                    for (int i = counter; i < size; i++)
                    {

                        playerGrid[index - temp].Background = selectColor();
                        playerGrid[index - temp].Tag = ship;
                        playerGrid[index - temp].MouseRightButtonDown += shipRemovalMouseDown;

                        temp++;
                    }
                }
                //If one row
                else
                {

                    for (int i = 0; i < size; i++)
                    {
                        playerGrid[index + i].Background = selectColor();
                        playerGrid[index + i].Tag = ship;
                        playerGrid[index + i].MouseRightButtonDown += shipRemovalMouseDown;

                    }
                }
            }
            else
            {
                if(index + (10 * size) > 99)
                {
                    index = index-10*((index + (10 * size) - 100) / 10 );
                }


                //If two columns
                if (index + (size * 10) > 100)
                {

                    counter = 0;
                    temp = 10;
                    while ((index / 10 + counter) % 100 < 10)
                    {
                        playerGrid[index + counter * 10].Background = selectColor();
                        playerGrid[index + counter * 10].Tag = ship;
                        playerGrid[index + counter * 10].MouseRightButtonDown += shipRemovalMouseDown;

                        counter++;
                    }
                    for (int i = counter; i < size; i++)
                    {
                        playerGrid[index - temp].Background = selectColor();
                        playerGrid[index - temp].Tag = ship;
                        playerGrid[index - temp].MouseRightButtonDown += shipRemovalMouseDown;

                        temp += 10;
                    }
                }
                //If one column
                else
                {

                    counter = 0;
                    for (int i = 0; i < size * 10; i += 10)
                    {
                        playerGrid[index + i].Background = selectColor();
                        playerGrid[index + i].Tag = ship;
                        playerGrid[index + i].MouseRightButtonDown += shipRemovalMouseDown;

                    }
                }
            }
            lastShip.IsEnabled = false;
            lastShip.Opacity = 0.5;
            lastShip.Stroke = unselected;
            lastShip = null;
            numShipsPlaced++;
        }

        /// <summary>
        /// Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (numShipsPlaced != 5)
            {
                return;
            }
            play(this, e);
        }

        /// <summary>
        /// Reset button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            reset();
        }

        /// <summary>
        /// Sets grid randomnly
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRandomize_Click(object sender, RoutedEventArgs e)
        {
            reset();
            Random random = new Random();
            int[] shipSizes = new int[] { 2, 3, 3, 4, 5 };
            string[] shipNames = new string[] { "destroyer", "cruiser", "submarine", "battleship", "carrier" };
            int size, index;
            string ship;
            Orientation orientation;
            bool unavailableIndex = true;


            for (int i = 0; i < shipSizes.Length; i++)
            {
                //Set size and ship type
                size = shipSizes[i];
                ship = shipNames[i];
                unavailableIndex = true;

                if (random.Next(0, 2) == 0)
                    orientation = Orientation.HORIZONTAL;
                else
                    orientation = Orientation.VERTICAL;

                //Set ships
                if (orientation.Equals(Orientation.HORIZONTAL))
                {
                    index = random.Next(0, 100);
                    while (unavailableIndex == true)
                    {
                        unavailableIndex = false;

                        while ((index + size - 1) % 10 < size - 1)
                        {
                            index = random.Next(0, 100);
                        }

                        for (int j = 0; j < size; j++)
                        {
                            if (index + j > 99 || !playerGrid[index + j].Tag.Equals("water"))
                            {
                                index = random.Next(0, 100);
                                unavailableIndex = true;
                                break;
                            }
                        }
                    }
                    for (int j = 0; j < size; j++)
                    {
                        playerGrid[index + j].Tag = ship;
                        playerGrid[index + j].Background = selectColor(ship, orientation);
                        playerGrid[index + j].MouseRightButtonDown += shipRemovalMouseDown;

                    }
                }
                else
                {
                    index = random.Next(0, 100);
                    while (unavailableIndex == true)
                    {
                        unavailableIndex = false;

                        while (index / 10 + size * 10 > 100)
                        {
                            index = random.Next(0, 100);
                        }

                        for (int j = 0; j < size * 10; j += 10)
                        {
                            if (index + j > 99 || !playerGrid[index + j].Tag.Equals("water"))
                            {
                                index = random.Next(0, 100);
                                unavailableIndex = true;
                                break;
                            }
                        }
                    }
                    for (int j = 0; j < size * 10; j += 10)
                    {
                        playerGrid[index + j].Tag = ship;
                        playerGrid[index + j].Background = selectColor(ship, orientation);
                        playerGrid[index + j].MouseRightButtonDown += shipRemovalMouseDown;

                    }
                }

            }
            numShipsPlaced = 5;
            foreach (var element in ships)
            {
                element.IsEnabled = false;
                element.Opacity = .5;
                if (element.Stroke != unselected)
                {
                    element.Stroke = unselected;
                }

            }


        }

        /// <summary>
        /// Choose the background
        /// </summary>
        /// <returns></returns>
        private ImageBrush selectColor()
        {
            if (desCount >= 2)
            {
                desCount = 0;
            }
            else if (cruCount >= 3)
            {
                cruCount = 0;
            }
            else if (subCount >= 3)
            {
                subCount = 0;
            }
            else if (acCount >= 5)
            {
                acCount = 0;
            }
            else if (bsCount >= 4)
            {
                bsCount = 0;
            }

            switch (ship)
            {
                case "destroyer":

                    if (orientation == Orientation.VERTICAL)
                    {

                        RotateTransform aRotateTransform = new RotateTransform();
                        aRotateTransform.CenterX = 0.5;
                        aRotateTransform.CenterY = 0.5;
                        aRotateTransform.Angle = 90;
                        ImageBrush br = destroyerImages[desCount++];
                        br.RelativeTransform = aRotateTransform;
                        return br;
                    }
                    else
                    {
                        ImageBrush br = destroyerImages[desCount++];
                        return br;
                    }

                case "cruiser":

                    if (orientation == Orientation.VERTICAL)
                    {

                        RotateTransform aRotateTransform = new RotateTransform();
                        aRotateTransform.CenterX = 0.5;
                        aRotateTransform.CenterY = 0.5;
                        aRotateTransform.Angle = 90;
                        ImageBrush br = cruiserImages[cruCount++];
                        br.RelativeTransform = aRotateTransform;
                        return br;
                    }
                    else
                    {
                        ImageBrush br = cruiserImages[cruCount++];
                        return br;
                    }

                case "submarine":
                    if (orientation == Orientation.VERTICAL)
                    {

                        RotateTransform aRotateTransform = new RotateTransform();
                        aRotateTransform.CenterX = 0.5;
                        aRotateTransform.CenterY = 0.5;
                        aRotateTransform.Angle = 90;
                        ImageBrush br = submarineImages[subCount++];
                        br.RelativeTransform = aRotateTransform;
                        return br;
                    }
                    else
                    {
                        ImageBrush br = submarineImages[subCount++];
                        return br;
                    }

                case "carrier":

                    if (orientation == Orientation.VERTICAL)
                    {

                        RotateTransform aRotateTransform = new RotateTransform();
                        aRotateTransform.CenterX = 0.5;
                        aRotateTransform.CenterY = 0.5;
                        aRotateTransform.Angle = 90;
                        ImageBrush br = carrierImages[acCount++];
                        br.RelativeTransform = aRotateTransform;
                        return br;
                    }
                    else
                    {
                        ImageBrush br = carrierImages[acCount++];
                        return br;
                    }


                case "battleship":

                    if (orientation == Orientation.VERTICAL)
                    {

                        RotateTransform aRotateTransform = new RotateTransform();
                        aRotateTransform.CenterX = 0.5;
                        aRotateTransform.CenterY = 0.5;
                        aRotateTransform.Angle = 90;
                        ImageBrush br = battleshipImages[bsCount++];
                        br.RelativeTransform = aRotateTransform;
                        return br;
                    }
                    else
                    {
                        ImageBrush br = battleshipImages[bsCount++];
                        return br;
                    }

            }
            return destroyerImages[0];
        }

        private ImageBrush selectColor(string shipName, Orientation orientation)
        {
            if (desCount >= 2)
            {
                desCount = 0;
            }
            else if (cruCount >= 3)
            {
                cruCount = 0;
            }
            else if (subCount >= 3)
            {
                subCount = 0;
            }
            else if (acCount >= 5)
            {
                acCount = 0;
            }
            else if (bsCount >= 4)
            {
                bsCount = 0;
            }

            switch (shipName)
            {
                case "destroyer":

                    if (orientation == Orientation.VERTICAL)
                    {

                        RotateTransform aRotateTransform = new RotateTransform();
                        aRotateTransform.CenterX = 0.5;
                        aRotateTransform.CenterY = 0.5;
                        aRotateTransform.Angle = 90;
                        ImageBrush br = destroyerImages[desCount++];
                        br.RelativeTransform = aRotateTransform;
                        return br;
                    }
                    else
                    {
                        ImageBrush br = destroyerImages[desCount++];
                        return br;
                    }

                case "cruiser":

                    if (orientation == Orientation.VERTICAL)
                    {

                        RotateTransform aRotateTransform = new RotateTransform();
                        aRotateTransform.CenterX = 0.5;
                        aRotateTransform.CenterY = 0.5;
                        aRotateTransform.Angle = 90;
                        ImageBrush br = cruiserImages[cruCount++];
                        br.RelativeTransform = aRotateTransform;
                        return br;
                    }
                    else
                    {
                        ImageBrush br = cruiserImages[cruCount++];
                        return br;
                    }

                case "submarine":
                    if (orientation == Orientation.VERTICAL)
                    {

                        RotateTransform aRotateTransform = new RotateTransform();
                        aRotateTransform.CenterX = 0.5;
                        aRotateTransform.CenterY = 0.5;
                        aRotateTransform.Angle = 90;
                        ImageBrush br = submarineImages[subCount++];
                        br.RelativeTransform = aRotateTransform;
                        return br;
                    }
                    else
                    {
                        ImageBrush br = submarineImages[subCount++];
                        return br;
                    }

                case "carrier":

                    if (orientation == Orientation.VERTICAL)
                    {

                        RotateTransform aRotateTransform = new RotateTransform();
                        aRotateTransform.CenterX = 0.5;
                        aRotateTransform.CenterY = 0.5;
                        aRotateTransform.Angle = 90;
                        ImageBrush br = carrierImages[acCount++];
                        br.RelativeTransform = aRotateTransform;
                        return br;
                    }
                    else
                    {
                        ImageBrush br = carrierImages[acCount++];
                        return br;
                    }


                case "battleship":

                    if (orientation == Orientation.VERTICAL)
                    {

                        RotateTransform aRotateTransform = new RotateTransform();
                        aRotateTransform.CenterX = 0.5;
                        aRotateTransform.CenterY = 0.5;
                        aRotateTransform.Angle = 90;
                        ImageBrush br = battleshipImages[bsCount++];
                        br.RelativeTransform = aRotateTransform;
                        return br;
                    }
                    else
                    {
                        ImageBrush br = battleshipImages[bsCount++];
                        return br;
                    }

            }
            return destroyerImages[0];
        }
        /// <summary>
        /// Sets up starting grid for ship placement
        /// </summary>
        private void InitializeGrid()
        {
            char[] letters = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J' };
            playerGrid = new Grid[100];
            List<Grid> gridList = new List<Grid>();
            for (int i = 0; i < 10; i++)
            {
                shipyardGrid.RowDefinitions.Add(new RowDefinition());
                shipyardGrid.ColumnDefinitions.Add(new ColumnDefinition());
                for (int m = 0; m < 10; m++)
                {
                    Grid PlayerSubGrid = new Grid();
                    Grid.SetColumn(PlayerSubGrid, m);
                    Grid.SetRow(PlayerSubGrid, i);
                    PlayerSubGrid.Name = ($"grid{letters[i]}{m + 1}");
                    PlayerSubGrid.MouseLeftButtonDown += gridMouseDown;
                    PlayerSubGrid.Background = (Brush)(new BrushConverter().ConvertFrom("#FFF0F0F0"));
                    Rectangle rect = new Rectangle();
                    rect.MouseLeftButtonDown += gridMouseDown;
                    System.Windows.Shapes.Path p = new System.Windows.Shapes.Path();
                    p.Name = ($"cell{letters[i]}{m + 1}");
                    p.MouseDown += shipRemovalMouseDown;
                    PlayerSubGrid.Children.Add(rect);
                    PlayerSubGrid.Children.Add(p);
                    gridList.Add(PlayerSubGrid);

                    shipyardGrid.Children.Add(PlayerSubGrid);
                }
            }
            for (var k = 0; k < gridList.Count(); k++)
            {
                playerGrid[k] = gridList[k];
            }
        }

        /// <summary>
        /// Sets up area below grid where user can select ships to placed
        /// </summary>
        private void SetShips()
        {
            ships = new System.Windows.Shapes.Path[shipNames.Length];

            for (var i = 0; i < shipNames.Length; i++)
            {
                System.Windows.Shapes.Path p = new System.Windows.Shapes.Path();
                string imgSRC = "";
                p.Name = shipNames[i];
                p.MouseLeftButtonDown += ship_MouseLeftButtonDown;
                p.Height = 29;
                p.Data = new EllipseGeometry(new Point(110, 10), 100, 90);
                p.Stretch = Stretch.Fill;

                switch (p.Name)
                {
                    case "destroyer":
                        p.Width = 57;
                        Canvas.SetLeft(p, 38.4);
                        Canvas.SetTop(p, 19.4);
                        imgSRC = "pack://application:,,,/Images/Destroyer.png";
                        ImageBrush brush = new ImageBrush();
                        brush.ImageSource = new BitmapImage(new Uri(imgSRC));
                        brush.Stretch = Stretch.Uniform;
                        p.Fill = brush;
                        ships[0] = p;
                        break;
                    case "cruiser":
                        p.Width = 74;
                        Canvas.SetLeft(p, 107);
                        Canvas.SetTop(p, 20);
                        imgSRC = "pack://application:,,,/Images/Cruiser.png";
                        brush = new ImageBrush();
                        brush.ImageSource = new BitmapImage(new Uri(imgSRC));
                        brush.Stretch = Stretch.Uniform;
                        p.Fill = brush;
                        ships[1] = p;
                        break;
                    case "submarine":
                        p.Width = 74;
                        Canvas.SetLeft(p, 193);
                        Canvas.SetTop(p, 20);
                        imgSRC = "pack://application:,,,/Images/Submarine.png";
                        brush = new ImageBrush();
                        brush.ImageSource = new BitmapImage(new Uri(imgSRC));
                        brush.Stretch = Stretch.Uniform;
                        p.Fill = brush;
                        ships[2] = p;
                        break;
                    case "battleship":
                        p.Width = 117;
                        Canvas.SetLeft(p, 17);
                        Canvas.SetTop(p, 52);
                        imgSRC = "pack://application:,,,/Images/BattleShip.png";
                        brush = new ImageBrush();
                        brush.ImageSource = new BitmapImage(new Uri(imgSRC));
                        brush.Stretch = Stretch.Uniform;
                        p.Fill = brush;
                        ships[3] = p;
                        break;
                    case "carrier":
                        p.Width = 148;
                        Canvas.SetLeft(p, 138);
                        Canvas.SetTop(p, 52);
                        imgSRC = "pack://application:,,,/Images/AircraftCarrier.png";
                        brush = new ImageBrush();
                        brush.ImageSource = new BitmapImage(new Uri(imgSRC));
                        brush.Stretch = Stretch.Uniform;
                        p.Fill = brush;
                        ships[4] = p;
                        break;
                }
                canvas.Children.Add(p);

            }

        }

        /// <summary>
        /// Creation of Letters and Numbers labels alongside the grid
        /// </summary>
        private void AddLabels()
        {
            for (int i = 0; i < alphabet.Length; i++)
            {
                Label l = new Label();
                switch (i)
                {
                    case 0:
                        l.Margin = new Thickness(-33, 1, 0, 0);
                        Grid.SetRowSpan(l, 2);
                        break;
                    case 1:
                        l.Margin = new Thickness(-33, 30, 0, 0);
                        Grid.SetRowSpan(l, 2);
                        break;
                    case 2:
                        l.Margin = new Thickness(-33, 1, 0, 0);
                        Grid.SetRow(l, 2);
                        break;
                    case 3:
                        l.Margin = new Thickness(-33, 1, 0, 0);
                        Grid.SetRow(l, 3);
                        break;
                    case 4:
                        l.Margin = new Thickness(-33, 1, 0, 0);
                        Grid.SetRow(l, 4);
                        break;
                    case 5:
                        l.Margin = new Thickness(-33, 1, 0, 0);
                        Grid.SetRow(l, 5);
                        break;
                    case 6:
                        l.Margin = new Thickness(-33, 1, 0, 0);
                        Grid.SetRow(l, 6);
                        break;
                    case 7:
                        l.Margin = new Thickness(-33, 1, 0, 0);
                        Grid.SetRow(l, 7);
                        break;
                    case 8:
                        l.Margin = new Thickness(-33, 1, 0, 0);
                        Grid.SetRow(l, 8);
                        break;
                    case 9:
                        l.Margin = new Thickness(-33, 30, 0, 0);
                        Grid.SetRow(l, 8);
                        Grid.SetRowSpan(l, 2);
                        break;
                }
                l.Name = "lblVertical" + alphabet[i];
                l.Content = alphabet[i];
                l.VerticalAlignment = VerticalAlignment.Top;
                l.Height = 29;
                l.Width = 29;
                l.HorizontalAlignment = HorizontalAlignment.Left;
                l.FontFamily = new FontFamily("Stencil");
                l.FontSize = 15;

                shipyardGrid.Children.Add(l);
            }

        }

        private void shipImages()
        {
            for (int i = 0; i < 2; i++)
            {
                string imgstring = "pack://application:,,,/Images/D" + (i + 1) + ".png";
                ImageBrush brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri(imgstring));
                destroyerImages[i] = brush;
            }

            for (int i = 0; i < 3; i++)
            {
                string imgstring = "pack://application:,,,/Images/C" + (i + 1) + ".png";
                ImageBrush brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri(imgstring));
                cruiserImages[i] = brush;
            }
            for (int i = 0; i < 3; i++)
            {
                string imgstring = "pack://application:,,,/Images/S" + (i + 1) + ".png";
                ImageBrush brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri(imgstring));
                submarineImages[i] = brush;
            }
            for (int i = 0; i < 4; i++)
            {
                string imgstring = "pack://application:,,,/Images/BS" + (i + 1) + ".png";
                ImageBrush brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri(imgstring));
                battleshipImages[i] = brush;
            }
            for (int i = 0; i < 5; i++)
            {
                string imgstring = "pack://application:,,,/Images/AC" + (i + 1) + ".png";
                ImageBrush brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri(imgstring));
                carrierImages[i] = brush;
            }
        }


        /// <summary>
        /// Removes previously placed ship on right click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void shipRemovalMouseDown(object sender, MouseButtonEventArgs e)
        {
            Grid griddy = (Grid)sender;
            for (var i = 0; i < playerGrid.Length; i++)
            {
                switch (griddy.Tag)
                {
                    case "destroyer":
                        for (var j = 0; j < playerGrid.Length; j++)
                        {
                            if (playerGrid[j].Tag.ToString() == "destroyer")
                            {
                                playerGrid[j].Tag = "water";
                                playerGrid[j].Background = BSR.New;
                            }
                        }
                        ships[0].Opacity = 100;
                        ships[0].IsEnabled = true;
                        ships[0].Stroke = unselected;
                        desCount = 0;
                        numShipsPlaced--;
                        break;
                    case "cruiser":
                        for (var j = 0; j < playerGrid.Length; j++)
                        {
                            if (playerGrid[j].Tag.ToString() == "cruiser")
                            {

                                playerGrid[j].Tag = "water";
                                playerGrid[j].Background = BSR.New;
                            }
                        }
                        ships[1].Opacity = 100;
                        ships[1].IsEnabled = true;
                        ships[1].Stroke = unselected;
                        cruCount = 0;
                        numShipsPlaced--;

                        break;
                    case "submarine":
                        for (var j = 0; j < playerGrid.Length; j++)
                        {
                            if (playerGrid[j].Tag.ToString() == "submarine")
                            {

                                playerGrid[j].Tag = "water";
                                playerGrid[j].Background = BSR.New;
                            }
                        }
                        ships[2].Opacity = 100;
                        ships[2].IsEnabled = true;
                        ships[2].Stroke = unselected;
                        subCount = 0;
                        numShipsPlaced--;

                        break;
                    case "battleship":
                        for (var j = 0; j < playerGrid.Length; j++)
                        {
                            if (playerGrid[j].Tag.ToString() == "battleship")
                            {

                                playerGrid[j].Tag = "water";
                                playerGrid[j].Background = BSR.New;
                            }
                        }
                        ships[3].Opacity = 100;
                        ships[3].IsEnabled = true;
                        ships[3].Stroke = unselected;
                        numShipsPlaced--;
                        bsCount = 0;

                        break;
                    case "carrier":
                        for (var j = 0; j < playerGrid.Length; j++)
                        {
                            if (playerGrid[j].Tag.ToString() == "carrier")
                            {

                                playerGrid[j].Tag = "water";
                                playerGrid[j].Background = BSR.New;
                            }
                        }
                        ships[4].Opacity = 100;
                        ships[4].IsEnabled = true;
                        ships[4].Stroke = unselected;
                        numShipsPlaced--;
                        acCount = 0;
                        break;
                }

            }
            lastShip = null;
            orientation = Orientation.HORIZONTAL;
        }

    }
}


