using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.Remoting.Services;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleGameofLife
{
    internal class Program
    {
        static void CommandLineArt()
        {
            Console.WriteLine(" A_A      A_A      A_A      A_A      A_A      A_A      A_A");
            Console.WriteLine("('^')    ('^')    ('^')    ('^')    ('^')    ('^')    ('^')");
            Console.WriteLine("c|c|_/   c|c|_/   c|c|_/   c|c|_/   c|c|_/   c|c|_/   c|c|_/");

            Console.WriteLine("  _____         _____         _____         _____         _____");
            Console.WriteLine(" /     \\/|     /     \\/|     /     \\/|     /     \\/|     /     \\/|");
            Console.WriteLine("|        |    |        |    |        |    |        |    |        |");
            Console.WriteLine(" \\_____/\\|     \\_____/\\|     \\_____/\\|     \\_____/\\|     \\_____/\\|");

            Console.WriteLine();
            Console.WriteLine(" A_A      A_A      A_A      A_A      A_A      A_A      A_A");
            Console.WriteLine("('^')    ('^')    ('^')    ('^')    ('^')    ('^')    ('^')");
            Console.WriteLine("c|c|_/   c|c|_/   c|c|_/   c|c|_/   c|c|_/   c|c|_/   c|c|_/");
        }

        //Print grid
        static void Print2DArray(String[,] grid, int gen)
        {
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    Console.Write(grid[i, j] + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine("Generation: " + gen + "\n");
            /*String input = Console.ReadLine();
            if (input == "exit") Environment.Exit(0);*/
            Thread.Sleep(500);
        }

        //Find cell neighbours
        private int FindNeighbours(String[,] grid, int x, int y)
        {
            int liveNeighbours = 0;
            for (int i = x - 1; i < x + 2; i++)
            {
                for (int j = y - 1; j < y + 2; j++)
                {
                    //If the cell is inside the grid
                    if ((i >= 0 && i < grid.GetLength(0)) && 
                        (j >= 0 && j < grid.GetLength(1))) {
                        if (grid[i, j] == "#") liveNeighbours++;
                    }
                }
            }
            return liveNeighbours;
        }

        //Check that there are live cells
        private bool CheckLiveness(String[,] grid)
        {
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    if (grid[i, j] == "#") return true;
                }
            }
            return false;
        }

        //Check that there is change between generations
        private bool CheckStagnation(String[,] grid, String[,] newGrid)
        {
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    if (grid[i, j] != newGrid[i, j]) return false;
                }
            }
            return true;
        }

        //Recursive call
        static void NextGen(Program p, String[,] grid, String[,] oldGrid, int gen)
        {
            int x = grid.GetLength(0);
            int y = grid.GetLength(1);

            //Grid to be updated with the new generation
            String[,] newGrid = new String[x, y];

            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    int neightbours = p.FindNeighbours(grid, i, j);
                    //If less than 2 or more than 3 neighbours, a live cell dies (-1 to account for itself being alive)
                    if (grid[i, j] == "#" && (neightbours-1 < 2 || neightbours-1 > 3)) newGrid[i, j] = ".";
                    //If exactly 3 live neighbours, a dead cell is revived
                    else if (grid[i, j] == "." && neightbours == 3) newGrid[i, j] = "#";
                    //else it stays the same
                    else newGrid[i, j] = grid[i, j];
                }
            }

            Print2DArray(newGrid, gen);

            String endComment = "The Game has ended at " + gen + " generation(s)"; String exitComment = "\nExit the game by pressing 'Enter'!";
            //If there are no live cells
            if (!p.CheckLiveness(newGrid))
            {
                Console.Write(endComment + ", with the death of the last cell. RIP" + exitComment);
                Console.ReadLine();
            } //If there is a stable structure
            else if (p.CheckStagnation(grid, newGrid) || p.CheckStagnation(oldGrid, newGrid))
            {
                Console.Write(endComment + ", as the cells have found a stable structure. Go life go!" + exitComment);
                Console.ReadLine();
            }
            else NextGen(p, newGrid, grid, ++gen);
        }

        //Game of Life runs automatically from here
        static void GameOfLife(int x, int y)
        {
            Program p = new Program();
            //Create grid
            String[,] grid = new String[x, y];
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    grid[i, j] = ".";
                }
            }

            //Add random live cells at a max of half the grid
            Random random = new Random();
            for (int i = 0; i < ((x*y)/2); i++)
            {
                int xPos = random.Next(x);
                int yPos = random.Next(y);
                grid[xPos, yPos] = "#";
            }

            //Start Game of Life
            Print2DArray(grid, 0);
            NextGen(p, grid, new String[x, y], 1); 
        }

        //Default
        static void Default(String message)
        {
            Console.WriteLine("Entered numbers could not be used due to '" + message + "'. Defaulting to a 20,20 grid");
        }

        static void StartGame()
        {
            Console.WriteLine("Welcome to Game of Life!\nEnter a number for the x-axis that is greater than 0: ");
            String x = Console.ReadLine();
            Console.WriteLine("Enter a number for the y-axis that is greater than 0: ");
            String y = Console.ReadLine();

            int res1, res2;
            try
            {
                res1 = Int32.Parse(y);
                res2 = Int32.Parse(x);
                if (res1 <= 0 || res2 <= 0)
                {
                    Default("One of the values are 0 or lower"); res1 = 20; res2 = 20;
                }
            }
            catch
            {
                Default("One of the values could not be parsed into a number"); res1 = 20; res2 = 20;
            }

            Console.WriteLine("\nThe game will be started with a " + res1 + ", " + res2 + " grid." +
                            "\nExit the game by writing 'exit'." +
                            "\nRe-enter values by writing 'retry'" +
                            "\nStart the game by pressing 'Enter'.");
            String input = Console.ReadLine();
            if (input == "exit") Environment.Exit(0);
            else if (input == "retry") StartGame();
            else GameOfLife(res1, res2);
        }

        static void Main(string[] args)
        {
            CommandLineArt();
            Console.WriteLine("\n");
            StartGame();
        }
    }
}
