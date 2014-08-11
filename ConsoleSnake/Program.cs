using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace ConsoleGames.ConsoleSnake
{
    class Position
    {
        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }
        public int X { get; set; }
        public int Y { get; set; }
    }

    enum SnakeDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    class Program
    {
        #region Constants

        const int CONSOLE_HEIGHT = 20;
        const int CONSOLE_WIDTH = 30;
        const int CONSOLE_PADDING = 2;

        const int MILLIS_PER_UPDATE = 100;

        const int INITIAL_SNAKE_LENGTH = 6;
        const int INITIAL_MILLIS_PER_MOVEMENT = 100;

        const char SNAKE_CHAR = '#';
        const ConsoleColor SNAKE_COLOR = ConsoleColor.Yellow;

        const char FOOD_CHAR = '+';
        const ConsoleColor FOOD_COLOR = ConsoleColor.Red;

        const char BORDER_CORNER_CHAR = '@';
        const char BORDER_HORIZONTAL_CHAR = '-';
        const char BORDER_VERTICAL_CHAR = '|';
        const ConsoleColor BORDER_COLOR = ConsoleColor.Gray;

        const ConsoleColor SCORE_COLOR = ConsoleColor.Magenta;

        #endregion

        static Queue<Position> Points;
        static SnakeDirection Direction;
        static float SnakeSpeed;

        static DateTime LastMovement;
        static ConsoleKey LastKeyPressed;

        static Position FoodLocation;
        static int Score;

        static bool isDead;

        public Program()
        {
        }

        static void Main(string[] args)
        {
            Initialise();

            while (true)
            {
                Update();
                Draw();
                System.Threading.Thread.Sleep(MILLIS_PER_UPDATE);
            }
        }

        static void Initialise()
        {
            Points = new Queue<Position>();
            SnakeSpeed = INITIAL_MILLIS_PER_MOVEMENT;
            LastMovement = DateTime.Now;
            Direction = SnakeDirection.Right;
            LastKeyPressed = ConsoleKey.RightArrow;

            // Set window size
            Console.SetWindowSize(CONSOLE_WIDTH + (CONSOLE_PADDING * 2), CONSOLE_HEIGHT + (CONSOLE_PADDING * 2));
            Console.SetBufferSize(CONSOLE_WIDTH + (CONSOLE_PADDING * 2), CONSOLE_HEIGHT + (CONSOLE_PADDING * 2));

            // Initialise the scoreboard
            Score = 0;
            isDead = false;

            // Create the snake
            int MidScreenWidth = CONSOLE_WIDTH / 2;
            int MidScreenHeight = CONSOLE_HEIGHT / 2;
            for (int i = 0; i < INITIAL_SNAKE_LENGTH; i++)
            {
                Points.Enqueue(new Position(MidScreenWidth + i, MidScreenHeight));
            }

            // Create the food
            SpawnFood();
        }

        static void Update()
        {
            if (isDead)
            {
                return;
            }

            // Read key input if its availible
            if (Console.KeyAvailable)
            {
                LastKeyPressed = Console.ReadKey(true).Key;
            }

            // Calculate whether movement is needed
            TimeSpan timeSinceLastMovement = DateTime.Now - LastMovement;
            if (timeSinceLastMovement.Milliseconds > SnakeSpeed)
            {
                // Change direction
                if (LastKeyPressed == ConsoleKey.DownArrow && Direction != SnakeDirection.Up) Direction = SnakeDirection.Down;
                if (LastKeyPressed == ConsoleKey.LeftArrow && Direction != SnakeDirection.Right) Direction = SnakeDirection.Left;
                if (LastKeyPressed == ConsoleKey.RightArrow && Direction != SnakeDirection.Left) Direction = SnakeDirection.Right;
                if (LastKeyPressed == ConsoleKey.UpArrow && Direction != SnakeDirection.Down) Direction = SnakeDirection.Up;

                // Create a position at the front of the snake
                Position Front = Points.Last();
                Position NextPoint = new Position(-1, -1);
                switch (Direction)
                {
                    case SnakeDirection.Down:
                        NextPoint = new Position(Front.X, Front.Y + 1);
                        break;
                    case SnakeDirection.Left:
                        NextPoint = new Position(Front.X - 1, Front.Y);
                        break;
                    case SnakeDirection.Right:
                        NextPoint = new Position(Front.X + 1, Front.Y);
                        break;
                    case SnakeDirection.Up:
                        NextPoint = new Position(Front.X, Front.Y - 1);
                        break;
                }

                // check against walls
                bool offSide = false;
                if (NextPoint.X < 0 ||
                    NextPoint.X >= CONSOLE_WIDTH ||
                    NextPoint.Y < 0 ||
                    NextPoint.Y >= CONSOLE_HEIGHT) offSide = true;

                // check for collisions
                bool collision = false;
                if (Points.ToList().Where(x => x.X == NextPoint.X && x.Y == NextPoint.Y).Count() > 0) collision = true;

                // check if dead
                if (offSide || collision)
                    isDead = true;

                // move forward
                Points.Enqueue(NextPoint);

                // If the snake isnt on food
                if (Points.ToList().Where(x => x.X == FoodLocation.X && x.Y == FoodLocation.Y).Count() == 0)
                {
                    // Remove a point at the end of the snake
                    Points.Dequeue();
                }
                else
                {
                    SnakeSpeed = SnakeSpeed - 2;
                    Score = Score + 1;
                    SpawnFood();
                }

                LastMovement = DateTime.Now;
            }
        }

        static void Draw()
        {
            // Clear Console
            Console.Clear();

            // Write the score
            Console.ForegroundColor = SCORE_COLOR;
            Console.SetCursorPosition(CONSOLE_PADDING, 0);
            Console.Write("SCORE: " + Score);

            DrawBorder();

            // Draw Snake
            foreach (Position Point in Points)
            {
                Console.ForegroundColor = SNAKE_COLOR;
                Console.SetCursorPosition(Point.X + CONSOLE_PADDING, Point.Y + CONSOLE_PADDING);
                Console.Write(SNAKE_CHAR);
            }

            // Draw Food
            Console.ForegroundColor = FOOD_COLOR;
            Console.SetCursorPosition(FoodLocation.X + CONSOLE_PADDING, FoodLocation.Y + CONSOLE_PADDING);
            Console.Write(FOOD_CHAR);

            if (isDead)
            {
                Console.SetCursorPosition(CONSOLE_WIDTH / 2, CONSOLE_HEIGHT / 2);
                Console.Write("DEAD");
            }
        }

        static void DrawBorder()
        {
            Console.ForegroundColor = BORDER_COLOR;

            // Top
            Console.SetCursorPosition(CONSOLE_PADDING - 1, CONSOLE_PADDING - 1);
            Console.Write(BORDER_CORNER_CHAR);
            for (int i = 0; i < CONSOLE_WIDTH; i++)
            {
                Console.Write(BORDER_HORIZONTAL_CHAR);
            }
            Console.Write(BORDER_CORNER_CHAR);

            // LeftWall
            for (int i = 0; i < CONSOLE_HEIGHT; i++)
            {
                Console.SetCursorPosition(CONSOLE_PADDING - 1, CONSOLE_PADDING + i);
                Console.Write(BORDER_VERTICAL_CHAR);
            }

            // RightWall
            for (int i = 0; i < CONSOLE_HEIGHT; i++)
            {
                Console.SetCursorPosition(CONSOLE_PADDING + CONSOLE_WIDTH, CONSOLE_PADDING + i);
                Console.Write(BORDER_VERTICAL_CHAR);
            }

            // Bottom
            Console.SetCursorPosition(CONSOLE_PADDING - 1, CONSOLE_PADDING + CONSOLE_HEIGHT);
            Console.Write(BORDER_CORNER_CHAR);
            for (int i = 0; i < CONSOLE_WIDTH; i++)
            {
                Console.Write(BORDER_HORIZONTAL_CHAR);
            }
            Console.Write(BORDER_CORNER_CHAR);
        }

        /// <summary>
        /// Spawns food in a valid position
        /// </summary>
        static void SpawnFood()
        {
            Random R = new Random(DateTime.Now.Millisecond);
            bool PositionFound = false;
            Position PotentialPosition = new Position(0, 0);

            while (!PositionFound)
            {
                // create a potential position
                PotentialPosition = new Position(R.Next(CONSOLE_WIDTH), R.Next(CONSOLE_HEIGHT));

                // check it isnt on the snake
                if (Points.ToList().Where(x => x.X == PotentialPosition.X && x.Y == PotentialPosition.Y).Count() == 0) PositionFound = true;
            }

            FoodLocation = PotentialPosition;
        }
    }
}
