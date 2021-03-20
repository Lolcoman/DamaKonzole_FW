using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace DamaKonzole_Framework
{
    class GameController
    {
        private Board board = new Board();
        private Rules rules;
        private UI ui;
        private Brain brain;
        private Data data;   

        //proměnné hráčů, pro uživatele 0, 1-4 obtížnost PC
        private int player1 = 0;
        private int player2 = 0;

        public GameController()
        {
            rules = new Rules(board);
            ui = new UI(board);
            brain = new Brain(board, rules);
            data = new Data();
        }
        /// <summary>
        /// Hlavní herní smyčka
        /// </summary>
        public void Game()
        {
            rules.InitBoard(); //inicializace desky
            ui.SelectPlayer(out player1, out player2); //výběr hráče na tahu
            rules.InitPlayer(); //inicializace hráče na tahu
            rules.MovesGenerate(); //vygenerování všech tahů pro aktuálního hráče tj. 1-bílý
            rules.TahuBezSkoku = 0;
            int kolo = 0; //počítadlo kol
            int ptrTah = board.HistoryMove.Count;//ukazatel na poslední tah v historii tahů
            int[] posledniTah = null; //uložen poslední tah

            while (!rules.IsGameFinished()) //cyklus dokud platí že oba hráči mají figurky, jinak konec
            {
                Console.Clear();
                ui.PocetKol(kolo);
                ui.PocetTahuBezSkoku(rules.TahuBezSkoku);
                ui.PrintBoard();

                //Tahy počítače
                if (rules.PlayerOnMove() == 1 && player1 > 0 || rules.PlayerOnMove() == -1 && player2 > 0) //pokud hráč na tahu je 1 a player1 > 0 tak true, provede tah a continue na dalšího hráče
                {
                    ui.PcInfo();
                    int[] move = null;
                    Brain brain = new Brain(board, rules);
                    Thread pc = new Thread(() => move = brain.GetBestMove(rules.PlayerOnMove() == 1 ? player1 : player2));
                    pc.IsBackground = true;
                    pc.Start();

                    ConsoleKey pressKey = ConsoleKey.A;

                    while (pc.IsAlive && pressKey != ConsoleKey.Escape && pressKey != ConsoleKey.Z)
                    {
                        if (Console.KeyAvailable)
                        {
                            pressKey = Console.ReadKey().Key;
                        }
                    }
                    if (pressKey == ConsoleKey.Escape)
                    {
                        pc.Abort();
                        Start(); //zobrazení menu
                        Game(); //start hry
                        continue;
                    }
                    if (pressKey == ConsoleKey.Z)
                    {
                        pc.Abort();
                        ui.SelectPlayer(out player1, out player2);
                        continue;
                    }
                    else
                    {
                        board.Move(move, true, false);
                    }

                    //pokud tah není skok tak se navýší počítadlo TahuBezSkoku
                    if (move.Length == 8)
                    {
                        rules.TahuBezSkoku++;
                    }
                    else
                    {
                        rules.TahuBezSkoku = 0;
                    }

                    kolo = board.HistoryMove.Count / 2; //přičtení do počítadla kol

                    rules.ChangePlayer();
                    rules.MovesGenerate();
                    //Thread.Sleep(1500);
                    continue;
                }

                //Tahy Hráče
                int[] vstup = null;
                int[] plnyVstup = null;
                bool platnyVstup = false;

                while (!platnyVstup) //Dokud je vstup !playtnyVstup tak pokračuje
                {
                    vstup = ui.InputUser(rules.PlayerOnMove()); //pokud -1 tak se podmínka neprovede protože -1 >= 0, pokud 0 tak se provede 0=0 a zkontroluje se platnost tahu

                    //Výpis historie tahu
                    if (vstup[0] == -4)
                    {
                        ui.PrintHelpMove(board.HistoryMove);
                    }

                    //Možnost tahu zpět/undo
                    if (vstup[0] == -3)
                    {
                        if (ptrTah > 0)
                        {
                            ptrTah--;
                            posledniTah = board.HistoryMove[ptrTah];
                            board.Move(posledniTah, false, true);
                            rules.TahuBezSkoku--;
                            rules.ChangePlayer();
                            Console.Clear();
                            ui.PocetKol(kolo);
                            ui.PocetTahuBezSkoku(rules.TahuBezSkoku);
                            ui.PrintBoard();
                            rules.MovesGenerate();

                        }
                    }
                    //Možnost tahu vpřed/redo
                    if (vstup[0] == -6)
                    {
                        if (ptrTah < board.HistoryMove.Count && board.HistoryMove.Count > 0)
                        {
                            posledniTah = board.HistoryMove[ptrTah];
                            board.Move(posledniTah, false, false);
                            ptrTah++;
                            rules.TahuBezSkoku++;
                            rules.ChangePlayer();
                            Console.Clear();
                            ui.PocetKol(kolo);
                            ui.PocetTahuBezSkoku(rules.TahuBezSkoku);
                            ui.PrintBoard();
                            rules.MovesGenerate();
                        }
                    }

                    //Pokud hráč do konzole zadá HELP
                    if (vstup[0] == -2)
                    {
                        if (vstup.Length > 1) //Pokud ještě zadá pro jakou figurku chce help
                        {
                            ui.PrintHelpMove(rules.GetMovesList(vstup[1], vstup[2])); //pro zadanou figurku
                        }
                        else //Vypíše všechny možné tahy hráče na tahu
                        {
                            ui.PrintHelpMove(rules.GetMovesList()); //všechny možné tahy hráče
                            //ui.PrintHelpMove(board.HistoryMove); //všechny možné tahy hráče
                        }
                    }

                    //SPRÁVNĚ
                    if (vstup[0] >= 0) //pokud je zadán správný pohyb tj A2-B3
                    {
                        plnyVstup = rules.FullMove(vstup); //převedení na kompletní pohyb který se skládá ze 4 souřadnic X,Y, stav před, stav po

                        platnyVstup = plnyVstup[0] != -1; //ověření zda je táhnuto dle pravidel, typ bool ve while cyklu

                        if (!platnyVstup) //pokud není vypíše uživately chybu
                        {
                            ui.Mistake(); //chyba
                        }
                    }

                    //Uložení hry
                    if (vstup[0] == -8)
                    {
                        data.SaveGame(player1, player2, ptrTah, board.HistoryMove);
                    }

                    //Načítání hry
                    if (vstup[0] == -9)
                    {
                        using (StreamReader sr = new StreamReader(@"test.txt"))
                        {
                            string prvniRadek = sr.ReadLine();
                            char hrac1 = prvniRadek[8];
                            int bily = (int)(hrac1 - '0');

                            string druhyRadek = sr.ReadLine();
                            char hrac2 = druhyRadek[8];
                            int cerny = (int)(hrac2 - '0');

                            string tretiRadek = sr.ReadLine();
                            char ptr = tretiRadek[8];
                            int ukazatel = (int)(ptr - '0');


                            Console.WriteLine(bily);
                            Console.WriteLine(cerny);
                            Console.WriteLine(ukazatel);

                            List<int[]> seznam = new List<int[]>();
                            string historieTahu;
                            while ((historieTahu = sr.ReadLine()) != null)
                            {
                                //char x1 = rozdeleno[0];

                                //char x1, y1, x2, y2; // X1Y1 vybrany kamen, X2Y2 kam pohnout
                                //x1 = input[0];
                                //int X1 = (int)(x1 - 'a'); //převod v tabulce ASCII
                                //y1 = input[1];
                                //int Y1 = (int)(y1 - '1'); //1, protože 0 není v herní desce
                                //x2 = input[3];
                                //int X2 = (int)(x2 - 'a');
                                //y2 = input[4];
                                //int Y2 = (int)(y2 - '1');

                                //for (int i = 0; i < rozdeleno.Length - 1; i++)

                                string[] rozdeleno = historieTahu.Split('|');
                                for (int i = rozdeleno.Length - 4; i >= 0; i -= 4) // i = 4; 4 >=0; i = 4 - 4 
                                {
                                    seznam.Add(new int[] { (int)(historieTahu[i] - 'a'), (int)(historieTahu[i + 2] - '1'), data.CharToStone(historieTahu[i + 4]), data.CharToStone(historieTahu[i + 6]) });
                                }
                                for (int i = 0; i < rozdeleno.Length - 1; i++)
                                {
                                    Console.Write(rozdeleno[i]);
                                }
                                Console.WriteLine();

                                for (int i = 0; i < seznam.Count; i++)
                                {
                                    for (int j = 0; j < seznam[i].Length; j++)
                                    {
                                        Console.Write(seznam[i][j]);
                                    }
                                    Console.WriteLine();
                                }

                                //foreach (int[] item in seznam)
                                //{
                                //    for (int i = 0; i < item.Length; i = i + 4)
                                //    {
                                //        Console.WriteLine("{0}{1}{2}{3}", (int)(item[0 + i] - 'a'), (int)(item[1 + i] - '1'), (int)(item[2 + i] - 'v'), (int)(item[3 + i] - '0'));
                                //    }
                                //}

                                //for (int i = 0; i < rozdeleno.Length - 1; i++)
                                //{

                                //    Console.Write(rozdeleno[i]);


                                //    for (int j = 0; j < rozdeleno.Length - 1; j++)
                                //    {
                                //        //Console.WriteLine(rozdeleno[i][j]);
                                //    }
                                //}
                                //Console.WriteLine();
                                //Console.WriteLine(x1);
                                //Console.WriteLine(rozdeleno[0][1]);

                            }
                            //for (int i = 0; i < historieTahu.Length; i = i + 2) // i = 4; 4 >=0; i = 4 - 4 
                            //{
                            //    seznam.Add(new int[] { historieTahu[i], historieTahu[i + 2], historieTahu[i + 4], historieTahu[i + 6] });
                            //}
                        }
                    }

                    //Zpět do menu
                    if (vstup[0] == -5)
                    {
                        Console.Clear();
                        Start();
                        Game();
                    }
                }
                board.Move(plnyVstup, true, false); //pokud je zadáno správně, metoda nastaví pohyb na desce
                ptrTah = board.HistoryMove.Count;

                //počítání kol
                kolo = board.HistoryMove.Count / 2;

                if (plnyVstup.Length == 8)
                {
                    rules.TahuBezSkoku++;
                }
                else
                {
                    rules.TahuBezSkoku = 0;
                }

                if (rules.ListMove.Count == 0) //pokud je ListMove prázdnej tak se změní hráč na tahu a vygenerují se pro něj nové možné tahy
                {
                    rules.ChangePlayer();
                    rules.MovesGenerate();
                }
                else //pokud v listu stále je možnost, tak pokračuje hráč, vícenásobné skoky
                {
                    continue;
                }
            }
            ui.PrintBoard();
            ui.Finished();
        }
        /// <summary>
        /// Metoda pro nastavení hodnoty políčka
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="hodnota"></param>
        public void SetValueOnBoard(int posX, int posY, int value)
        {
            board.SetValue(posX, posY, value);
        }

        public void Start()
        {
            ui.HlavniMenu();
        }
    }
}
