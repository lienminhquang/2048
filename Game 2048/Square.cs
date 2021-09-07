using System;
using System.Drawing;
using System.Windows.Forms;

namespace ver3
{
    public class Square
    {
        public Point location; //theo ma tran
        public int value;
        public Label lblFigure;
        public Status status;        
        public bool isMerged;

        private SCaleAnimation sCaleAni;
        private SCaleAnimation mergeAni;

        public event EventHandler<MergeEventArg> MergeEvent;
        private static Random random = new Random();


        /// <summary>
        /// Khoi tao va tao gia tri ngau nhien ban dau cho square
        /// </summary>
        /// <param name="location"></param>
        /// <param name="createAnimation"> Co khoi dong che do create animation ko</param>
        public Square(Point location, bool createAnimation)
        {
            this.location = location;
            value = GenerateValue();
            isMerged = false;


            //Design
            lblFigure = new Label();           
            lblFigure.Text = value.ToString();
            lblFigure.TextAlign = ContentAlignment.MiddleCenter;
            //

            if (createAnimation)
            {
                lblFigure.Size = new Size(Setting.LBL_START_SIZE, Setting.LBL_START_SIZE);

                lblFigure.Location = new Point(this.location.Y * Setting.LBL_SIZE + (this.location.Y + 1) * Setting.line_Width + (Setting.LBL_SIZE - Setting.LBL_START_SIZE) / 2,
                                                this.location.X * Setting.LBL_SIZE + (this.location.X + 1) * Setting.line_Width + (Setting.LBL_SIZE - Setting.LBL_START_SIZE) / 2);
                this.status = Status.CREATE;
            }
            else
            {
                lblFigure.Size = new Size(Setting.LBL_SIZE, Setting.LBL_SIZE);

                lblFigure.Location = new Point(this.location.Y * Setting.LBL_SIZE + (this.location.Y + 1) * Setting.line_Width ,
                                                this.location.X * Setting.LBL_SIZE + (this.location.X + 1) * Setting.line_Width);
                this.status = Status.NONE;
            }

            UpdateColorAndText();
            Setting.SetDoubleBuffered(lblFigure);

            sCaleAni = new SCaleAnimation(0, Setting.LBL_SIZE, Setting.SCALE_STEP, Setting.LBL_START_SIZE);
            mergeAni = new SCaleAnimation(Setting.LBL_SIZE, Setting.LBL_SIZE + Setting.line_Width *2, 8, Setting.LBL_SIZE); 
        }

        public Square()
        {

        }

