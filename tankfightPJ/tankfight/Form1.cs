using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace tankfight
{
    public partial class Form1 : Form
    {
        private static Thread t;//子线程负责游戏主要框架的运行
        private static Graphics windowG;//整个窗体的画布
        private static Bitmap tempBmp; //游戏实体画布
        private static Bitmap tempBmp1;//Icon画布
        private static Bitmap tempBmp2;//Waitstart画布,loadgame画布
        public Form1()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            //阻塞
            windowG = this.CreateGraphics();//设置为窗体的画布
            tempBmp = new Bitmap(450, 450);//游戏实体图片
            tempBmp1 = new Bitmap(150, 450);//Icon图片
            tempBmp2 = new Bitmap(600, 450);//Waitstart图片,loadgame图片
            Graphics bmpG = Graphics.FromImage(tempBmp);
            Graphics bmpG1 = Graphics.FromImage(tempBmp1);
            Graphics bmpG2 = Graphics.FromImage(tempBmp2);//将对应图片的尺寸传入画布
            GameFramework.g = bmpG;
            GameFramework.g1 = bmpG1;
            GameFramework.g2 = bmpG2;//再将以确定好尺寸的画布赋给所需要的画布
            t = new Thread(new ThreadStart(GameMainThread));
            SoundMananger.InitSound();
            t.Start();
        }
        private static void GameMainThread()
        {

            //GameFramework

            

            int sleepTime = 1000 /60;

            //60
            while (true)
            {
                try
                {
                    if (GameFramework.gameState == GameState.WaitStart)
                    {
                        GameFramework.g2.Clear(Color.Black);
                        GameFramework.Update();
                        windowG.DrawImage(tempBmp2, 0, 0);
                    }
                    else if (GameFramework.gameState == GameState.Gameload) {
                        GameFramework.g2.Clear(Color.Black);
                        GameFramework.Update();
                        windowG.DrawImage(tempBmp2, 0, 0);
                    }
                    else
                    {
                        if (!GameFramework.pause)
                        {
                            GameFramework.g.Clear(Color.Black);
                            GameFramework.g1.Clear(Color.Gray);
                            GameFramework.Update();// 60
                            windowG.DrawImage(tempBmp, 0, 0);
                            windowG.DrawImage(tempBmp1, 450, 0);
                        }
                    }
                }
                catch (InvalidOperationException e) { }

                Thread.Sleep(sleepTime);
            }

        }



        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Dispose();
            t.Abort();
        }
        //事件  消息  事件消息
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (GameFramework.gameState == GameState.Running)//游戏状态下才接受坦克控制的键盘响应               
                if (!GameFramework.pause)
                    GameObjectManager.KeyDown(e);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F)
            {  if (GameFramework.gameState==GameState.WaitStart)
                if (GameFramework.doublePlayer == false) { 
                    GameFramework.doublePlayer = true;
                       
                        GameFramework.chose = 2;
                    SoundMananger.Playchose();
                }
                else {
                    GameFramework.doublePlayer = false;
                    GameFramework.chose = 1;
                    SoundMananger.Playchose();
                        
                }
            }
            if (GameFramework.gameState == GameState.WaitStart)
            {
                switch (e.KeyCode)
                {
                    case Keys.H:
                        GameFramework.Startgame();
                        break;
                    case Keys.Q:                     
                        GameFramework.Backgameload();
                        break;
                }
            }
            else if (GameFramework.gameState == GameState.Gameload) {
                switch (e.KeyCode)
                {
                    case Keys.G:GameFramework.Entergamemenu();
                        break;
                }

            }
            else 
            {
                switch (e.KeyCode)
                {
                    case Keys.Q:
                        GameFramework.Quitgame();
                        break;
                }

            }
            if (GameFramework.gameState == GameState.Running)
            { //游戏状态下才接受坦克控制的键盘响应

                if (e.KeyCode == Keys.Space)
                {
                    if (!GameFramework.pause)//非暂停态设置为暂停态
                    {
                        GameFramework.pause = true;
                        SoundMananger.Playpause();
                    }
                    else
                    {
                        GameFramework.pause = false;//暂停态设置为非暂停态
                        SoundMananger.Playpause();
                    }
                }
                    if (!GameFramework.pause)//游戏中没暂停则可以接受游戏操控按键
                GameObjectManager.KeyUp(e);
            }
        }
        
    }
}
