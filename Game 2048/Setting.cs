

namespace ver3
{
    public static class Setting
    {
        public static int WIDTH = 4; // so luong phan tu cua mang
        public static int line_Width = 10;
        public static int LBL_SIZE = 100;
        public static int LBL_START_SIZE = 76;
        public static int SCALE_STEP = 8;

        public static int[,] colorArray = new int[,]
        {
            {204, 192, 180 , 50},
            {228, 228, 218 , 50}, //2
            {237, 224, 195 , 50}, //4
            {254, 183, 113 , 50}, //8 
            {255, 162, 89  , 40}, //16
            {255, 143, 87  , 40}, //32
            {255, 117, 49  , 40}, //64
            {241, 203, 102 , 30}, //128
            {245, 198, 86  , 30}, //256
            {245, 195, 65  , 30}, //512
            {247, 190, 48  , 25}, //1024
            {247, 187, 30  , 25}, //2048
            {60 , 58 , 49  , 25}, //4096
            {70 , 50 , 59  , 25}
        };

        
        public static int[] GET_COLOR_AND_TEXTSIZE(int x)
        {
            switch (x)
            {
                case 2:
                    return new int[] { colorArray[1, 0], colorArray[1, 1], colorArray[1, 2] , colorArray[1,3]};
                case 4:
                    return new int[] { colorArray[2, 0], colorArray[2, 1], colorArray[2, 2], colorArray[2, 3] };
                case 8:
                    return new int[] { colorArray[3, 0], colorArray[3, 1], colorArray[3, 2], colorArray[3, 3] };
                case 16:
                    return new int[] { colorArray[4, 0], colorArray[4, 1], colorArray[4, 2], colorArray[4, 3] };
                case 32:
                    return new int[] { colorArray[5, 0], colorArray[5, 1], colorArray[5, 2], colorArray[5, 3] };
                case 64:
                    return new int[] { colorArray[6, 0], colorArray[6, 1], colorArray[6, 2], colorArray[6, 3] };
                case 128:
                    return new int[] { colorArray[7, 0], colorArray[7, 1], colorArray[7, 2], colorArray[7, 3] };
                case 256:
                    return new int[] { colorArray[8, 0], colorArray[8, 1], colorArray[8, 2], colorArray[8, 3] };
                case 512:
                    return new int[] { colorArray[9, 0], colorArray[9, 1], colorArray[9, 2], colorArray[9, 3] };
                case 1024:
                    return new int[] { colorArray[10, 0], colorArray[10, 1], colorArray[10, 2], colorArray[10, 3] };
                case 2048:
                    return new int[] { colorArray[11, 0], colorArray[11, 1], colorArray[11, 2], colorArray[11, 3] };
                case 4096:
                    return new int[] { colorArray[12, 0], colorArray[12, 1], colorArray[12, 2], colorArray[12, 3] };
                default:
                    return new int[] { colorArray[13, 0], colorArray[13, 1], colorArray[13, 2], colorArray[13, 3] };
            }
        }

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
    }
}
