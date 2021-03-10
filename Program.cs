using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DamaKonzole_Framework
{
    class Program
    {
        static void Main(string[] args)
        {
            GameController gameController = new GameController();
            //gameController.Start();
            gameController.Game();


            Console.WriteLine("Stiskni ENTER pro ukonceni");
            Console.ReadLine();
        }
    }
}
