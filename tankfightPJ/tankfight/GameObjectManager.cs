using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using tankfight.Properties;

namespace tankfight
{
    class GameObjectManager
    {
        private static List<NotMovething> wallList = new List<NotMovething>();//静态元素墙的列表
        private static List<NotMovething> steelList = new List<NotMovething>();//静态元素钢墙的列表
        private static List<NotMovething> propsList = new List<NotMovething>();//静态元素道具的列表
        private static NotMovething boss;//老巢的boos静态对象
        public static MyTank myTank;//玩家一的静态对象
        public static MyTank myTank1;// 玩家二的静态对象
        public static List<EnemyTank> tankList = new List<EnemyTank>();//敌人坦克的列表
        private static List<Bullet> bulletList = new List<Bullet>();//子弹的列表
        private static List<Explosion> expList = new List<Explosion>();//爆炸特效的列表
        private static int enemyBornSpeed = 180;//敌人坦克的出生速度
        private static int enemyBornCount = 0;// 敌人坦克出生的计数器
        private static int enemycount = 0;//控制产生敌人
        public static int enemysum = 15;//用来记录敌人总数的变化
        public static int enemynum = 15;//用来记录所需要产生的敌人总数
        private static Random r = new Random((int)DateTime.Now.Ticks);//产生随机数来确定击杀敌人坦克后是否有道具,或对应敌人数组下标
        private static Point[] points = new Point[3];//Point 对象数组包含三个随机点的坐标。
        #region 敌人坦克的随机生成点
        public static void Start()
        {
            points[0].X = 0; points[0].Y = 0;

            points[1].X = 7 * 30; points[1].Y = 0;

            points[2].X = 14 * 30; points[2].Y = 0;

        }//敌人坦克的随机生成点
        #endregion
        #region 游戏地图的更新
        public static void Update()
        {
            try
            {
                foreach (NotMovething nm in wallList)
                {
                    nm.Update();
                }
                foreach (NotMovething nm in steelList)
                {
                    nm.Update();
                }

                foreach (EnemyTank tank in tankList)
                {
                    tank.Update();
                }
            }
            catch (Exception e) { }
            CheckAndDestroyBullet();
            try
            {
                foreach (Bullet bullet in bulletList)
                {
                    bullet.Update();
                }
            }
            catch (Exception e) { }//子弹已经被删除，那就跳出不做任何操作 


               CheckAndDestroyExplosion();
            foreach (Explosion exp in expList)
            {
                exp.Update();
            }
            

            if (boss != null)
            {
                boss.Update();
            }
            CheckPlayerisdeadAndgamelose();
            if (myTank != null)
            {
                myTank.Update();
            }
            if (myTank1 != null) {
                myTank1.Update();
            }
            CheckAndDestroyprops();
            foreach (NotMovething props in propsList)
            {
                props.Update();
            }
            EnemyBorn();
        }

