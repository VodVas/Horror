using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneActivator : MonoBehaviour
{
    public void LoadScene(int sceneNumber)
    {
        SceneManager.LoadScene(sceneNumber);
    }
}