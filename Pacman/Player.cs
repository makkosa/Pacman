using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace Pacman
{
    class Player
    {
        public Bitmap Image { get; set; }
        public int Health { get; set; }
        public Direction Direction { get; set; }
        public Direction NextDirection { get; set; }
        public Point Location { get; set; }
        public int Score { get; set; }
        public Stopwatch FrameStopwatch = new Stopwatch();
        public Stopwatch CollisionFrameStopwatch = new Stopwatch();
        public int KillStreak { get; set; }

        public static bool IsMoving { get; set; }

        private const int SPEED = 4;

        private Bitmap[] imageFrames = new Bitmap[8];
        private Bitmap[] collisionImage = new Bitmap[21];
        private Bitmap roundPacman = new Bitmap(@"Images\Pacman\Round.png");

        public Player(Point startLocation, Direction startDirection)
        {
            imageFrames[0] = new Bitmap(@"Images\Pacman\DownOpen.png");
            imageFrames[1] = new Bitmap(@"Images\Pacman\UpOpen.png");
            imageFrames[2] = new Bitmap(@"Images\Pacman\RightOpen.png");
            imageFrames[3] = new Bitmap(@"Images\Pacman\LeftOpen.png");
            imageFrames[4] = new Bitmap(@"Images\Pacman\DownSemiOpen.png");
            imageFrames[5] = new Bitmap(@"Images\Pacman\UpSemiOpen.png");
            imageFrames[6] = new Bitmap(@"Images\Pacman\RightSemiOpen.png");
            imageFrames[7] = new Bitmap(@"Images\Pacman\LeftSemiOpen.png");

            for (int i = 0; i < collisionImage.Length; i++)
            {
                collisionImage[i] = new Bitmap(@"Images\PacmanCollision\" + (i + 1) + ".png");
            }

            Direction = startDirection;
            NextDirection = startDirection;
            Health = 3;
            Score = 0;
            KillStreak = 1;

            Image = GetImage(Direction);
            Location = startLocation;

            FrameStopwatch = Stopwatch.StartNew();
        }

        //  Метод возвращает изображение в зависимости от направления
        public Bitmap GetImage(Direction direction)
        {
            switch (direction)
            {
                default:
                case Direction.Down:
                    return imageFrames[0];
                case Direction.Up:
                    return imageFrames[1];
                case Direction.Right:
                    return imageFrames[2];
                case Direction.Left:
                    return imageFrames[3];
            }
        }

        public Bitmap GetIntermediateImage(Direction direction)
        {
            switch (direction)
            {
                default:
                case Direction.Down:
                    return imageFrames[4];
                case Direction.Up:
                    return imageFrames[5];
                case Direction.Right:
                    return imageFrames[6];
                case Direction.Left:
                    return imageFrames[7];
            }
        }

        // Метод, который изменяет положение объекта и его изображение в звисимости от направления
        public void Move(Map map)
        {
            for (int i = 0; i < SPEED; i++)
            {
                // Проверяем, доступен ли нам поворот, если доступен, меняем напрвление движения
                if (Direction != NextDirection && Line.IsPathOpen(Location, map.Grid, NextDirection))
                {
                    Direction = NextDirection;
                }
                Image = GetImage(Direction);

                // Проверяем, доступен ли нам путь в котором сейчас движется пакман
                if (Line.IsPathOpen(Location, map.Grid, Direction))
                {
                    switch (Direction)
                    {
                        default:
                        case Direction.Down:
                            Location = new Point(Location.X, Location.Y + 1);
                            break;
                        case Direction.Up:
                            Location = new Point(Location.X, Location.Y - 1);
                            break;
                        case Direction.Right:
                            Location = new Point(Location.X + 1, Location.Y);
                            break;
                        case Direction.Left:
                            Location = new Point(Location.X - 1, Location.Y);
                            break;
                    }

                    if (FrameStopwatch.ElapsedMilliseconds > 100 && FrameStopwatch.ElapsedMilliseconds < 151)
                        Image = GetIntermediateImage(Direction);
                    else if (FrameStopwatch.ElapsedMilliseconds > 150 && FrameStopwatch.ElapsedMilliseconds < 191)
                    {
                        Image = roundPacman;
                    }
                    else if (FrameStopwatch.ElapsedMilliseconds > 190 && FrameStopwatch.ElapsedMilliseconds < 231)
                        Image = GetIntermediateImage(Direction);
                    else if (FrameStopwatch.ElapsedMilliseconds > 230)
                    {
                        FrameStopwatch.Reset();
                        FrameStopwatch.Start();
                    }
                    else Image = GetImage(Direction);
                }
                else
                {
                    return;
                }

                // Перебираем точки в заданном радусе вокруг пакмана, проверяем содержиться ли точка в списке еды,
                // если содержится, то удаляем ее из списка.
                int foodOffset = 13;
                for (int e = -foodOffset; e <= foodOffset; e++)
                {
                    Point foodHorizontalLocation = new Point(Location.X + e, Location.Y);
                    Point foodVerticalLocation = new Point(Location.X, Location.Y + e);
                    int foodHorizontalIndex = map.Food.IndexOf(foodHorizontalLocation);
                    int foodVerticalIndex = map.Food.IndexOf(foodVerticalLocation);
                    if (foodHorizontalIndex >= 0)
                    {
                        Score++;
                        map.Food.RemoveAt(foodHorizontalIndex);
                        Sound.GetChompSound();
                    }
                    if (foodVerticalIndex >= 0)
                    {
                        Score++;
                        map.Food.RemoveAt(foodVerticalIndex);
                        Sound.GetChompSound();
                    }
                }

                if (Location == map.TeleportationPoints[0]) Location = map.TeleportationPoints[1];
                else if (Location == map.TeleportationPoints[1]) Location = map.TeleportationPoints[0];
            }
        }

        public Bitmap GetCollisionImage(Stopwatch sw)
        {
            int frameTime = 57;
            int frameNumber = (int)(sw.ElapsedMilliseconds / frameTime);
            if (frameNumber < collisionImage.Length)
            {
                return collisionImage[0 + frameNumber];
            }
            else return collisionImage[20];
        }
    }
}
