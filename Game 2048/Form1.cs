using System;
using System.Drawing;
using System.Windows.Forms;

namespace ver3
{
    public partial class Form1 : Form
    {
        Game2048 game;
        public Form1()
        {
            InitializeComponent();

            try
            {
                GameInfo gameInfo = Loger.LoadData<GameInfo>(@"Game2048-Data\DATA.xml");
                if(gameInfo.isEndGame == false)
                    game = gameInfo.ToGame2048();   
                else
                    game = new Game2048(new Point(0, 100), true);

            }
            catch (Exception)
            {
                game = new Game2048(new Point(0, 100), true);               
            }


            game.EndGameEvent += Game_EndGameEvent;
            game.TangDiemEvent += Game_TangDiemEvent;
            Setting.SetDoubleBuffered(game);

            this.Controls.Add(game);
            game.Show();
        }

        private void Game_EndGameEvent(object sender, EventArgs e)
        {
            pnlEndGame.Visible = true;
        }

        private void Game_TangDiemEvent(object sender, EventArgs e)
        {
            int score = (sender as Game2048).score;
            lblScore.Text = "SCORE\r\n" + score.ToString();

            score = (sender as Game2048).bestScore;
            lblBest.Text = "BEST\r\n" + score.ToString();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            game.Game2048_KeyDown(sender, e);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lbl2048.ForeColor = Color.FromArgb(119, 110, 101);
            pnlControl.BackColor = Color.FromArgb(250, 248, 239);          
            lblBest.BackColor = lblScore.BackColor = Color.FromArgb(187, 173, 160);


            int score = game.score;
            lblScore.Text = "SCORE\r\n" + score.ToString();
            score = game.bestScore;
            lblBest.Text = "BEST\r\n" + score.ToString();
        }

        private void picUndo_Click(object sender, EventArgs e)
        {
            game.Undo();
        }

        private void picReplay_Click(object sender, EventArgs e)
        {
            game.Replay();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Loger.SaveData(@"Game2048-Data\DATA.xml", game.Get_GameInfo());
        }

        private void btnTryAgain_Click(object sender, EventArgs e)
        {
            pnlEndGame.Visible = false;
            game.isEndGame = false;
            game.Replay();
        }
    }
}
