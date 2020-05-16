using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public KeyCode QuitButton = KeyCode.Escape;
    public KeyCode RestartButton = KeyCode.F5;

    void Update()
    {
        if (Input.GetKeyDown(QuitButton))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(RestartButton))
        {
            GameManager.Instance.Restart();    
        }
    }
}
