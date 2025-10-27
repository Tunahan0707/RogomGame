using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GoToMainMenu : MonoBehaviour
{
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        SceneManager.LoadScene(Consts.Scenes.MAIN_MENU);
    }
}
