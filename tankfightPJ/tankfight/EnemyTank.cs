using System;
using System.Drawing;
using tankfight.Properties;

namespace tankfight
{
    enum TankTag {
        Gray,
        Green,
        Quick,
        Slow
    }
    class EnemyTank : Movething
    {
        public TankTag tankTag;
        public int ChangeDirSpeed { get; set; }
        private int changeDirCount = 0;
        public int AttackSpeed { get; set; }
        public bool notmove = false;//作为敌人行动的状态量 但会有持续时间(定时道具触发时)
        public int pasttime = 0;//定时器触发的时间
        public int nowtime = 0;//定时器需要结束的时间
        private int attackCount = 0;
        private  int startchangesum = 0;//
        private  Random r = new Random();
        public EnemyTank tank = null;
        Rectangle rect;
        private static Bitmap[] bmpArray1 = new Bitmap[] {//敌人出生前的提示图数组
            Resources.Star1,
            Resources.Star2,
            Resources.Star3
        };
        private int startEffecttime = 30;//最大绘制时间
        private int startEffectcount = 0;
        private bool start = false;
        public EnemyTank(int x, int y, int speed, Bitmap bmpDown, Bitmap bmpUp, Bitmap bmpRight, Bitmap bmpLeft,TankTag tankTag)
        {
            this.X = x;
            this.Y = y;
            this.Speed = speed;
            BitmapDown = bmpDown;
            BitmapUp = bmpUp;
            BitmapRight = bmpRight;
            BitmapLeft = bmpLeft;
            this.Dir = Direction.Down;
            Width = BitmapDown.Width;
            Height = BitmapDown.Height;
            AttackSpeed = 50;
            ChangeDirSpeed = 80;
            this.tankTag = tankTag;
        }
        public  void DrawEffect(int x, int y)
        {
            Graphics g = GameFramework.g;


            if (startchangesum < 3)
            {
                bmpArray1[startchangesum].MakeTransparent(Color.Black);
                g.DrawImage(bmpArray1[startchangesum], x, y);
                startchangesum++;
                return;
            }
            startchangesum = 0;
        }

