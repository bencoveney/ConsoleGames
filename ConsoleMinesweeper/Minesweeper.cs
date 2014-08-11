using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGames.ConsoleMinesweeper
{
    class Minesweeper
    {
        const char BORDER_CORNER_CHAR = '@';
        const char BORDER_HORIZONTAL_CHAR = '-';
        const char BORDER_VERTICAL_CHAR = '|';
        const ConsoleColor BORDER_COLOR = ConsoleColor.Gray;

        int _padding;
        GameState _state;

        private MinesweeperGrid _grid;

        public Minesweeper(int xSize, int ySize, int padding)
        {
            this._padding = padding;
            this._state = GameState.Running;

            // Set console size
            Console.SetWindowSize(xSize + (padding * 2), ySize + (padding * 2) + 3);
            Console.SetBufferSize(xSize + (padding * 2), ySize + (padding * 2) + 3);

            DrawBoard(xSize, ySize);

            // Create the game board
            _grid = new MinesweeperGrid(xSize, ySize, padding);

            // Run the game
            Run();
        }

        private void DrawBoard(int xSize, int ySize)
        {
            Console.ForegroundColor = BORDER_COLOR;

            // Top
            Console.SetCursorPosition(_padding - 1, _padding - 1);
            Console.Write(BORDER_CORNER_CHAR);
            for (int i = 0; i < xSize; i++)
            {
                Console.Write(BORDER_HORIZONTAL_CHAR);
            }
            Console.Write(BORDER_CORNER_CHAR);

            // LeftWall
            for (int i = 0; i < ySize; i++)
            {
                Console.SetCursorPosition(_padding - 1, _padding + i);
                Console.Write(BORDER_VERTICAL_CHAR);
            }

            // RightWall
            for (int i = 0; i < ySize; i++)
            {
                Console.SetCursorPosition(_padding + xSize, _padding + i);
                Console.Write(BORDER_VERTICAL_CHAR);
            }

            // Bottom
            Console.SetCursorPosition(_padding - 1, _padding + ySize);
            Console.Write(BORDER_CORNER_CHAR);
            for (int i = 0; i < xSize; i++)
            {
                Console.Write(BORDER_HORIZONTAL_CHAR);
            }
            Console.Write(BORDER_CORNER_CHAR);

            // Show instructions
            Console.SetCursorPosition(0, ySize + (_padding * 2));
            Console.Write(" - [Arrow Keys] to Move");
            Console.SetCursorPosition(0, ySize + (_padding * 2) + 1);
            Console.Write(" - [D] to Dig");
            Console.SetCursorPosition(0, ySize + (_padding * 2) + 2);
            Console.Write(" - [F] to Flag");
        }

        public void Run()
        {
            while(_state == GameState.Running)
            {
                // Read key input if its availible
                ConsoleKey? Key = null;
                if (Console.KeyAvailable)
                {
                    Key = Console.ReadKey(true).Key;
                }

                // If there was a keypress
                if (Key != null)
                {
                    switch (Key)
                    {
                        case ConsoleKey.LeftArrow:
                            _grid.MoveSelection(NavDirection.Left);
                            break;
                        case ConsoleKey.RightArrow:
                            _grid.MoveSelection(NavDirection.Right);
                            break;
                        case ConsoleKey.UpArrow:
                            _grid.MoveSelection(NavDirection.Up);
                            break;
                        case ConsoleKey.DownArrow:
                            _grid.MoveSelection(NavDirection.Down);
                            break;

                        case ConsoleKey.D:
                            // Dig the cell, handle finding a bomb
                            if (_grid.Dig())
                                _state = GameState.Lost;
                            break;

                        case ConsoleKey.F:
                            // Place a flag
                            _grid.Flag();

                            // Check if placing this flag wins the game
                            if (IsGameWon())
                                _state = GameState.Won;
                            break;
                    }

                    _grid.Draw();
                }
            }

            // when reaching this point the game is no longer running

            // Move to the middle ( account for text length)
            Console.SetCursorPosition(_padding + (_grid.Width / 2) - 4, _padding + (_grid.Height / 2));

            // "Pretty" colours
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.ForegroundColor = ConsoleColor.Yellow;


            switch (_state)
            {
                case GameState.Running:
                    break;
                case GameState.Won:
                    Console.WriteLine("WINNER!!");
                    break;
                case GameState.Lost:
                    Console.WriteLine("LOSER!!!");
                    break;
                default:
                    break;
            }
        }

        public bool IsGameWon()
        {
            // For each cell in the grid, check for discrepancies
            for (int x = 0; x < _grid.Width; x++)
            {
                for(int y = 0; y < _grid.Height; y++)
                {
                    // If you encounter any unflagged bombs or flagged empty cells the game hasnt been won
                    if ((_grid.Cells[x, y].HasBomb && !_grid.Cells[x, y].HasFlag) ||
                       (!_grid.Cells[x, y].HasBomb && _grid.Cells[x, y].HasFlag))
                        return false;
                }
            }

            // If there were no discrepancies the game is won
            return true;
        }
    }

    enum GameState
    {
        Running,
        Won,
        Lost
    }
}
