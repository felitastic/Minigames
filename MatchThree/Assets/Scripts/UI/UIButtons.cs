
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Functions for alle UI Buttons
/// </summary>
public class UIButtons : MonoBehaviour
{
    private UIManager manager { get { return UIManager.Instance; } }

    [Header("Keys")]
    [SerializeField]
    private KeyCode quitButton = KeyCode.Escape;
    [SerializeField]
    private KeyCode restartButton = KeyCode.F5;

    private void Update()
    {
        if (Input.GetKeyDown(quitButton))
        {
            if (manager.Scene == eMenuScene.Help)
                ShowIngameUI();
            else if (manager.Scene == eMenuScene.Ingame)
                OpenHelp();
            else
                return;
        }

        //if (Input.GetKeyDown(restartButton))
        //{
        //    GameManager.Instance.ResetGame();
        //}
    }

    public void StartNewGame()
    {
        manager.SwitchScene(eMenuScene.GameMode);
    }

    public void OpenHighscores()
    {
        manager.SwitchScene(eMenuScene.Highscore);
    }

    public void OpenCredits()
    {
        manager.SwitchScene(eMenuScene.Credits);
    }

    public void QuitGAme()
    {
        Application.Quit();
    }

    public void ReturnToTitle()
    {
        manager.SwitchScene(eMenuScene.Start);
    }

    public void ResumeGame()
    {
        manager.SwitchScene(eMenuScene.Ingame);
    }

    public void ConfirmGamemode()
    {
        Vector2Int _values = manager.GameModeValues();
        GameManager.Instance.StartNewGame(_values.x, _values.y);
        ShowIngameUI();
    }

    public void ShowIngameUI()
    {
        manager.SwitchScene(eMenuScene.Ingame);
    }

    public void OpenHelp()
    {
        manager.SwitchScene(eMenuScene.Help);
    }

    public void HowTo()
    {
        manager.SwitchScene(eMenuScene.Tutorial);
    }

    public void CloseTutorial()
    {
        OpenHelp();
    }

    public void ConfirmNameInput()
    {
        manager.SwitchScene(eMenuScene.NormalResult);
    }

    public void Replay()
    {
        GameManager.Instance.ResetGame();
        manager.SwitchScene(eMenuScene.Ingame);
    }

    public void ChangeHighscorePage(bool plus)
    {
        manager.UpdateHighscorePage(plus);
    }
}
