using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DamaKonzole_Framework
{
    class Data
    {
        //private Board board = new Board();
        //private Rules rules;

        //public Data()
        //{
        //    rules = new Rules(board);
        //}
        /// <summary>
        /// Metoda pro uložení hry
        /// </summary>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        /// <param name="ptrTah"></param>
        /// <param name="historie"></param>
        public void SaveGame(int player1, int player2, int ptrTah, List<int[]> historie)
        {
            using (StreamWriter sw = new StreamWriter(@"save.txt"))
            {
                sw.WriteLine("player1:{0}", player1);
                sw.WriteLine("player2:{0}", player2);
                sw.WriteLine("pointer:{0}", ptrTah);
                foreach (int[] item in historie)
                {
                    string vystup = null;
                    for (int i = 0; i < item.Length; i = i + 4)
                    {
                        vystup = String.Format("{0}{1}{2}{3}", (char)(item[0 + i] + 'a'), (char)(item[1 + i] + '1'), (StoneToString(item[2 + i])),(StoneToString(item[3 + i])));
                        sw.Write(vystup);
                    }
                    sw.WriteLine();
                }
                sw.Flush();
            }
        }
        public bool LoadGame(out Board board, out Rules rules, out int player1, out int player2)
        {
            using (StreamReader sr = new StreamReader(@"save.txt"))
            {
                board = new Board();
                rules = new Rules(board);
                player1 = 0;
                player2 = 0;

                string prvniRadek = sr.ReadLine();
                char hrac1 = prvniRadek[8];
                player1 = (int)(hrac1 - '0');

                string druhyRadek = sr.ReadLine();
                char hrac2 = druhyRadek[8];
                player2 = (int)(hrac2 - '0');

                string tretiRadek = sr.ReadLine();
                char ptr = tretiRadek[8];
                int ukazatel = (int)(ptr - '0');

                List<int[]> seznam = new List<int[]>();
                string historieTahu;
                while ((historieTahu = sr.ReadLine()) != null)
                {
                    int[] celyPohyb = new int[0];
                    for (int i = 0; i < historieTahu.Length; i += 4)
                    {
                        int[] castPohybu = new int[] { (historieTahu[i] - 'a'), (historieTahu[i + 1] - '1'), CharToStone(historieTahu[i + 2]), CharToStone(historieTahu[i + 3]) };
                        celyPohyb = celyPohyb.Concat(castPohybu).ToArray();
                    }
                    seznam.Add(celyPohyb);
                }

                if (player1 < 0 || player1 > 4)
                {
                    return false;
                }
                if (player2 < 0 ||player2 > 4)
                {
                    return false;
                }
                if (ukazatel > seznam.Count)
                {
                    return false;
                }

                rules.InitBoard();
                rules.InitPlayer();
                foreach (int[] pohyb in seznam)
                {
                    rules.MovesGenerate();
                    //Kontrola zda se tahy se seznamu nachází v rules.ListMove

                    board.Move(pohyb, true, false);
                    rules.ChangePlayer();
                }
                return true;


                //foreach (int[] item in seznam)
                //{
                //    for (int i = 0; i < item.Length; i = i + 4)
                //    {
                //        Console.Write("{0}{1}{2}{3} ", (char)(item[i] + 'a'), (char)(item[1 + i] + '1'), StoneToString(item[2 + i]), StoneToString(item[3 + i]));
                //    }
                //    Console.WriteLine();
                //}
            }
        }
        /// <summary>
        /// Metoda pro převod kamenů na string
        /// </summary>
        /// <param name="stone"></param>
        /// <returns></returns>
        public string StoneToString(int stone)
        {
            switch (stone)
            {
                case -2:
                    return "B";
                case -1:
                    return "b";
                case 1:
                    return "w";
                case 2:
                    return "W";
                default:
                    return "0";
            }
        }
        /// <summary>
        /// Metoda pro převod znaků na kameny
        /// </summary>
        /// <param name="stone"></param>
        /// <returns></returns>
        public int CharToStone(char stone)
        {
            switch (stone)
            {
                case 'w':
                    return 1;
                case 'W':
                    return 2;
                case 'b':
                    return -1;
                case 'B':
                    return -2;
                default:
                    return 0;
            }
        }
    }
}
