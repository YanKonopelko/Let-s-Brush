using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    private bool isPlaying;
    private static MusicManager instance;
    private AudioSource _audioSource;
    [SerializeField] private Sprite[] Sprites = new Sprite[2];
    [SerializeField] private Image _image;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(transform.root.gameObject);
        }
        else if (instance != this)
            Destroy(transform.root.gameObject);

        instance.isPlaying = PlayerPrefs.GetInt("isPlaying") == 1;
        _image.sprite = Sprites[instance.isPlaying ? 1 : 0];
        _audioSource = GetComponent<AudioSource>();
        if (instance.isPlaying)
            _audioSource.Play();
        else
            _audioSource.Stop();
    }

    public void ChangeMusState()
    {
        instance.isPlaying = !instance.isPlaying;
        _image.sprite = Sprites[instance.isPlaying ? 1 : 0];
        PlayerPrefs.SetInt("isPlaying", instance.isPlaying ? 1 : 0);
        if (instance.isPlaying)
            _audioSource.Play();
        else
            _audioSource.Stop();
    }

}
