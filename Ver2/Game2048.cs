using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ver2
{
    public class Game2048 : Form
    {
        private int width, height;
        private Random rd;
        private Square[,] arrSquare;
        private bool isMoved;
        private Square[,] oldSquareArr;
        private Timer timerMove;
        private Status status;
        private bool ready;
        private List<Square> lstDisapose_Square;
        private Timer timerAnimation;
        private Panel panel1;
        private Label lblBest;
        private Label lblScore;
        private Label label1;
        private ImageList imageList1;
        private System.ComponentModel.IContainer components;
        private Label label2;
        private Label label4;
        private Label label3;
        private Label label5;
        private Panel panel2;
        private Panel panel3;
        private Panel pnlGame;
        private Square creatingSquare;



        public Game2048()
        {
            lstDisapose_Square = new List<Square>();
            rd = new Random();

            timerMove = new Timer();
            timerMove.Interval = 1;
            timerMove.Tick += Timer_Tick;
            timerAnimation = new Timer();
            timerAnimation.Interval = 1;
            timerAnimation.Tick += TimerAnimation_Tick;

            this.width = 4;
            this.height = 4;
            arrSquare = new Square[height, width];

            CreateNewSquare();
            //CreateNewSquare();

            isMoved = false;
            ready = true;

            this.InitializeComponent();
        }

        private void TimerAnimation_Tick(object sender, EventArgs e)
        {
            if (status == Status.CREATE)
            {
                if (creatingSquare == null || creatingSquare.CreateAnimation())
                {
                    timerAnimation.Stop();
                    this.status = Status.NONE; //// compine xong -- create --- tiep tuc
                    ready = true;
                }
            }
            else if (status == Status.COMPINE)
            {
                int temp = 1;
                foreach (var item in arrSquare)
                {
                    if (item != null)
                        if (item.status == Status.COMPINE)
                        {
                            if (!item.CompineAnimation()) temp = 0; // compine chua xong
                        }
                }

                //Neu compine xong
                if (temp == 1)
                {
                    this.status = Status.CREATE;
                    timerAnimation.Stop();
                    foreach (var item in arrSquare)
                    {
                        if (item != null)
                        {
                            item.isCompined = false;
                            item.status = Status.NONE;
                            //item.CanChinhLaiLable();
                        }
                    }

                    CreateNewSquare();
                    timerAnimation.Start(); // bat dau create
                }
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            //------------Kiem tra xem da di chuyen xong het chua---------//
            int temp = 1;
            foreach (var item in arrSquare)
            {
                if (item != null)
                    if (item.status != Status.NONE && item.status != Status.COMPINE)
                    {
                        temp = 0;
                        break;
                    }
            }

            foreach (var item in lstDisapose_Square)
            {
                if (item.status != Status.NONE && item.status != Status.COMPINE)
                {
                    temp = 0;
                    break;
                }
            }
            //---------------------------------------------------------//


            //-------------------------Di chuyen xong------------------//
            if (temp == 1)
            {
                timerMove.Stop();
                foreach (var item in lstDisapose_Square)
                {
                    item.btnFigure.Dispose();
                }
                lstDisapose_Square.Clear(); //clear lst

                //foreach (var item in arrSquare)
                //{
                //    if (item != null)
                //    {
                //        item.isCompined = false;
                //        //item.CanChinhLaiLable();
                //    }
                //}

                //compine 
                this.status = Status.COMPINE;
                timerAnimation.Start();

                //Tao moi square
                //this.CreateNewSquare();

            }

            //-------------------di chua chuyen---------------------//
            else
            {
                foreach (var item in arrSquare)
                {
                    if (item != null)
                        item.Process();
                }
                foreach (var item in lstDisapose_Square)
                {
                    item.Process();
                }
            }

            //----------------------------------------------------//
        }

        private bool CreateNewSquare()
        {
            bool isCreated = false;

            //kiem tra xem con cho trong khong
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (arrSquare[i, j] == null)
                    {
                        isCreated = true;
                        break;
                    }
                }
            }

            if (isCreated == false)
            {
                MessageBox.Show("Không thể tạo mới");
                ready = true;
                return isCreated;
            }

            int x, y;
            do
            {
                x = rd.Next(height);
                y = rd.Next(width);
            } while (arrSquare[x, y] != null);

            creatingSquare = new Square(new Point(x, y), this, arrSquare, lstDisapose_Square);
            arrSquare[x, y] = creatingSquare;

            this.status = Status.CREATE;
            timerAnimation.Start();


            return isCreated;
        }

        public void MoveAllSquare(Status st)
        {
            if (st == Status.UP)
            {
                for (int i = 1; i < arrSquare.GetLength(0); i++)
                {
                    for (int j = 0; j < arrSquare.GetLength(1); j++)
                    {
                        if (arrSquare[i, j] != null)
                        {
                            arrSquare[i, j].isCompined = false;
                            arrSquare[i, j].SetViTriDich(st);
                        }
                    }
                }
            }
            else if (st == Status.DOWN)
            {
                for (int i = arrSquare.GetLength(0) - 2; i >= 0; i--)
                {
                    for (int j = 0; j < arrSquare.GetLength(1); j++)
                    {
                        if (arrSquare[i, j] != null)
                        {
                            arrSquare[i, j].isCompined = false;
                            arrSquare[i, j].SetViTriDich(st);
                        }
                    }
                }
            }
            else if (st == Status.LEFT)
            {
                for (int j = 1; j < arrSquare.GetLength(1); j++)
                {
                    for (int i = 0; i < arrSquare.GetLength(0); i++)
                    {
                        if (arrSquare[i, j] != null)
                        {
                            arrSquare[i, j].isCompined = false;
                            arrSquare[i, j].SetViTriDich(st);
                        }
                    }
                }
            }
            else if (st == Status.RIGHT)
            {
                for (int j = arrSquare.GetLength(1) - 2; j >= 0; j--)
                {
                    for (int i = 0; i < arrSquare.GetLength(0); i++)
                    {
                        if (arrSquare[i, j] != null)
                        {
                            arrSquare[i, j].isCompined = false;
                            arrSquare[i, j].SetViTriDich(st);
                        }
                    }
                }
            }

            //Kiem tra xem co label nao di chuyen khong
            foreach (var item in arrSquare)
            {
                if (item != null)
                    if (item.status != Status.NONE)
                    {
                        this.status = st;
                        timerMove.Start();
                        return;
                    }
            }

            ready = true;
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Game2048));
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.lblBest = new System.Windows.Forms.Label();
            this.lblScore = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.pnlGame = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.lblBest);
            this.panel1.Controls.Add(this.lblScore);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(457, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(239, 452);
            this.panel1.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.Image = global::Ver2.Properties.Resources.undo1;
            this.label2.Location = new System.Drawing.Point(30, 246);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 58);
            this.label2.TabIndex = 2;
            // 
            // lblBest
            // 
            this.lblBest.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.lblBest.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBest.Location = new System.Drawing.Point(29, 196);
            this.lblBest.Name = "lblBest";
            this.lblBest.Size = new System.Drawing.Size(180, 37);
            this.lblBest.TabIndex = 1;
            this.lblBest.Text = "Best: 0";
            this.lblBest.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblScore
            // 
            this.lblScore.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.lblScore.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblScore.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblScore.Location = new System.Drawing.Point(29, 142);
            this.lblScore.Name = "lblScore";
            this.lblScore.Size = new System.Drawing.Size(180, 37);
            this.lblScore.TabIndex = 1;
            this.lblScore.Text = "Score: 0";
            this.lblScore.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Consolas", 48F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(37, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(172, 75);
            this.label1.TabIndex = 0;
            this.label1.Text = "2048";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "exit.png");
            this.imageList1.Images.SetKeyName(1, "like.png");
            this.imageList1.Images.SetKeyName(2, "play.png");
            this.imageList1.Images.SetKeyName(3, "replay.png");
            this.imageList1.Images.SetKeyName(4, "take_photo.png");
            this.imageList1.Images.SetKeyName(5, "undo.png");
            // 
            // label3
            // 
            this.label3.Image = global::Ver2.Properties.Resources.replay1;
            this.label3.Location = new System.Drawing.Point(17, 29);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 58);
            this.label3.TabIndex = 2;
            // 
            // label4
            // 
            this.label4.Image = global::Ver2.Properties.Resources.exit;
            this.label4.Location = new System.Drawing.Point(151, 246);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 58);
            this.label4.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.Image = global::Ver2.Properties.Resources.like;
            this.label5.Location = new System.Drawing.Point(119, 29);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 58);
            this.label5.TabIndex = 2;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.Control;
            this.panel2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel2.BackgroundImage")));
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Location = new System.Drawing.Point(16, 307);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(211, 137);
            this.panel2.TabIndex = 1;
            this.panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.panel2_Paint);
            // 
            // panel3
            // 
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(200, 100);
            this.panel3.TabIndex = 3;
            // 
            // pnlGame
            // 
            this.pnlGame.BackgroundImage = global::Ver2.Properties.Resources.BackGround;
            this.pnlGame.Location = new System.Drawing.Point(2, 0);
            this.pnlGame.Name = "pnlGame";
            this.pnlGame.Size = new System.Drawing.Size(449, 452);
            this.pnlGame.TabIndex = 1;
            // 
            // Game2048
            // 
            this.BackgroundImage = global::Ver2.Properties.Resources.BackGround;
            this.ClientSize = new System.Drawing.Size(696, 452);
            this.Controls.Add(this.pnlGame);
            this.Controls.Add(this.panel1);
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.Name = "Game2048";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Game2048_KeyDown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void btnStart_Click(object sender, EventArgs e)
        {

        }

        private void Game2048_KeyDown(object sender, KeyEventArgs e)
        {
            if (ready == false) return;

            ready = false;
            if (e.KeyData == Keys.Up)
            {
                MoveAllSquare(Status.UP);
            }
            else if (e.KeyData == Keys.Down)
            {
                MoveAllSquare(Status.DOWN);
            }
            else if (e.KeyData == Keys.Left)
            {
                MoveAllSquare(Status.LEFT);
            }
            else if (e.KeyData == Keys.Right)
            {
                MoveAllSquare(Status.RIGHT);
            }
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            panel2.BackColor = Color.Transparent;
        }

        public void Undo()
        {
            arrSquare = oldSquareArr;
            this.Invalidate();
        }
    }
}
