using UnityEngine;
using UnityEngine.UI;
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;
    public AudioSource SoundSource;

    [SerializeField] private Image _soundButtonImage;
    [SerializeField] private Sprite _soundOnSprite;
    [SerializeField] private Sprite _soundOffSprite;

    private bool _soundEnabled = true;

    private void Start()
    {
        if (PlayerPrefs.HasKey("Sound"))
        {
            _soundEnabled = PlayerPrefs.GetInt("Sound") == 1;
        }

        UpdateSoundState();
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject); 
            return;
        }
    }

    public void ToggleSound()
    {
        _soundEnabled = !_soundEnabled;
        PlayerPrefs.SetInt("Sound", _soundEnabled ? 1 : 0);
        UpdateSoundState();

        if (_soundEnabled && SoundSource != null)
        {
            SoundSource.Play();
        }
    }

    private void UpdateSoundState()
    {
        if (SoundSource != null)
        {
            SoundSource.mute = !_soundEnabled;
        }

        if (_soundButtonImage != null)
        {
            _soundButtonImage.sprite = _soundEnabled ? _soundOnSprite : _soundOffSprite;
            _soundButtonImage.transform.localScale = _soundEnabled ? Vector3.one : new Vector3(1.2f,1,1);
        }
    }
}
