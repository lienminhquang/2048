using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ver3
{
    public class Game2048:Panel
    {
        private int previousScore = -1;
        private int soLanUndo = 5;
        private bool readyToUndo = false;
        private int width, height;
        private Random rd;
        private Square[,] arrSquare;
        private Square[,] previousArr;
        private Timer timerMove;
        private Status status;
        private bool ready;
        private List<Square> lstDisapose_Square;
        private Timer timerAnimation;
        private List<Square> lstCreatingSquare;
        public int score { get; private set; }
        public int bestScore { get; private set; }
        public bool isEndGame { get; set; }


        public event EventHandler TangDiemEvent;
        public event EventHandler EndGameEvent;

        public Game2048(Point position, bool createStartValue)
        {
            isEndGame = false;
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
            previousArr = new Square[width, height];
            arrSquare = new Square[width, height];
            lstCreatingSquare = new List<Square>();

            if (createStartValue)
            {
                CreateNewSquare();
                CreateNewSquare();

                ready = false;
                this.status = Status.CREATE;
                timerAnimation.Start();
            }

            ready = true;

            this.Location = position;
            this.Width = 450;
            this.Height = 450;
            this.BackgroundImage = Image.FromFile(@"Game2048-Data\BackGround.png");
        }

        public void Recovery(Square[,] arrSquare, Square[,] previousArr, int bestScore, int score, int soLanUndo, bool readyToUndo, int previousScore)
        {
            this.arrSquare = arrSquare;
            this.previousArr = previousArr;
            this.bestScore = bestScore;
            this.score = score;
            this.soLanUndo = soLanUndo;
            this.readyToUndo = readyToUndo;
            this.previousScore = previousScore;

            foreach (var item in arrSquare)
            {
                if (item != null)
                {
                    this.Controls.Add(item.lblFigure);
                    item.MergeEvent += CreatingSquare_MergeEvent;
                }
            }
        }

        #region Methods
        private void TimerAnimation_Tick(object sender, EventArgs e)
        {
            if (status == Status.CREATE)
            {
                bool kq = false;

                foreach (var item in lstCreatingSquare)
                {
                    kq = item.CreateAnimation();
                }

                if(kq == true) // Da thuc hien xong create animation
                {
                    timerAnimation.Stop();
                    this.status = Status.NONE;
                    lstCreatingSquare.Clear();

                    //Kiểm tra kết thúc.
                    CheckEndGame();
                    
                    ready = true; // Tiep tuc cho bat phim
                }
            }
            else if (status == Status.MERGE)
            {
                int temp = 1;
                foreach (var item in arrSquare)
                {
                    if (item != null)
                        if (item.status == Status.MERGE)
                        {
                            if (!item.MergeAnimation()) temp = 0; // merge chua xong
                        }
                }

                //Neu merge xong kiểm tra xem có create được không
                if (temp == 1)
                {
                    timerAnimation.Stop();
                    foreach (var item in arrSquare)
                    {
                        if (item != null)
                        {
                            item.isMerged = false;
                            item.status = Status.NONE;
                        }
                    }

                    if (CreateNewSquare())
                    {
                        this.status = Status.CREATE;
                        timerAnimation.Start(); // bat dau create
                    }
                }
            }
        }

        private void CheckEndGame()
        {
            if (!MoveAble(Status.UP) && !MoveAble(Status.DOWN) && !MoveAble(Status.LEFT) && !MoveAble(Status.RIGHT))
            {
                isEndGame = true;
                EndGameEvent(this, EventArgs.Empty);
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            //------------Kiem tra xem da di chuyen xong het chua---------//
            int temp = 1;
            foreach (var item in arrSquare)
            {
                if (item != null)
                    if (item.status != Status.NONE && item.status != Status.MERGE)
                    {
                        temp = 0;
                        break;
                    }
            }

            foreach (var item in lstDisapose_Square)
            {
                if (item.status != Status.NONE && item.status != Status.MERGE)
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
                foreach (var item in lstDisapose_Square) // Xoa het cac label rac
                {
                    item.lblFigure.Dispose();
                }
                lstDisapose_Square.Clear(); //clear lst               

                //compine 
                this.status = Status.MERGE;
                timerAnimation.Start();
            }

            //-------------------Chua di chuyen xong---------------------//
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
                ready = true;
            }
            else
            {
                int x, y;
                do
                {
                    x = rd.Next(height);
                    y = rd.Next(width);
                } while (arrSquare[x, y] != null);

                arrSquare[x, y] = new Square(new Point(x, y), true);
                lstCreatingSquare.Add(arrSquare[x, y]);

                this.Controls.Add(arrSquare[x, y].lblFigure);

                arrSquare[x, y].MergeEvent += CreatingSquare_MergeEvent;
            }

            return isCreated;
        }

        private void CreatingSquare_MergeEvent(object sender, MergeEventArg e)
        {
            this.lstDisapose_Square.Add(e.squareDis);

            this.score += e.score;
            if (this.score > bestScore)
                bestScore = this.score;

            TangDiemEvent(this, EventArgs.Empty);
        }

        /// <summary>
        /// Tim va set vi tri dich cho tat ca cac phan tu trong mang
        /// </summary>
        /// <param name="st"></param>
        public void MoveAllSquare(Status st)
        {
            if (MoveAble(st))
            {
                CloneArrSquare();
                this.previousScore = score;
            }
            else
            {
                ready = true;
                return;
            }

            if (st == Status.UP)
            {
                for (int i = 1; i < arrSquare.GetLength(0); i++)
                {
                    for (int j = 0; j < arrSquare.GetLength(1); j++)
                    {
                        if (arrSquare[i, j] != null)
                        {
                            arrSquare[i, j].isMerged = false;
                            arrSquare[i, j].SetViTriDich(st, arrSquare);
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
                            arrSquare[i, j].isMerged = false;
                            arrSquare[i, j].SetViTriDich(st, arrSquare);
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
                            arrSquare[i, j].isMerged = false;
                            arrSquare[i, j].SetViTriDich(st, arrSquare);
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
                            arrSquare[i, j].isMerged = false;
                            arrSquare[i, j].SetViTriDich(st, arrSquare);
                        }
                    }
                }
            }

            this.status = st;
            timerMove.Start();
            readyToUndo = true;
        }

        private void InitializeComponent()
        {
        
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Game2048));           
            this.SuspendLayout();            
            this.DoubleBuffered = true;
            this.Name = "Game2048";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Game2048_KeyDown);
            this.ResumeLayout(false);

        }

        /// <summary>
        /// Kiểm tra xem có thể di chuyển theo hướng của st được không.
        /// </summary>
        /// <param name="st"></param>
        /// <returns></returns>
        public bool MoveAble(Status st)
        {
            for (int i = 0; i < arrSquare.GetLength(0); i++)
            {
                for (int j = 0; j < arrSquare.GetLength(1); j++)
                {
                    if (arrSquare[i, j] == null) continue;
                    switch (st)
                    {
                        case Status.UP:
                            if (i >= 1)
                            {
                                if (arrSquare[i - 1, j] == null || arrSquare[i - 1, j].value == arrSquare[i, j].value)
                                    return true;
                            }
                            break;
                        case Status.DOWN:
                            if (i < arrSquare.GetLength(0) - 1)
                            {
                                if (arrSquare[i + 1, j] == null || arrSquare[i + 1, j].value == arrSquare[i, j].value)
                                    return true;
                            }
                            break;
                        case Status.LEFT:
                            if (j >= 1)
                            {
                                if (arrSquare[i, j - 1] == null || arrSquare[i, j - 1].value == arrSquare[i, j].value)
                                    return true;
                            }
                            break;
                        case Status.RIGHT:
                            if (j < arrSquare.GetLength(1) - 1)
                            {
                                if (arrSquare[i, j + 1] == null || arrSquare[i, j + 1].value == arrSquare[i, j].value)
                                    return true;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Sao lưu lại mảng
        /// </summary>
        private void CloneArrSquare()
        {
            previousArr = new Square[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (arrSquare[i, j] == null) previousArr[i, j] = null;
                    else
                        previousArr[i, j] = arrSquare[i, j].Clone();
                }
            }
        }

        /// <summary>
        /// Lay ra thong tin hien tai cua game de luu
        /// </summary>
        /// <returns></returns>
        public GameInfo Get_GameInfo()
        {
            GameInfo inf = new GameInfo(this.bestScore, this.score, this.previousScore, soLanUndo, readyToUndo, arrSquare, previousArr, isEndGame);
            return inf;
        }
        #endregion

        #region Events
        public void Game2048_KeyDown(object sender, KeyEventArgs e)
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
            else
            {
                ready = true;
            }
        }
        public void Replay()
        {
            for(int i = 0; i < arrSquare.GetLength(0); i++)
            {
                for (int j = 0; j < arrSquare.GetLength(1); j++)
                {
                    if (arrSquare[i, j] != null)
                    {
                        arrSquare[i, j].Dispose();
                        arrSquare[i, j] = null;
                    }

                    if (previousArr[i, j] != null)
                    {
                        previousArr[i, j].Dispose();
                        previousArr[i, j] = null;
                    }
                }
            }

            this.soLanUndo = 5;
            this.previousScore = -1;
            this.score = 0;
            TangDiemEvent(this, EventArgs.Empty);

            CreateNewSquare();
            CreateNewSquare();

            this.status = Status.CREATE;
            timerAnimation.Start();
        }
        public void Undo()
        {
            if (soLanUndo <= 0 || previousScore == -1 || !readyToUndo) return;
            soLanUndo--;

            foreach (var item in arrSquare)
            {
                if(item != null)item.lblFigure.Dispose();
            }


            arrSquare = previousArr;
            foreach (var item in arrSquare)
            {
                if (item != null)
                {
                    this.Controls.Add(item.lblFigure);
                    item.MergeEvent += CreatingSquare_MergeEvent;
                }
            }
            this.score = previousScore;
            TangDiemEvent(this, EventArgs.Empty);
            readyToUndo = false;

            this.Invalidate();
        }
        #endregion
    }
}
