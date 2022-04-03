using System.Drawing;
namespace tankfight
{

    enum GameState
    {   WaitStart,
        Running,
        GameOver,
        Win,
        Gameload
    }//区分游戏模式的枚举变量

    class GameFramework
    {
        public static Graphics g;//游戏实体画布
        public static Graphics g1;//Iocn画布
        public static Graphics g2;//waitstart,loadGame画布
        public static GameState gameState = GameState.Gameload;
        public static bool pause = false;//游戏模式下的状态
        public static bool doublePlayer = false;
        public static int chose=1;//chose=1 为单人模式，chose=2为双人模式
        public static void Start()
        {
            GameObjectManager.Start();
            GameObjectManager.CreateMap();           
            GameObjectManager.CreateMyTank();           
            SoundMananger.PlayStart();
        }
        #region 游戏在不同的游戏模式选择下对应的更新
        public static void Update()
        {

            if (gameState == GameState.WaitStart) {
                GameWaitStartUpdate();
            }
            if (gameState == GameState.Running)
            {
                GameObjectManager.CreateIcon();
                GameObjectManager.Update();
            }
            else if (gameState == GameState.GameOver)
            {
                GameOverUpdate();
            }
            else if (gameState == GameState.Gameload) {
                GameloadUpdate();
            }
            else
            {

                GameWinUpdate();
            }
        }
        #endregion
        private static void GameOverUpdate()
        {
            Bitmap bmp = Properties.Resources.GameOver;
            bmp.MakeTransparent(Color.Black);
            int x = 450 / 2 - Properties.Resources.GameOver.Width / 2;
            int y = 450 / 2 - Properties.Resources.GameOver.Height / 2;
            g.DrawImage(bmp, x, y);
        }
        private static void GameWinUpdate() {
            Bitmap bmp = Properties.Resources.win;
            bmp.MakeTransparent(Color.Black);
            int x = 450 / 2 - Properties.Resources.GameOver.Width / 2;
            int y = 450 / 2 - Properties.Resources.GameOver.Height / 2;
            g.DrawImage(bmp, x, y);

        }

        private static void GameWaitStartUpdate() {
            Bitmap bmp = Properties.Resources.Gamestart;          
            bmp.MakeTransparent(Color.Black);
            int x = 450 / 2 - Properties.Resources.GameOver.Width / 2+50;
            int y = 450 / 2 - Properties.Resources.GameOver.Height / 2+20;
            g2.DrawImage(bmp, x, y);
            selectIcon();

        }
        private static void selectIcon() {
            Bitmap bmp1 = Properties.Resources.MyTankRight;
            bmp1.MakeTransparent(Color.Black);
            if (chose == 1) { g2.DrawImage(bmp1, 175, 290); }
            else { g2.DrawImage(bmp1, 175, 328); }
           
        }
        private static void GameloadUpdate() {

            Bitmap bmp = Properties.Resources.gameload;
            bmp.MakeTransparent(Color.Black);           
            int x = 450 / 2 - Properties.Resources.gameload.Width / 2 + 50;
            int y = 450 / 2 - Properties.Resources.gameload.Height / 2 + 20;
            g2.DrawImage(bmp, x, y);
        }
        public static void Quitgame()
        {
            SoundMananger.StartPlayer.Stop();
            gameState = GameState.WaitStart;
            GameObjectManager.Gamequitclear();
        }
        public static void Startgame() {
            Start();
            gameState = GameState.Running;
        }
        public static void Entergamemenu() {
            gameState = GameState.WaitStart;     
        }
        public static void Backgameload() {

            gameState = GameState.Gameload;
        }
        public static void ChangeToGameOver()
        {
            gameState = GameState.GameOver;
            SoundMananger.PlayGamelose();
        }
        public static void ChangeToWin() {
            gameState = GameState.Win;
            SoundMananger.PlayGamewin();
        }
    }
}
