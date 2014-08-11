using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGames.ConsoleMinesweeper
{
    /// <summary>
    /// An individual cell in the minesweeper game
    /// </summary>
    class MinesweeperCell
    {

        private const char BOMB_CELL_CHAR = '#';
        private const char EMPTY_CELL_CHAR = '-';
        private const char FLAG_CELL_CHAR = '?';
        private const char ERROR_CELL_CHAR = '@';

        private const ConsoleColor BOMB_CELL_COLOR = ConsoleColor.Red;
        private const ConsoleColor EMPTY_CELL_COLOR = ConsoleColor.White;
        private const ConsoleColor FLAG_CELL_COLOR = ConsoleColor.Cyan;
        private const ConsoleColor ERROR_CELL_COLOR = ConsoleColor.Gray;

        int _nearbyBombs;

        public MinesweeperCell()
        {
            _nearbyBombs = 0;

            HasBeenDug = false;
            HasBomb = false;
            HasFlag = false;
        }

        public bool HasBeenDug { get; private set; }
        public bool HasBomb { get; set; }
        public bool HasFlag { get; set; }

        public int NearbyBombs
        {
            get
            {
                return _nearbyBombs;
            }
            set 
            {
                if(value < 0 || value > 8) throw new ArgumentOutOfRangeException("Invalid Nearby Bombs");

                _nearbyBombs = value;
            }
        }

        public void Draw()
        {
            // If theres an exposed bomb
            if (HasBomb && HasBeenDug)
            {
                // Draw a bomb
                Console.ForegroundColor = BOMB_CELL_COLOR;
                Console.Write(BOMB_CELL_CHAR);
            }
            else
            {
                // If theres no exposed bomb but there is a flag
                if (HasFlag)
                {
                    // Draw a flag
                    Console.ForegroundColor = FLAG_CELL_COLOR;
                    Console.Write(FLAG_CELL_CHAR);
                }
                else
                {
                    // if theres no bomb of flag and this has been dug
                    if(HasBeenDug)
                    {
                        // Draw the uncovered colour
                        switch (NearbyBombs)
                        {
                            case 0:
                                Console.ForegroundColor = ERROR_CELL_COLOR;
                                Console.Write(' ');
                                break;
                            case 1:
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.Write(NearbyBombs);
                                break;
                            case 2:
                                Console.ForegroundColor = ConsoleColor.DarkGreen;
                                Console.Write(NearbyBombs);
                                break;
                            case 3:
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.Write(NearbyBombs);
                                break;
                            case 4:
                                Console.ForegroundColor = ConsoleColor.DarkYellow;
                                Console.Write(NearbyBombs);
                                break;
                            case 5:
                                Console.ForegroundColor = ConsoleColor.Magenta;
                                Console.Write(NearbyBombs);
                                break;
                            case 6:
                                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                                Console.Write(NearbyBombs);
                                break;
                            case 7:
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write(NearbyBombs);
                                break;
                            case 8:
                                Console.ForegroundColor = ConsoleColor.DarkRed;
                                Console.Write(NearbyBombs);
                                break;
                        }
                    }
                    else
                    {
                        // Draw Empty
                        Console.ForegroundColor = EMPTY_CELL_COLOR;
                        Console.Write(EMPTY_CELL_CHAR);
                    }
                }
            }
        }

        public void Dig()
        {
            this.HasBeenDug = true;
        }

        public void Flag()
        {
            // Dont allow flagging uncovered 
            if (!HasBeenDug)
            {
                this.HasFlag = !this.HasFlag;
            }
        }

        private static void DrawEmpty()
        {
        }
    }
}
