using AxWMPLib;

namespace Pacman
{
    class Sound
    {
        public static AxWindowsMediaPlayer Start = new AxWindowsMediaPlayer();
        public static AxWindowsMediaPlayer Siren = new AxWindowsMediaPlayer();
        public static AxWindowsMediaPlayer Chomp = new AxWindowsMediaPlayer();
        public static AxWindowsMediaPlayer Death = new AxWindowsMediaPlayer();
        public static AxWindowsMediaPlayer EatFruit = new AxWindowsMediaPlayer();
        public static AxWindowsMediaPlayer Intermission = new AxWindowsMediaPlayer();
        public static AxWindowsMediaPlayer EatGhost = new AxWindowsMediaPlayer();

        public static void Load()
        {
            Start.CreateControl();
            Start.URL = @"Sounds\start.wav";
            Start.Ctlcontrols.stop();
            Start.settings.volume = 10;

            Siren.CreateControl();
            Siren.URL = @"Sounds\siren.wav";
            Siren.Ctlcontrols.stop();
            Siren.settings.setMode("loop", true);
            Siren.settings.volume = 2;

            Chomp.CreateControl();
            Chomp.URL = @"Sounds\waza.mp3";
            Chomp.Ctlcontrols.stop();
            Chomp.settings.volume = 4;

            Death.CreateControl();
            Death.URL = @"Sounds\death.wav";
            Death.Ctlcontrols.stop();
            Death.settings.volume = 10;

            EatFruit.CreateControl();
            EatFruit.URL = @"Sounds\eatfruit.wav";
            EatFruit.Ctlcontrols.stop();
            EatFruit.settings.volume = 10;

            Intermission.CreateControl();
            Intermission.URL = @"Sounds\intermission.wav";
            Intermission.Ctlcontrols.stop();
            Intermission.settings.volume = 10;

            EatGhost.CreateControl();
            EatGhost.URL = @"Sounds\eatghost.wav";
            EatGhost.Ctlcontrols.stop();
            EatGhost.settings.volume = 10;

            // Add a delegate for the PlayStateChange event.
            Death.PlayStateChange += new _WMPOCXEvents_PlayStateChangeEventHandler(Game.DeathSoundStopped);
            EatGhost.PlayStateChange += new _WMPOCXEvents_PlayStateChangeEventHandler(Game.EatGhostSoundStopped);
        }

        public static void GetChompSound()
        {
            if (Chomp.playState != WMPLib.WMPPlayState.wmppsPlaying)
            {
                Chomp.Ctlcontrols.play();
            }
        }
    }
}
