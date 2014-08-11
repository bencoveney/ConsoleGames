using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleMinesweeper
{
    class Program
    {
        static void Main(string[] args)
        {
            Minesweeper game = new Minesweeper(10, 10, 4);
            Console.Read();
        }
    }
}