        #region Move Methods
        /// <summary>
        /// Tìm và thay đổi location của nó.
        /// </summary>
        /// <param name="st"></param>
        public void SetViTriDich(Status st, Square[,] squareArr)
        {
            switch (st)
            {
                case Status.UP:
                    FindToTop(squareArr);
                    break;
                case Status.DOWN:
                    FindToBot(squareArr);
                    break;
                case Status.LEFT:
                    FindToLeft(squareArr);
                    break;
                case Status.RIGHT:
                    FindToRight(squareArr);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Tìm đường đi qua phải.
        /// </summary>
        /// <returns></returns>
        private void FindToRight(Square[,] squareArr)
        {
            Point dichDen = this.location;
            for (int i = this.location.Y + 1; i < squareArr.GetLength(1); i++)
            {
                if (squareArr[this.location.X, i] == null) dichDen = new Point(this.location.X, i);
                else if (squareArr[this.location.X, i].value == this.value && squareArr[this.location.X, i].isMerged == false)
                {
                    dichDen = new Point(this.location.X, i);
                    Merge(squareArr[dichDen.X, dichDen.Y]);

                    break;
                }
                else break;
            }

            if (this.location != dichDen)
            {
                squareArr[dichDen.X, dichDen.Y] = this;
                squareArr[this.location.X, this.location.Y] = null;
                this.location = dichDen;
                this.status = Status.RIGHT;

            }
            else this.status = Status.NONE;
        }

        /// <summary>
        /// Tìm đường đi qua trái.
        /// </summary>
        /// <returns></returns>
        private void FindToLeft(Square[,] squareArr)
        {
            Point dichDen = this.location;
            for (int i = this.location.Y - 1; i >= 0; i--)
            {
                if (squareArr[this.location.X, i] == null) dichDen = new Point(this.location.X, i);
                else if (squareArr[this.location.X, i].value == this.value && squareArr[this.location.X, i].isMerged == false)
                {
                    dichDen = new Point(this.location.X, i);
                    Merge(squareArr[dichDen.X, dichDen.Y]);
                    break;
                }
                else break;
            }

            if (this.location != dichDen)
            {
                squareArr[dichDen.X, dichDen.Y] = this;
                squareArr[this.location.X, this.location.Y] = null;
                this.location = dichDen;
                this.status = Status.LEFT;
            }
            else this.status = Status.NONE;
        }

        /// <summary>
        /// Tìm đường đi xuống dưới.
        /// </summary>
        /// <returns></returns>
        private void FindToBot(Square [,] squareArr)
        {
            Point dichDen = this.location;
            for (int i = this.location.X + 1; i < squareArr.GetLength(0); i++)
            {
                if (squareArr[i, this.location.Y] == null) dichDen = new Point(i, this.location.Y);
                else if (squareArr[i, this.location.Y].value == this.value && squareArr[i, this.location.Y].isMerged == false)
                {
                    dichDen = new Point(i, this.location.Y);
                    Merge(squareArr[dichDen.X, dichDen.Y]);

                    break;
                }
                else break;
            }

            if (this.location != dichDen)
            {
                squareArr[dichDen.X, dichDen.Y] = this;
                squareArr[this.location.X, this.location.Y] = null;
                this.location = dichDen;
                this.status = Status.DOWN;
            }
            else
            {
                this.status = Status.NONE;
            }
        }

        /// <summary>
        /// Tim đường đi lên trên.
        /// </summary>
        /// <returns></returns>
        private void FindToTop(Square[,] squareArr)
        {
            Point dichDen = this.location;
            for (int i = this.location.X - 1; i >= 0; i--)
            {
                if (squareArr[i, this.location.Y] == null) dichDen = new Point(i, this.location.Y);
                else if (squareArr[i, this.location.Y].value == this.value && squareArr[i, this.location.Y].isMerged == false)
                {
                    dichDen = new Point(i, this.location.Y);
                    Merge(squareArr[dichDen.X, dichDen.Y]);

                    break;
                }
                else break;
            }

            if (this.location != dichDen)
            {
                squareArr[dichDen.X, dichDen.Y] = this;
                squareArr[this.location.X, this.location.Y] = null;
                this.location = dichDen;
                this.status = Status.UP;
            }
            else this.status = Status.NONE;
        }

        /// <summary>
        /// Xu ly khi hai lable co the ket hop
        /// </summary>
        private void Merge(Square squareDis)
        {
            isMerged = true;
            this.value *= 2;

            MergeEvent(this, new MergeEventArg(squareDis, this.value));
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
            Point viTriDichCuaLbl = new Point(this.location.Y * Setting.LBL_SIZE + (this.location.Y + 1) * Setting.line_Width, this.location.X * Setting.LBL_SIZE + (this.location.X + 1) * Setting.line_Width);
            Point viTriTiepTheo = this.lblFigure.Location;

            switch (status)
            {
                case Status.UP:
                    viTriTiepTheo.Y -= perStep;

                    if (viTriTiepTheo.Y <= this.location.X * Setting.LBL_SIZE + (this.location.X + 1) * Setting.line_Width)
                    {
                        this.lblFigure.Location = viTriDichCuaLbl;
                        if (isMerged)
                        {
                            this.status = Status.MERGE;
                            UpdateColorAndText();
                        }
                        else
                            this.status = Status.NONE;
                    }
                    else
                        this.lblFigure.Location = viTriTiepTheo;

                    break;
                case Status.DOWN:
                    viTriTiepTheo.Y += perStep;

                    if (viTriTiepTheo.Y >= this.location.X * Setting.LBL_SIZE + (this.location.X + 1) * Setting.line_Width)
                    {
                        this.lblFigure.Location = viTriDichCuaLbl;
                        if (isMerged)
                        {
                            this.status = Status.MERGE;
                            UpdateColorAndText();
                        }
                        else
                            this.status = Status.NONE;
                    }
                    else
                        this.lblFigure.Location = viTriTiepTheo;
                    break;
                case Status.LEFT:
                    viTriTiepTheo.X -= perStep;

                    if (viTriTiepTheo.X <= this.location.Y * Setting.LBL_SIZE + (this.location.Y + 1) * Setting.line_Width)
                    {
                        this.lblFigure.Location = viTriDichCuaLbl;
                        if (isMerged)
                        {
                            this.status = Status.MERGE;
                            UpdateColorAndText();
                        }
                        else
                            this.status = Status.NONE;
                    }
                    else
                        this.lblFigure.Location = viTriTiepTheo;
                    break;
                case Status.RIGHT:
                    viTriTiepTheo.X += perStep;
                    
                    if (viTriTiepTheo.X >= this.location.Y * Setting.LBL_SIZE + (this.location.Y + 1) * Setting.line_Width)
                    {
                        this.lblFigure.Location = viTriDichCuaLbl;
                        
                        if (isMerged)
                        {
                            this.status = Status.MERGE;
                            UpdateColorAndText();
                        }
                        else
                            this.status = Status.NONE;
                    }
                    else
                        this.lblFigure.Location = viTriTiepTheo;
                    break;
                default:
                    break;
            }

        }
        #endregion

        #region Animation

        /// <summary>
        /// Trả về true nếu đã đạt trạng thái kết thúc.
        /// </summary>
        /// <returns></returns>
        public bool CreateAnimation()
        {
            int x = sCaleAni.Next();
            this.lblFigure.Size = new Size(x, x);
            this.lblFigure.Location = new Point(lblFigure.Location.X - Setting.SCALE_STEP / 2, lblFigure.Location.Y - Setting.SCALE_STEP / 2); // size tang 1 khoang x thi location phai giam 1 khoang x/2

            if (this.lblFigure.Size.Width >= Setting.LBL_SIZE)
            {
                this.status = Status.NONE;
                return true;
            }

            return false;
        }


        /// <summary>
        /// Trả về true nếu đã đạt trạng thái kết thúc.
        /// </summary>
        /// <returns></returns>
        public bool MergeAnimation()
        {
            int x = mergeAni.Next();
            this.lblFigure.Size = new Size(x, x);
            //lblFigure.Scale(new SizeF(50, 50));

            if (mergeAni.thuan)
            {
                this.lblFigure.Location = new Point(lblFigure.Location.X - 6 / 2, lblFigure.Location.Y - 6 / 2); // size tang 1 khoang x thi location phai giam 1 khoang x/2
                if (x == mergeAni.endValue) mergeAni.thuan = false;
            }
            else
            {
                this.lblFigure.Location = new Point(lblFigure.Location.X + 6 / 2, lblFigure.Location.Y + 6 / 2); // size giam 1 khoang x thi location phai tang 1 khoang x/2
                if (x == mergeAni.startValue) mergeAni.thuan = true;
            }
            if (this.lblFigure.Size.Width <= Setting.LBL_SIZE)
                return true;

            return false;
        }
        #endregion

        #region Static Method
        

        /// <summary>
        /// Tạo giá trị ban đầu cho đối tượng.
        /// </summary>
        private static int GenerateValue()
        {
            int temp = random.Next(10);
            if (temp == 5)
                return 4;
            else return 2;
        }
        #endregion

        #region Methos
        /// <summary>
        /// Sao chép đối tượng hiện tại.
        /// </summary>
        /// <returns></returns>
        public Square Clone()
        {
            Square newSquare = new Square(this.location, false);
            newSquare.value = this.value;
            newSquare.lblFigure.Size = this.lblFigure.Size;
            newSquare.lblFigure.Location = this.lblFigure.Location;
            newSquare.UpdateColorAndText();

            return newSquare;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            lblFigure.Dispose();
        }

        public void UpdateColorAndText()
        {
            int[] temp = Setting.GET_COLOR_AND_TEXTSIZE(this.value);
            lblFigure.BackColor = Color.FromArgb(temp[0], temp[1], temp[2]);
            if (this.value > 4)
                this.lblFigure.ForeColor = Color.White;
            else
                this.lblFigure.ForeColor = Color.Gray;
            lblFigure.Text = this.value.ToString();
            lblFigure.Font = new Font("Consolas", temp[3], FontStyle.Bold);
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
        MERGE,
        NONE
    }

    public class SCaleAnimation
    {
        public int endValue;
        public int step;
        public int curValue;
        public bool thuan;
        public int startValue;

        public SCaleAnimation()
        {

        }
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


    public class MergeEventArg : EventArgs
    {
        public int score;
        public Square squareDis;

        public MergeEventArg(Square squareDis, int score)
        {
            this.score = score;
            this.squareDis = squareDis;
        }
    }
}
