using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ver2
{
    public class Square
    {
        private static int line_Width = 10;
        public static int LBL_SIZE = 100;
        private static int LBL_START_SIZE = 76;
        private static int SCALE_STEP = 8;

        public int value;
        public Label btnFigure;
        public Status status;
        private Square[,] arrSquare;
        public Point location; //theo ma tran
        public bool isCompined;
        private List<Square> lstDisaposeSquare;
        private SCaleAnimation sCaleAni;
        private SCaleAnimation compineAni;

        public Square(Point location, Game2048 gameBoard, Square[,] arrSquare, List<Square> lstDisaposeSquare)
        {
            this.lstDisaposeSquare = lstDisaposeSquare;
            this.location = location;
            value = GenerateValue();
            isCompined = false;
            this.arrSquare = arrSquare;

            btnFigure = new Label();           
            btnFigure.Size = new Size(LBL_START_SIZE, LBL_START_SIZE);
            btnFigure.Text = value.ToString();
            btnFigure.TextAlign = ContentAlignment.MiddleCenter;            
            btnFigure.Location = new Point(this.location.Y * LBL_SIZE + (this.location.Y + 1) * line_Width + (LBL_SIZE - LBL_START_SIZE) / 2, this.location.X * LBL_SIZE + (this.location.X + 1) * line_Width + (LBL_SIZE - LBL_START_SIZE) / 2);          
            SetColorAndText();
            SetDoubleBuffered(btnFigure);

            gameBoard.Controls.Add(btnFigure);

            sCaleAni = new SCaleAnimation(0,LBL_SIZE, SCALE_STEP, LBL_START_SIZE);
            compineAni = new SCaleAnimation(LBL_SIZE, LBL_SIZE + line_Width*2, 8, LBL_SIZE); 

            status = Status.NONE;
        }

        public void SetColorAndText()
        {
            int[] temp = Setting.GET_COLOR_AND_TEXTSIZE(this.value);
            btnFigure.BackColor = Color.FromArgb(temp[0], temp[1], temp[2]);         
            if (this.value > 4)
                this.btnFigure.ForeColor = Color.White;
            btnFigure.Text = this.value.ToString();
            btnFigure.Font = new Font("Consolas", temp[3], FontStyle.Bold);
        }

        #region Move Methods
        /// <summary>
        /// thay doi trang thai khi nguoi dung nhan phim, return 1 neu di chuyen duoc, 0 neu nguoc lai
        /// </summary>
        /// <param name="st"></param>
        public void SetViTriDich(Status st)
        {
            switch (st)
            {
                case Status.UP:
                    FindToTop();
                    break;
                case Status.DOWN:
                    FindToBot();
                    break;
                case Status.LEFT:
                    FindToLeft();
                    break;
                case Status.RIGHT:
                    FindToRight();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// return 1 neu di chuyen duoc, nguoc lai return 0
        /// </summary>
        /// <returns></returns>
        private void FindToRight()
        {
            Point dichDen = this.location;
            for (int i = this.location.Y + 1; i < arrSquare.GetLength(1); i++)
            {
                if (arrSquare[this.location.X, i] == null) dichDen = new Point(this.location.X, i);
                else if (arrSquare[this.location.X, i].value == this.value && arrSquare[this.location.X, i].isCompined == false)
                {
                    dichDen = new Point(this.location.X, i);
                    Compine(dichDen);

                    break;
                }
                else break;
            }

            if (this.location != dichDen)
            {
                arrSquare[dichDen.X, dichDen.Y] = this;
                arrSquare[this.location.X, this.location.Y] = null;
                this.location = dichDen;
                this.status = Status.RIGHT;

            }
            else this.status = Status.NONE;
        }

        /// <summary>
        /// return 1 neu di chuyen duoc, nguoc lai return 0
        /// </summary>
        /// <returns></returns>
        private void FindToLeft()
        {
            Point dichDen = this.location;
            for (int i = this.location.Y - 1; i >= 0; i--)
            {
                if (arrSquare[this.location.X, i] == null) dichDen = new Point(this.location.X, i);
                else if (arrSquare[this.location.X, i].value == this.value && arrSquare[this.location.X, i].isCompined == false)
                {
                    dichDen = new Point(this.location.X, i);
                    Compine(dichDen);
                    break;
                }
                else break;
            }

            if (this.location != dichDen)
            {
                arrSquare[dichDen.X, dichDen.Y] = this;
                arrSquare[this.location.X, this.location.Y] = null;
                this.location = dichDen;
                this.status = Status.LEFT;
            }
            else this.status = Status.NONE;
        }

        /// <summary>
        /// return 1 neu di chuyen duoc, nguoc lai return 0
        /// </summary>
        /// <returns></returns>
        private void FindToBot()
        {
            Point dichDen = this.location;
            for (int i = this.location.X + 1; i < arrSquare.GetLength(0); i++)
            {
                if (arrSquare[i, this.location.Y] == null) dichDen = new Point(i, this.location.Y);
                else if (arrSquare[i, this.location.Y].value == this.value && arrSquare[i, this.location.Y].isCompined == false)
                {
                    dichDen = new Point(i, this.location.Y);
                    Compine(dichDen);

                    break;
                }
                else break;
            }

            if (this.location != dichDen)
            {
                arrSquare[dichDen.X, dichDen.Y] = this;
                arrSquare[this.location.X, this.location.Y] = null;
                this.location = dichDen;
                this.status = Status.DOWN;
            }
            else
            {
                this.status = Status.NONE;
            }
        }

        /// <summary>
        /// return 1 neu di chuyen duoc, nguoc lai return 0
        /// </summary>
        /// <returns></returns>
        private void FindToTop()
        {
            Point dichDen = this.location;
            for (int i = this.location.X - 1; i >= 0; i--)
            {
                if (arrSquare[i, this.location.Y] == null) dichDen = new Point(i, this.location.Y);
                else if (arrSquare[i, this.location.Y].value == this.value && arrSquare[i, this.location.Y].isCompined == false)
                {
                    dichDen = new Point(i, this.location.Y);
                    Compine(dichDen);

                    break;
                }
                else break;
            }

            if (this.location != dichDen)
            {
                arrSquare[dichDen.X, dichDen.Y] = this;
                arrSquare[this.location.X, this.location.Y] = null;
                this.location = dichDen;
                this.status = Status.UP;
            }
            else this.status = Status.NONE;
        }


        /// <summary>
        /// Xu ly khi hai lable co the ket hop
        /// </summary>
        private void Compine(Point dichDen)
        {
            isCompined = true;
            this.value *= 2;

            this.lstDisaposeSquare.Add(arrSquare[dichDen.X, dichDen.Y]);
        }


        /// <summary>
        /// Su ly su kien moi khoang thoi gian
        /// </summary>
        public void Process()
        {
            MoveLable(50);
        }

        private void MoveLable(int perStep)
        {
            Point viTriDichCuaLbl = new Point(this.location.Y * LBL_SIZE + (this.location.Y + 1) * line_Width, this.location.X * LBL_SIZE + (this.location.X + 1) * line_Width);
            Point viTriTiepTheo = this.btnFigure.Location;

            switch (status)
            {
                case Status.UP:
                    viTriTiepTheo.Y -= perStep;

                    if (viTriTiepTheo.Y <= this.location.X * LBL_SIZE + (this.location.X + 1) * line_Width)
                    {
                        this.btnFigure.Location = viTriDichCuaLbl;
                        if (isCompined)
                        {
                            this.status = Status.COMPINE;
                            SetColorAndText();
                        }
                        else
                            this.status = Status.NONE;
                    }
                    else
                        this.btnFigure.Location = viTriTiepTheo;

                    break;
                case Status.DOWN:
                    viTriTiepTheo.Y += perStep;

                    if (viTriTiepTheo.Y >= this.location.X * LBL_SIZE + (this.location.X + 1) * line_Width)
                    {
                        this.btnFigure.Location = viTriDichCuaLbl;
                        if (isCompined)
                        {
                            this.status = Status.COMPINE;
                            SetColorAndText();
                        }
                        else
                            this.status = Status.NONE;
                    }
                    else
                        this.btnFigure.Location = viTriTiepTheo;
                    break;
                case Status.LEFT:
                    viTriTiepTheo.X -= perStep;

                    if (viTriTiepTheo.X <= this.location.Y * LBL_SIZE + (this.location.Y + 1) * line_Width)
                    {
                        this.btnFigure.Location = viTriDichCuaLbl;
                        if (isCompined)
                        {
                            this.status = Status.COMPINE;
                            SetColorAndText();
                        }
                        else
                            this.status = Status.NONE;
                    }
                    else
                        this.btnFigure.Location = viTriTiepTheo;
                    break;
                case Status.RIGHT:
                    viTriTiepTheo.X += perStep;
                    
                    if (viTriTiepTheo.X >= this.location.Y * LBL_SIZE + (this.location.Y + 1) * line_Width)
                    {
                        this.btnFigure.Location = viTriDichCuaLbl;
                        
                        if (isCompined)
                        {
                            this.status = Status.COMPINE;
                            SetColorAndText();
                        }
                        else
                            this.status = Status.NONE;
                    }
                    else
                        this.btnFigure.Location = viTriTiepTheo;
                    break;
                default:
                    break;
            }

        }
        #endregion

        #region Animation
        public bool CreateAnimation()
        {
            int x = sCaleAni.Next();
            this.btnFigure.Size = new Size(x, x);
            this.btnFigure.Location = new Point(btnFigure.Location.X - SCALE_STEP / 2, btnFigure.Location.Y - SCALE_STEP / 2); // size tang 1 khoang x thi location phai giam 1 khoang x/2

            if (this.btnFigure.Size.Width >= LBL_SIZE)
                return true;
            
            return false;
        }

        public bool CompineAnimation()
        {
            int x = compineAni.Next();
            this.btnFigure.Size = new Size(x, x);
            //lblFigure.Scale(new SizeF(50, 50));

            if (compineAni.thuan)
            {
                this.btnFigure.Location = new Point(btnFigure.Location.X - 6 / 2, btnFigure.Location.Y - 6 / 2); // size tang 1 khoang x thi location phai giam 1 khoang x/2
                if (x == compineAni.endValue) compineAni.thuan = false;
            }
            else
            {
                this.btnFigure.Location = new Point(btnFigure.Location.X + 6 / 2, btnFigure.Location.Y + 6 / 2); // size giam 1 khoang x thi location phai tang 1 khoang x/2
                if (x == compineAni.startValue) compineAni.thuan = true;
            }
            if (this.btnFigure.Size.Width <= LBL_SIZE)
                return true;

            return false;
        }
        #endregion

        #region Static Method
        public static void SetDoubleBuffered(System.Windows.Forms.Control c)
        {
            //Taxes: Remote Desktop Connection and painting
            //http://blogs.msdn.com/oldnewthing/archive/2006/01/03/508694.aspx
            if (System.Windows.Forms.SystemInformation.TerminalServerSession)
                return;

            System.Reflection.PropertyInfo aProp =
                  typeof(System.Windows.Forms.Control).GetProperty(
                        "DoubleBuffered",
                        System.Reflection.BindingFlags.NonPublic |
                        System.Reflection.BindingFlags.Instance);

            aProp.SetValue(c, true, null);
        }

        private static Random random = new Random();
        /// <summary>
        /// Tạo giá trị ban đầu cho đối tượng.
        /// </summary>
        private static int GenerateValue()
        {
            int temp = random.Next();
            if (temp % 3 == 0)
                return 4;
            else return 2;
        }
        #endregion


    }

    public enum Status
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,
        CREATE,
        COMPINE,
        NONE
    }

    public class SCaleAnimation
    {
        public int endValue;
        public int step;
        public int curValue;
        public bool thuan;
        public int startValue;

        public SCaleAnimation(int start, int end, int step, int curValue)
        {
            this.startValue = start;
            this.endValue = end;
            this.step = step;
            this.curValue = curValue;
            thuan = true;
        }

        public int Next()
        {
            if (thuan)
            {
                curValue += step;
                if (curValue >= endValue)
                {
                    curValue = endValue;
                    //thuan = false;
                }
            }
            else
            {
                curValue -= step;
                if(curValue <= startValue)
                {
                    curValue = startValue;
                    //thuan = true;
                }
            }

            return curValue;
        }
    }
}
