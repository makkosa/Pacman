using System;
using System.Collections.Generic;
using System.Drawing;

namespace Pacman
{
    struct Line
    {
        public Point P1 { get; set; }
        public Point P2 { get; set; }

        public Line(Point p1, Point p2)
        {
            P1 = p1;
            P2 = p2;
        }

        public Line(int x1, int y1, int x2, int y2) : this(new Point(x1, y1), new Point(x2, y2)) { }

        // Метод проверяет содержится ли рассматриваемая точка в линии
        public bool ContainsPoint(Point point)
        {
            if (GetLineType() == Type.Vertical && P1.X == point.X &&
                point.Y <= Math.Max(P1.Y, P2.Y) && point.Y >= Math.Min(P1.Y, P2.Y))
            {
                return true;
            }
            else if (GetLineType() == Type.Horizontal && P1.Y == point.Y &&
                     point.X <= Math.Max(P1.X, P2.X) && point.X >= Math.Min(P1.X, P2.X))
            {
                return true;
            }
            return false;
        }

        // Метод проверяет содержится ли рассматриваемая точка в списке линий
        public static bool LinesContainPoint(List<Line> lines, Point point)
        {
            foreach (var line in lines)
            {
                if (line.ContainsPoint(point)) return true;
            }
            return false;
        }

        // Метод для нахождения длины линии
        public int GetLength()
        {
            int lineLength = 0;

            if (GetLineType() == Type.Horizontal) lineLength = P2.X - P1.X;
            else lineLength = P2.Y - P1.Y;

            return lineLength;
        }

        public List<Line> Divide(Point point)
        {
            List<Line> dividedLines = new List<Line>();

            if (ContainsPoint(point) && P1 != point && P2 != point)
            { 
                dividedLines.Add(new Line(P1, point));
                dividedLines.Add(new Line(point, P2));
            }

            return dividedLines;
        }

        // Находим точки пересечения линий
        public List<Point> GetIntersections(List<Line> lines)
        {
            List<Point> intersectPoints = new List<Point>();

            foreach (var line in lines)
            {
                if (GetLineType() == Type.Horizontal && line.GetLineType() == Type.Vertical)
                {
                    for (int i = 1; i < GetLength() - 1; i++)
                    {
                        if (LinesContainPoint(lines, new Point(P1.X + i, P1.Y + 1)) ||
                            LinesContainPoint(lines, new Point(P1.X + i, P1.Y - 1)))
                        {
                            intersectPoints.Add(new Point(P1.X + i, P1.Y));
                        }
                    }
                }
                else if (GetLineType() == Type.Vertical && line.GetLineType() == Type.Horizontal)
                {
                    for (int i = 1; i < GetLength() - 1; i++)
                    {
                        if (LinesContainPoint(lines, new Point(P1.X + 1, P1.Y + i)) ||
                            LinesContainPoint(lines, new Point(P1.X - 1, P1.Y + i)))
                        {
                            intersectPoints.Add(new Point(P1.X, P1.Y + i));
                        }
                    }
                }
            }
            return intersectPoints;
        }

        // Метод проверяет, доступно ли нам следующее направление для конкретного пикселя
        public static bool IsPathOpen(Point location, List<Line> grid, Direction direction)
        {
            switch (direction)
            {
                default:
                case Direction.Down:
                    if (Line.LinesContainPoint(grid, new Point(location.X, location.Y + 1))) return true;
                    return false;
                case Direction.Up:
                    if (Line.LinesContainPoint(grid, new Point(location.X, location.Y - 1))) return true;
                    return false;
                case Direction.Right:
                    if (Line.LinesContainPoint(grid, new Point(location.X + 1, location.Y))) return true;
                    return false;
                case Direction.Left:
                    if (Line.LinesContainPoint(grid, new Point(location.X - 1, location.Y))) return true;
                    return false;
            }
        }

        public Type GetLineType()
        {
            if (P1.X == P2.X) return Type.Vertical;
            else if (P1.Y == P2.Y) return Type.Horizontal;
            else return Type.None;
        }

        public enum Type
        {
            Horizontal,
            Vertical,
            None
        }
    }
}
