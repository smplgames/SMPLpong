using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CameraController : MonoBehaviour
{
    public AudioClip AudioIntro;
    public AudioClip AudioLoop;
    private AudioSource _audio;

    void Start()
    {
        StartMusic();
    }

    private void StartMusic() {
        GetComponent<AudioSource> ().loop = true;
        StartCoroutine(PlayLoopMusic());
    }

    IEnumerator PlayLoopMusic()
    {
        _audio = GetComponent<AudioSource> ();
        _audio.Play();
        yield return new WaitForSeconds(_audio.clip.length);
        _audio.clip = AudioLoop;
        _audio.Play();
    }
}
