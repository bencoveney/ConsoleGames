using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGames.ConsoleMinesweeper
{
    class MinesweeperGrid
    {
        private const float PERCENTAGE_OF_CELLS_AS_BOMBS = 0.1f;

        /// <summary>
        /// The grid cells
        /// </summary>
        private MinesweeperCell[,] _gridCells;

        /// <summary>
        /// The padding
        /// </summary>
        private int _padding;

        /// <summary>
        /// The selection x
        /// </summary>
        private int _selectionX;
        /// <summary>
        /// The selection y
        /// </summary>
        private int _selectionY;

        /// <summary>
        /// Initializes a new instance of the <see cref="MinesweeperGrid"/> class.
        /// </summary>
        /// <param name="xSize">Size of the x.</param>
        /// <param name="ySize">Size of the y.</param>
        public MinesweeperGrid(int xSize, int ySize, int padding)
        {
            this._padding = padding;

            this._selectionX = 0;
            this._selectionY = 0;

            // create grid and initialise
            this._gridCells = new MinesweeperCell[xSize, ySize];
            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    _gridCells[x, y] = new MinesweeperCell();
                }
            }

            GenerateBombs();

            // Initial draw
            Draw();
        }

        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        /// 
        public int Width { get { return _gridCells.GetLength(0); } }
        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public int Height { get { return _gridCells.GetLength(1); } }
        /// <summary>
        /// Gets the grid's cells
        /// </summary>
        /// <value>
        /// The cells.
        /// </value>
        public MinesweeperCell[,] Cells { get { return _gridCells; } }

        /// <summary>
        /// Draws the board's cells
        /// </summary>
        public void Draw()
        {
            // For each cell on the grid
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    // Show the selection if this is the selected cell
                    Console.BackgroundColor = x == _selectionX && y == _selectionY ? ConsoleColor.Gray : ConsoleColor.Black;

                    // Draw the cell
                    Console.SetCursorPosition(OffsetPosition(x), OffsetPosition(y));
                    _gridCells[x, y].Draw();
                }
            }
        }

        /// <summary>
        /// Generates the bombs and caches each cell's number of surrounding bombs
        /// </summary>
        private void GenerateBombs()
        {
            Random r = new Random(DateTime.Now.Millisecond);

            // Calculate how many bombs to generate
            int numberOfCells = Width * Height;
            int numberOfBombs = (int)(numberOfCells * PERCENTAGE_OF_CELLS_AS_BOMBS);

            // Generate the bombs
            for (int i = 0; i < numberOfBombs; i++)
            {
                // Find a location until a valid location is found
                bool isLocationValid = false;
                int xLocation = -1, yLocation = -1;
                while (!isLocationValid)
                {
                    // Random position
                    xLocation = r.Next(Width);
                    yLocation = r.Next(Height);

                    isLocationValid = _gridCells[xLocation, yLocation].HasBomb ? false : true;
                }

                // Set as a bomb
                _gridCells[xLocation, yLocation].HasBomb = true;
            }

            // For each cell calculate the number of bombs
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    int nearbyBombs = 0;

                    // top left
                    if (isPositionBomb(x - 1, y - 1)) nearbyBombs++;
                    // top
                    if (isPositionBomb(x, y - 1)) nearbyBombs++;
                    // top right
                    if (isPositionBomb(x + 1, y - 1)) nearbyBombs++;

                    // left
                    if (isPositionBomb(x - 1, y)) nearbyBombs++;
                    // right
                    if (isPositionBomb(x + 1, y)) nearbyBombs++;

                    // bottom left
                    if (isPositionBomb(x - 1, y + 1)) nearbyBombs++;
                    // bottom
                    if (isPositionBomb(x, y + 1)) nearbyBombs++;
                    // bottom right
                    if (isPositionBomb(x + 1, y + 1)) nearbyBombs++;

                    // store the calculated number
                    _gridCells[x, y].NearbyBombs = nearbyBombs;
                }
            }
        }

        /// <summary>
        /// Determines whether the given position contains a bomb
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        private bool isPositionBomb(int x, int y)
        {
            // If off the board
            if (x < 0 || x == Width || y < 0 || y == Height)
            {
                return false;
            }

            // Check for the bomb
            if (_gridCells[x, y].HasBomb) return true;

            return false;
        }

        /// <summary>
        /// Offsets the position by the padding amount
        /// </summary>
        /// <param name="numberToOffset">The number to offset.</param>
        /// <returns></returns>
        private int OffsetPosition(int numberToOffset)
        {
            return numberToOffset + _padding;
        }

        /// <summary>
        /// Moves the selected cell in a certain direction.
        /// </summary>
        /// <param name="direction">The direction.</param>
        public void MoveSelection(NavDirection direction)
        {
            switch (direction)
            {
                case NavDirection.Up:
                    _selectionY = _selectionY - 1 >= 0 ? _selectionY - 1 : _selectionY;
                    break;
                case NavDirection.Down:
                    _selectionY = _selectionY + 1 < Height ? _selectionY + 1 : _selectionY;
                    break;
                case NavDirection.Left:
                    _selectionX = _selectionX - 1 >= 0 ? _selectionX - 1 : _selectionX;
                    break;
                case NavDirection.Right:
                    _selectionX = _selectionX + 1 < Width ? _selectionX + 1 : _selectionX;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Digs the cell under the cursor and returns true if a bomb was dug
        /// </summary>
        /// <returns></returns>
        public bool Dig()
        {
            // Uncover the ground
            Dig(_selectionX, _selectionY);

            // Check if there was a bomb
            if (_gridCells[_selectionX, _selectionY].HasBomb)
                return true;

            return false;
        }

        // Digs the cell at a certain location
        public void Dig(int xPos, int yPos)
        {
            if (xPos < 0 || xPos == Width || yPos < 0 || yPos == Height) return;
            if (_gridCells[xPos, yPos].HasBeenDug) return;

            // Digs the cell
            _gridCells[xPos, yPos].Dig();

            // If this was an empty cell dig the nearby ones
            if (_gridCells[xPos, yPos].NearbyBombs == 0)
            {
                Dig(xPos - 1, yPos - 1);
                Dig(xPos, yPos - 1);
                Dig(xPos + 1, yPos - 1);

                Dig(xPos - 1, yPos);
                Dig(xPos + 1, yPos);

                Dig(xPos - 1, yPos + 1);
                Dig(xPos, yPos + 1);
                Dig(xPos + 1, yPos + 1);
            }
        }

        public void Flag()
        {
            _gridCells[_selectionX, _selectionY].Flag();
        }
    }

    enum CellType
    {
        Bomb,
        BombHidden,
        Empty
    }

    enum NavDirection
    {
        Up,
        Down,
        Left,
        Right
    }
}
