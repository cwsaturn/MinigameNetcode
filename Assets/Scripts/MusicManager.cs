using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private AudioSource _audiosource;
    public AudioClip[] songs;

    [SerializeField] private float _songsPlayed;
    [SerializeField] private bool[] _beenPlayed;


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
            ChangeSong(Random.Range(0, songs.Length));

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
        if (!_beenPlayed[songPicked])
        {
        _songsPlayed++;
        _beenPlayed[songPicked] = true;
        _audiosource.clip = songs[songPicked];
        _audiosource.Play();
        }
        else
        {
            _audiosource.Stop();
        }
    }
}
