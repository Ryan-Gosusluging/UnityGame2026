using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButton : MonoBehaviour
{
    public void Back(int index)
    {
        SceneManager.LoadScene(index);
    }
}