        private static void CheckAndDestroyBullet()
        {
            List<Bullet> needToDestroy = new List<Bullet>();
            foreach (Bullet bullet in bulletList)
            {
                if (bullet.IsDestroy == true)
                {
                    needToDestroy.Add(bullet);
                }
            }
            foreach (Bullet bullet in needToDestroy)
            {
                bulletList.Remove(bullet);
            }
        }
        private static void CheckAndDestroyprops() {
            List<NotMovething> needToDestroy = new List<NotMovething>();
            foreach (NotMovething props in propsList)
            {
                if (props.Isneeddestory1 == true)
                {
                    needToDestroy.Add(props);
                }
            }
            foreach (NotMovething props in needToDestroy)
            {
                propsList.Remove(props);
            }
        }
        private static void CheckPlayerisdeadAndgamelose() {
            if(myTank!=null)
            if (myTank.isneedDestory == true) {
                myTank = null;
            }
            if(myTank1!=null)
            if (myTank1.isneedDestory == true) {
                myTank1 = null;
            }
            if (GameFramework.doublePlayer == false)
            { 
                if(myTank==null)GameFramework.ChangeToGameOver();
            }
            if (GameFramework.doublePlayer == true)
                if (myTank == null && myTank1 == null)
                {
                    GameFramework.ChangeToGameOver();
                }
        }
        private static void CheckAndDestroyExplosion()
        {
            List<Explosion> needToDestroy = new List<Explosion>();
            foreach (Explosion exp in expList)
            {
                if (exp.IsNeedDestroy == true)
                {
                    needToDestroy.Add(exp);
                }
            }
            foreach (Explosion exp in needToDestroy)
            {
                expList.Remove(exp);
            }
        }
        #endregion
        #region 游戏模式下的UI的创建
        public static void CreateIcon()
        {
            float c = GameObjectManager.enemysum - 10;
            if (c > 10) c = 10;//如果敌人多余20那么也只绘制20个图标
            Bitmap enemyIcon = Resources.enemyIcon;
            enemyIcon.MakeTransparent();
            if (GameObjectManager.enemysum < 11)

                for (float i = 1; i < GameObjectManager.enemysum + 1; i++) //根据enemysum计算循环条件
                    GameFramework.g1.DrawImage(enemyIcon, 50, 40 * (0.5f * i + 0.5f) - 30);
            if (GameObjectManager.enemysum > 10)
            {
                for (float i = 1; i < 10 + 1; i++) //根据enemysum计算循环条件
                    GameFramework.g1.DrawImage(enemyIcon, 50, 40 * (0.5f * i + 0.5f) - 30);

                for (float i = 1; i < c + 1; i++) //根据enemysum计算循环条件
                    GameFramework.g1.DrawImage(enemyIcon, 75, 40 * (0.5f * i + 0.5f) - 30);

            }
            if (myTank != null) { CreateMytankIcon(myTank); }
            if (myTank1 != null) { CreateMytankIcon(myTank1); }

        }
        public static void CreateMytankIcon(MyTank tank)
        {
            if (tank != null)
            {
                Bitmap MytanklifeIcon = Resources.MyTankUp; int Mytanklife = 0;
                if (tank.life > 5)
                {
                    Mytanklife = 5;
                }
                else
                {

                    Mytanklife = tank.life;
                }
                for (float i = 1; i < Mytanklife + 1; i++)
                {
                    if (tank.player == PlayerTag.Player1)
                        GameFramework.g1.DrawImage(MytanklifeIcon, 50, 55 * (0.5f * i + 0.5f) + 180);
                    else
                    {
                        MytanklifeIcon = Resources.YellowUp;
                        GameFramework.g1.DrawImage(MytanklifeIcon, 80, 55 * (0.5f * i + 0.5f) + 180);
                    }
                }
            }
        }
        #endregion
        #region 道具、子弹、爆炸特效的创建
        public static void CreateExplosion(int x, int y)
        {
            Explosion.effectTag = EffectTag.explosion;
            Explosion exp = new Explosion(x, y);
            expList.Add(exp);
        }
        public static void Createpropos(int x,int y){
            int a = r.Next(0, 5); PropsTag props=PropsTag.none;
            if (a==0)
          props = PropsTag.boom;
            if (a == 1)
                props = PropsTag.timer;
            if (props == PropsTag.boom)
            {
                NotMovething props1 = new NotMovething(x, y, Resources.GEMGRENADE);
                props1.Props = PropsTag.boom;
                propsList.Add(props1);
            }
            if (props == PropsTag.timer)
            {  
                NotMovething props1 = new NotMovething(x, y, Resources.GEMCLOCK);
                props1.Props = PropsTag.timer;
                propsList.Add(props1);
            }

        }
        public static void CreateBullet(int x, int y, Tag tag, Direction dir)
        {
            Bullet bullet;
            if (tag == Tag.MyTank) { bullet = new Bullet(x, y, 50, dir, tag); }
            bullet = new Bullet(x, y, 8, dir, tag);


            bulletList.Add(bullet);

        }
        #endregion
        #region 摧毁地图上的元素
        public static void DestroyWall(NotMovething wall)
        {
            wallList.Remove(wall);
        }
        public static void Destorysteel(NotMovething wall) {

            steelList.Remove(wall);
        }
        public static void DestoryBullet(Bullet bullet)
        {

            bulletList.Remove(bullet);
        }
        public static void DestroyTank(EnemyTank tank)
        {         
            tankList.Remove(tank);
            enemysum--;
            if (enemysum == 0)
            {
                GameFramework.ChangeToWin();
            }
            
        }
        #endregion
        #region 根据计数器和随机数在地图以固定间隔时间产生随机敌人坦克
        private static void EnemyBorn()
        {


            enemyBornCount++;
            if (enemyBornCount < enemyBornSpeed) return;
            if (enemycount > enemynum-1) return;
            enemycount++;
            //0-2
            Random rd = new Random();
            int index = rd.Next(0, 3);
            Point position = points[index];
            Rectangle rect=new Rectangle(points[index].X - 10, points[index].Y, 60, 40);
            if ((GameObjectManager.IsCollidedEnemyTank(rect)) != null) { enemycount--; return ; }//因为回滚坦克并未产生，所以将计数器减一
            if ((GameObjectManager.IsCollidedMyTank(rect)) != null) { enemycount--;  return ; }//检测出生点是否有坦克有则暂时不产生坦克;
                int enemyType = rd.Next(1, 5);
            
            switch (enemyType)
            {
                case 1:
                    CreateEnemyTank1(position.X, position.Y);
                    break;
                case 2:
                    CreateEnemyTank2(position.X, position.Y);
                    break;
                case 3:
                    CreateEnemyTank3(position.X, position.Y);
                    break;
                case 4:
                    CreateEnemyTank4(position.X, position.Y);
                    break;

            }

            enemyBornCount = 0;
        }
        #endregion
        #region 产生不同敌人坦克的函数
        private static void CreateEnemyTank1(int x, int y)
        {
            EnemyTank tank = new EnemyTank(x, y, 1, Resources.GrayDown, Resources.GrayUp, Resources.GrayRight, Resources.GrayLeft,TankTag.Gray);
            tankList.Add(tank);
        }
        private static void CreateEnemyTank2(int x, int y)
        {

            EnemyTank tank = new EnemyTank(x, y, 1, Resources.GreenDown, Resources.GreenUp, Resources.GreenRight, Resources.GreenLeft,TankTag.Green);
            tankList.Add(tank);
        }
        private static void CreateEnemyTank3(int x, int y)
        {

            EnemyTank tank = new EnemyTank(x, y, 3, Resources.QuickDown, Resources.QuickUp, Resources.QuickRight, Resources.QuickLeft,TankTag.Quick);
            tankList.Add(tank);
        }
        private static void CreateEnemyTank4(int x, int y)
        {

            EnemyTank tank = new EnemyTank(x, y, 2, Resources.SlowDown, Resources.SlowUp, Resources.SlowRight, Resources.SlowLeft,TankTag.Slow);
            tankList.Add(tank);
        }
        #endregion
        #region 游戏中的碰撞系统
        public static NotMovething IsCollidedWall(Rectangle rt)
        {
            foreach (NotMovething wall in wallList)
            {
                if (wall.GetRectangle().IntersectsWith(rt))
                {
                    return wall;
                }
            }
            return null;
        }
        public static NotMovething IsCollidedSteel(Rectangle rt)
        {
            foreach (NotMovething wall in steelList)
            {
                if (wall.GetRectangle().IntersectsWith(rt))
                {
                    return wall;
                }
            }
            return null;
        }
        public static bool IsCollidedBoss(Rectangle rt)
        {
            return boss.GetRectangle().IntersectsWith(rt);
        }
        public static MyTank IsCollidedMyTank(Rectangle rt)
        {
            if (myTank != null)
            {
                if (myTank.GetRectangle().IntersectsWith(rt)) return myTank;
            }
            if (myTank1 != null)
            {
                if (myTank1.GetRectangle().IntersectsWith(rt)) return myTank1;
            }
             return null;
        }
        public static EnemyTank IsCollidedEnemyTank(Rectangle rt)
        {
            foreach (EnemyTank tank in tankList)
            {                           
                    if (tank.GetRectangle().IntersectsWith(rt)) 
                    {

                        return tank;
                    }          
            }
                return null;
        }
        public static EnemyTank EnemyIsCollidedEnemyTank(EnemyTank self,Rectangle rt)
        {
            foreach (EnemyTank tank in tankList)
            {
                if (self.Equals(tank))
                    return null;
                else if(tank.GetRectangle().IntersectsWith(rt))
                {                                                           
                       return tank;
                }
            }
            return null;
        }
        public static EnemyTank MytankIsCollideMyTank(MyTank self, Rectangle rt)
        {
            foreach (EnemyTank tank in tankList)
            {
                if (self.Equals(tank))
                    return null;
                else if (tank.GetRectangle().IntersectsWith(rt))
                {
                    return tank;
                }
            }
            return null;
        }
        public static Bullet IsCollidedBullet(Rectangle rt)
        {
            foreach (Bullet bullet in bulletList)
            {
                if (bullet.GetRectangle().IntersectsWith(rt))
                {
                    if (bullet.Tag == Tag.EnemyTank)
                        return bullet;
                }
            }
            return null;
        }
        #endregion
        #region Mytank的创建
        public static void CreateMyTank()
        {
            int x = 5 * 30;
            int y = 14 * 30;
            int x1 = 9 * 30+5;
            int y1 = 14 * 30;
            if(!(GameFramework.doublePlayer))
            myTank = new MyTank(x, y, 2,PlayerTag.Player1);
            else
            {
                myTank = new MyTank(x, y, 2, PlayerTag.Player1);
                myTank1 = new MyTank(x1, y1, 2, PlayerTag.Player2);
            }

        }
        #endregion
        #region 地图上静态元素的生成
        public static void CreateMap()
        {
            CreateWall(1, 1, 5, Resources.wall, wallList);
            CreateWall(3, 1, 5, Resources.wall, wallList);
            CreateWall(5, 1, 4, Resources.wall, wallList);
            CreateWall(7, 1, 3, Resources.wall, wallList);
            CreateWall(9, 1, 4, Resources.wall, wallList);
            CreateWall(11, 1, 5, Resources.wall, wallList);
            CreateWall(13, 1, 5, Resources.wall, wallList);

            CreateWall(7, 5, 1, Resources.steel, steelList);

            CreateWall(0, 7, 1, Resources.steel, steelList);

            CreateWall(3, 6, 1, Resources.wall, wallList);
            CreateWall(4, 6, 1, Resources.wall, wallList);
            CreateWall(5, 6, 1, Resources.wall, wallList);
            CreateWall(6, 7, 1, Resources.wall, wallList);
            CreateWall(7, 6, 2, Resources.wall, wallList);
            CreateWall(8, 7, 1, Resources.wall, wallList);
            CreateWall(9, 6, 1, Resources.wall, wallList);
            CreateWall(10, 6, 1, Resources.wall, wallList);
            CreateWall(11, 6, 1, Resources.wall, wallList);

            CreateWall(14, 7, 1, Resources.steel, steelList);

            CreateWall(1, 9, 5, Resources.wall, wallList);
            CreateWall(3, 9, 5, Resources.wall, wallList);
            CreateWall(5, 9, 3, Resources.wall, wallList);

            CreateWall(6, 10, 1, Resources.wall, wallList);
            CreateWall(7, 10, 2, Resources.wall, wallList);
            CreateWall(8, 10, 1, Resources.wall, wallList);

            CreateWall(9, 9, 3, Resources.wall, wallList);
            CreateWall(11, 9, 5, Resources.wall, wallList);
            CreateWall(13, 9, 5, Resources.wall, wallList);


            CreateWall(6, 13, 2, Resources.wall, wallList);
            CreateWall(7, 13, 1, Resources.wall, wallList);
            CreateWall(8, 13, 2, Resources.wall, wallList);
            Bitmap bitmap = new Bitmap(Resources.Boss);
            bitmap.MakeTransparent(Color.Black);
            CreateBoss(7, 14, bitmap) ;
        }
        private static void CreateBoss(int x, int y, Image img)
        {
            int xPosition = x * 30;
            int yPosition = y * 30;
            boss = new NotMovething(xPosition, yPosition, img);
        }

