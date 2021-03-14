using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DamaKonzole_Framework
{
    class Board
    {
        //Historie tahů
        public List<int[]> HistoryMove = new List<int[]>();

        //inicializace proměnné board, jako 2D pole o rozmeřech 8x8
        private int[,] board;

        public Board()
        {
            board = new int[8, 8];
            ClearBoard();
        }
        /// <summary>
        /// Stará deska se zkopíruje
        /// </summary>
        /// <param name="old"></param>
        public Board(Board old)
        {
            foreach (int[] item in old.HistoryMove)
            {
                HistoryMove.Add((int[])item.Clone());
            }
            board = new int[8, 8];
            for (int posY = 0; posY < 8; posY++)
            {
                for (int posX = 0; posX < 8; posX++)
                {
                    board[posX, posY] = old.GetValue(posX, posY);
                }
            }
        }
        /// <summary>
        /// Vymazání desky a nastavení na 0
        /// </summary>
        public void ClearBoard()
        {
            for (int posY = 0; posY < 8; posY++)
            {
                for (int posX = 0; posX < 8; posX++)
                {
                    board[posX, posY] = 0;
                }
            }
        }
        /// <summary>
        /// Metoda nastaví hodnotu na zvolených souřadnicích
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="hodnota"></param>
        public void SetValue(int posX, int posY, int value)
        {
            board[posX, posY] = value;
        }
        /// <summary>
        /// Metoda pro získaní hodnot z pole board
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <returns></returns>
        public int GetValue(int posX, int posY)
        {
            return board[posX, posY];
        }
        /// <summary>
        /// Metoda pro nastavení hodnoty v poli
        /// </summary>
        /// <param name="move"></param>
        private void MakeMove(int[] move, bool tahZpet) //{ X1, Y1, S0, S1, X2, Y2, S2, S3 };
        {
            if (tahZpet) //zpětný tah, true
            {
                SetValue(move[0], move[1], move[2]);
            }
            else
            {
                SetValue(move[0], move[1], move[3]);
            }
        }
        /// <summary>
        /// Metoda která prodeve nastavení hodnot z plného vstupu
        /// </summary>
        /// <param name="kompletniPohyb"></param>
        public void Move(int[] kompletniPohyb, bool ulozitDoHistorie, bool tahZpet) //metoda rozdělí tah na části po 4 souřadnicích
        {
            //např pohyb a2-b3= obsahuje 8 souřadnic
            if (tahZpet)
            {
                for (int i = kompletniPohyb.Length - 4; i >= 0; i = i - 4) // i = 4; 4 >=0; i = 4 - 4 
                {
                    //4,5,6,7,8
                    //0,1,2,3,4
                    MakeMove(new int[] { kompletniPohyb[i], kompletniPohyb[i + 1], kompletniPohyb[i + 2], kompletniPohyb[i + 3] }, tahZpet);
                }
            }
            else
            {
                for (int i = 0; i < kompletniPohyb.Length; i = i + 4) // i = 0; 0 < 8; i = 0 + 4
                {
                    //0,1,2,3,4
                    //4,5,6,7,8
                    MakeMove(new int[] { kompletniPohyb[i], kompletniPohyb[i + 1], kompletniPohyb[i + 2], kompletniPohyb[i + 3] }, tahZpet);
                }
            }
            if (ulozitDoHistorie)
            {
                HistoryMove.Add(kompletniPohyb.Clone() as int[]);
            }

        }
        /// <summary>
        /// Metoda pro kontrolou zda není X a Y za hranicí pole
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool IsValidCoordinates(int x, int y)
        {
            if (x > 7)
            {
                return false;
            }
            if (x < 0)
            {
                return false;
            }
            if (y > 7)
            {
                return false;
            }
            if (y < 0)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// Metoda pro převod tahu na string, pro uživatele
        /// </summary>
        /// <param name="pohyb"></param>
        /// <returns></returns>
        public string PohybNaString(int[] pohyb) //return new int[] { X1, Y1, S0, S1, X2, Y2, S2, S3 };
                                                 //int[] move1 = new int[] { 0, 1, 0, 1, 1, 2, 0, 1 };
                                                 // { 0, 1, 2, 3, 4, 5, 6, 7 }
        {
            string vystup = "|";

            for (int i = 0; i < pohyb.Length; i = i + 4)
            {
                //                     "| [a2] " " > o"             "|"        {1} =[0+0]= 0 + 97= 'a'  {2} =[1+0]= 1 + 1= 2        {3} =[2+0]= 0= " "   {4} =[3+0]= 1= "o"
                //                     "| [b3] " " > o"             "|"        {1} =[0+4]= 1 + 97= 'b'  {2} =[1+4]= 2 + 1= 3        {3} =[2+4]= 0= " ",  {4} =[3+4]= 1= "o"
                vystup = String.Format("{0} [{1}{2}] {3} --> {4}", vystup, (char)(pohyb[0 + i] + 'a'), pohyb[1 + i] + 1, StoneToString(pohyb[2 + i]), StoneToString(pohyb[3 + i]));
            }
            return vystup;
        }
        /// <summary>
        /// Metoda pro pro převod hodnoty na kamen, pro uživatele
        /// </summary>
        /// <param name="stone"></param>
        /// <returns></returns>
        private string StoneToString(int stone)
        {
            switch (stone)
            {
                case -2:
                    return "X";
                case -1:
                    return "x";
                case 1:
                    return "o";
                case 2:
                    return "O";
                default:
                    return " ";
            }
        }

        /// <summary>
        /// Metoda pro počítání kamenů na hrací desce
        /// </summary>
        /// <param name="bilyPesak"></param>
        /// <param name="bilaDama"></param>
        /// <param name="cernyPesak"></param>
        /// <param name="cernaDama"></param>
        public void CountStones(out int bilyPesak, out int bilaDama, out int cernyPesak, out int cernaDama)
        {
            //inicializace proměnných 
            cernyPesak = 0;
            bilyPesak = 0;
            bilaDama = 0;
            cernaDama = 0;
            for (int posY = 0; posY < 8; posY++) //cykly kreté projedou celé pole a spočítají figurky
            {
                for (int posX = 0; posX < 8; posX++)
                {
                    if (GetValue(posX, posY) == 1)
                    {
                        bilyPesak++;
                    }
                    if (GetValue(posX, posY) == -1)
                    {
                        cernyPesak++;
                    }
                    if (GetValue(posX, posY) == -2)
                    {
                        cernaDama++;
                    }
                    if (GetValue(posX, posY) == 2)
                    {
                        bilaDama++;
                    }
                }
            }
        }
    }
}
