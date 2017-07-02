using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pacman
{
    class Fruit
    {
        public Bitmap Image = new Bitmap(@"Images\Fruit\Fruit.png");
        public Point Location = new Point();

        public bool IsFruitExist { get; set; }
        public bool IsFruitExpected { get; set; } = true;
        public bool IsFruitEated { get; set; } = false;

        public Stopwatch EmergenceStopwatch = new Stopwatch();
        public Stopwatch ScoreStopwatch = new Stopwatch();
        public int RandomEmergenceTime;
        public int ExistTime = 7000;
        public int FruitScore = 500;

        private Random random = new Random();
        private const int MIN_TIME_EMERGENCE = 13000;
        private const int MAX_TIME_EMERGENCE = 40000;

        public Fruit()
        {
            EmergenceStopwatch.Start();
            IsFruitExist = false;
            RandomEmergenceTime = random.Next(MIN_TIME_EMERGENCE, MAX_TIME_EMERGENCE);
        }

        public void LoadFruit()
        {
            if (IsFruitExpected && EmergenceStopwatch.ElapsedMilliseconds >= RandomEmergenceTime)
            {
                IsFruitExpected = false;
                IsFruitExist = true;
                Location = new Point(281, 413);
            }
        }
    }
}
