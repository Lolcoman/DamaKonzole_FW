using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DamaKonzole_Framework
{
    class MoveServices
    {
        /// <summary>
        /// Metoda pro tahZpět
        /// </summary>
        /// <param name="board"></param>
        /// <param name="rules"></param>
        /// <param name="ui"></param>
        /// <param name="ptrTah"></param>
        /// <param name="posledniTah"></param>
        /// <param name="kolo"></param>
        public void TahZpet(Board board,Rules rules,UI ui,int ptrTah, int[] posledniTah, int kolo)
        {
            //ptrTah--;
            //posledniTah = board.HistoryMove[ptrTah];
            board.Move(posledniTah, false, true);
            rules.ChangePlayer();
            Console.Clear();
            kolo = board.HistoryMove.Count / 2;
            ui.PocetKol(kolo);
            board.VypocitejTahyBezSkoku(ptrTah);
            ui.PocetTahuBezSkoku(board.tahuBezSkoku);
            ui.PrintBoard(board);
            rules.MovesGenerate();
        }
        /// <summary>
        /// Metoda pro tahVpřed
        /// </summary>
        /// <param name="board"></param>
        /// <param name="rules"></param>
        /// <param name="ui"></param>
        /// <param name="ptrTah"></param>
        /// <param name="posledniTah"></param>
        /// <param name="kolo"></param>
        public void TahVpred(Board board, Rules rules, UI ui, int ptrTah, int[] posledniTah, int kolo)
        {
            board.Move(posledniTah, false, false);
            ptrTah++;
            rules.ChangePlayer();
            Console.Clear();
            kolo = board.HistoryMove.Count / 2;
            ui.PocetKol(kolo);
            board.VypocitejTahyBezSkoku(ptrTah);
            ui.PocetTahuBezSkoku(board.tahuBezSkoku);
            ui.PrintBoard(board);
            rules.MovesGenerate();
        }
    }
}
