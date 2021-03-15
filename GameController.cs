using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace DamaKonzole_Framework
{
    class GameController
    {
        private Board board = new Board();
        private Rules rules;
        private UI ui;
        private Brain brain;

        //proměnné hráčů, pro uživatele 0, 1-4 obtížnost PC
        private int player1 = 0;
        private int player2 = 0;

        public GameController()
        {
            rules = new Rules(board);
            ui = new UI(board);
            brain = new Brain(board, rules);
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
            int ptrTah = 0;//ukazatel na poslední tah v historii tahů
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
                    //ptrTah++;
                    vstup = ui.InputUser(rules.PlayerOnMove()); //pokud -1 tak se podmínka neprovede protože -1 >= 0, pokud 0 tak se provede 0=0 a zkontroluje se platnost tahu

                    //Výpis historie tahu
                    if (vstup[0] == -4)
                    {
                        ui.PrintHelpMove(board.HistoryMove);
                    }

                    //Možnost tahu zpět
                    if (vstup[0] == -3)
                    {
                        if (ptrTah > 0)
                        {
                            ptrTah--;
                            board.Move(posledniTah, false, true);
                        }
                        //chyba pokud černý ještě netáhl
                        if (rules.PlayerOnMove() == -1 && board.HistoryMove.Count - 1 == 0)
                        {
                            ui.Mistake();
                            ui.InputUser(rules.PlayerOnMove());
                        }

                        if (rules.PlayerOnMove() == -1)
                        {
                            posledniTah = board.HistoryMove[ptrTah - 1] ;
                        }
                        if (rules.PlayerOnMove() == 1)  
                        {
                            posledniTah = board.HistoryMove[ptrTah];
                        }
                        board.Move(posledniTah, false, true);
                        rules.TahuBezSkoku--;
                        rules.ChangePlayer();
                        ui.PrintBoard();
                        rules.MovesGenerate();
                        continue;
                    }

                    if (vstup[0] == -2) //Pokud hráč do konzole zadá HELP
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
                    if (vstup[0] >= 0) //pokud je zadán správný pohyb tj A2-B3
                    {
                        plnyVstup = rules.FullMove(vstup); //převedení na kompletní pohyb který se skládá ze 4 souřadnic X,Y, stav před, stav po

                        platnyVstup = plnyVstup[0] != -1; //ověření zda je táhnuto dle pravidel, typ bool ve while cyklu

                        if (!platnyVstup) //pokud není vypíše uživately chybu
                        {
                            ui.Mistake(); //chyba
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
