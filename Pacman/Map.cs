using System;
using System.Collections.Generic;
using System.Drawing;

namespace Pacman
{
    class Map
    {
        public Bitmap Image { get; set; }
        public List<Line> Grid { get; set; }
        public List<Point> Food { get; set; }
        public List<Point> Cookies { get; set; }
        public Point PlayerStartLocation { get; set; }
        public Point[] TeleportationPoints { get; set; } = new Point[2];

        public Point[] GhostsStartLocations { get; set; } = new Point[4];

        public static Map GetMap()
        {
            Map map = new Map();
            map.Image = new Bitmap(@"Images\Map.png");
            map.PlayerStartLocation = new Point(281, 413);
            map.Grid = new List<Line>();
            map.TeleportationPoints[0] = new Point(12, 258);
            map.TeleportationPoints[1] = new Point(548, 258);

            map.GhostsStartLocations[0] = new Point(192, 207);
            map.GhostsStartLocations[1] = new Point(251, 205);
            map.GhostsStartLocations[2] = new Point(310, 205);
            map.GhostsStartLocations[3] = new Point(369, 207);

            // Horizontal
            map.Grid.Add(new Line(new Point(35, 33), new Point(133, 33)));
            map.Grid.Add(new Line(new Point(133, 33), new Point(250, 33))); 

            map.Grid.Add(new Line(new Point(310, 33), new Point(428, 33)));
            map.Grid.Add(new Line(new Point(428, 33), new Point(527, 33))); 

            map.Grid.Add(new Line(new Point(35, 154), new Point(133, 154)));

            map.Grid.Add(new Line(new Point(428, 154), new Point(527, 154)));

            map.Grid.Add(new Line(new Point(35, 102), new Point(133, 102)));
            map.Grid.Add(new Line(new Point(133, 102), new Point(192, 102)));
            map.Grid.Add(new Line(new Point(192, 102), new Point(250, 102)));
            map.Grid.Add(new Line(new Point(250, 102), new Point(310, 102)));
            map.Grid.Add(new Line(new Point(310, 102), new Point(369, 102)));
            map.Grid.Add(new Line(new Point(369, 102), new Point(428, 102)));
            map.Grid.Add(new Line(new Point(428, 102), new Point(527, 102)));

            map.Grid.Add(new Line(new Point(192, 154), new Point(251, 154)));
            map.Grid.Add(new Line(new Point(310, 154), new Point(369, 154)));

            map.Grid.Add(new Line(new Point(192, 206), new Point(251, 206)));
            map.Grid.Add(new Line(new Point(251, 206), new Point(310, 206)));
            map.Grid.Add(new Line(new Point(310, 206), new Point(369, 206)));

            map.Grid.Add(new Line(new Point(192, 310), new Point(369, 310)));

            map.Grid.Add(new Line(new Point(12, 258), new Point(133, 258)));
            map.Grid.Add(new Line(new Point(133, 258), new Point(192, 258)));

            map.Grid.Add(new Line(new Point(34, 361), new Point(133, 361)));
            map.Grid.Add(new Line(new Point(133, 361), new Point(192, 361)));
            map.Grid.Add(new Line(new Point(192, 361), new Point(251, 361)));

            map.Grid.Add(new Line(new Point(369, 258), new Point(428, 258)));
            map.Grid.Add(new Line(new Point(428, 258), new Point(548, 258)));

            map.Grid.Add(new Line(new Point(310, 361), new Point(369, 361)));
            map.Grid.Add(new Line(new Point(369, 361), new Point(428, 361)));
            map.Grid.Add(new Line(new Point(428, 361), new Point(526, 361)));

            map.Grid.Add(new Line(new Point(34, 413), new Point(73, 413)));

            map.Grid.Add(new Line(new Point(34, 465), new Point(73, 465)));
            map.Grid.Add(new Line(new Point(73, 465), new Point(133, 465))); 

            map.Grid.Add(new Line(new Point(34, 516), new Point(251, 516)));
            map.Grid.Add(new Line(new Point(251, 516), new Point(310, 516)));
            map.Grid.Add(new Line(new Point(310, 516), new Point(526, 516)));

            map.Grid.Add(new Line(new Point(133, 413), new Point(192, 413)));
            map.Grid.Add(new Line(new Point(192, 413), new Point(251, 413)));
            map.Grid.Add(new Line(new Point(251, 413), new Point(310, 413)));
            map.Grid.Add(new Line(new Point(310, 413), new Point(369, 413)));
            map.Grid.Add(new Line(new Point(369, 413), new Point(428, 413)));

            map.Grid.Add(new Line(new Point(488, 413), new Point(526, 413)));

            map.Grid.Add(new Line(new Point(428, 465), new Point(488, 465)));
            map.Grid.Add(new Line(new Point(488, 465), new Point(526, 465)));

            map.Grid.Add(new Line(new Point(310, 465), new Point(369, 465)));
            map.Grid.Add(new Line(new Point(192, 465), new Point(251, 465)));

            // Vertical
            map.Grid.Add(new Line(new Point(35, 33), new Point(35, 102)));
            map.Grid.Add(new Line(new Point(35, 102), new Point(35, 154))); 

            map.Grid.Add(new Line(new Point(133, 33), new Point(133, 102)));
            map.Grid.Add(new Line(new Point(133, 102), new Point(133, 154)));
            map.Grid.Add(new Line(new Point(133, 154), new Point(133, 258)));
            map.Grid.Add(new Line(new Point(133, 258), new Point(133, 361)));
            map.Grid.Add(new Line(new Point(133, 361), new Point(133, 413)));
            map.Grid.Add(new Line(new Point(133, 413), new Point(133, 465)));

            map.Grid.Add(new Line(new Point(192, 102), new Point(192, 154)));
            map.Grid.Add(new Line(new Point(369, 102), new Point(369, 154)));

            map.Grid.Add(new Line(new Point(251, 154), new Point(251, 206)));
            map.Grid.Add(new Line(new Point(310, 154), new Point(310, 206)));

            map.Grid.Add(new Line(new Point(192, 206), new Point(192, 258)));
            map.Grid.Add(new Line(new Point(192, 258), new Point(192, 310)));
            map.Grid.Add(new Line(new Point(192, 310), new Point(192, 361)));

            map.Grid.Add(new Line(new Point(369, 206), new Point(369, 258)));
            map.Grid.Add(new Line(new Point(369, 258), new Point(369, 310)));
            map.Grid.Add(new Line(new Point(369, 310), new Point(369, 361)));

            map.Grid.Add(new Line(new Point(428, 33), new Point(428, 102)));
            map.Grid.Add(new Line(new Point(428, 102), new Point(428, 154)));
            map.Grid.Add(new Line(new Point(428, 154), new Point(428, 258)));
            map.Grid.Add(new Line(new Point(428, 258), new Point(428, 361)));
            map.Grid.Add(new Line(new Point(428, 361), new Point(428, 413)));
            map.Grid.Add(new Line(new Point(428, 413), new Point(428, 465)));

            map.Grid.Add(new Line(new Point(527, 33), new Point(527, 102)));
            map.Grid.Add(new Line(new Point(527, 102), new Point(527, 154)));

            map.Grid.Add(new Line(new Point(250, 33), new Point(250, 102)));
            map.Grid.Add(new Line(new Point(310, 33), new Point(310, 102)));

            map.Grid.Add(new Line(new Point(34, 361), new Point(34, 413)));
            map.Grid.Add(new Line(new Point(73, 413), new Point(73, 465)));
            map.Grid.Add(new Line(new Point(34, 465), new Point(34, 516)));

            map.Grid.Add(new Line(new Point(251, 361), new Point(251, 413)));
            map.Grid.Add(new Line(new Point(310, 361), new Point(310, 413)));

            map.Grid.Add(new Line(new Point(192, 413), new Point(192, 465)));
            map.Grid.Add(new Line(new Point(369, 413), new Point(369, 465)));

            map.Grid.Add(new Line(new Point(526, 361), new Point(526, 413)));
            map.Grid.Add(new Line(new Point(488, 413), new Point(488, 465)));
            map.Grid.Add(new Line(new Point(526, 465), new Point(526, 516)));

            map.Grid.Add(new Line(new Point(310, 465), new Point(310, 516)));
            map.Grid.Add(new Line(new Point(251, 465), new Point(251, 516)));

            map.Food = Pacman.Food.Generate(map.Grid);
            map.Cookies = Pacman.Cookies.Get();

            return map;
        }
    }
}
