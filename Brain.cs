using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DamaKonzole_Framework
{
    class Brain
    {
        const int MNOHO = 96; //konstanta MHONO pro preferování vzdálenější prohry a blížící se výhry
        const int MAX = 99; //konstanta MAX pro ohodnocení

        private Random random = new Random();
        private Rules rules;
        private Board board;

        //public Brain(Board board, Rules rules)
        //{
        //    this.board = new Board(board);
        //    this.rules = new Rules(rules);
        //}
        public Brain(Board boa, Rules rul)
        {
            board = boa;
            rules = rul;
        }

        /// <summary>
        /// Metoda která vybere náhodný tah, pomocí náhodného generování 
        /// </summary>
        /// <param name="moves"></param>
        /// <returns></returns>
        public int[] GetRandomMove(List<int[]> moves)
        {
            int count = moves.Count; //Zjistí celkový počet tahů, nejlepšíchTahů
            //int randomIndex = random.Next(count - 1);
            int randomIndex = random.Next(count); //Vybere náhodné číslo od 0, nezáporné až po "celkový počet tahů"
            int[] randomMove = moves[randomIndex]; //Zkopíruje konkrétní náhodně vybraný tah a vrátí jej
            return randomMove; //zde se vrátí vybraný tah a provede se
        }
        /// <summary>
        /// Metoda která vybere nejlepší tah
        /// </summary>
        /// <param name="hloubka"></param>
        /// <returns></returns>
        public int[] GetBestMove(int hloubka)
        {
            //GetBestMove vybírá náš nejlepší tah z listu nejlepších tahů, pomocí metody GetRandomMove
            return GetRandomMove(GetBestMoves(hloubka)); //GetBestMoves vrací list nejlepších tahů
        }

        /// <summary>
        /// Metoda pro vytvoření nejlepšího tahu
        /// </summary>
        /// <param name="hloubka"></param>
        /// <returns></returns>
        private List<int[]> GetBestMoves(int hloubka)
        {
            //vytvořime kopii seznamu všech tahů a pak ještě kolekci těch nejlepších tahů, které vrátíme
            List<int[]> nejlepsiTahy = new List<int[]>();
            List<int[]> tahy = new List<int[]>(); //všechny tahy hráče na tahu, tj.bílého 
            foreach (int[] tah in rules.GetMovesList())
            {
                tahy.Add(tah.Clone() as int[]);
            }
            if (tahy.Count <= 1) //Pokud seznam obsahuje jen jeden, nebo žádný tah, tak se rovnou vrátí. Protože je povinost skákat. Tento tah je nejlepší, není potřeba nic prohledávat
            {
                return tahy;
            }

            int hodnotaNejlepsihoTahu = -MAX;

            //Smyčka projede všechny tahy a provede je
            foreach (int[] tah in tahy)
            {
                board.Move(tah, false, false); //provedení 
                rules.ChangePlayer(); //změna hráče
                rules.MovesGenerate(); //generování všech tahů pro hráče na tahu

                /*Tady bude první volání minimaxu, které nám vrátí nejvýhodnější hodnotu v daném směru. 
                Takže budeme vědět jak moc výhodný je pro nás tento směr. 
                Pokud je je lepší, než ty co už máme, tak je nahradíme. Pokud je stejný, tak ho k nim přidáme. 
                Pokud je horší, tak jdem na další z možných tahů...
                Tyto Ify
                */

                int hodnota = -MiniMax(hloubka - 1);

                if (hodnota >= hodnotaNejlepsihoTahu) //pokud hodnota vrácená hodnota z minimaxu je větší nebo rovna -99, tak true a uloží se do seznamu nejlepších tahů
                {
                    if (hodnota > hodnotaNejlepsihoTahu) //pokud je pouze větší tj. 95 > -99
                    {
                        nejlepsiTahy.Clear(); //seznam se vymaže
                        hodnotaNejlepsihoTahu = hodnota; //a naše vrácená hodnota je nyní naše hodnotaNejlepsihoTahu
                    }
                    nejlepsiTahy.Add(tah.Clone() as int[]);
                }

                board.Move(tah, false, true); //tahy se provedou zpět
                rules.ChangePlayer(); // změní se hráč na tahu
            }
            rules.MovesGenerate(); //vygenerují se zase tahy pro hráče na tahu
            return nejlepsiTahy; //náš nejlepšítah je vrácen
        }
        /// <summary>
        /// Metoda MiniMaxu
        /// </summary>
        /// <param name="hloubka"></param>
        /// <returns></returns>
        private int MiniMax(int hloubka)
        {
            if (rules.IsGameFinished()) //pokud je hra u konce tj., cerne figurku = 0 nebo bile = 0
            {
                int minePesak, mineDama, enemyPesak, enemyDama;
                CountStones(out minePesak, out mineDama, out enemyPesak, out enemyDama);

                if (minePesak + mineDama > enemyPesak + enemyDama) //Výhra
                {
                    return MAX; //vrátí největší 99
                }
                if (minePesak + mineDama < enemyPesak + enemyDama) //Prohra
                {
                    return -MAX; //vrátí nejmenší -99
                }
                if (minePesak + mineDama == enemyPesak + enemyDama) //Remíza
                {
                    return 0; //0
                }
            }

            if (hloubka == 0) //dosaženo požadované hloubky koncová pozice, ohodnocení pozic
            {
                int hodnota = 0;
                int minePesak, mineDama, enemyPesak, enemyDama;
                CountStones(out minePesak, out mineDama, out enemyPesak, out enemyDama);
                //tabulka ohodnocení figurek
                //příklad 1 cerna a 1 bíla, na tahu bílý
                hodnota = hodnota + minePesak * 2; //vlastní pěšák, 0 + 1 * 2 = 2 
                hodnota = hodnota + mineDama * 4; //vlastní dáma, 0 
                hodnota = hodnota - enemyPesak * 3; //nepřátelský pěšák, 0 - 1 * 3 = -3
                hodnota = hodnota - enemyDama * 6; //nepřátelská dáma, 0

                return hodnota;
            }

            int ohodnoceni = -MAX; // -99
            List<int[]> tahy = new List<int[]>(); //pro generování tahů

            foreach (int[] tah in rules.GetMovesList()) //smyčka, pro každý tah, vygenerovaný ze všech tahů GetMoveList
            {
                tahy.Add(tah.Clone() as int[]); //přidá se to našeho Listu "tahy"
            }
            foreach (int[] tah in tahy) //smyčka, tah z kolekce "tahy"
            {
                board.Move(tah, false, false); //provedou se všechny možné tahy
                rules.ChangePlayer(); //změní se hráč
                rules.MovesGenerate(); //vygenerují se jeho tahy
                ohodnoceni = Math.Max(ohodnoceni, -MiniMax(hloubka - 1)); //ohodnocení se vybere z Max(ohodnocení, -(rekurzivní volání MiniMaxu(hloubka-1))
                board.Move(tah, false, true); //tahy se provedou zpět at máme výchozí desku
                rules.ChangePlayer(); //změní se hráč na tahu
            }

            //oddálení prohry a preferování výhry
            if (ohodnoceni > MNOHO)
            {
                ohodnoceni = ohodnoceni - 1;
            }
            if (ohodnoceni < -MNOHO)
            {
                ohodnoceni = ohodnoceni + 1;
            }
            return ohodnoceni;
        }
        /// <summary>
        /// Metoda pro počet kamenů
        /// </summary>
        /// <param name="minePesak"></param>
        /// <param name="mineDama"></param>
        /// <param name="enemyPesak"></param>
        /// <param name="enemyDama"></param>
        private void CountStones(out int minePesak, out int mineDama, out int enemyPesak, out int enemyDama)
        {
            //inicializace proměnných
            int bilyPesak, bilaDama, cernyPesak, cernaDama;

            //Metoda z boardu nám vrátí celkový počet kamenů na desce
            board.CountStones(out bilyPesak, out bilaDama, out cernyPesak, out cernaDama);

            minePesak = rules.PlayerOnMove() > 0 ? bilyPesak : cernyPesak; //minePesak, vezme hráče na tahu, např. bílý = 1, a 1 > 0, tak se nastaví jako bilyPesak
            mineDama = rules.PlayerOnMove() > 0 ? bilaDama : cernaDama; //mineDama, vezme 1 > 0, true takže bilaDama
            enemyPesak = rules.PlayerOnMove() > 0 ? cernyPesak : bilyPesak; //enemyPesak, 1 > 0, true takže cernyPesak
            enemyDama = rules.PlayerOnMove() > 0 ? cernaDama : bilaDama; //enemyDama, 1 > 0, true takže cernaDama
            //minePesak=bilyPesak
            //mineDama=bilaDama
            //enemyPesak=cernyPesak
            //enemyDama=cernaDama
        }
    }
}
