// http://www.icweb.es/programming/igp_console.php
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkanoid
{
    class MyGame
    {
        public struct brick
        {
            public byte x;
            public byte y;
            public byte numberOfHits;
            public ConsoleColor color;
            public bool destroyed;
            public byte score;
        }

        public struct ball
        {
            public byte x;
            public byte y;
            public sbyte xDirection;
            public sbyte yDirection;
            public sbyte xAngle;
        }

        // Constant to define bar width
        const int BAR_WIDTH = 5;

        // Function to draw borders 
        public static void PrintBorder()
        {
            // Print right panel
            Console.BackgroundColor = ConsoleColor.Gray;

            for (int i = 40; i < 79; i++)
            {
                Console.SetCursorPosition(i, 0);
                Console.WriteLine(" ");
                Console.SetCursorPosition(i, 23);
                Console.WriteLine(" ");
            }

            for (int i = 0; i < 24; i++)
            {
                Console.SetCursorPosition(40, i);
                Console.WriteLine(" ");
                Console.SetCursorPosition(79, i);
                Console.WriteLine(" ");
            }

            Console.ResetColor();
        }

        // Function to draw texts
        public static void PrintTexts()
        {
            // Print right texts
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(56, 4);
            Console.Write("ARKANOID");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.SetCursorPosition(42, 7);
            Console.Write("LIVES");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.SetCursorPosition(42, 12);
            Console.WriteLine("POINTS");

            Console.ResetColor();
        }

        public static brick[] CreateBricks()
        {
            brick[] bricks = new brick[40];

            for (int i = 0; i < bricks.Length; i++)
            {
                bricks[i].x = (byte) ((i % 10) * 4);
                bricks[i].y = (byte)(2 + (i / 10));
                bricks[i].destroyed = false;
                if (i >= 14 && i<= 15)
                {
                    bricks[i].numberOfHits = 2;
                    bricks[i].color = ConsoleColor.Yellow;
                    bricks[i].score = 50;
                }
                else if (i % 2 == 0 && (i/10 % 2 == 0) || i % 2 == 1 && (i/10 % 2 == 1))
                {
                    bricks[i].numberOfHits = 1;
                    bricks[i].color = ConsoleColor.Blue;
                    bricks[i].score = 20;
                }
                else
                {
                    bricks[i].numberOfHits = 1;
                    bricks[i].color = ConsoleColor.Red;
                    bricks[i].score = 10;
                }
            }

            return bricks;
        }

        public static void DrawBricks(brick[] bricks)
        {
            for (int i = 0; i < bricks.Length; i++)
            {
                Console.SetCursorPosition(bricks[i].x, bricks[i].y);
                Console.BackgroundColor = bricks[i].color;
                Console.Write("    ");
            }
        }

        public static ball InitBall()
        {
            ball gameBall;

            gameBall.x = 20;
            gameBall.y = 22;
            gameBall.xDirection = 0;
            gameBall.xAngle = 0;
            gameBall.yDirection = -1;

            return gameBall;
        }

        public static void GetUserInput ( ref int characterX, int characterY, ref bool exitGame)
        {
            ConsoleKeyInfo key;

            Console.SetCursorPosition(0, 24);

            if (Console.KeyAvailable)
            {
                do
                {
                    key = Console.ReadKey(false);
                }
                while (Console.KeyAvailable);

                Console.SetCursorPosition(characterX, characterY);
                Console.ResetColor();
                Console.Write("     ");

                if (key.Key == ConsoleKey.LeftArrow && characterX > 0)
                {
                    characterX--;
                }
                if (key.Key == ConsoleKey.RightArrow && characterX + BAR_WIDTH < 40)
                {
                    characterX++;
                }
                if (key.Key == ConsoleKey.Escape)
                {
                    exitGame = true;
                }
            }
        }

        public static void MoveBall(ref ball gameBall)
        {
            Console.ResetColor();
            Console.SetCursorPosition(gameBall.x, gameBall.y);
            Console.Write(" ");

            if (gameBall.xDirection < 0 && gameBall.xAngle == -2 && gameBall.x < 2)
            {
                gameBall.x -= 1;
            }
            else if (gameBall.xDirection > 0 && gameBall.xAngle == 2 && gameBall.x > 37)
            {
                gameBall.x += 1;
            }
            else
            {
                gameBall.x += (byte) gameBall.xAngle;
            }
            gameBall.y += (byte) gameBall.yDirection;
        }

        public static void CollisionBallBar(ref ball gameBall, int characterX, int characterY)
        {
            if (gameBall.y == 22 && gameBall.x >= characterX && gameBall.x <= characterX + BAR_WIDTH)
            {
                gameBall.yDirection = (sbyte)-gameBall.yDirection;
                switch (gameBall.x - characterX)
                {
                    case 0:
                        gameBall.xDirection = -1;
                        gameBall.xAngle = -2;
                        break;
                    case 1:
                        gameBall.xDirection = -1;
                        gameBall.xAngle = -1;
                        break;
                    case 2:
                        gameBall.xDirection = 0;
                        gameBall.xAngle = 0;
                        break;
                    case 3:
                        gameBall.xDirection = 1;
                        gameBall.xAngle = 1;
                        break;
                    case 4:
                        gameBall.xDirection = 1;
                        gameBall.xAngle = 2;
                        break;
                }
            }
        }

        public static void CollisionBallBricks(ref ball gameBall, brick[] bricks, ref int totalScore)
        {
            for (int i = 0; i < bricks.Length; i++)
            {
                if (!bricks[i].destroyed)
                {
                    // Lower and upper border
                    if ((gameBall.y == bricks[i].y + 1 && gameBall.yDirection < 0 ||
                        gameBall.y == bricks[i].y -1 && gameBall.yDirection > 0) && 
                        gameBall.x >= bricks[i].x && gameBall.x < bricks[i].x + 4)
                    {
                        bricks[i].numberOfHits--;
                        gameBall.yDirection = (sbyte)-gameBall.yDirection;
                    }
                    // Left and right border
                    if ((gameBall.x == bricks[i].x - 1 && gameBall.xDirection > 0 || 
                        gameBall.x == bricks[i].x + 4 && gameBall.xDirection < 0) && gameBall.y == bricks[i].y)
                    {
                        bricks[i].numberOfHits--;
                        gameBall.xDirection = (sbyte)-gameBall.xDirection;
                        gameBall.xAngle = (sbyte)-gameBall.xAngle;
                    }
                    // Check if brick must be destroyed, then destroy the brick
                    if (bricks[i].numberOfHits == 0)
                    {
                        bricks[i].destroyed = true;
                        totalScore += bricks[i].score;
                        PrintScore(totalScore);
                        // Remove brick from the scene
                        Console.BackgroundColor = ConsoleColor.Black;

                        Console.SetCursorPosition(bricks[i].x, bricks[i].y);
                        Console.Write("    ");
                        Console.ResetColor();
                    }
                }
            }
        }

        public static bool CollisionBallBoundaries(ref ball gameBall)
        {
            // Collision between the ball and left/right bounds
            if (gameBall.x == 0 || gameBall.x == 39)
            {
                gameBall.xDirection = (sbyte)-gameBall.xDirection;
                gameBall.xAngle = (sbyte)-gameBall.xAngle;
            }

            // Collision between the upper bound 
            if (gameBall.y == 0)
            {
                gameBall.yDirection = (sbyte)-gameBall.yDirection;
            }

            // Ball getting out of reach (player loses one life)
            return gameBall.y >= 23;
        }

        public static void PrintScore(int totalScore)
        {
            Console.SetCursorPosition(50, 12);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(totalScore);
            Console.ResetColor();
        }

        public static void PrintLives(int lives)
        {
            Console.SetCursorPosition(50, 7);
            for (int i = 0; i < 25; i++)
            {
                Console.Write(" ");
            }
            Console.BackgroundColor = ConsoleColor.White;
            for (int i = 0; i< lives; i++)
            {
                Console.SetCursorPosition(50 + i * (BAR_WIDTH + 2), 7);
                Console.Write("     ");
            }
            Console.ResetColor();
        }

        public static void ResetPositions(ref ball gameBall, ref int characterX, int characterY)
        {
            Console.SetCursorPosition(characterX, characterY);
            Console.Write("     ");
            Console.SetCursorPosition(gameBall.x, gameBall.y);
            Console.Write(" ");
            characterX = 18;
            gameBall = InitBall();
        }

        public static void WelcomeScreen()
        {
            Console.SetCursorPosition(36, 10);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("ARKANOID");

            Console.SetCursorPosition(28, 13);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Press a key to start...");

            Console.ResetColor();
            Console.ReadKey();
            Console.Clear();
        }

        public static void CreditsScreen()
        {
            Console.Clear();
       
            Console.SetCursorPosition(28, 10);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("<c> Andres Saez 2018");

            Console.SetCursorPosition(28, 13);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Press a key to exit...");

            Console.ResetColor();
     
        }

        static void Main(string[] args)
        {
            ball gameBall = InitBall();

            bool moveBall = true;

            int characterX = 18, characterY = 23;

            brick[] bricks = CreateBricks();

            bool exitGame = false;

            int totalScore = 0;
            int lives = 3;

          
            WelcomeScreen();
                  
            PrintBorder();
            PrintTexts();
            PrintLives(lives);
            PrintScore(totalScore);
            DrawBricks(bricks);

            // Game loop
            while (!exitGame)
            {
                moveBall = !moveBall;

                // 1 Draw elements

                // Print ball

                Console.SetCursorPosition(gameBall.x, gameBall.y);
                Console.Write("o");

                // Print main character

                Console.SetCursorPosition(characterX, characterY);
                Console.BackgroundColor = ConsoleColor.White;
                Console.Write("     ");
                Console.ResetColor();

                // 5 Pause

                System.Threading.Thread.Sleep(50);

                // 2 Read input and calculate player's new

                GetUserInput(ref characterX, characterY, ref exitGame);

                // 3 Move enemies and other objects

                // Move the ball

                if (moveBall)
                {
                    MoveBall(ref gameBall);
                }

                // Check collisions and update game state

                if (moveBall)
                {
                    CollisionBallBar(ref gameBall, characterX, characterY);
                    CollisionBallBricks(ref gameBall, bricks, ref totalScore);
                    if (CollisionBallBoundaries(ref gameBall))
                    {
                        lives--;
                        PrintLives(lives);
                        if (lives > 0)
                        {
                            ResetPositions(ref gameBall, ref characterX, characterY);
                        }
                        else
                        {
                            exitGame = true;
                        }
                    }
                }
            }

            CreditsScreen();

            // Place the cursor at the end of the console
            Console.SetCursorPosition(0, 23);
        }
    }
}
