using System.Collections;
using UnityEngine;

namespace ForsakenLegacy
{
    public class MusicManager : MonoBehaviour
    {
        public static MusicManager Instance { get; private set; }

        [Header("Audio Clips")]
        public AudioSource ExploreMusic;
        public AudioSource CombatMusic;

        [Header("Transition Settings")]
        public float TransitionDuration = 1.5f;

        [Header("Master Volume")]
        public float MasterVolume = 1f;


        private bool _isTransitioning;
        private bool _isPlayingExploration;

        private void Awake()
        {
            // Ensure there's only one instance of MusicManager
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }

            if(ExploreMusic.isPlaying) _isPlayingExploration = true;
            CombatMusic.Stop();
        }

        // Start playing a new track
        public void Combat()
        {
            if (_isTransitioning) return;

            if (_isPlayingExploration)
            {
                Debug.Log("Playing Combat Music");
                StartCoroutine(TransitionMusic(ExploreMusic, CombatMusic));
            }
            else
            {
                return;
            }
        }

        public void Exploration()
        {
            if (_isTransitioning) return;

            if (!_isPlayingExploration)
            {
                StartCoroutine(TransitionMusic(CombatMusic, ExploreMusic));
            }
            else
            {
                return;
            }
        }

        // Coroutine to handle music transition
        private IEnumerator TransitionMusic(AudioSource fromSource, AudioSource toSource)
        {
            _isTransitioning = true;

            toSource.Play();

            float time = 0f;

            while (time < TransitionDuration)
            {
                time += Time.deltaTime;
                float t = time / TransitionDuration;

                fromSource.volume = Mathf.Lerp(MasterVolume, 0f, t);
                toSource.volume = Mathf.Lerp(0f, MasterVolume, t);

                yield return null;
            }

            fromSource.Stop();
            fromSource.volume = MasterVolume;  // Reset volume for next use

            _isPlayingExploration = !_isPlayingExploration;
            _isTransitioning = false;
        }
    }
}
