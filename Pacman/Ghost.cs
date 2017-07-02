using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Diagnostics;

namespace Pacman
{
    class Ghost
    {
        public Bitmap Image { get; set; }
        public Type Color { get; set; }
        public Direction Direction { get; set; }
        public Point Location { get; set; }
        public bool IsMovementAllowed { get; set; }
        public Stopwatch RestartTimer { get; set; } = new Stopwatch();

        public bool IsRunningAway
        {
            get
            {
                if (scaredTimer.ElapsedMilliseconds > 0 && scaredTimer.ElapsedMilliseconds < SCARED_TIME) return true;
                else return false;
            }
        }
        public int Speed
        {
            get
            {
                if (IsRunningAway) return SLOW_SPEED;
                else return NORMAL_SPEED;
            }
        }

        public const int SCARED_TIME = 5000;
        public Stopwatch scaredTimer = new Stopwatch();

        private const int NORMAL_SPEED = 3;
        private const int SLOW_SPEED = 2;
        private const int BLINK_TIME = 2000;

        private Bitmap[] normalImageFrames = new Bitmap[4];
        private Bitmap[] scaredImageFrames = new Bitmap[2];
        private Random random = new Random();

        private Point restartLocation = new Point(281, 260);

        public Ghost(Point startLocation, Direction startDirection, Type color)
        {
            switch (color)
            {
                default:
                case Type.Blue:
                    normalImageFrames[0] = new Bitmap(@"Images\Ghost\BlueLeft.png");
                    normalImageFrames[1] = new Bitmap(@"Images\Ghost\BlueRight.png");
                    normalImageFrames[2] = new Bitmap(@"Images\Ghost\BlueDown.png");
                    normalImageFrames[3] = new Bitmap(@"Images\Ghost\BlueUp.png");
                    break;
                case Type.Pink:
                    normalImageFrames[0] = new Bitmap(@"Images\Ghost\PinkLeft.png");
                    normalImageFrames[1] = new Bitmap(@"Images\Ghost\PinkRight.png");
                    normalImageFrames[2] = new Bitmap(@"Images\Ghost\PinkDown.png");
                    normalImageFrames[3] = new Bitmap(@"Images\Ghost\PinkUp.png");
                    break;
                case Type.Red:
                    normalImageFrames[0] = new Bitmap(@"Images\Ghost\RedLeft.png");
                    normalImageFrames[1] = new Bitmap(@"Images\Ghost\RedRight.png");
                    normalImageFrames[2] = new Bitmap(@"Images\Ghost\RedDown.png");
                    normalImageFrames[3] = new Bitmap(@"Images\Ghost\RedUp.png");
                    break;
                case Type.Yellow:
                    normalImageFrames[0] = new Bitmap(@"Images\Ghost\YellowLeft.png");
                    normalImageFrames[1] = new Bitmap(@"Images\Ghost\YellowRight.png");
                    normalImageFrames[2] = new Bitmap(@"Images\Ghost\YellowDown.png");
                    normalImageFrames[3] = new Bitmap(@"Images\Ghost\YellowUp.png");
                    break;
            }

            scaredImageFrames[0] = new Bitmap(@"Images\Ghost\ScaredGhostBlue.png");
            scaredImageFrames[1] = new Bitmap(@"Images\Ghost\ScaredGhostWhite.png");

            Color = color;
            Direction = startDirection;
            Image = GetImage(Direction);
            Location = startLocation;
            IsMovementAllowed = true;
        }

