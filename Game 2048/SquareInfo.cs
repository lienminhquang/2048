using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ver3
{
    [Serializable]
    public class SquareInfo
    {
        public int value;
        public Point location;
       
        public SquareInfo(Square s)
        {
            if(s == null)
            {
                value = -1;
                location = new Point(-1,-1);
            }
            else
            {
                value = s.value;
                location = s.location;
            }
            
        }

        public SquareInfo()
        {

        }


        /// <summary>
        /// Tra ve square tuong ung
        /// </summary>
        /// <param name="gameBoard"></param>
        /// <param name="squareArr"></param>
        /// <param name="lst"></param>
        /// <returns></returns>
        public Square GetSquare()
        {
            if (value == -1)
                return null;
            Square s = new Square(this.location, false);
            s.value = value;
            s.UpdateColorAndText();
            s.lblFigure.Size = new Size(Setting.LBL_SIZE, Setting.LBL_SIZE);
            Setting.SetDoubleBuffered(s.lblFigure);
            return s;
        }
    }
}
