using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DamaKonzole_Framework
{
    class Data
    {
        public void SaveGame(int player1, int player2, int ptrTah, List<int[]> historie)
        {
            using (StreamWriter sw = new StreamWriter(@"test.txt"))
            {
                sw.WriteLine("player1:{0}", player1);
                sw.WriteLine("player2:{0}", player2);
                sw.WriteLine("pointer:{0}", ptrTah);
                foreach (int[] item in historie)
                {
                    string vystup = null;
                    for (int i = 0; i < item.Length; i = i + 4)
                    {
                        vystup = String.Format("{0}|{1}|{2}|{3}|", (char)(item[0 + i] + 'a'), (char)(item[1 + i] + '1'), (StoneToString(item[2 + i])),(StoneToString(item[3 + i])));
                        sw.Write(vystup);
                    }
                    sw.WriteLine();
                }
                sw.Flush();
            }
        }
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
    }
}
