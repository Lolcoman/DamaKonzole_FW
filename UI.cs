using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DamaKonzole_Framework
{
    class UI
    {
        private int SelectedIndex;
        private string[] Options;
        private string Prompt;

        //private Board board;
        //public UI(Board bo)
        //{
        //    board = bo;
        //}
        /// <summary>
        /// Metoda pro vybrání hráčů, pc nebo člověk
        /// </summary>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        public void SelectPlayer(out int player1, out int player2)
        {
            bool vstupSmycky1 = true;
            bool vstupSmycky2 = true;
            string vstupUzivatele1;
            string vstupUzivatele2;
            player1 = 0;
            player2 = 0;

            Console.WriteLine("Vyberte bílého hráče pro lidského 0, pro počítač 1-4");

            vstupUzivatele1 = Console.ReadLine();

            while (vstupSmycky1)
            {
                if ((int.TryParse(vstupUzivatele1, out player1)) && player1 <= 4 && player1 >= 0)
                {
                    vstupSmycky1 = false;
                }
                else
                {
                    Console.WriteLine("Vyberte bílého hráče pro lidského 0, pro počítač 1-4");
                    vstupUzivatele1 = Console.ReadLine();
                }
            }
            Console.WriteLine("Vyberte černého hráče pro lidského 0, pro počítač 1-4");
            vstupUzivatele2 = Console.ReadLine();

            while (vstupSmycky2)
            {
                if ((int.TryParse(vstupUzivatele2, out player2)) && player2 <= 4 && player2 >= 0)
                {
                    vstupSmycky2 = false;
                }
                else
                {
                    Console.WriteLine("Vyberte černého hráče pro lidského 0, pro počítač 1-4");
                    vstupUzivatele2 = Console.ReadLine();
                }
            }
        }
        public void PcInfo()
        {
            Console.WriteLine("Stiskni 'ESC' pro ukončení");
            Console.WriteLine("Stiskni 'Z' pro změnu hráčů");
        }

        public int[] InputUser(int playerOnMove, Board board) //příjmá int, který hráč je na tahu
        {
            Console.WriteLine("Hraje {0} zadej pohyb ve formátu(A2-B3):", playerOnMove > 0 ? "BÍLÝ s o/O" : "ČERNÝ s x/X"); //použití ternárního operátoru pro informaci pro uživatele který hráč je na tahu, pokud > 0 tak hraje bílý
            Console.WriteLine("'help' pro nápovědu tahů");
            Console.WriteLine("'hist' pro historii tahů");
            Console.WriteLine("'zpet' a pro tah zpět");
            Console.WriteLine("'vpred' pro tah vpřed");
            Console.WriteLine("'save' a 'load' pro uložení/načtení");
            Console.WriteLine("'exit' pro návrat");
            //Vstup uživatele s převedením na malá písmena
            //Špatný vstup vrácena -1, Správný vstup vráceno pole {X1,Y1,X2,Y2}
            //Ověření správnosti provádní až třída GAME CONTROLLER  

            string input = Console.ReadLine().ToLower();

            if (input == "help") //Pro nápovědu všech možných tahů!
            {
                Console.Write("Zadej pro jaký kámen chceš nápovědu nebo 'all' pro všechny tahy:");
                string help = Console.ReadLine().ToLower();
                if (help.Length == 0)
                {
                    return new int[] { -1 };
                }
                if (help == "all")
                {
                    return new int[] { -2 };
                }
                //Kontrola správného zadání
                char helpX = help[0];
                int helpX1 = (int)(helpX - 'a');
                char helpY = help[1];
                int helpY1 = (int)(helpY - '1');
                if (board.IsValidCoordinates(helpX1, helpY1))
                {
                    return new int[] { -2, helpX1, helpY1 };
                }

                return new int[] { -1 };
                //return new int[] { -2, helpX1, helpY1 };
            }

            //Uložení hry
            if (input == "save")
            {
                return new int[] { -8 };
            }
            //Načtení hry
            if (input == "load")
            {
                return new int[] { -9 };
            }
            //Pro zpět do menu
            if (input == "exit")
            {
                return new int[] { -5 };
            }

            //Tah zpět undo
            if (input == "zpet")
            {
                if (board.HistoryMove.Count == 0)
                {
                    Console.WriteLine("Prázdná historie!");
                    return new int[] { -1 };
                }
                return new int[] { -3 };
            }
            //Tah vpřed redo
            if (input == "vpred")
            {
                if (board.HistoryMove.Count == 0)
                {
                    Console.WriteLine("Prázdná historie!");
                    return new int[] { -1 };
                }
                return new int[] { -6 };
            }
            //Historie tahů
            if (input == "hist")
            {
                Console.WriteLine("Historie všech tahů:\n");
                return new int[] { -4 };
            }

            if (input.Length != 5)
            {
                return new int[] { -1 };
            }

            char x1, y1, x2, y2; // X1Y1 vybrany kamen, X2Y2 kam pohnout
            x1 = input[0];
            int X1 = (int)(x1 - 'a'); //převod v tabulce ASCII
            y1 = input[1];
            int Y1 = (int)(y1 - '1'); //1, protože 0 není v herní desce
            x2 = input[3];
            int X2 = (int)(x2 - 'a');
            y2 = input[4];
            int Y2 = (int)(y2 - '1');
            return Parsing(X1, Y1, X2, Y2);
        }

        /// <summary>
        /// Vykreslení herní desky
        /// </summary>
        public void PrintBoard(Board board)
        {
            Console.WriteLine("     A   B   C   D   E   F   G   H");
            for (int y = 7; y >= 0; y--)
            {
                if (y == 7)
                {
                    Console.WriteLine("   ┌───┬───┬───┬───┬───┬───┬───┬───┐");
                }
                else
                    Console.WriteLine("   ├───┼───┼───┼───┼───┼───┼───┼───┤");
                Console.Write(" {0} │ ", y + 1); //výpis souřadnic vlevo
                for (int x = 0; x < 8; x++)
                {
                    int figure = board.GetValue(x, y);
                    string set;
                    switch (figure)
                    {
                        case 2:
                            set = "O"; //bílá dáma
                            break;
                        case 1:
                            set = "o"; //bílá
                            break;
                        case -1:
                            set = "x"; //černá
                            break;
                        case -2:
                            set = "X"; //černá dáma
                            break;
                        default:
                            set = " ";
                            break;
                    }
                    Console.Write("{0} │ ", set);
                }
                Console.Write("{0}\n", y + 1); //výpis souřadnic vpravo
            }
            Console.WriteLine("   └───┴───┴───┴───┴───┴───┴───┴───┘");
            Console.WriteLine("     A   B   C   D   E   F   G   H");
        }

        /// <summary>
        /// Metoda pro výpis chyby
        /// </summary>
        public void Mistake()
        {
            Console.WriteLine("Špatně zadáno!");
        }
        /// <summary>
        /// Metoda pro kontrolu parsování souřadnic
        /// </summary>
        /// <param name="X1"></param>
        /// <param name="Y1"></param>
        /// <param name="X2"></param>
        /// <param name="Y2"></param>
        /// <returns></returns>
        public int[] Parsing(int X1, int Y1, int X2, int Y2)
        {
            if (X1 > 7 || X1 < 0) //pokud X1 je větší než 7 a zároveň X1 je menší než 0 tak vrať -1
            {
                return new int[] { -1 };
            }
            if (Y1 > 7 || Y1 < 0) //pokud Y1 je větší než 7 a zároveň Y1 je menší než 0 tak vrať -1
            {
                return new int[] { -1 };
            }
            if (X2 > 7 || X2 < 0) //pokud X2 je větší než 7 a zároveň X2 je menší než 0 tak vrať -1
            {
                return new int[] { -1 };
            }
            if (Y2 > 7 || Y2 < 0) //pokud Y2 je větší než 7 a zároveň Y2 je menší než 0 tak vrať -1
            {
                return new int[] { -1 };
            }
            return new int[] { X1, Y1, X2, Y2 };
        }
        /// <summary>
        /// Metoda vypíše tahy
        /// </summary>
        /// <param name="seznam"></param>
        public void PrintHelpMove(List<int[]> seznam, Board board)
        {
            if (seznam.Count == 0)
            {
                Console.WriteLine("Prázdný seznam tahů!");
            }
            foreach (int[] prvek in seznam)
            {
                Console.WriteLine(board.PohybNaString(prvek));
            }
        }
        /// <summary>
        /// Metoda provádí pouze výpis kol do konzole pro uživatele
        /// </summary>
        /// <param name="kolo"></param>
        public void PocetKol(int kolo)
        {
            Console.WriteLine("Počet kol: {0}", kolo);
        }
        /// <summary>
        /// Metoda provádí výpis tahů bez skoku
        /// </summary>
        /// <param name="bezSkoku"></param>
        public void PocetTahuBezSkoku(int bezSkoku)
        {
            Console.WriteLine("Počet tahu bez skoku: {0}", bezSkoku);
        }
        /// <summary>
        /// Metoda pro vykresleního hlavního menu 
        /// </summary>
        public void HlavniMenu()
        {
            string prompt = @"

 ██████╗  ██████╗ ████████╗██╗ ██████╗██╗  ██╗ █████╗     ██████╗  █████╗ ███╗   ███╗ █████╗ 
██╔════╝ ██╔═══██╗╚══██╔══╝██║██╔════╝██║ ██╔╝██╔══██╗    ██╔══██╗██╔══██╗████╗ ████║██╔══██╗
██║  ███╗██║   ██║   ██║   ██║██║     █████╔╝ ███████║    ██║  ██║███████║██╔████╔██║███████║
██║   ██║██║   ██║   ██║   ██║██║     ██╔═██╗ ██╔══██║    ██║  ██║██╔══██║██║╚██╔╝██║██╔══██║
╚██████╔╝╚██████╔╝   ██║   ██║╚██████╗██║  ██╗██║  ██║    ██████╔╝██║  ██║██║ ╚═╝ ██║██║  ██║
 ╚═════╝  ╚═════╝    ╚═╝   ╚═╝ ╚═════╝╚═╝  ╚═╝╚═╝  ╚═╝    ╚═════╝ ╚═╝  ╚═╝╚═╝     ╚═╝╚═╝  ╚═╝                                                                             
Pro výběr použij šipky";
            string[] options = { "Hrát", "Pravidla", "Konec" };
            Menu(prompt, options);
            DisplayOptions();
            int selectedIndex = Run();
            switch (selectedIndex)
            {
                case 0:
                    Hra();
                    break;
                case 1:
                    Pravidla();
                    break;
                case 2:
                    Konec();
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Metoda pro spuštění hry
        /// </summary>
        private void Hra()
        {
            Console.Clear();
        }
        /// <summary>
        /// Metoda pro výpis pravidel
        /// </summary>
        private void Pravidla()
        {
            Console.Clear();
            Console.WriteLine("CÍL HRY:");
            Console.WriteLine("■ Zajmout všechny soupeřovy kameny.\n");
            Console.WriteLine("VÝCHOZÍ SITUACE");
            Console.WriteLine("■ Hraje se na desce 8x8 polí.");
            Console.WriteLine("■ Na začátku hry stojí figurky ve dvou krajníchřadách.\n");
            Console.WriteLine("PRAVIDLA HRY:");
            Console.WriteLine("■ Hráči se v tazích pravidelně střídají.");
            Console.WriteLine("■ Hráč na tahu smí posunout svůj obyčejný kámen o jedno pole na sousední volné pole a to směrem vpřed, do stran nebo diagonálně vpřed. Nesmí diagonálně ani ortogonálně vzad.");
            Console.WriteLine("■ Obyčejný kámen může také zajmout přeskočením kámen soupeře.");
            Console.WriteLine("■ Soupeřův kámen musí ležet na sousedním poli a bezprostředně za tímto přeskakovaným kamenem musí být volné pole.");
            Console.WriteLine("■ Kámen je při realizaci skoku přemístěn na toto volné pole a přeskočený soupeřůvkámen je odstraněn z desky.");
            Console.WriteLine("■ Obyčejný kámen smí skákat také pouze vpřed, diagonálněvpřed a do stran.");
            Console.WriteLine("■ Vícenásobné skoky jsou dovoleny.");
            Console.WriteLine("■ Pokud obyčejný kámen ukončí svůj tah v poslednířadě na protější straně desky, je povýšennadámu.");
            Console.WriteLine("■ Dáma se smí pohybovat všemi směry (i vzad) o libovolný počet volných polí.");
            Console.WriteLine("■ Dáma může zajímat kámen soupeře přeskočením, přičemž může přeskočit libovolný počet volných polípřed kamenem, zajímaný kámen a libovolný počet volných polí za tímto kamenem (podobně jako v klasické dámě).");
            Console.WriteLine("■ Zajímání soupeřových kamenů je povinné a hráč musí zajmout co nejvíce kamenů, pokudmá více možností jak skákat.");
            Console.WriteLine("■ Pokud některý hráč nemůže provést žádný tah, pokračuje ve hře jeho soupeř.\n");
            Console.WriteLine("KONEC HRY:");
            Console.WriteLine("■ Hráč, který zajme všechny soupeřovy kameny vyhrává");
            Console.WriteLine("■ Pokud po 30 tahů není zajat žádný kámen, vyhrává hráč, kterému zbývá více kamenů.\n");
            Console.WriteLine("Stisknutím libovolného tlačítka se vrátíte do HLAVNÍHO MENU.");
            Console.ReadKey(true);
            HlavniMenu();
        }
        /// <summary>
        /// Metoda ukončení z nabídáky
        /// </summary>
        private void Konec()
        {
            Console.WriteLine("Stiskni libovolné tlačítko pro konec....");
            Console.ReadKey(true);
            Environment.Exit(0);
        }
        /// <summary>
        /// Metoda nastavení menu
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="options"></param>
        public void Menu(string prompt, string[] options)
        {
            Prompt = prompt;
            Options = options;
            SelectedIndex = 0;
        }
        /// <summary>
        /// Metoda vypsání možností v menu
        /// </summary>
        public void DisplayOptions()
        {
            Console.WriteLine(Prompt);
            for (int i = 0; i < Options.Length; i++)
            {
                string currentOption = Options[i];
                string prefix;
                if (i == SelectedIndex)
                {
                    prefix = "*";
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.White;
                }
                else
                {
                    prefix = " ";
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Black;
                }
                Console.WriteLine($"{prefix} << {currentOption} >>");
            }
            Console.ResetColor();
        }
        /// <summary>
        /// Metoda pro spuštění
        /// </summary>
        /// <returns></returns>
        public int Run()
        {
            ConsoleKey keyPressed;
            do
            {
                Console.Clear();
                DisplayOptions();

                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                keyPressed = keyInfo.Key;

                //Když zmáčkneme šipku tak se zvětší index
                if (keyPressed == ConsoleKey.UpArrow)
                {
                    SelectedIndex--;
                    if (SelectedIndex == -1)
                    {
                        SelectedIndex = Options.Length - 1;
                    }
                }
                else if (keyPressed == ConsoleKey.DownArrow)
                {
                    SelectedIndex++;
                    if (SelectedIndex == Options.Length)
                    {
                        SelectedIndex = 0;
                    }
                }
            } while (keyPressed != ConsoleKey.Enter);
            return SelectedIndex;
        }
        /// <summary>
        /// Metoda pro info kdo vyhrál
        /// </summary>
        public void Finished(Board board)
        {
            int bilyPesak, cernyPesak, bilaDama, cernaDama;
            board.CountStones(out bilyPesak, out bilaDama, out cernyPesak, out cernaDama);
            int cerna = cernyPesak + cernaDama;
            int bila = bilyPesak + bilaDama;

            if (cerna == 0)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("VÍTĚŽ JE BÍLÝ!");
                Console.ResetColor();
            }
            if (bila == 0)
            {
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("VÍTĚZ JE ČERNÝ!");
                Console.ResetColor();
            }
        }
    }
}
