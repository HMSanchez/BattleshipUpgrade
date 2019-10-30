using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship
{
    public class Score
    {
        public string Name { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public string GameMode { get; set; }
        public Score(string line)
        {
            string[] param = line.Split(' ');
            Name = param[0];
            Wins = int.Parse(param[1]);
            Losses = int.Parse(param[2]);
            GameMode = param[3];
        }
        public string SaveString()
        {
            return $"{Name} {Wins} {Losses} {GameMode}";
        }
    }
}
