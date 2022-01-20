using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    class Program
    {

        static void Main(string[] args)
        {

            Game game = new Game(10, 20);

            game.RunGame();

            Console.ReadKey();

        }

    }

}
