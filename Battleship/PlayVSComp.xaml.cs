using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
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
    /// Interaction logic for PlayVSComp.xaml
    /// </summary>
    public partial class PlayVSComp : UserControl
    {
        public const int SEAPLANETURN = 30;
        public const int DELUXESEAPLANETURN = 8;
        public const bool DEBUGMODE = true;

        public event EventHandler replay;

        public Difficulty difficulty;
        public string playerName;
        public int highScore;
        public Grid[] playerGrid;
        public Grid[] compGrid;
        public List<int> hitList;
        public List<int> SalvoHitlistConcat;
        int turnCount = 0;
        public Random random = new Random();
        public GameMode Mode { get; set; }


        int pCarrierCount = 5, cCarrierCount = 5;
        int pBattleshipCount = 4, cBattleshipCount = 4;
        int pSubmarineCount = 3, cSubmarineCount = 3;
        int pCruiserCount = 3, cCruiserCount = 3;
        int pDestroyerCount = 2, cDestroyerCount = 2;
        int pPlaneCount = -1, cPlaneCount = -1;
        int pShots = 5, cShots = 5;
        int cCellsLeft = 100;

        List<Grid> pQueue = new List<Grid>();

        LeaderBoard lb;
        SoundPlayer HitSound = new SoundPlayer((new Uri(Directory.GetCurrentDirectory() + "\\Sounds\\Hit_Louder.wav").ToString()));
        SoundPlayer MissSound = new SoundPlayer((new Uri(Directory.GetCurrentDirectory() + "\\Sounds\\Miss2.wav").ToString()));
        public PlayVSComp(Difficulty difficulty, Grid[] playerGrid, string playerName, GameMode gamemode)
        {

            InitializeComponent();

            this.playerName = playerName;
            this.difficulty = difficulty;
            Mode = gamemode;
            InitializeGrids();
            initiateSetup(playerGrid);
            hitList = new List<int>();
            SalvoHitlistConcat = new List<int>();

        }
        private void InitializeGrids()
        {
            char[] letters = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J' };

            for (int i = 0; i < 10; i++)
            {
                PlayerGrid.ColumnDefinitions.Add(new ColumnDefinition());
                PlayerGrid.RowDefinitions.Add(new RowDefinition());
                CompGrid.ColumnDefinitions.Add(new ColumnDefinition());
                CompGrid.RowDefinitions.Add(new RowDefinition());

                Numbers.Children.Add(new Label() { Content = $"{i}", FontSize = 15, Width=30 , FontFamily=new FontFamily("Stencil")});
                Letters.Children.Add(new Label() { Content = $"{letters[i]}", FontSize = 15, Height=30, FontFamily=new FontFamily("Stencil")});
                for (int m = 0; m < 10; m++)
                {
                    Grid PlayerSubGrid = new Grid();
                    Grid.SetColumn(PlayerSubGrid, m);
                    Grid.SetRow(PlayerSubGrid, i);
                    PlayerSubGrid.Name = ($"grid{letters[i]}{m + 1}");
                    PlayerGrid.Children.Add(PlayerSubGrid);

                    Grid CompSubGrid = new Grid();
                    Grid.SetColumn(CompSubGrid, m);
                    Grid.SetRow(CompSubGrid, i);
                    CompSubGrid.Background = BSR.New;
                    CompSubGrid.MouseDown += gridMouseDown;
                    CompSubGrid.Name = ($"{letters[i]}{m + 1}");
                    CompGrid.Children.Add(CompSubGrid);
                }
            }

            //Initializes Leaderboard in fourth columnn
            lb = new LeaderBoard(playerName, Mode.ToString());
            Column4StackPanel.Children.Insert(0, lb);
        }

        /// <summary>
        /// Initial setup for grid
        /// </summary>
        /// <param name="userGrid"></param>
        private void initiateSetup(Grid[] userGrid)
        {
            //Set computer grid
            compGrid = new Grid[100];
            CompGrid.Children.CopyTo(compGrid, 0);
            for (int i = 0; i < 100; i++)
            {
                compGrid[i].Tag = "water";
            }
            setupCompGrid();
            //Set player grid
            playerGrid = new Grid[100];
            PlayerGrid.Children.CopyTo(playerGrid, 0);

            //Set ships
            for (int i = 0; i < 100; i++)
            {
                playerGrid[i].Background = userGrid[i].Background;
                playerGrid[i].Tag = userGrid[i].Tag;
            }
        }

        /// <summary>
        /// Initiate the computer's grid
        /// </summary>
        private void setupCompGrid()
        {
            Random random = new Random();
            int[] shipSizes = new int[] { 2, 3, 3, 4, 5 };
            string[] ships = new string[] { "destroyer", "cruiser", "submarine", "battleship", "carrier" };
            int size, index;
            string ship;
            Orientation orientation;
            bool unavailableIndex = true;

            for (int i = 0; i < shipSizes.Length; i++)
            {
                //Set size and ship type
                size = shipSizes[i];
                ship = ships[i];
                unavailableIndex = true;

                if (random.Next(0, 2) == 0)
                    orientation = Orientation.Horizontal;
                else
                    orientation = Orientation.Vertical;

                //Set ships
                if (orientation.Equals(Orientation.Horizontal))
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
                            if (index + j > 99 || !compGrid[index + j].Tag.Equals("water"))
                            {
                                index = random.Next(0, 100);
                                unavailableIndex = true;
                                break;
                            }
                        }
                    }
                    for (int j = 0; j < size; j++)
                    {
                        compGrid[index + j].Tag = ship;
                        if (DEBUGMODE)
                        {
                            compGrid[index + j].Background = BSR.VisibleShip;
                        }
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
                            if (index + j > 99 || !compGrid[index + j].Tag.Equals("water"))
                            {
                                index = random.Next(0, 100);
                                unavailableIndex = true;
                                break;
                            }
                        }
                    }
                    for (int j = 0; j < size * 10; j += 10)
                    {
                        compGrid[index + j].Tag = ship;
                        if (DEBUGMODE)
                        {
                            compGrid[index + j].Background = BSR.VisibleShip;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Attack event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Set sender to square chosen
            Grid square = (Grid)sender;

            //Check if player turn yet
            if (turnCount % 2 != 0)
            {
                return;
            }

            if (Mode == GameMode.Default || Mode == GameMode.Seaplane)
            {
                SingleShot(square);
            }
            else
            {
                QueueShot(square);
            }
        }

        private void SingleShot(Grid square)
        {
            turnLabel.Content = ((turnCount / 2) + 2).ToString();

            switch (square.Tag.ToString())
            {
                case "water":
                    square.Tag = "miss";
                    square.Background = BSR.Miss;
                    Beeper.PlayBeep(500, 300);
                    MissSound.Play();
                    turnCount++;
                    compTurn();
                    return;
                case "miss":
                case "hit":
                    Console.WriteLine("User hit a miss/hit");
                    return;
                case "destroyer":
                    cDestroyerCount--;
                    break;
                case "cruiser":
                    cCruiserCount--;
                    break;
                case "submarine":
                    cSubmarineCount--;
                    break;
                case "battleship":
                    cBattleshipCount--;
                    break;
                case "carrier":
                    cCarrierCount--;
                    break;
                case "seaplane":
                    cPlaneCount--;
                    break;
            }
            square.Tag = "hit";
            HitSound.Play();
            square.Background = BSR.Hit;
            turnCount++;
            Beeper.PlayBeep(1000, 300);
            checkPlayerWin();
            compTurn();
        }

        private void QueueShot(Grid square)
        {
            if (pShots != 1)
            {
                if (square.Tag.ToString() == "miss" || square.Tag.ToString() == "hit")
                {
                    return;
                }
                else if (square.Background == BSR.QueuedShot)
                {
                    return;
                }
                else
                {
                    pQueue.Add(square);
                    square.Background = BSR.QueuedShot;
                    pShots--;
                    if (--cCellsLeft == 0)
                    {
                        TakeTheShotMickey();
                        checkPlayerWin();
                    }
                }
            }
            else
            {
                if (square.Tag.ToString() == "miss" || square.Tag.ToString() == "hit")
                {
                    return;
                }
                else if (square.Background == BSR.QueuedShot)
                {
                    return;
                }
                else
                {
                    pQueue.Add(square);
                    square.Background = BSR.QueuedShot;
                    pShots--;
                    cCellsLeft--;
                }
                TakeTheShotMickey();


                turnLabel.Content = ((turnCount / 2) + 2).ToString();
                turnCount++;
                checkPlayerWin();
                RestoreCompShots();
                compTurn();
            }
        }

        private void TakeTheShotMickey()
        {
            foreach (Grid square in pQueue)
            {
                switch (square.Tag.ToString())
                {
                    case "water":
                        square.Tag = "miss";
                        square.Background = BSR.Miss;
                        break;
                    case "destroyer":
                        cDestroyerCount--;
                        square.Tag = "hit";
                        square.Background = BSR.Hit;
                        break;
                    case "cruiser":
                        cCruiserCount--;
                        square.Tag = "hit";
                        square.Background = BSR.Hit;
                        break;
                    case "submarine":
                        cSubmarineCount--;
                        square.Tag = "hit";
                        square.Background = BSR.Hit;
                        break;
                    case "battleship":
                        cBattleshipCount--;
                        square.Tag = "hit";
                        square.Background = BSR.Hit;
                        break;
                    case "carrier":
                        cCarrierCount--;
                        square.Tag = "hit";
                        square.Background = BSR.Hit;
                        break;
                    case "seaplane":
                        cPlaneCount--;
                        square.Tag = "hit";
                        square.Background = BSR.Hit;
                        break;
                }
            }
        }

        /// <summary>
        /// Writes the input string to the combat log
        /// </summary>
        /// <param name="message">The string to be written</param>
        private void WriteLog(string message)
        {
            Log.Log(message);
            LogScroller.ScrollToEnd();
        }

        private void compTurn()
        {
            if (Mode != GameMode.Salvo && Mode != GameMode.Deluxe)
            {

                if (difficulty == Difficulty.Simple)
                {
                    hunterMode();
                }
                else
                {
                    intelligentMoves();
                }
            }
            else
            {
                if (difficulty == Difficulty.Simple)
                {
                    for (int i = 0; i < cShots; i++)
                    {
                        hunterMode();
                    }
                }
                else
                {
                    //intelligentMoves();
                    SalvoIntelligent();
                }
            }



            turnCount++;
            checkComputerWin();

            //for salvo and deluxe
            RestorePlayerShots();
            //for seaplanes
            if (Mode == GameMode.Seaplane)
            {
                if ((turnCount / 2) + 1 == SEAPLANETURN) AddPlanes();
            }
            else if (Mode == GameMode.Deluxe)
            {
                if ((turnCount / 2) + 1 == DELUXESEAPLANETURN) AddPlanes();
            }
        }

        private void SalvoIntelligent()
        {
            while (cShots > 0)
            {
                if (hitList != null && hitList.Count > 0)
                {
                    int r = random.Next(hitList.Count);
                    if (playerGrid[hitList[r]].Tag.Equals("hit") || playerGrid[hitList[r]].Tag.Equals("miss"))
                    {
                        hitList.RemoveAt(r);
                    }
                    else
                    {
                        fireAtLocation(hitList[r]);
                        cShots--;
                    }
                }
                else
                {
                    hunterMode();
                    cShots--;
                }
            }
            hitList.AddRange(SalvoHitlistConcat);
        }

        private void RestorePlayerShots()
        {
            if (pCarrierCount > 0) pShots++;
            if (pBattleshipCount > 0) pShots++;
            if (pSubmarineCount > 0) pShots++;
            if (pCruiserCount > 0) pShots++;
            if (pDestroyerCount > 0) pShots++;
            if (pPlaneCount > 0) pShots++;
        }
        private void RestoreCompShots()
        {
            cShots = 0;
            if (cCarrierCount > 0) cShots++;
            if (cBattleshipCount > 0) cShots++;
            if (cSubmarineCount > 0) cShots++;
            if (cCruiserCount > 0) cShots++;
            if (cDestroyerCount > 0) cShots++;
            if (cPlaneCount > 0) cShots++;
        }

        private void AddPlanes()
        {
            Grid cell = null;

            if (pCarrierCount > 0)
            {
                do
                {
                    cell = playerGrid[random.Next(100)];
                }
                while (!cell.Tag.Equals("water"));
                cell.Background = BSR.Seaplane;
                cell.Tag = "seaplane";
                pPlaneCount = 1;
                WriteLog("Your Aircraft Carrier has launched a Seaplane onto the battlefield!");
            }

            if (cCarrierCount > 0)
            {
                do
                {
                    cell = compGrid[random.Next(100)];
                }
                while (!cell.Tag.Equals("water"));
                if (DEBUGMODE)
                {
                    cell.Background = BSR.Seaplane;
                }
                cell.Tag = "seaplane";
                cPlaneCount = 1;
                WriteLog("The enemy has launched a Seaplane from their Aircraft Carrier");
            }
        }

        private void checkPlayerWin()
        {
            if (cCarrierCount == 0)
            {
                cCarrierCount = -1;
                WriteLog("You sunk my Aircraft Carrier!");
                Beeper.PlayEnemyShipSink();
            }
            if (cCruiserCount == 0)
            {
                cCruiserCount = -1;
                WriteLog("You sunk my Cruiser!");
                Beeper.PlayEnemyShipSink();
            }
            if (cDestroyerCount == 0)
            {
                cDestroyerCount = -1;
                WriteLog("You sunk my Destroyer!");
                Beeper.PlayEnemyShipSink();
            }
            if (cBattleshipCount == 0)
            {
                cBattleshipCount = -1;
                WriteLog("You sunk my Battleship!");
                Beeper.PlayEnemyShipSink();
            }
            if (cSubmarineCount == 0)
            {
                cSubmarineCount = -1;
                WriteLog("You sunk my Submarine!");
                Beeper.PlayEnemyShipSink();
            }
            if (cPlaneCount == 0)
            {
                cPlaneCount = -1;
                WriteLog("You sunk my Seaplane!");
                Beeper.PlayEnemyShipSink();
            }

            if (cCarrierCount == -1 && cBattleshipCount == -1 && cSubmarineCount == -1 &&
                cCruiserCount == -1 && cDestroyerCount == -1 && cPlaneCount == -1)
            {
                WriteLog("You win!");
                disableGrids();
                lb.saveHighScores(true);
            }
        }
        private void checkComputerWin()
        {
            if (pCarrierCount == 0)
            {
                pCarrierCount = -1;
                WriteLog("Your Aircraft Carrier was destroyed!");
                Beeper.PlayFriendlyShipSink();
            }
            if (pCruiserCount == 0)
            {
                pCruiserCount = -1;
                WriteLog("Your Cruiser was destroyed!");
                Beeper.PlayFriendlyShipSink();
            }
            if (pDestroyerCount == 0)
            {
                pDestroyerCount = -1;
                WriteLog("Your Destroyer was destroyed!");
                Beeper.PlayFriendlyShipSink();
            }
            if (pBattleshipCount == 0)
            {
                pBattleshipCount = -1;
                WriteLog("Your Battleship was destroyed!");
                Beeper.PlayFriendlyShipSink();
            }
            if (pSubmarineCount == 0)
            {
                pSubmarineCount = -1;
                WriteLog("Your Submarine was destroyed!");
                Beeper.PlayFriendlyShipSink();
            }
            if (pPlaneCount == 0)
            {
                pPlaneCount = -1;
                WriteLog("Your Seaplane was destroyed!");
                Beeper.PlayFriendlyShipSink();
            }

            if (pCarrierCount == -1 && pBattleshipCount == -1 && pSubmarineCount == -1 &&
                pCruiserCount == -1 && pDestroyerCount == -1 && pPlaneCount == -1)
            {
                WriteLog("You lose!");
                disableGrids();
                lb.saveHighScores(false);
            }
        }
        private void disableGrids()
        {
            foreach (var element in compGrid)
            {
                if (element.Tag.Equals("water"))
                {
                    element.Background = BSR.Miss;
                }
                else if (element.Tag.Equals("carrier") || element.Tag.Equals("cruiser") ||
                  element.Tag.Equals("destroyer") || element.Tag.Equals("battleship") || element.Tag.Equals("submarine") || element.Tag.Equals("seaplane"))
                {
                    element.Background = BSR.VisibleShip;
                }
                element.IsEnabled = false;
            }
            foreach (var element in playerGrid)
            {
                if (element.Tag.Equals("water"))
                {
                    element.Background = BSR.Miss;
                }
                element.IsEnabled = false;
            }
        }
        private string validateXCoordinate(string X)
        {
            if (X.Length != 1)
            {
                return "";
            }

            if (Char.IsLetter(X[0]))
            {
                return X;
            }
            return "";
        }

        /// <summary>
        /// Validate Y coordinate
        /// </summary>
        /// <param name="Y">Y coordinate</param>
        /// <returns>char Y coordinate if good. Otherwise char '-'</returns>
        private string validateYCoordinate(string Y)
        {
            if (Y.Length > 2 || Y == "")
            {
                return "";
            }

            if (int.Parse(Y) > 0 || int.Parse(Y) <= 10)
            {
                return Y;
            }
            return "";
        }

        private void btnStartOver_Click(object sender, RoutedEventArgs e)
        {
            replay(this, e);
        }

        /// <summary>
        /// Computer AI moves based on if it is in "Hunter" mode
        /// (has not found a ship) or "Killer" mode (is attempting
        /// to destroy a ship).
        /// </summary>
        private void intelligentMoves()
        {
            // If there are no squares to hit
            if (hitList.Count == 0)
            {
                Console.WriteLine("hitlist is empty");
                hunterMode();
            }
            // assumes there is a ship found
            else
                killerMode();
        }

        /// <summary>
        /// Hunter Mode fires randoming, attempting to find a ship
        /// </summary>
        private void hunterMode()
        {
            int position;
            do
            {
                position = random.Next(100);
                Console.WriteLine(playerGrid[position].Tag);
                Console.WriteLine("Randomizing position");
            } while ((playerGrid[position].Tag.Equals("miss")) || (playerGrid[position].Tag.Equals("hit")));


            if (difficulty == Difficulty.Simple)
            {
                Console.WriteLine("Going simple");
                simpleMode(position);
            }
            else
            {
                fireAtLocation(position);
            }

        }

        /// <summary>
        /// Simple Difficulty fires randomly with no other algorithm
        /// </summary>
        /// <param name="position"></param>
        private void simpleMode(int position)
        {
            if (!(playerGrid[position].Tag.Equals("water")))
            {
                // If ship is hit mark it down
                switch (playerGrid[position].Tag.ToString())
                {
                    case "destroyer":
                        pDestroyerCount--;
                        break;
                    case "cruiser":
                        pCruiserCount--;
                        break;
                    case "submarine":
                        pSubmarineCount--;
                        break;
                    case "battleship":
                        pBattleshipCount--;
                        break;
                    case "carrier":
                        pCarrierCount--;
                        break;
                    case "seaplane":
                        pPlaneCount--;
                        break;
                }
                // Mark the grid as hit
                playerGrid[position].Tag = "hit";
                playerGrid[position].Background = BSR.Hit;
            }
            else
            {
                playerGrid[position].Tag = "miss";
                playerGrid[position].Background = BSR.Miss;
            }
        }

        /// <summary>
        /// Determines if the shot is a hit or miss. In the event
        /// of a hit the ship is checked if it's destroyed or not
        /// and if so checks for a winner before going back to hunter
        /// mode. In the event of a miss the grid is changed to
        /// reflect that
        /// </summary>
        /// <param name="position"></param>
        private void fireAtLocation(int position)
        {
            //If the position contains one of the ships (therefore, not water, missed shot, or already hit ship)
            if (!(playerGrid[position].Tag.Equals("water")))
            {
                // If this grid is in the hitList, remove it
                if (hitList != null && hitList.Contains(position))
                    hitList.Remove(position);

                // If ship is hit mark it down
                switch (playerGrid[position].Tag.ToString())
                {
                    case "destroyer":
                        pDestroyerCount--;
                        break;
                    case "cruiser":
                        pCruiserCount--;
                        break;
                    case "submarine":
                        pSubmarineCount--;
                        break;
                    case "battleship":
                        pBattleshipCount--;
                        break;
                    case "carrier":
                        pCarrierCount--;
                        break;
                    case "seaplane":
                        pPlaneCount--;
                        break;

                }
                // Mark the grid as hit
                playerGrid[position].Tag = "hit";
                playerGrid[position].Background = BSR.Hit;

                // If a ship is destroyed clear the hitList to return to Hunter Mode
                if (pDestroyerCount == 0 || pCruiserCount == 0 || pSubmarineCount == 0 || pBattleshipCount == 0 || pCarrierCount == 0 || pPlaneCount == 0)
                {
                    hitList.Clear();
                    SalvoHitlistConcat.Clear();
                }
                // If a ship is not destroyed add adjacent grids to hitList
                else
                {
                    // Computer hit a ship, add the adjacent grids to hitList
                    // If the position is on the left side
                    if (position % 10 == 0)
                        if (Mode == GameMode.Salvo || Mode == GameMode.Deluxe)
                        {
                            SalvoHitlistConcat.Add(position + 1);
                        }
                        else
                        {
                            hitList.Add(position + 1);
                        }
                    // If the position is on the  right side
                    else if (position % 10 == 9)
                        if (Mode == GameMode.Salvo || Mode == GameMode.Deluxe)
                        {
                            SalvoHitlistConcat.Add(position - 1);
                        }
                        else
                        {
                            hitList.Add(position - 1);
                        }
                    // Is the position is not on the left or right
                    else
                    {
                        if (Mode == GameMode.Salvo || Mode == GameMode.Deluxe)
                        {
                            SalvoHitlistConcat.Add(position + 1);
                            SalvoHitlistConcat.Add(position - 1);
                        }
                        else
                        {
                            hitList.Add(position + 1);
                            hitList.Add(position - 1);
                        }
                    }
                    // If the position is on the top
                    if (position < 10)
                        if (Mode == GameMode.Salvo || Mode == GameMode.Deluxe)
                        {
                            SalvoHitlistConcat.Add(position + 10);
                        }
                        else
                        {
                            hitList.Add(position + 10);
                        }
                    // If the position is on the bottom
                    else if (position > 89)
                        if (Mode == GameMode.Salvo || Mode == GameMode.Deluxe)
                        {
                            SalvoHitlistConcat.Add(position - 10);
                        }
                        else
                        {
                            hitList.Add(position - 10);
                        }
                    // If the position is not on the top or bottom
                    else
                    {
                        if (Mode == GameMode.Salvo || Mode == GameMode.Deluxe)
                        {
                            SalvoHitlistConcat.Add(position + 10);
                            SalvoHitlistConcat.Add(position - 10);
                        }
                        else
                        {
                            hitList.Add(position + 10);
                            hitList.Add(position - 10);
                        }
                    }

                    // The following code should improve the AI's options by removing squares that are likely to be misses
                    try
                    {
                        hitList.Remove(position - 11);
                    }
                    catch (Exception e) { }
                    try
                    {
                        hitList.Remove(position - 9);
                    }
                    catch (Exception e) { }
                    try
                    {
                        hitList.Remove(position + 9);
                    }
                    catch (Exception e) { }
                    try
                    {
                        hitList.Remove(position + 11);
                    }
                    catch (Exception e) { }
                }
            }
            else
            {
                playerGrid[position].Tag = "miss";
                playerGrid[position].Background = BSR.Miss;
            }
        }

        /// <summary>
        /// Fire on one of the grid squares from the hitList
        /// </summary>
        private void killerMode()
        {
            int position;
            // Prepare to fight at a random grid of the hitList
            do
            {
                Console.WriteLine("killerMode loop");
                position = random.Next(hitList.Count);
            } while (playerGrid[hitList[position]].Tag.Equals("miss") || playerGrid[hitList[position]].Tag.Equals("hit"));

            //Find the index for the grid in the Grid Array and fire
            Console.WriteLine("HitList count: " + hitList.Count);
            Console.WriteLine(hitList);
            fireAtLocation(hitList[position]);
        }
    }
}
