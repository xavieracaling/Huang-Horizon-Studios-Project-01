using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UISoundManager : MonoBehaviour
{
    public AudioClip rusherClip;
    public AudioClip idleClip;
    public AudioClip Win;
    public AudioClip Lose;
    public int poolSize = 5;
    AudioSource _win;
    AudioSource _lose ;

    private List<AudioSource> rusherClickSourcePool = new List<AudioSource>();
    private List<AudioSource> idleClickSourcePool = new List<AudioSource>();
    public static UISoundManager Instance;
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Create pool
        _win = gameObject.AddComponent<AudioSource>();
        _lose = gameObject.AddComponent<AudioSource>();
        _win.playOnAwake = true;
        _lose.playOnAwake =  true;

        _win.clip = Win;
        _lose.clip =  Lose;


        for (int i = 0; i < poolSize; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = rusherClip;
            source.playOnAwake = false;
            rusherClickSourcePool.Add(source);

            AudioSource source2 = gameObject.AddComponent<AudioSource>();
            source2.clip = idleClip;
            source2.playOnAwake = false;
            idleClickSourcePool.Add(source2);
            
        }
    }
    public void PlayWin() => _win.Play() ;
    public void PlayLose() => _lose.Play() ;
    public void PlayRusher()
    {
        AudioSource availableSource = rusherClickSourcePool.FirstOrDefault(a => !a.isPlaying);
        if (availableSource != null)
        {
            availableSource.Play();
        }
        else
        {
            // Optionally expand the pool or reuse a source
            Debug.Log("All audio sources are busy. Consider increasing pool size.");
        }
    }
    public void PlayIdle()
    {
        AudioSource availableSource = idleClickSourcePool.FirstOrDefault(a => !a.isPlaying);
        if (availableSource != null)
        {
            availableSource.Play();
        }
        else
        {
            // Optionally expand the pool or reuse a source
            Debug.Log("All audio sources are busy. Consider increasing pool size.");
        }
    }
}
