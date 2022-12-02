using UnityEngine;
public class MusicManager : MonoBehaviour
{
    private bool isPlaying;
    public static MusicManager instance;
    private AudioSource _audioSource;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
            Destroy(gameObject);

        instance.isPlaying = PlayerPrefs.GetInt("isPlaying") == 1;

        _audioSource = GetComponent<AudioSource>();
        if (instance.isPlaying)
            _audioSource.Play();
        else
            _audioSource.Stop();
    }

    public void ChangeMusState()
    {
        instance.isPlaying = !instance.isPlaying;
        PlayerPrefs.SetInt("isPlaying", instance.isPlaying ? 1 : 0);
        if (instance.isPlaying)
            _audioSource.Play();
        else
            _audioSource.Stop();
    }

}