        private static void CreateWall(int x, int y, int count, Image img, List<NotMovething> wallList)
        {
            int xPosition = x * 30;
            int yPosition = y * 30;
            for (int i = yPosition; i < yPosition + count * 30; i += 15)
            {
                // i xPosition     i xPosition+15
                NotMovething wall1 = new NotMovething(xPosition, i, img);//左边
                NotMovething wall2 = new NotMovething(xPosition + 15, i, img);//右边
                wallList.Add(wall1);
                wallList.Add(wall2);
            }
        }
        #endregion
        #region 退出游戏后的画布上的元素清除以及其它元素值回归初值
        public static void Gamequitclear()
        {
            wallList.Clear();
            steelList.Clear();
            tankList.Clear();
            bulletList.Clear();
            expList.Clear();
            propsList.Clear();
            enemysum = 15;
            enemycount = 0;
            enemynum = 15;
            boss = null;
            myTank = null;
            myTank1 = null;
        }
        #endregion
        #region 接受窗体发来的按键消息发送给Mytank的按键事件
        public static void KeyDown(KeyEventArgs args)
        {
            if (myTank!=null)
            {
                myTank.KeyDown(args);               
            }
            if (myTank1 != null)
            {
                myTank1.KeyDown(args);              
            }
        }
        public static void KeyUp(KeyEventArgs args)
        {

            if (myTank != null)
            {
                myTank.KeyUp(args);
            }
            if (myTank1 != null)
            {
                myTank1.KeyUp(args);
            }
        }
        #endregion

    }
}
