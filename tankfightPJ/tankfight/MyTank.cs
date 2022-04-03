using System.Drawing;
using System.Windows.Forms;
using tankfight.Properties;
namespace tankfight
{
    enum PlayerTag
    {
        Player1,
        Player2
    }
    class MyTank : Movething
    {  
        public PlayerTag player;
        public int attckspeed = 5;
        public int attckspeedcount = 5;
        public bool isneedDestory = false;
        public bool IsMoving { get; set; }
        public  int HP { get; set; }
        private int originalX;
        private int originalY;
        public int life= 5;//加上自身血条  三条命
        //
        public MyTank(int x, int y, int speed,PlayerTag player)
        {
            IsMoving = false;
            this.X = x;
            this.Y = y;
            originalX = x;
            originalY = y;
            this.Speed = speed;
            this.player = player;
            if (player == PlayerTag.Player1)
            {
                BitmapDown = Resources.MyTankDown;
                BitmapUp = Resources.MyTankUp;
                BitmapRight = Resources.MyTankRight;
                BitmapLeft = Resources.MyTankLeft;
                Width = Resources.MyTankUp.Width;
                Height = Resources.MyTankUp.Height;
            }
            else {
                BitmapDown = Resources.YellowDown;
                BitmapUp = Resources.YellowUp;
                BitmapRight = Resources.YellowRight;
                BitmapLeft = Resources.YellowLeft;
                Width = Resources.YellowUp.Width;
                Height = Resources.YellowUp.Height;
            }
            this.Dir = Direction.Up;
            HP = 2;
        }

        public override void Update()
        {
            MoveCheck();//移动检查
            Move();

            base.Update();
        }
        #region 玩家的运动系统
        private void MoveCheck()
        {

            #region 检查有没有超过窗体边界
            if (Dir == Direction.Up)
            {
                if (Y - Speed < 0)
                {
                    IsMoving = false; return;
                }
            }
            else if (Dir == Direction.Down)
            {
                if (Y + Speed + Height > 450)
                {
                    IsMoving = false; return;
                }
            }
            else if (Dir == Direction.Left)
            {
                if (X - Speed < 0)
                {
                    IsMoving = false; return;
                }
            }
            else if (Dir == Direction.Right)
            {
                if (X + Speed + Width > 450)
                {
                    IsMoving = false; return;
                }
            }
            #endregion


            //检查有没有和其他元素发生碰撞

            Rectangle rect = GetRectangle();

            switch (Dir)
            {
                case Direction.Up:
                    rect.Y -= Speed;
                    break;
                case Direction.Down:
                    rect.Y += Speed;
                    break;
                case Direction.Left:
                    rect.X -= Speed;
                    break;
                case Direction.Right:
                    rect.X += Speed;
                    break;
            }

            if (GameObjectManager.IsCollidedWall(rect) != null)
            {
                IsMoving = false; return;
            }
            if (GameObjectManager.IsCollidedSteel(rect) != null)
            {
                IsMoving = false; return;
            }
            if (GameObjectManager.IsCollidedBoss(rect))
            {
                IsMoving = false; return;
            }
            if ((GameObjectManager.IsCollidedEnemyTank(rect)) != null)
            {
                IsMoving = false; return;
            }
        }

        private void Move()
        {
            if (IsMoving == false) return;
                switch (Dir)
                {
                    case Direction.Up:
                        Y -= Speed;
                        break;
                    case Direction.Down:
                        Y += Speed;
                        break;
                    case Direction.Left:
                        X -= Speed;
                        break;
                    case Direction.Right:
                        X += Speed;
                        break;
                }
        }
        #endregion
        // GameMainThread  KeyDown
        // 1 2 
        #region 玩家1和玩家2的按键映射
        public void KeyDown(KeyEventArgs args)
        {
            if (player == PlayerTag.Player1)
            switch (args.KeyCode)
            {
                case Keys.W:
                    Dir = Direction.Up;
                    IsMoving = true;
                    break;
                case Keys.S:
                    Dir = Direction.Down;
                    IsMoving = true;
                    break;
                case Keys.A:
                    Dir = Direction.Left;
                    IsMoving = true;
                    break;
                case Keys.D:
                    Dir = Direction.Right;
                    IsMoving = true;
                    break;
               
            }
            else
                switch (args.KeyCode)
                {
                    case Keys.Up:
                        Dir = Direction.Up;
                        IsMoving = true;
                        break;
                    case Keys.Down:
                        Dir = Direction.Down;
                        IsMoving = true;
                        break;
                    case Keys.Left:
                        Dir = Direction.Left;
                        IsMoving = true;
                        break;
                    case Keys.Right:
                        Dir = Direction.Right;
                        IsMoving = true;
                        break;
                }

        }
        public void KeyUp(KeyEventArgs args)
        {
            if (player == PlayerTag.Player1)
            {
                switch (args.KeyCode)
                {
                    case Keys.W:
                        IsMoving = false;
                        break;
                    case Keys.S:
                        IsMoving = false;
                        break;
                    case Keys.A:
                        IsMoving = false;
                        break;
                    case Keys.D:
                        IsMoving = false;
                        break;
                    case Keys.J:
                            Attack();
                        break;
                  
                }
            }
            else {
                switch (args.KeyCode)
                {
                    case Keys.Up:
                        IsMoving = false;
                        break;
                    case Keys.Down:
                        IsMoving = false;
                        break;
                    case Keys.Left:
                        IsMoving = false;
                        break;
                    case Keys.Right:
                        IsMoving = false;
                        break;
                    case Keys.ControlKey:
                            Attack();
                        break;
                }
            }
        }
        #endregion
        #region 玩家的攻击函数
        private void Attack()
        {
            attckspeedcount++;
            if (attckspeedcount < attckspeed) return;
            SoundMananger.PlayFire();//播放开火的声音
            //发射子弹
            int x = this.X;
            int y = this.Y;

            switch (Dir)
            {
                case Direction.Up:
                    x += Width / 2;
                    break;
                case Direction.Down:
                    x += Width / 2;
                    y += Height;
                    break;
                case Direction.Left:
                    y += Height / 2;
                    break;
                case Direction.Right:
                    x += Width;
                    y += Height / 2;
                    break;
            }
            attckspeedcount = 0;

            GameObjectManager.CreateBullet(x, y, Tag.MyTank, Dir);
        }
        #endregion
        #region 玩家被到攻击后受到伤害的函数
        public void TakeDamage()
        {
            HP--;
            if (life == 0) { isneedDestory = true; }
            if (HP <= 0&&life>0)
            {
                X = originalX;
                Y = originalY;
                HP = 2;
                life--;
            }
           
        }
        #endregion
    }
}
