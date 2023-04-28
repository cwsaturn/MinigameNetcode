using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    private AudioSource _audiosource;
    public AudioClip[] songs;

    public Animator musicAnim;

    [SerializeField] private float _songsPlayed;
    [SerializeField] private bool[] _beenPlayed;

    private bool init_music = false;

    // Start is called before the first frame update
    void Start()
    {

        _audiosource = GetComponent<AudioSource>();
        DontDestroyOnLoad(_audiosource);

        _beenPlayed = new bool[songs.Length];

        if(!_audiosource.isPlaying)
            ChangeSong(Random.Range(0, songs.Length));
    }

    // Update is called once per frame
    void Update()
    {
        if(!_audiosource.isPlaying)
        {
            ChangeSong(Random.Range(0, songs.Length));
        }

        if(_songsPlayed == songs.Length)
        {
            _songsPlayed = 0;
            for(int i = 0; i < songs.Length; i++)
            {
                if (i == songs.Length)
                {
                    break;
                }
                else
                {
                    _beenPlayed[i] = false;
                }
            }
        }
    }

    public void ChangeSong(int songPicked)
    {
        Debug.Log("init music: " + init_music);
        if (!_beenPlayed[songPicked])
        {
            _songsPlayed++;
            _beenPlayed[songPicked] = true;
            _audiosource.clip = songs[songPicked];
            if(!init_music)
            {
                float song_length = _audiosource.clip.length;
                _audiosource.time = Random.Range(0, _audiosource.clip.length * 0.75f);
                musicAnim.Play("fadein");
                init_music = true;
            }
            else
            {
                _audiosource.time = 0;
            }
            _audiosource.Play();
        }
        else
        {
            _audiosource.Stop();
        }
    }

}
