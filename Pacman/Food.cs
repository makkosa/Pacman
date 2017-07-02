using System;
using System.Collections.Generic;
using System.Drawing;

namespace Pacman
{
    class Food
    {
        public static List<Point> Generate(List<Line> grid)
        {
            List<Point> foodList = new List<Point>();

            foreach (var line in grid)
            {
                int remainder = 0;
                int dotNumber = 0;
                int minRemainder = 20;
                int finalInterval = 1;

                for (int i = 16; i <= 19; i++) // Нахожу интервал с минимальным остатком
                {
                    remainder = line.GetLength() % i; // Нахожу остаток от деления длины линии на интервал

                    if (minRemainder > remainder)
                    {
                        minRemainder = remainder;
                        finalInterval = i;
                    }
                }

                dotNumber = (line.GetLength() / finalInterval) + 1; // Нахожу количеств точек, которые надо расставить

                int offset = 0;
                for (int i = 0; i < dotNumber; i++)
                {
                    if (minRemainder > 0 && i > 0)
                    {
                        int x = (int)Math.Ceiling((double)((double)minRemainder / (double)(dotNumber - 1)));

                        for (int c = 0; c < x; c++)
                        {
                            if (minRemainder > 0)
                            {
                                offset++;
                                minRemainder--;
                            }
                        }
                    }

                    Point newFood = Point.Empty;
                    if (line.GetLineType() == Line.Type.Horizontal)
                    {
                        newFood = new Point((line.P1.X + offset + (finalInterval * i)), line.P1.Y);
                    }
                    else
                    {
                        newFood = new Point(line.P1.X, line.P1.Y + offset + (finalInterval * i));
                    }

                    if (foodList.IndexOf(newFood) < 0)
                    {
                        foodList.Add(newFood);
                    }
                }
            }
            return foodList;
        }
    }
}