        public void Move(Map map, Player player)
        {
            for (int i = 0; i < Speed; i++)
            {
                SortedList <Direction, Point> allowedDirection = new SortedList<Direction, Point>();

                if (Line.LinesContainPoint(map.Grid, Location))
                {
                    if (Direction == Direction.Left || Direction == Direction.Right)
                    {
                        if (Line.IsPathOpen(Location, map.Grid, Direction.Up))
                        {
                            allowedDirection.Add(Direction.Up, GetNextPoint(Location, Direction.Up));
                        }
                        if (Line.IsPathOpen(Location, map.Grid, Direction.Down))
                        {
                            allowedDirection.Add(Direction.Down, GetNextPoint(Location, Direction.Down));
                        }
                    }
                    else if (Direction == Direction.Up || Direction == Direction.Down)
                    {
                        if (Line.IsPathOpen(Location, map.Grid, Direction.Left))
                        {
                            allowedDirection.Add(Direction.Left, GetNextPoint(Location, Direction.Left));
                        }
                        if (Line.IsPathOpen(Location, map.Grid, Direction.Right))
                        {
                            allowedDirection.Add(Direction.Right, GetNextPoint(Location, Direction.Right));
                        }
                    }

                    if (Line.IsPathOpen(Location, map.Grid, Direction))
                    {
                        allowedDirection.Add(Direction, GetNextPoint(Location, Direction));
                    }
                }
                else
                {
                    allowedDirection.Add(Direction.Up, GetNextPoint(Location, Direction.Up));
                }

                if (allowedDirection.Count > 0)
                {
                    List<double> pathLengths = new List<double>();

                    foreach (var item in allowedDirection.Values)
                    {
                        double distance = Math.Sqrt(Math.Pow(item.X - player.Location.X, 2) +
                                                    Math.Pow(item.Y - player.Location.Y, 2));
                        pathLengths.Add(distance);
                    }
                    double c = pathLengths.Min();
                    int minPathIndex = pathLengths.IndexOf(c);

                    if (allowedDirection.Count > 1)
                    {
                        int randomIndex = random.Next(0, 100);
                        if (randomIndex < 77)
                        {
                            Direction = allowedDirection.Keys[minPathIndex];
                        }
                        else
                        {
                            allowedDirection.RemoveAt(minPathIndex);
                            randomIndex = random.Next(0, allowedDirection.Count);
                            Direction = allowedDirection.Keys[randomIndex];
                        }
                    }
                    else Direction = allowedDirection.Keys[minPathIndex];
                }

                if (IsMovementAllowed)
                {
                    Location = GetNextPoint(Location, Direction);
                }

                if (Location == map.TeleportationPoints[0]) Location = map.TeleportationPoints[1];
                else if (Location == map.TeleportationPoints[1]) Location = map.TeleportationPoints[0];
            }

            if (scaredTimer.ElapsedMilliseconds > SCARED_TIME)
            {
                scaredTimer.Reset();
                player.KillStreak = 1;
            }

            Image = GetImage(Direction);
        }

        private Point GetNextPoint(Point startPoint, Direction direction)
        {
            switch (direction)
            {
                default:
                case Direction.Down:
                    return new Point(startPoint.X, startPoint.Y + 1);
                case Direction.Up:
                    return new Point(startPoint.X, startPoint.Y - 1);
                case Direction.Right:
                    return new Point(startPoint.X + 1, startPoint.Y);
                case Direction.Left:
                    return new Point(startPoint.X - 1, startPoint.Y);
            }
        }

        public void RunAway()
        {
            scaredTimer = Stopwatch.StartNew();
        }

        public void Kill()
        {
            scaredTimer.Reset();
            RestartTimer = Stopwatch.StartNew();
            IsMovementAllowed = false;
            Location = restartLocation;
            Direction = Direction.Up;
        }

        public Bitmap GetImage(Direction direction)
        {
            if (!IsRunningAway)
            {
                switch (direction)
                {
                    default:
                    case Direction.Left:
                        return normalImageFrames[0];
                    case Direction.Right:
                        return normalImageFrames[1];
                    case Direction.Down:
                        return normalImageFrames[2];
                    case Direction.Up:
                        return normalImageFrames[3];
                }
            }
            else
            {
                if (scaredTimer.ElapsedMilliseconds < SCARED_TIME - BLINK_TIME) return scaredImageFrames[0];
                else if (scaredTimer.ElapsedMilliseconds < SCARED_TIME && scaredTimer.ElapsedMilliseconds % 1000 <= 500)
                         return scaredImageFrames[1];
                else if (scaredTimer.ElapsedMilliseconds < SCARED_TIME && scaredTimer.ElapsedMilliseconds % 1000 > 500)
                         return scaredImageFrames[0];
                else return GetImage(direction);
            }
        }

        public enum Type
        {
            Blue,
            Pink,
            Red,
            Yellow
        }
    }
}
