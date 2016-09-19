using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Hanabi
{
    class Tests
    {
        FileInfo myOutputFile = new FileInfo(@"Y:\Desktop\my1-big.out");
        FileInfo originalOutputFile = new FileInfo(@"Y:\Desktop\all1.out");



        [Test]
        public void CompareInputOutput()
        {
            Game game = new Game();
            string input;
            System.IO.StreamReader file = new System.IO.StreamReader(@"Y:\Desktop\1-big.in");

            while ((input = file.ReadLine()) != null)
            //while ((input = Console.ReadLine()) != null)
            {
                Command command = new Command(input);
                game.ExecuteCommand(command);
            }
            file.Close();
        }
    }
}
