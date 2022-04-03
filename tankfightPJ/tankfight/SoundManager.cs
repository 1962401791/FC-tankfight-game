using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using tankfight.Properties;
namespace tankfight
{
    class SoundMananger
    {
        
        private static SoundPlayer startPlayer = new SoundPlayer();
        private static SoundPlayer addPlayer = new SoundPlayer();
        private static SoundPlayer blastPlayer = new SoundPlayer();
        private static SoundPlayer firePlayer = new SoundPlayer();
        private static SoundPlayer hitPlayer = new SoundPlayer();
        private static SoundPlayer WinPlayer = new SoundPlayer();
        private static SoundPlayer LosePlayer = new SoundPlayer();
        private static SoundPlayer Pauseplayer = new SoundPlayer();
        private static SoundPlayer choseplayer = new SoundPlayer();

        public static SoundPlayer StartPlayer { get => startPlayer; set => startPlayer = value; }

        public static void InitSound()
        {
            StartPlayer.Stream = Resources.start;
               addPlayer.Stream = Resources.add;
                blastPlayer.Stream = Resources.blast;
                firePlayer.Stream = Resources.fire;
                hitPlayer.Stream = Resources.hit;
             WinPlayer.Stream = Resources.wingame;
            LosePlayer.Stream = Resources.gamelose;
            Pauseplayer.Stream = Resources.pausing;
            choseplayer.Stream = Resources.chose;
        }
        public static void Playchose() { 
            choseplayer.Play();
            }
        public static void Playpause() {

            Pauseplayer.Play();
        
        }
        public static void PlayStart()
        {
           
                StartPlayer.Play();
        }
        public static void PlayGamewin() {
       
            WinPlayer.Play();
        }

        public static void PlayGamelose() {
         
            LosePlayer.Play();
        }
        public static void PlayAdd()
        {

    
            addPlayer.Play();
          
        }
        public static void PlayBlast()
        {
       
            blastPlayer.Play();
         
        }
        public static void PlayFire()
        {
          
            firePlayer.Play();
           
        }
        public static void PlayHit()
        {
     
            hitPlayer.Play();
          
        }



    }
}
