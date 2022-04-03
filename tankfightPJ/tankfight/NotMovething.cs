using System.Drawing;
namespace tankfight
{
    /***
      * 不可以移动的物体
      */
    enum PropsTag
    {   none,
        boom,
        timer
    }
    class NotMovething : GameObject
    {
        private PropsTag props=PropsTag.none;
        private Image img;
        private int isneedDestorytime=360;//道具出现后需要销毁的时间
        private int isneedDestorytimecount =0;
        private bool Isneeddestory = false;
        private int drawtimeinterval = 14;
        private int drawtimecount = 10;//与道具闪动绘制有关的变量
        private int drawtime = 10;
        public Image Img
        {
            get { return img; }
            set
            {
                img = value;
                Width = img.Width;
                Height = img.Height;
            }
        }
        public PropsTag Props { get { return props; }
            set            {
                props = value;   }
             }

        public bool Isneeddestory1 { get => Isneeddestory; set => Isneeddestory = value; }
      
        protected override Image GetImage()
        {
            return Img;
        }
        public override void Update()
        {
            switch (props) {
                case PropsTag.boom:
            BooMeffect();break;//      根据枚举类型确定 道具触发方法
                case PropsTag.timer:
            Timereffect();break;
            }
            if (props != PropsTag.none)
            {
                isneedDestorytimecount++;
                if (isneedDestorytimecount > isneedDestorytime) 
                { 
                    Isneeddestory = true; 
                }
                drawtimecount++;
                if (drawtimecount < drawtime)
                {   if(Isneeddestory==false)
                    base.Update();
                    return;
                }
                else if (drawtimecount < drawtimeinterval) {

                    return;
                }
                drawtimecount = 0;
            }
            base.Update();
           
        }
        #region 道具产生的效果方法
        public void BooMeffect() {
            Rectangle rect = this.GetRectangle();

            if ((GameObjectManager.IsCollidedMyTank(rect)) != null)
                {

                    foreach (EnemyTank tank in GameObjectManager.tankList)
                    {
                    SoundMananger.PlayBlast();
                    GameObjectManager.DestroyTank(tank);
                        
                        GameObjectManager.CreateExplosion(tank.X + tank.Width / 2, tank.Y + tank.Height / 2);
                        GameObjectManager.Createpropos(tank.X, tank.Y);
                }
                   
                Isneeddestory = true;
            }
        }
        public void Timereffect() {
            Rectangle rect = this.GetRectangle();
            
                if ((GameObjectManager.IsCollidedMyTank(rect)) != null)
                {

                    foreach (EnemyTank tank in GameObjectManager.tankList)
                    {
                        tank.notmove = true; //使敌人坦克不能动     
                    tank.nowtime = tank.pasttime; //这里再次将过去的时间再次给到nowtime是为了                                      
                    }                             //防止定时器还未结束时如果你又捡了一个那么nowtime要重新计时
                SoundMananger.PlayAdd();
                    Isneeddestory1 = true;
                }           
        }
        #endregion
        public NotMovething(int x, int y, Image img)
        {
            this.X = x;
            this.Y = y;
            this.Img = img;
        }
    }
}
