using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace GunFight
{
    class GunFight
    {
        class Point
        {
            public int col;
            public int row;
        }
        const int WindowWidth = 50;
        const int WindowHeight = 15;
        const int ScreenUpperBorder = 2;
        const int ScreenLowerBorder = WindowHeight - 2;
        static Random randGenerator = new Random();
        static int CollisionAOE = 1;
        static bool IsGameOver = false;
        static int scoreCnt = 0;
        static int scoreRow = 0;
        static int scoreCol = 0;
        static int liveCnt = 5;
        static int liveRow = 1;
        static int liveCol = 0;
        static int specialBulletCnt = 3;
        static int specialBulletRow = 0;
        static int specialBulletCol = WindowWidth-19;
        /*Player Info*/
        static int playerRow = WindowHeight / 2;
        static int playerCol = 0;
        static string playerFigure = "{@}";
        static ConsoleColor playerColor = ConsoleColor.White;
        /*Enemy Info*/
        static List<Point> enemies = new List<Point>();
        static string enemiesFigure = "$";
        static ConsoleColor enemiesColor = ConsoleColor.Red;
        static int EnemySpawnChance = 6;
        static int EnemyStartOffset = 50;
        static List<Point> enemies1 = new List<Point>();
        static string enemies1Figure = "#";
        static ConsoleColor enemies1Color = ConsoleColor.Yellow;
        static int Enemy1SpawnChance = 5;
        static int Enemy1StartOffset = 50;
        /*Bullet Info*/
        static List<Point> bullets = new List<Point>();
        static string bulletsFigure = "-";
        static ConsoleColor bulletsColor = ConsoleColor.White;
        static List<Point> bullets1 = new List<Point>();
        static string bullets1Figure = "*";
        static ConsoleColor bullets1Color = ConsoleColor.DarkMagenta;
        static void Main(string[] args)
        {
            while (!IsGameOver)
            {
                Clear();
                Update();
                CheckCollisions();
                Draw();

                Thread.Sleep(100);
            }
        }
        #region Utilyti Methods
        static void PrintOnPosition(int row, int col, string text, ConsoleColor color)
        {
            Console.SetCursorPosition(col, row);
            Console.ForegroundColor = color;
            Console.Write(text);
        }
        static void InitialiseSettings()
        {
            Console.WindowHeight = WindowHeight;
            Console.WindowWidth = WindowWidth;
            Console.BufferHeight = WindowHeight;
            Console.BufferWidth = WindowWidth;
            Console.CursorVisible = false;
        }
        static void Statistic()
        {
            PrintOnPosition(scoreRow, scoreCol, $"Score : {scoreCnt}", playerColor);
            PrintOnPosition(liveRow, liveCol, $"Lives : {liveCnt}", playerColor);
            PrintOnPosition(specialBulletRow, specialBulletCol, $"Special Bullet : {specialBulletCnt}", playerColor);
        }
        static bool IsObjectInBounds(int row, int col)
        {
            return row >= ScreenUpperBorder
                    && row <= ScreenLowerBorder
                    && col >= 0
                    && col <= WindowWidth - 1;
        }
        static bool DoObjectsCollide(int firstRow, int firstCol, int secondRow, int secondCol)
        {
            int colOffset = Math.Abs(firstCol - secondCol);
            return firstRow == secondRow && colOffset <= CollisionAOE;
        }
        static void ReadInput()
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo userInput = Console.ReadKey();
                while (Console.KeyAvailable)
                {
                    Console.ReadKey(true);
                }
                if (userInput.Key == ConsoleKey.LeftArrow && playerCol > 0)
                {
                    playerCol--;
                }
                else if (userInput.Key == ConsoleKey.RightArrow && playerCol < WindowWidth - 3)
                {
                    playerCol++;
                }
                else if (userInput.Key == ConsoleKey.UpArrow && playerRow > ScreenUpperBorder)
                {
                    playerRow--;
                }
                else if (userInput.Key == ConsoleKey.DownArrow && playerRow < ScreenLowerBorder)
                {
                    playerRow++;
                }
                else if (userInput.Key == ConsoleKey.Spacebar)
                {
                    bullets.Add(new Point()
                    {
                        col = playerCol + 2,
                        row = playerRow
                    });
                }
                else if (userInput.Key == ConsoleKey.Enter)
                {
                    if (specialBulletCnt>0)
                    {
                        bullets1.Add(new Point()
                        {
                            col = playerCol + 2,
                            row = playerRow
                        });
                    }
                    }
            }
        }
        #endregion
        #region Player
        static void ClearPlayer()
        {
            PrintOnPosition(playerRow, playerCol, "   ", playerColor);
        }
        static void DrawPlayer()
        {
            PrintOnPosition(playerRow, playerCol, playerFigure, playerColor);
        }
        static void UpdatePlayer()
        {
            ReadInput();
        }
        #endregion
        #region ClearMethods
        static void ClearEnemies()
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                if (IsObjectInBounds(enemies[i].row, enemies[i].col))
                {
                    PrintOnPosition(enemies[i].row, enemies[i].col, "  ", enemiesColor);
                }
            }
            for (int i = 0; i < enemies1.Count; i++)
            {
                if (IsObjectInBounds(enemies1[i].row, enemies1[i].col))
                {
                    PrintOnPosition(enemies1[i].row, enemies1[i].col, "  ", enemies1Color);
                }
            }
        }
        static void ClearBulets()
        {
            for (int i = 0; i < bullets.Count; i++)
            {
                if (IsObjectInBounds(bullets[i].row, bullets[i].col))
                {
                    PrintOnPosition(bullets[i].row, bullets[i].col, " ", bulletsColor);
                }
            }
            for (int i = 0; i < bullets1.Count; i++)
            {
                if (IsObjectInBounds(bullets1[i].row, bullets1[i].col))
                {
                    PrintOnPosition(bullets1[i].row, bullets1[i].col, " ", bullets1Color);
                }
            }
        }
        #endregion
        #region Enemies
        static void DrawEnemies()
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                if (IsObjectInBounds(enemies[i].row, enemies[i].col))
                {
                    PrintOnPosition(enemies[i].row, enemies[i].col, enemiesFigure, enemiesColor);
                }
            }
            //for (int i = 0; i < enemies1.Count; i++)
            //{
            //    if (IsObjectInBounds(enemies1[i].row, enemies1[i].col))
            //    {
            //        PrintOnPosition(enemies1[i].row, enemies1[i].col, enemies1Figure, enemies1Color);
            //    }
            //}
        }
        static void UpdateEnemies()
        {
            SpawnEnemies();
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].col--;
                if (enemies[i].col < 0)
                {
                    liveCnt--;
                    enemies.RemoveAt(i);
                    i--;
                }
            }
            //SpawnEnemies1();
            //for (int i = 0; i < enemies1.Count; i++)
            //{
            //    enemies1[i].col--;
            //    if (enemies1[i].col < 0)
            //    {
            //        liveCnt--;
            //        enemies1.RemoveAt(i);
            //        i--;
            //    }
            //}
        }
        static void SpawnEnemies()
        {
            int chance = randGenerator.Next(100);
            if (chance < EnemySpawnChance)
            {
                int row = randGenerator.Next(ScreenUpperBorder, ScreenLowerBorder);
                int col = randGenerator.Next(0, WindowWidth) + EnemyStartOffset;
                enemies.Add(new Point()
                {
                    row = row,
                    col = col
                });
            }
        }
        //static void SpawnEnemies1()
        //{
        //    int chance = randGenerator.Next(100);
        //    if (chance < Enemy1SpawnChance)
        //    {
        //        int row = randGenerator.Next(ScreenUpperBorder, ScreenLowerBorder);
        //        int col = randGenerator.Next(0, WindowWidth) + Enemy1StartOffset;
        //        enemies1.Add(new Point()
        //        {
        //            row = row,
        //            col = col
        //        });
        //    }
        //}
        #endregion
        #region Bullets
        static void DrawBullets()
        {
            for (int i = 0; i < bullets.Count; i++)
            {
                if (IsObjectInBounds(bullets[i].row, bullets[i].col))
                {
                    PrintOnPosition(bullets[i].row, bullets[i].col, bulletsFigure, bulletsColor);
                }
            }
            for (int i = 0; i < bullets1.Count; i++)
            {
                if (IsObjectInBounds(bullets1[i].row, bullets1[i].col))
                {
                    PrintOnPosition(bullets1[i].row, bullets1[i].col, bullets1Figure, bullets1Color);
                }
            }
        }
        static void UpdateBullets()
        {
            for (int i = 0; i < bullets.Count; i++)
            {
                bullets[i].col++;
                if (bullets[i].col > WindowWidth - 1)
                {
                    bullets.RemoveAt(i);
                    i--;
                }
            }
                for (int i = 0; i < bullets1.Count; i++)
                {
                    bullets1[i].col++;
                    if (bullets1[i].col > WindowWidth - 1)
                    {
                        bullets1.RemoveAt(i);
                        i--;
                    }
                }
        }
        #endregion
        #region Collision Methods
        static void CheckEnemiesBulletsCollision()
        {
            for (int bulletIndex = 0; bulletIndex < bullets.Count; bulletIndex++)
            {
                for (int enemyIndex = 0; enemyIndex < enemies.Count; enemyIndex++)
                {
                    if (DoObjectsCollide(
                                        bullets[bulletIndex].row,
                                        bullets[bulletIndex].col - 1,
                                        enemies[enemyIndex].row,
                                        enemies[enemyIndex].col))
                    {
                        bullets.RemoveAt(bulletIndex);
                        enemies.RemoveAt(enemyIndex);
                        bulletIndex--;
                        enemyIndex--;
                        scoreCnt++;
                        break;
                    }
                    //for (int enemy1Index = 0; enemy1Index < enemies1.Count; enemy1Index++)
                    //{
                    //    if (DoObjectsCollide(
                    //                    bullets[bulletIndex].row,
                    //                    bullets[bulletIndex].col - 1,
                    //                    enemies1[enemy1Index].row,
                    //                    enemies1[enemy1Index].col))
                    //    {

                    //        bullets.RemoveAt(bulletIndex);
                    //        enemies1.RemoveAt(enemy1Index);
                    //        bulletIndex--;
                    //        enemy1Index--;
                    //        scoreCnt+=3;


                    //    }
                    //}
              }
            }
            for (int bullet1Index = 0; bullet1Index < bullets1.Count; bullet1Index++)
            {
                for (int enemyIndex = 0; enemyIndex < enemies.Count; enemyIndex++)
                {
                    if (DoObjectsCollide(
                        bullets1[bullet1Index].row,
                        bullets1[bullet1Index].col - 1,
                        enemies[enemyIndex].row,
                        enemies[enemyIndex].col))
                    {
                        bullets1.RemoveAt(bullet1Index);
                        enemies.RemoveRange(0,enemies.Count);
                        bullet1Index--;
                        enemyIndex--;
                        scoreCnt++;
                        specialBulletCnt--;
                        break;
                    }
                }
            }
        }
        static void CheckEnemiesPlayerCollision()
        {
            for (int enemyIndex = 0; enemyIndex < enemies.Count; enemyIndex++)
            {
                if (DoObjectsCollide(
                    enemies[enemyIndex].row,
                    enemies[enemyIndex].col,
                    playerRow,
                    playerCol + 1)
                    )
                {
                    enemies.RemoveAt(enemyIndex);
                    liveCnt--;
                }
                if (liveCnt == 0)
                {
                    IsGameOver = true;
                }
            }
            }
        #endregion
        static void CheckCollisions()
        {
                CheckEnemiesBulletsCollision();
                CheckEnemiesPlayerCollision();
            }
        static void Clear()
        {
                ClearPlayer();
                ClearEnemies();
                ClearBulets();
            }
        static void Draw()
        {
                Statistic();
                DrawPlayer();
                DrawEnemies();
                DrawBullets();
            }
        static void Update()
        {
                UpdatePlayer();
                UpdateEnemies();
                UpdateBullets();
            }
        }
    }
