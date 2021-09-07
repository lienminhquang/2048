

namespace ver3
{
    public class GameInfo
    {
        public bool isEndGame;
        public int bestScore = 0;
        public int score;
        public int previousScore = -1;
        public int soLanUndo = 5;
        public bool readyToUndo = false;

        public SquareInfo[][] arrSquareInf;
        public SquareInfo[][] previousArrInf;

        public GameInfo(int best, int score, int previousScore, int soLanUndo, bool readyToUndo, Square[,] arrSquare, Square[,] previousArr, bool isEndGame)
        {
            this.isEndGame = isEndGame;
            this.bestScore = best;
            this.score = score;
            this.previousScore = previousScore;
            this.soLanUndo = soLanUndo;
            this.readyToUndo = readyToUndo;

            this.arrSquareInf = GetArrSquareInf(arrSquare);
            this.previousArrInf = GetArrSquareInf(previousArr);
        }

        public GameInfo()
        {

        }

        public Game2048 ToGame2048()
        {
            Game2048 g = new Game2048(new System.Drawing.Point(0,100),false);
            Square[,] arr = GetArrSquare(arrSquareInf);
            Square[,] previousArr = GetArrSquare(previousArrInf);

            g.Recovery(arr, previousArr, bestScore, score, soLanUndo, readyToUndo, previousScore);

            return g;
        }


        /// <summary>
        /// Lay mang squareInfo tu mang square
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static SquareInfo[][] GetArrSquareInf(Square[,] arr)
        {
            SquareInfo[][] result = new SquareInfo[4][];
            for (int i = 0; i < 4; i++)
            {
                result[i] = new SquareInfo[4];
                for (int j = 0; j < 4; j++)
                {
                    result[i][j] = new SquareInfo(arr[i, j]);
                }
            }

            return result;
        }

        public static Square[,] GetArrSquare(SquareInfo[][] arrInf)
        {
            Square[,] result = new Square[Setting.WIDTH, Setting.WIDTH];

            for (int i = 0; i < Setting.WIDTH; i++)
            {
                for (int j = 0; j < Setting.WIDTH; j++)
                {
                    result[i, j] = arrInf[i][j].GetSquare();
                }
            }

            return result;
        }
    }
}