        public override void Update()//完整运动逻辑
        {
            rect = GetRectangle();
            if (!start)
            {
                DrawEffect(X - 5, Y - 5);

            }
            startEffectcount++;
            if (startEffectcount < startEffecttime) return;
            if (!notmove)
            {
                pasttime++;//随着坦克的所能移动的时间一直增加
                Movesystem();
                AttackCheck();               
                AutoChangeDirection();
                nowtime = pasttime;//nowtime记录下pasttime方便计算时间差
            }
            if (notmove) {
                if ((nowtime - pasttime) > 150)
                {
                    notmove = false;
                    pasttime = 0;//数据归零以防溢出
                }
                nowtime++;//从坦克不能动是开始计算
                          //
            }
            start = true;
            base.Update();//基类在地图上绘制图片的方法
        }
        #region 敌人坦克的运动系统
        public void Movesystem() {
               MoveCheck();
           if (tank != null) {
                enemyTankmeessage(tank); //如果和另一辆敌人坦克碰撞那么立马向他发送消息
            }                            //让两辆将要对碰的坦克同时做出响应
            if (MoveCheck())//移动检查
                Move();
        }
        public void enemyTankmeessage(EnemyTank tank) {
            int time = 0;
            do
            {
                time++;
                ChangeDirection();
            } while (( GameObjectManager.EnemyIsCollidedEnemyTank(this, gettankcolidedRectangle())) == null && time < 1000);
           
            int time1 = 0;
            if (tank != null)
            {
                do
                {
                    time1++;
                    tank.ChangeDirection();
                } while ((GameObjectManager.EnemyIsCollidedEnemyTank(tank, tank.gettankcolidedRectangle())) == null && time1 < 1000);//因为我在计算碰撞矩形时并没有像实际开发游戏时的准确性那么高
            }                                               //为了防止线程阻塞，给一个最大计算次数,当然这也可能导致敌人坦克碰撞后没有完全分离，这在并不是很精确的开发中
                                                            //也是无法避免的，但坦克最终还是会分                                                                 
        }
        public bool MoveCheck()
        {
            #region 检查有没有超过窗体边界
            if (Dir == Direction.Up)
            {
                if (Y - Speed < 0)
                {
                    ChangeDirection(); return false;
                }
            }
            else if (Dir == Direction.Down)
            {
                if (Y + Speed + Height > 450)
                {
                    ChangeDirection(); return false;
                }
            }
            else if (Dir == Direction.Left)
            {
                if (X - Speed < 0)
                {
                    ChangeDirection(); return false;
                }
            }
            else if (Dir == Direction.Right)
            {
                if (X + Speed + Width > 450)
                {
                    ChangeDirection(); return false;
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
                if (Y >= (14 * 30)) {
                    for (int i = 0; i < 3; i++)  //Y坐标>=14*30说明在boss附近要连续攻击
                        Attack();
                        }
                AttackCheck(); ChangeDirection(); return false;
            }
            if (GameObjectManager.IsCollidedSteel(rect) != null)
            {
                ChangeDirection(); return false;
            }
            if (GameObjectManager.IsCollidedBoss(rect))
            {
                Attack(); ChangeDirection(); return false;
            }
            if ((GameObjectManager.IsCollidedMyTank(rect)) != null)
            {
                Attack(); ChangeDirection(); return false;
            }
              
            if ((tank = GameObjectManager.EnemyIsCollidedEnemyTank(this, gettankcolidedRectangle())) != null)
            {
                ChangeDirection(); return false;
            }
            return true;
        }
        #endregion
        #region  敌人坦克与敌人坦克碰撞时选取的不同矩形碰撞
        public Rectangle gettankcolidedRectangle()
        {
            Rectangle rectangle = new Rectangle(X, Y, Width, Height);
            if (tankTag == TankTag.Slow) {
                switch (Dir)
                {
                    case Direction.Up:
                        rectangle = new Rectangle(X, Y - Speed, Width, Height - 24 + Speed);
                        break;
                    case Direction.Down:
                        rectangle = new Rectangle(X, Y + 24, Width, Height - 24 + Speed);
                        break;
                    case Direction.Left:
                        rectangle = new Rectangle(X - Speed, Y, Width - 24 + Speed, Height);
                        break;
                    case Direction.Right:
                        rectangle = new Rectangle(X + 24, Y, Width - 24 + Speed, Height);
                        break;
                }
            }
            else {
                switch (Dir)
                {
                    case Direction.Up:
                        rectangle = new Rectangle(X, Y - Speed, Width, Height -26+Speed);
                        break;
                    case Direction.Down:
                        rectangle = new Rectangle(X, Y + 26, Width, Height - 26 + Speed);
                        break;
                    case Direction.Left:
                        rectangle = new Rectangle(X - Speed, Y, Width - 26 + Speed, Height);
                        break;
                    case Direction.Right:
                        rectangle = new Rectangle(X + 26, Y, Width - 26 + Speed, Height);
                        break;
                }
                }
            return rectangle;
        }
        #endregion
        #region 坦克的改变运动方向的系统
        private void AutoChangeDirection()
        {
            changeDirCount++;
            if (changeDirCount < ChangeDirSpeed) return;
            ChangeDirection();
            int nextchangDirspeed;
            while (true)
            {
                nextchangDirspeed = r.Next(80, 120);// 80~119
                if (ChangeDirSpeed == nextchangDirspeed)
                {
                    continue;                  //必须要得到跟之前不一样的改变方向的时间才退出
                }
                {
                    ChangeDirSpeed = nextchangDirspeed; break;
                }
            }
            changeDirCount = 0;
        }
        #region 让坦克更容易打到boss的算法(改变方向时根据权值来选择)
        public void ChangeDirection()
        {

            int a = r.Next(0, 100);//用一个区间来作为权值让坦克优先选择那个方向
            Direction dir=Direction.Up;
            if (Dir == Direction.Up) {
                if (a < 50)
                    dir = Direction.Down;
                else if (a < 70)
                    dir = Direction.Left;
                else
                dir = Direction.Right; 
            }
            else if (Dir == Direction.Down) {
                if (a < 30)
                    dir = Direction.Left;
                else if (a < 80)
                    dir = Direction.Right;
                else 
                    dir = Direction.Up;
            }
            else if (Dir == Direction.Right) {
                if (a < 40) dir = Direction.Down;
                else if (a < 80) dir = Direction.Left;
                else dir = Direction.Up;
            }
             else if (Dir == Direction.Left) {//0 1 2 3
                if (a < 30)                   //上下右左
                    dir = Direction.Right;
                else if (a < 80)
                    dir = Direction.Down;
                else
                    dir = Direction.Up;
            }
            // 0 1 2 3
            Dir = dir;
        }
        #endregion
        #endregion
        private void Move()
        {  
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
#region 敌人坦克的攻击系统
        private void AttackCheck()
        {
            attackCount++;
            if (attackCount < AttackSpeed) return;
            AttackSpeed = r.Next(0,90);
            Attack();
            attackCount = 0;
        }

        private void Attack()
        {
            //发射子弹
            int x = this.X;
            int y = this.Y;

            switch (Dir)
            {
                case Direction.Up:
                    x = x + Width / 2;
                    break;
                case Direction.Down:
                    x = x + Width / 2;
                    y += Height;
                    break;
                case Direction.Left:
                    y = y + Height / 2;
                    break;
                case Direction.Right:
                    x += Width;
                    y = y + Height / 2;
                    break;
            }

            if (tankTag == TankTag.Green)
            {
                GameObjectManager.CreateBullet(x, y, Tag.Green, Dir);
                return   ;
            }
            GameObjectManager.CreateBullet(x, y, Tag.EnemyTank, Dir);
        }
        #endregion
    }
}
