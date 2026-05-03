using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    [SerializeField] private Button[] _levelButtons;

    private void Start()
    {
        int reachedLevel = PlayerPrefs.GetInt("ReachedLevel", 1);

        for (int i = 0; i < _levelButtons.Length; i++)
        {
            if (i + 1 > reachedLevel)
            {
                _levelButtons[i].interactable = false; 
            }
        }
    }

    public void LoadLevel(int levelIndex)
    {
        SceneManager.LoadScene(levelIndex);
    }
}