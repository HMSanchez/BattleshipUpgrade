using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Battleship
{
    public static class Beeper
    {
        private static int Freq = 40;
        private static int Dur = 0;
        public static bool PlayBeeps { get; set; } = false;

        public static void PlayBeep(int fre, int dur)
        {
            if(PlayBeeps)
            {
                Freq = fre;
                Dur = dur;
                ThreadStart ts = new ThreadStart(play);
                Thread t = new Thread(ts);
                t.Start();
            }
        }

        public static void PlayEnemyShipSink()
        {
            if(PlayBeeps)
            {
                ThreadStart ts = new ThreadStart(_PlayEnemyShipSink);
                Thread t = new Thread(ts);
                t.Start();
            }
        }

        public static void PlayFriendlyShipSink()
        {
            if(PlayBeeps)
            {
                ThreadStart ts = new ThreadStart(_PlayFriendlyShipSink);
                Thread t = new Thread(ts);
                t.Start();
            }
        }

        private static void play()
        {
            Console.Beep(Freq, Dur);
        }

        private static void _PlayFriendlyShipSink()
        {
            Console.Beep(1000, 100);
            Thread.Sleep(20);
            Console.Beep(600, 100);
            Thread.Sleep(20);
            Console.Beep(200, 100);
        }
        private static void _PlayEnemyShipSink()
        {
            Console.Beep(200, 100);
            Thread.Sleep(20);
            Console.Beep(600, 100);
            Thread.Sleep(20);
            Console.Beep(1000, 100);
        }
    }
}
