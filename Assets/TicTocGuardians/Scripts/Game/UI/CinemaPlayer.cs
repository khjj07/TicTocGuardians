using UnityEngine;
using UnityEngine.Video;

namespace TicTocGuardians.Scripts.Game.UI
{
    public class CinemaPlayer : MonoBehaviour
    {
        private VideoPlayer _videoPlayer;


        private void Awake()
        {
            _videoPlayer = GetComponent<VideoPlayer>();
        }

        public void Play()
        {
            _videoPlayer.Play();
        }

        public void AddCallback(VideoPlayer.EventHandler eventHandler)
        {
            _videoPlayer.loopPointReached += eventHandler;
        }
    }
}