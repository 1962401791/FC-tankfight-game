using System.Drawing;
namespace tankfight
{
    enum Direction
    {
        Up = 0,
        Down = 1,
        Left = 2,
        Right = 3
    }
    class Movething : GameObject
    {
        public Bitmap BitmapUp { get; set; }
        public Bitmap BitmapDown { get; set; }
        public Bitmap BitmapLeft { get; set; }
        public Bitmap BitmapRight { get; set; }
        public object _Lock = new object();
        public int Speed { get; set; }

        private Direction dir;
        public Direction Dir
        {
            get { return dir; }
            set
            {
                dir = value;
                Bitmap bmp = null;

                switch (dir)
                {
                    case Direction.Up:
                        bmp = BitmapUp;
                        break;
                    case Direction.Down:
                        bmp = BitmapDown;
                        break;
                    case Direction.Left:
                        bmp = BitmapLeft;
                        break;
                    case Direction.Right:
                        bmp = BitmapRight;
                        break;
                }

                lock (_Lock)
                {
                    if (Width != bmp.Width && Height != bmp.Height)
                    { 
                        Width = bmp.Width;
                        Height = bmp.Height;
                    }

                }
            }
        }

        protected override Image GetImage()
        {
            lock (_Lock)
            {
                Bitmap bitmap = null;
                switch (Dir)
                {
                    case Direction.Up:
                        bitmap = BitmapUp;
                        break;
                    case Direction.Down:
                        bitmap = BitmapDown;
                        break;
                    case Direction.Left:
                        bitmap = BitmapLeft;
                        break;
                    case Direction.Right:
                        bitmap = BitmapRight;
                        break;
                }
                bitmap.MakeTransparent(Color.Black);

                return bitmap;
            }
        }

    }
}
