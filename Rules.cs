using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DamaKonzole_Framework
{
    class Rules
    {
        private Board board;
        //kolekce ListMove pro ukládání tahů
        public List<int[]> ListMove = new List<int[]>();

        public int kolo = 0;

        //2D pole pro dopočítávání pohybu figurek
        private int[,] smery =
        {
            {-1,0}, //vlevo 0
            {-1,1},  //diag.vlevo 1
            {0,1},  //nahoru 2
            {1,1},  //diag. doprava 3
            {1,0},  //doprava 4
            {1,-1}, //dozadu vpravo 5
            {0,-1}, //dozadu 6
            {-1,-1},//dozadu vlevo 7

        };
        public int TahuBezSkoku;
        //privátní proměnná hráče
        private int player;

        //První konstruktor
        public Rules(Board bo)
        {
            board = bo;
        }
        public Rules(Board bo,Rules rules)
        {
            board = bo;
            TahuBezSkoku = rules.TahuBezSkoku;
            player = rules.player;
            List<int[]> CopyListMove = new List<int[]>();
            foreach (int[] item in ListMove)
            {
                CopyListMove.Add((int[])item.Clone());
            }
        }
        /// <summary>
        /// Metoda pro postavení figurek, dle pravidel
        /// </summary>
        public void InitBoard()
        {
            for (int posY = 0; posY < 8; posY++)
            {
                for (int posX = 0; posX < 8; posX++)
                {
                    if (posY <= 1)
                    {
                        board.SetValue(posX, posY, 1);
                    }
                    else if (posY >= 6)
                    {
                        board.SetValue(posX, posY, -1);
                    }
                    else
                    {
                        board.SetValue(posX, posY, 0);
                    }
                }
            }
        }
        /// <summary>
        /// Metoda která vrací celý kompletní tah a porovná tah v seznamuTahu
        /// </summary>
        /// <param name="pohyb"></param>
        /// <returns></returns>
        public int[] FullMove(int[] pohyb)
        {
            //C4-D5-E6 = |2|3|1|0|-|3|4|-1|0|-|4|5|0|1| = delka 12
            //uživatel C4-E6 = jedná se o skok
            int X1 = pohyb[0]; //X1 = 2
            int Y1 = pohyb[1]; //Y1 = 3 
            int X2 = pohyb[2]; //X2 = 4
            int Y2 = pohyb[3]; //Y2 = 5

            int[] outMove;

            foreach (int[] move in ListMove) //pro každý move[] v listu ListMove
            {
                if (move.Length == 8) //Pokud se jedná pouze o tah
                {
                    if (X1 == move[0] && Y1 == move[1] && X2 == move[4] && Y2 == move[5]) //porovnání pokud jde jen o tah, první dvě souřadnice a poslední dvě(místo dopadu)
                    {
                        outMove = move; //pokud najde jen tah v ListMove tak uloží v outMove
                        FilterListMove(null); //nalezní tahu, filtrováníListMove a jeho smazaní
                        return outMove; //vrácen ten nalezený tah v ListMove
                    }
                }

                if (move.Length > 8) //Pokud se jedná o skok,dvoj,troj,....
                {
                    if (X1 == move[0] && Y1 == move[1] && X2 == move[8] && Y2 == move[9])  //Porovnání od uživatele s tahy v ListMove, X1,Y1,X2,Y2 = 2,3,4,5, zase první dvě a poslední dvě dopadové
                    {
                        //Hledá se počáteční souřadnice X1,Y1 a dopadová X2,Y2
                        outMove = new int[12]; //vytvoříme si pole o velikosti 12 prvků , protože single skok = 12
                        for (int i = 0; i < 12; i++) //vložíme náš nalezený tah do outMove
                        {
                            outMove[i] = move[i]; //vložení pole move do outMove
                        }
                        FilterListMove(outMove); //outMove obsahuje pole move a použita metoda FilterListMove
                        // obsah pole outMove je: C4-D5-E6 = |2|3|1|0|-|3|4|-1|0|-|4|5|0|1| = delka 12 znaků
                        return outMove;
                    }
                }
            }
            return new int[] { -1 };
        }
        /// <summary>
        /// Metoda pro filtrování ListMove
        /// </summary>
        /// <param name="smallMove"></param>
        private void FilterListMove(int[] smallMove) //Metoda bere jako proměnnou pole , C4-D5-E6 = |2|3|1|0|-|3|4|-1|0|-|4|5|0|1| = delka 12
        {
            if (smallMove == null) //pokud pole smallMove je  prázdné == null, tak se provede vymazání ListMove.Clear()
            {
                ListMove.Clear();
                return;
            }
            for (int i = 0; i < ListMove.Count(); i++) //ListMove.Count je počet všech možných tahů v listu
            {
                int[] move = ListMove[i]; //proměnná move, která obsahuje prvky z ListMove, po přiřazení z cyklu for, ListMove.Count()
                bool shoda = false;
                for (int indexShoda = 0; indexShoda < 12; indexShoda++) //cyklus indexShoda, dokud < 12
                {
                    if (move[indexShoda] == smallMove[indexShoda]) //pokud se naleznou shodné indexy v move[] a smallMove[], přiřadí se true
                    //hledají se shody v ListMove a našem smallMove C4-D5-E6 = |2|3|1|0|-|3|4|-1|0|-|4|5|0|1| = delka 12
                    {
                        shoda = true;
                    }
                    else
                    {
                        shoda = false;
                        break;
                    }
                }
                if (!shoda) //Pokud není shoda, tzn. !false = true, tj. nenalezli se shodné indexy
                {
                    ListMove.RemoveAt(i); //Prvek se odstraní tj, pokud move[0] != smallMove[0] tak se odebere
                    i--; //i se zmenší o 1
                    continue;
                }
                //Pokud je shoda v délce
                if (move.Length == smallMove.Length) //Shodné index, i shodná délka pole, tak se jedná o jediný poslední skok a ListMove se vymaže
                {
                    ListMove.Clear();
                    return;
                }
                //pokud je shoda, ale smallMove nezahrnuje celý move, jsou ještě další možné skoky v ListMove
                int[] result = new int[move.Length - 12]; //proměnná result obsahuje move.Length - 12, 12=single skok, pole již bez provedeného skoku
                for (int indexCopy = 12; indexCopy < move.Length; indexCopy++)
                {
                    result[indexCopy - 12] = move[indexCopy];
                }
                ListMove[i] = result; //zde se vrátí ListMove bez našeho skokou C4-D5-E6 = |2|3|1|0|-|3|4|-1|0|-|4|5|0|1| = delka 12
            }
        }
        /// <summary>
        /// Inicializace hráče na tahu
        /// </summary>
        public void InitPlayer()
        {
            player = 1;
        }
        /// <summary>
        /// Metoda pro zjištění hráče na tahu
        /// </summary>
        /// <returns>hodnota</returns>
        public int PlayerOnMove()
        {
            return player;
        }
        /// <summary>
        /// Meotda pro změnu hráče
        /// </summary>
        public void ChangePlayer()
        {
            player = player * -1;
        }

        /// <summary>
        /// Metoda pro sestavení tahů
        /// </summary>
        /// <param name="fromX"></param>
        /// <param name="fromY"></param>
        public void TryToMove(int fromX, int fromY)
        {
            int stone = board.GetValue(fromX, fromY); //3,1 = 1

            //X=3,Y=1
            for (int indexSmeru = 0; indexSmeru <= 7; indexSmeru++)
            {
                int hloubka = 0; //proměná hloubky vypisu tahu, kvůli kamenu
                int toX = fromX; //fromX = 3, tj. toX = 3
                int toY = fromY; //fromY = 1, tj. toY = 1

                while (board.IsValidCoordinates(toX + smery[indexSmeru, 0], toY + smery[indexSmeru, 1])) //[3,2] je true
                {
                    hloubka = hloubka + 1;
                    if ((stone == -1 || stone == 1) && hloubka > 1) //pokud je tah pěšák černý, nebo bílý a hloubka je větší než 1, tak se smyčka přeruší
                    {
                        break;
                    }

                    if (stone == -1 && indexSmeru >= 1 && indexSmeru <= 3) //pro černého
                    {
                        break;
                    }

                    if (stone == 1 && indexSmeru >= 5 && indexSmeru <= 7) //otočení pro bílého
                    {
                        break;
                    }

                    //VYPIS
                    toX = toX + smery[indexSmeru, 0]; //3+-1=2, první se začíná vlevo
                    toY = toY + smery[indexSmeru, 1]; //1+0=1
                    //toX = 2
                    //toY = 1
                    int destinationStone = board.GetValue(toX, toY); //pole kam dopadne kamen

                    if (stone < 0 && destinationStone < 0) //pokud hodnota kamene je menší než 0 a pole dopadu menší než 0, tak true a ukončí se, kontrola svého kamene 
                        break;
                    if (stone > 0 && destinationStone > 0) //pokud hodnota kamene je větší než 0 a pole dopadu větší než 0, tak true a ukončí se, kontrola svého kamene 
                        break;
                    if (destinationStone == 0) //pokud pole dopadu je 0 tak true a uloží se do listu tahů
                    {
                        ListMove.Add(new int[] { fromX, fromY, stone, 0, toX, toY, 0, stone });
                    }

                    //Skoky
                    else
                    {
                        if ((stone > 0 && destinationStone > 0) || (stone < 0 && destinationStone < 0))
                        {
                            break;
                        }
                        //toX = 3, toY = 1
                        int nextX = toX; //nové proměnné pro další souřadnice
                        int nextY = toY;
                        int hloubkaSkoku = 0;
                        while (board.IsValidCoordinates(nextX + smery[indexSmeru, 0], nextY + smery[indexSmeru, 1])) //ověření platnosti
                        {
                            hloubkaSkoku = hloubkaSkoku + 1;
                            if ((stone == -1 || stone == 1) && hloubkaSkoku > 1) //pokud se jedná o pěšáka tak pouze o jedno pole za něj ve směru
                            {
                                break;
                            }

                            //souřadnice pro daný směr
                            nextX = nextX + smery[indexSmeru, 0]; //2-1=1
                            nextY = nextY + smery[indexSmeru, 1]; //1-0=1 
                            int nextstone = board.GetValue(nextX, nextY); //hodnota následujícího pole 

                            if (nextstone != 0) //pokud není prázdné tak break cyklu
                            {
                                break;
                            }

                            int[] skok = { fromX, fromY, stone, 0, toX, toY, destinationStone, 0, nextX, nextY, 0, stone };
                            TryToJump(skok, new int[] { });
                        }
                        break;
                    }
                }
            }
        }
        //C4-D5-E6 = |2|3|1|0|-|3|4|-1|0|-|4|5|0|1|
        //E6-D6-C6 = |4|5|1|0|-|3|5|-1|0|-|2|5|0|1|
        //int[] skok = { fromX, fromY, stone, 0, toX, toY, destinationStone, 0, nextX, nextY, 0, stone };
        //C4-D5-E6 {0,1,2,3,4,5,6,7,8,9,10,11}
        /// <summary>
        /// Metoda pro sestavení skoků
        /// </summary>
        /// <param name="move"></param>
        /// <param name="oldMove"></param>
        public void TryToJump(int[] move, int[] oldMove)
        {
            board.Move(move, false, false); //tah se provede 
            int stone = board.GetValue(move[8], move[9]); //vezme se hodnota na souřanici
            int fromX = move[8];
            int fromY = move[9];

            for (int indexSmeru = 0; indexSmeru <= 7; indexSmeru++) //pro dámu všechny směry
            {
                int hloubka = 0; //omezení o jedno pole pro kamen
                int thruX = fromX;
                int thruY = fromY;
                while (board.IsValidCoordinates(thruX + smery[indexSmeru, 0], thruY + smery[indexSmeru, 1])) //Prohledávání polí kolem výchozího kamene
                {
                    //OMEZENÍ NA KAMENY
                    hloubka = hloubka + 1;
                    if ((stone == -1 || stone == 1) && hloubka > 1) //pokud je tah pěšák černý, nebo bílý a hloubka je větší než 1, tak se smyčka přeruší
                    {
                        break;
                    }
                    if (stone == -1 && indexSmeru >= 1 && indexSmeru <= 3) //pro černého
                    {
                        break;
                    }
                    if (stone == 1 && indexSmeru >= 5 && indexSmeru <= 7) //otočení pro bílého
                    {
                        break;
                    }

                    thruX = thruX + smery[indexSmeru, 0]; //hodnota pole X ve směru
                    thruY = thruY + smery[indexSmeru, 1]; //hodnota pole Y ve směru
                    int thruStone = board.GetValue(thruX, thruY); //hodnota kamene

                    if (thruStone == 0) //podmínka na prázdné pole, které nehledáme
                    {
                        break;
                    }

                    if ((stone > 0 && thruStone > 0) || (stone < 0 && thruStone < 0)) //podmínka na vlastní kámen, který nemůže být přeskočen
                    {
                        break;
                    }

                    int hloubkaSkoku = 0; //omezení na pole dopadu pro kámen
                    int destX = thruX;
                    int destY = thruY;

                    while (board.IsValidCoordinates(destX + smery[indexSmeru, 0], destY + smery[indexSmeru, 1]))
                    {
                        hloubkaSkoku = hloubkaSkoku + 1;
                        if ((stone == -1 || stone == 1) && hloubkaSkoku > 1) //pokud se jedná o pěšáka tak pouze o jedno pole za něj ve směru
                        {
                            break;
                        }

                        destX = destX + smery[indexSmeru, 0]; //cílová hodnota ve směru X
                        destY = destY + smery[indexSmeru, 1]; //cílová hodnota ve směru Y
                        int destinationStone = board.GetValue(destX, destY); //cílový kámen

                        if (destinationStone != 0) //pokud pole není rovno 0 tak break
                        {
                            break;
                        }

                        //pokud vše je OK, tak se uloží do listu
                        int[] skok = { fromX, fromY, stone, 0, thruX, thruY, thruStone, 0, destX, destY, 0, stone };

                        //Do newOldMove vložíme kopii oldMove
                        int[] newOldMove = (int[])oldMove.Clone(); //kopie starého tahu
                        newOldMove = newOldMove.Concat(move).ToArray(); //Concat spojí oba tahy do jednoho, tzn. starý + náš nový tah takže už dvojskok 

                        TryToJump(skok, newOldMove); //rekurzivní volaní zda je možné z našeho nového tahu ještě znovu někam skočit, bere náš skok + newOldMove
                    }
                    break;
                }
            }
            ListMove.Add(oldMove.Concat(move).ToArray()); //přidá do ListMove náš oldMove + move 
            board.Move(move, false, true); //tah se provede zpět 
        }
        /// <summary>
        /// Metoda vrátí true pokud cerna = 0 nebo bila = 0, pro testování zda hra neskončila
        /// </summary>
        /// <returns></returns>
        public bool IsGameFinished()
        {
            int bilyPesak, cernyPesak, bilaDama, cernaDama;
            board.CountStones(out bilyPesak, out bilaDama, out cernyPesak, out cernaDama);
            int cerna = cernyPesak + cernaDama;
            int bila = bilyPesak + bilaDama;

            if (cerna == 0 || bila == 0)
            {
                return true;
            }

            if (TahuBezSkoku >= 30)
            {
                return true;
            }
            return false;
        }

        /// <summary>       
        /// Genereuje seznam tahů
        /// </summary>
        public void MovesGenerate()
        {
            ListMove.Clear();
            for (int posY = 0; posY < 8; posY++)
            {
                for (int posX = 0; posX < 8; posX++)
                {
                    if ((board.GetValue(posX, posY) < 0 && PlayerOnMove() < 0) || (board.GetValue(posX, posY) > 0 && PlayerOnMove() > 0))
                    {
                        TryToMove(posX, posY);
                    }
                }
            }
            //Pravidlo na výběr nejdelšího tahu/skoku
            int maxDelka = 0;
            for (int i = 0; i < ListMove.Count; i++) //Cyklus který projede celý list a najde největší prvek
            {
                if (ListMove[i].Length > maxDelka) //Pokud je delká větší než maxDelka tak se přiřadí do proměnné
                {
                    maxDelka = ListMove[i].Length; //Přiřazení
                }
            }
            for (int i = ListMove.Count - 1; i >= 0; i--) //Cyklus, který porovná všechny prvky v poli a smaže ty co jsou menší než maxDelka, ListMove.Count=6, pokaždé co se provede odstranění se musí počet prvků snížit o jeden, protože se smazal 
            {
                if (ListMove[i].Length < maxDelka)
                {
                    ListMove.RemoveAt(i);
                }
            }

            //Smyčka pro povýšení kamene na dámu, pokud dosáhne konce desky
            foreach (int[] move in ListMove)
            {
                int delka = move.Length;
                if (move[delka - 1] == 1 && move[delka - 3] == 7)
                {
                    move[delka - 1] = 2;
                }
                if (move[delka - 1] == -1 && move[delka - 3] == 0)
                {
                    move[delka - 1] = -2;
                }
            }
        }
        /// <summary>
        /// Vrátí všechny tahy
        /// </summary>
        /// <returns></returns>
        public List<int[]> GetMovesList()
        {
            return GetMovesList(-1, -1);
        }
        /// <summary>
        /// Vrátí pouze tahy vybrané figurky
        /// </summary>
        /// <param name="filterX"></param>
        /// <param name="filterY"></param>
        /// <returns></returns>
        public List<int[]> GetMovesList(int filterX, int filterY)
        {
            List<int[]> NovySeznamTahu = new List<int[]>();
            foreach (int[] pohyb in ListMove)
            {
                if (filterX == -1)
                {
                    NovySeznamTahu.Add((int[])pohyb.Clone()); //vytvoření kopie ListMove do NovySeznamTahu
                }
                else
                {
                    if (pohyb[0] == filterX && pohyb[1] == filterY)
                    {
                        NovySeznamTahu.Add((int[])pohyb.Clone()); //vytvoření kopie ListMove do NovySeznamTahu ale jen pro figurky která začíná vybranou souřadnicí uživatele
                    }
                }
            }
            return NovySeznamTahu;
        }
    }
}
