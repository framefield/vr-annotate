using UnityEngine;

public class PlaySound : MonoBehaviour
{
    void Start()
    {
        _audio = GetComponent<AudioSource>();
    }


    void Update()
    {

    }

    public void PlayAudio(AudioClip clip)
    {
        _audio.clip = clip;
        _audio.Play();
    }

    public void StopAudio()
    {
        _audio.Stop();
    }

    private AudioSource _audio;
}
