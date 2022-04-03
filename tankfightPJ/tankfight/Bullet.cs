using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tankfight.Properties;

namespace tankfight
{
    enum Tag
    {
        MyTank,
        EnemyTank,
        Green
    }
    class Bullet : Movething
    {
        public Tag Tag { get; set; }

        public bool IsDestroy { get; set; }

        public Bullet(int x, int y, int speed, Direction dir, Tag tag)
        {
            IsDestroy = false;
            this.X = x;
            this.Y = y;
            this.Speed = speed;
            BitmapDown = Resources.BulletDown;
            BitmapUp = Resources.BulletUp;
            BitmapRight = Resources.BulletRight;
            BitmapLeft = Resources.BulletLeft;
            this.Dir = dir;
            this.Tag = tag;

            this.X -= Width / 2;
            this.Y -= Height / 2;
        }

        public override void Update()
        {
            MoveCheck();//移动检查
            Move();
            if (IsDestroy == false)
            {
                base.Update();
            }
        }

        private void MoveCheck()
        {

            #region 检查有没有超过窗体边界
            if (Dir == Direction.Up)
            {
                if (Y + Height / 2 + 3 < 0)
                {
                    IsDestroy = true; return;
                }
            }
            else if (Dir == Direction.Down)
            {
                if (Y + Height / 2 - 3 > 450)
                {
                    IsDestroy = true; return;
                }
            }
            else if (Dir == Direction.Left)
            {
                if (X + Width / 2 - 3 < 0)
                {
                    IsDestroy = true; return;
                }
            }
            else if (Dir == Direction.Right)
            {
                if (X + Width / 2 + 3 > 450)
                {
                    IsDestroy = true; return;
                }
            }
            #endregion


            //检查有没有和其他元素发生碰撞

            Rectangle rect = GetRectangle();

            rect.X = X + Width / 2 - 3;
            rect.Y = Y + Height / 2 - 3;
            rect.Height = 6;
            rect.Width = 6;

            //1、墙 2、钢墙 3、坦克
            int xExplosion = this.X + Width / 2;
            int yExplosion = this.Y + Height / 2;

            NotMovething wall = null;
            if ((wall = GameObjectManager.IsCollidedWall(rect)) != null)
            {
                IsDestroy = true;
                GameObjectManager.DestroyWall(wall);
                GameObjectManager.CreateExplosion(xExplosion, yExplosion);
                SoundMananger.PlayBlast();
                return;
            }
            if ((wall=GameObjectManager.IsCollidedSteel(rect) )!= null)
            {
                GameObjectManager.CreateExplosion(xExplosion, yExplosion);
                SoundMananger.PlayHit();
                IsDestroy = true;
                if (Tag == Tag.Green)
                {
                    GameObjectManager.Destorysteel(wall);
                }
                return;
            }   
            if (GameObjectManager.IsCollidedBoss(rect))
            {
                SoundMananger.PlayBlast();
                GameFramework.ChangeToGameOver(); return;
            }

            if (Tag == Tag.MyTank)
            {
                EnemyTank tank = null; Bullet bullet = null;
                if ((tank = GameObjectManager.IsCollidedEnemyTank(rect)) != null)
                {
                    IsDestroy = true;
                    GameObjectManager.Createpropos(tank.X,tank.Y);
                    SoundMananger.PlayHit();
                    GameObjectManager.DestroyTank(tank);                 
                    GameObjectManager.CreateExplosion(xExplosion, yExplosion);                    
                    return;
                }
                if ((bullet = GameObjectManager.IsCollidedBullet(rect)) != null)
                {
                    IsDestroy = true;
                    GameObjectManager.DestoryBullet(bullet);
                    GameObjectManager.CreateExplosion(xExplosion, yExplosion);

                }
            }
            else if (Tag == Tag.EnemyTank||Tag==Tag.Green)
            {
                MyTank damagetank =null;
                if ((damagetank = GameObjectManager.IsCollidedMyTank(rect)) != null)
                {
                    IsDestroy = true;
                    GameObjectManager.CreateExplosion(xExplosion, yExplosion);
                    SoundMananger.PlayBlast();
                    if (damagetank == GameObjectManager.myTank)
                        GameObjectManager.myTank.TakeDamage();
                    else {
                        GameObjectManager.myTank1.TakeDamage();
                    }
                    return;
                }
            }
        }



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
    }
}
