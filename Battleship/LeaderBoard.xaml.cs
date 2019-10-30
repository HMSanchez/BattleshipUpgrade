using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for LeaderBoard.xaml
    /// </summary>
    public partial class LeaderBoard : UserControl
    {
        private ObservableCollection<Score> Scores { get; set; }
        private string playerName { get; set; }
        private string GameMode { get; set; }
        public LeaderBoard( string PlayerName,string GameMode)
        {
            playerName = PlayerName.ToUpper();
            this.GameMode = GameMode;
            Scores = new ObservableCollection<Score>(loadHighScores());
            InitializeComponent();
            ScoresGrid.ItemsSource = Scores;
        }

        /// <summary>
        /// Saves high scores
        /// </summary>
        /// <param name="playerWins"></param>
        /// <returns></returns>
        public void saveHighScores(bool playerWins)
        {
            String filename = @"../../scores.txt";
            bool found = false;
            List<string> scoresString = new List<String>();
            for (int i = 0; i < Scores.Count; i++)
            {
                if (Scores[i].Name == playerName && Scores[i].GameMode == GameMode)
                {
                    if (playerWins)
                        Scores[i].Wins++;
                    else
                        Scores[i].Losses++;
                    found = true;
                }
                scoresString.Add(Scores[i].SaveString());
            }

            if(!found)
            {
                Scores.Add(new Score($"{playerName} {(playerWins ? 1 : 0)} {(playerWins ? 0 : 1)} {GameMode}"));
                scoresString.Add(Scores[Scores.Count-1].SaveString());
            }

            File.WriteAllLines(filename, scoresString);
        }

        /// <summary>
        /// Load the high scores (initiation)
        /// </summary>
        /// <returns></returns>
        public List<Score> loadHighScores()
        {
            String filename = @"../../scores.txt";

            //Create file if it doesn't exists
            if (!File.Exists(filename))
            {
                FileStream stream = File.Create(filename);
                stream.Close();
            }

            List<Score> Scores = new List<Score>();
            //Reads the files lines then foreaches through each line while adding a new score parsed in Score.cs contructer.
            File.ReadAllLines(filename).ToList().ForEach(sc => {
                Scores.Add(new Score(sc));
            });
            return Scores;
        }

        /// <summary>
        /// Clears the score list by deleting the files and clearing Scores collection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            string path = @"../../scores.txt";
            File.Delete(path);
            FileStream stream = File.Create(path);
            stream.Close();
            Scores.Clear();
        }
    }
}
