using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject _menuPanel;
    [SerializeField] private NewInputProvider _inputProvider;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _exitButton;

    private void Update()
    {
        if (_inputProvider.GetMenuInput())
        {
            ToggleMenu();
        }
    }

    private void OnEnable()
    {
        _restartButton.onClick.AddListener(RestartGame);
        _exitButton.onClick.AddListener(ExitGame);
    }

    private void OnDisable()
    {
        _restartButton.onClick.RemoveListener(RestartGame);
        _exitButton.onClick.RemoveListener(ExitGame);
    }

    private void ToggleMenu()
    {
        _menuPanel.SetActive(!_menuPanel.activeSelf);

        Time.timeScale = _menuPanel.activeSelf ? 0f : 1f;

        Cursor.lockState = _menuPanel.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = _menuPanel.activeSelf;
    }

    private void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    private void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

        Application.Quit();
    }
}