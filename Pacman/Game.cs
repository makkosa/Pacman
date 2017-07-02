using AxWMPLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace Pacman
{
    public partial class Game : Form
    {
        private const int WINDOW_WIDTH = 580;
        private const int WINDOW_HEIGHT = 619;

        private const int PERIOD = 29;

        private Bitmap frame;
        private Map map;
        private static bool paused = false;
        private static bool gameStarted = false;
        private static bool isMovementAllowed = true;
        private static bool isEntitiesMoving = true;

        private Player player;
        private Ghost[] ghosts = new Ghost[4];
        private Cookies cookies = new Cookies();
        private Fruit fruit;

        private static Point mapLocation = new Point(1, 30);
        private static Point scoreLocation = new Point(9, -4);
        private static Point healthLocation = new Point(375, -4);
        private static Point pauseLocation = new Point(227, 265);
        private static Point startButtonLocation = new Point(234, 265);

        private static Rectangle startButtonRect = new Rectangle(startButtonLocation.X + mapLocation.X,
                                 startButtonLocation.Y + mapLocation.Y, 110, 48);

        private PrivateFontCollection fontCollection = new PrivateFontCollection();
        private static List<Collision> Collisions = new List<Collision>();

        public Game()
        {
            InitializeComponent();

            LoadFonts();
            Sound.Load();

            Width = WINDOW_WIDTH;
            Height = WINDOW_HEIGHT;

            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);

            Paint += Game_Paint;
        }

        private void Game_Load(object sender, EventArgs e)
        {
            SetWorld();

            frame = new Bitmap(ClientSize.Width, ClientSize.Height);

            
            Timer gameTimer = new Timer();
            gameTimer.Interval = PERIOD;
            gameTimer.Tick += GameTick;
            gameTimer.Start();
        }

        // Метод, который отвечает за прорисовку формы
        private void Game_Paint(object sender, PaintEventArgs e)
        {
            if (frame != null)
            {
                e.Graphics.DrawImageUnscaled(frame, Point.Empty);
            }
        }

        private void SetWorld()
        {
            map = Map.GetMap();
            player = new Player(map.PlayerStartLocation, Direction.Left);
            fruit = new Fruit();
            ghosts[0] = new Ghost(map.GhostsStartLocations[0], Direction.Down, Ghost.Type.Red);
            ghosts[1] = new Ghost(map.GhostsStartLocations[1], Direction.Up, Ghost.Type.Blue);
            ghosts[2] = new Ghost(map.GhostsStartLocations[2], Direction.Up, Ghost.Type.Pink);
            ghosts[3] = new Ghost(map.GhostsStartLocations[3], Direction.Down, Ghost.Type.Yellow);
        }

        private void RefreshEntities()
        {
            player.Location = map.PlayerStartLocation;
            player.Direction = Direction.Left;
            player.NextDirection = player.Direction;
            player.Image = player.GetImage(player.Direction);

            ghosts[0] = new Ghost(map.GhostsStartLocations[0], Direction.Down, Ghost.Type.Red);
            ghosts[1] = new Ghost(map.GhostsStartLocations[1], Direction.Up, Ghost.Type.Blue);
            ghosts[2] = new Ghost(map.GhostsStartLocations[2], Direction.Up, Ghost.Type.Pink);
            ghosts[3] = new Ghost(map.GhostsStartLocations[3], Direction.Down, Ghost.Type.Yellow);
        }

        private void GameTick(object sender, EventArgs e)
        {
            bool isCollided = false;

            if (!paused && gameStarted && isMovementAllowed && isEntitiesMoving)
            {
                // Производим обновление координат объектов
                player.Move(map);

                for (int i = 0; i < ghosts.Length; i++)
                {
                    ghosts[i].Move(map, player);
                }

                CheckCookiesCollision();

                if (fruit.IsFruitExpected) fruit.LoadFruit();
                if (fruit.IsFruitExist) CheckFruitCollision();

                isCollided = CheckGhostsCollision();
                if (isCollided) player.Health--;
            }

            RenderFrame();

            Refresh();

            if (map.Food.Count == 0)
            {
                Sound.Siren.Ctlcontrols.stop();
                gameStarted = false;
            }

            if (isCollided)
            {
                isEntitiesMoving = false;
                player.CollisionFrameStopwatch.Start();

                if (fruit.IsFruitExist) fruit.IsFruitExist = false;

                if (player.Health == 0)
                {
                    gameStarted = false;
                }
            }

            if (!isEntitiesMoving && player.CollisionFrameStopwatch.ElapsedMilliseconds > 1300)
            {
                player.CollisionFrameStopwatch.Reset();
                RefreshEntities();
                isEntitiesMoving = true;
                System.Threading.Thread.Sleep(800);
            }
        }

        private bool CheckGhostsCollision()
        {
            for (int i = 0; i < ghosts.Length; i++)
            {
                int playerOffset = 14;
                for (int c = -playerOffset; c <= playerOffset; c++)
                {
                    if (ghosts[i].Location == new Point(player.Location.X + c, player.Location.Y) ||
                        ghosts[i].Location == new Point(player.Location.X, player.Location.Y + c))
                    {
                        Sound.Siren.Ctlcontrols.stop();

                        if (!ghosts[i].IsRunningAway)
                        {
                            Sound.Death.Ctlcontrols.play();
                            return true;
                        }
                        else
                        {
                            Sound.EatGhost.Ctlcontrols.play();
                            player.Score += 200 * player.KillStreak;
                            player.KillStreak++;

                            Collision col = new Collision()
                            {
                                Location = new Point(player.Location.X, player.Location.Y),
                                Stopwatch = Stopwatch.StartNew(),
                                Score = 200 * (player.KillStreak - 1)
                            };
                            Collisions.Add(col);

                            ghosts[i].Kill();

                            if (player.KillStreak > 4) player.KillStreak = 1;
                            break;
                        }
                    }
                }
                if (ghosts[i].RestartTimer.ElapsedMilliseconds > 2300)
                {
                    ghosts[i].IsMovementAllowed = true;
                    ghosts[i].RestartTimer.Reset();
                }
            }
            return false;
        }

        private void CheckCookiesCollision()
        {
            for (int i = 0; i < map.Cookies.Count; i++)
            {
                int playerOffset = 10;
                for (int c = -playerOffset; c <= playerOffset; c++)
                {
                    if (map.Cookies[i] == new Point(player.Location.X + c, player.Location.Y) ||
                        map.Cookies[i] == new Point(player.Location.X, player.Location.Y + c))
                    {
                        if (Sound.Intermission.playState == WMPLib.WMPPlayState.wmppsPlaying) {
                            Sound.Intermission.Ctlcontrols.stop();
                        }
                        Sound.Intermission.Ctlcontrols.play();

                        for (int e = 0; e < ghosts.Length; e++)
                        {
                            ghosts[e].RunAway();
                        }
                        map.Cookies.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        public void CheckFruitCollision()
        {
            if (fruit.EmergenceStopwatch.ElapsedMilliseconds > fruit.RandomEmergenceTime + fruit.ExistTime)
            {
                fruit.IsFruitExist = false;
            }
            else
            {
                int playerOffset = 10;
                for (int i = -playerOffset; i <= playerOffset; i++)
                {
                    if (fruit.Location == new Point(player.Location.X + i, player.Location.Y) ||
                        fruit.Location == new Point(player.Location.X, player.Location.Y + i))
                    {
                        Sound.EatFruit.Ctlcontrols.play();
                        player.Score += fruit.FruitScore;

                        fruit.IsFruitExist = false;
                        fruit.IsFruitEated = true;

                        fruit.EmergenceStopwatch.Stop();
                        fruit.ScoreStopwatch.Start();
                        break;
                    }
                }
            }
        }

        private void RenderFrame()
        {
            if (frame != null)
            {
                using (var g = Graphics.FromImage(frame))
                {
                    g.Clear(Color.Black);
                    g.CompositingMode = CompositingMode.SourceOver;
                    g.TextRenderingHint = TextRenderingHint.AntiAlias;
                    g.SmoothingMode = SmoothingMode.AntiAlias;

                    g.DrawImage(map.Image, mapLocation.X, mapLocation.Y, map.Image.Width, map.Image.Height);

                    int foodRadius = 2;
                    foreach (Point food in map.Food)
                    {
                        g.DrawEllipse(new Pen(Color.HotPink, 1), food.X - foodRadius + mapLocation.X,
                                                                 food.Y - foodRadius + mapLocation.Y,
                                                                 foodRadius * 2, foodRadius * 2);
                        g.FillEllipse(Brushes.HotPink, food.X - foodRadius + mapLocation.X,
                                                       food.Y - foodRadius + mapLocation.Y,
                                                       foodRadius * 2, foodRadius * 2);
                    }

                    float cookieRadius = cookies.GetRadius();
                    for (int i = 0; i < map.Cookies.Count; i++)
                    {
                        g.DrawEllipse(new Pen(Color.White, 1), map.Cookies[i].X - cookieRadius + mapLocation.X,
                                                               map.Cookies[i].Y - cookieRadius + mapLocation.Y,
                                                               cookieRadius * 2, cookieRadius * 2);
                        g.FillEllipse(Brushes.White, map.Cookies[i].X - cookieRadius + mapLocation.X,
                                                     map.Cookies[i].Y - cookieRadius + mapLocation.Y,
                                                     cookieRadius * 2, cookieRadius * 2);
                    }

                    if (fruit.IsFruitExist)
                    {
                        g.DrawImage(fruit.Image, fruit.Location.X + mapLocation.X - (fruit.Image.Width / 2),
                                    fruit.Location.Y + mapLocation.Y - (fruit.Image.Height / 2),
                                    fruit.Image.Width, fruit.Image.Height);
                    }

                    for (int i = 0; i < ghosts.Length; i++)
                    {
                        g.DrawImage(ghosts[i].Image, ghosts[i].Location.X + mapLocation.X - (ghosts[i].Image.Width / 2),
                                    ghosts[i].Location.Y + mapLocation.Y - (ghosts[i].Image.Height / 2),
                                    ghosts[i].Image.Width, ghosts[i].Image.Height);
                    }

                    if (isEntitiesMoving)
                    {
                        g.DrawImage(player.Image, player.Location.X + mapLocation.X - (player.Image.Width / 2),
                                              player.Location.Y + mapLocation.Y - (player.Image.Height / 2),
                                              player.Image.Width, player.Image.Height);
                    }

                    g.DrawString("score", new Font((FontFamily)fontCollection.Families[0], 19f), Brushes.White, scoreLocation);

                    g.DrawString(player.Score.ToString(), new Font((FontFamily)fontCollection.Families[0], 19f),
                                  Brushes.Yellow, scoreLocation.X + 91, scoreLocation.Y);

                    g.DrawString("lives", new Font((FontFamily)fontCollection.Families[0], 19f), Brushes.White, healthLocation);

                    for (int i = 0; i < player.Health; i++)
                    {
                        g.DrawImage(new Bitmap(@"Images\Pacman\RightOpen.png"), healthLocation.X + 85 + (32 * i),
                            healthLocation.Y + 11, 25, 25);
                    }

                    if (paused)
                    {
                        g.FillRectangle(new SolidBrush(Color.FromArgb(100, Color.Black)), mapLocation.X, mapLocation.Y + 4, 
                                        map.Image.Width, map.Image.Height);
                        g.DrawString("paused", new Font((FontFamily)fontCollection.Families[0], 19f), Brushes.White, pauseLocation);
                    }

                    if (!gameStarted)
                    {
                        Brush brush = Brushes.White;
                        float fontSize = 19f;
                        Size fontOffset = new Size(0, 0);
                        if (Cursor.Position.X >= Location.X + startButtonRect.X &&
                            Cursor.Position.X <= Location.X + startButtonRect.X + startButtonRect.Width &&
                            Cursor.Position.Y >= Location.Y + startButtonRect.Y &&
                            Cursor.Position.Y <= Location.Y + startButtonRect.Y + startButtonRect.Height)
                        {
                            brush = Brushes.Yellow;
                            fontSize = 20f;
                            fontOffset = new Size(-2, -1);
                        }
                        g.FillRectangle(new SolidBrush(Color.FromArgb(100, Color.Black)), mapLocation.X, mapLocation.Y + 4,
                                        map.Image.Width, map.Image.Height);
                        g.DrawString("start", new Font((FontFamily)fontCollection.Families[0], fontSize), brush,
                                     Point.Add(startButtonLocation, fontOffset));
                    }

                    foreach (var col in Collisions)
                    {
                        if (col.Stopwatch.ElapsedMilliseconds < 800)
                        {
                            g.DrawString(col.Score.ToString(), new Font((FontFamily)fontCollection.Families[0],
                                         GetScoreSize(col.Stopwatch)), Brushes.White, col.Location);
                        }
                    }

                    if (fruit.IsFruitEated)
                    {
                        if (fruit.ScoreStopwatch.ElapsedMilliseconds < 800)
                        {
                            g.DrawString(fruit.FruitScore.ToString(), new Font((FontFamily)fontCollection.Families[0],
                                         GetScoreSize(fruit.ScoreStopwatch)), Brushes.White, fruit.Location.X,
                                         fruit.Location.Y + fruit.Image.Width / 2);
                        }
                        else fruit.IsFruitEated = false;
                    }

                    if (!isEntitiesMoving)
                    {
                        Bitmap Image = player.GetCollisionImage(player.CollisionFrameStopwatch);
                        g.DrawImage(Image, player.Location.X + mapLocation.X - (Image.Width / 2),
                        player.Location.Y + mapLocation.Y - (Image.Height / 2), Image.Width, Image.Height);
                    }
                }
            }
        }

        private void LoadFonts()
        {
            fontCollection.AddFontFile("KOMIKAX.ttf");
        }

        private float GetScoreSize(Stopwatch sw)
        {
            int BlinkTime = 70;
            float baseSize = 5f;
            float sizeIncrement = 0.6f;
            return baseSize + sizeIncrement * (sw.ElapsedMilliseconds / BlinkTime);
        }

        private void Game_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                player.NextDirection = Direction.Down;
            }
            else if (e.KeyCode == Keys.Up)
            {
                player.NextDirection = Direction.Up;
            }
            else if(e.KeyCode == Keys.Right)
            {
                player.NextDirection = Direction.Right;
            }
            else if(e.KeyCode == Keys.Left)
            {
                player.NextDirection = Direction.Left;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                if (!paused && gameStarted) paused = true;
                else paused = false;
            }
        }

        private void Game_MouseClick(object sender, MouseEventArgs e)
        {
            if (!gameStarted)
            {
                if (Cursor.Position.X >= Location.X + startButtonRect.X &&
                    Cursor.Position.X <= Location.X + startButtonRect.X + startButtonRect.Width &&
                    Cursor.Position.Y >= Location.Y + startButtonRect.Y &&
                    Cursor.Position.Y <= Location.Y + startButtonRect.Y + startButtonRect.Height)
                {
                    SetWorld();
                    isMovementAllowed = false;
                    gameStarted = true;
                    Sound.Start.Ctlcontrols.play();

                    // Add a delegate for the PlayStateChange event.
                    Sound.Start.PlayStateChange += new _WMPOCXEvents_PlayStateChangeEventHandler(StartSoundStopped);
                }
            }
        }

        private void StartSoundStopped(object sender, _WMPOCXEvents_PlayStateChangeEvent e)
        {
            if (e.newState == 8)
            {
                isMovementAllowed = true;
                Sound.Siren.Ctlcontrols.play();
            }
        }

        public static void DeathSoundStopped(object sender, _WMPOCXEvents_PlayStateChangeEvent e)
        {
            if (e.newState == 8 && Game.gameStarted)
            {
                Sound.Siren.Ctlcontrols.play();
            }
        }

        public static void EatGhostSoundStopped(object sender, _WMPOCXEvents_PlayStateChangeEvent e)
        {
            if (e.newState == 8)
            {
                Sound.Siren.Ctlcontrols.play();
            }
        }
    }
}
