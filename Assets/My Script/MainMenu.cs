using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    Button newGameBtn;
    Button continueBtn;
    Button quitBtn;
    // Start is called before the first frame update
    void Awake()
    {
        newGameBtn =transform.GetChild(1).GetComponent<Button>();
        continueBtn =transform.GetChild(3).GetComponent<Button>();
        quitBtn= transform.GetChild(2).GetComponent<Button>();
        newGameBtn.onClick.AddListener(NewGame);
        continueBtn.onClick.AddListener(ContinueGame);
        quitBtn.onClick.AddListener(QuitGame);
        
    }

    void NewGame()
    {PlayerPrefs.DeleteAll();
    SceneController.Instance.TransitionToFirstLevel();
    }

    void ContinueGame()
    {
     SceneController.Instance.TransitionToLoadGame();
    }
    void QuitGame()
    {
        Application.Quit();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
