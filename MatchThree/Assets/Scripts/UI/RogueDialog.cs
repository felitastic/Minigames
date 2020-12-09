using UnityEngine;
using UnityEngine.UI;

public class RogueDialog : MonoBehaviour
{
    [SerializeField]
    private Text speechBubble;

    private void Awake()
    {
        speechBubble.text = "Today's a good day for\n a HEIST!";
    }

    public void PlayButton()
    {
        speechBubble.text = "The bigger the treasure\nthe easier the heist!";
    }

    public void HoverOverButton(int pButtonNo)
    {
        string dialog = speechBubble.text;
        switch (pButtonNo)
        {
            case 1:
                dialog = "I can explain the base rules for stealing!";
                break;
            case 2:
                dialog = "Let's get back to the heist!";
                break;
            case 3:
                dialog = "Wanna regroup in the hideout?";
                break;
            case 4:
                dialog = "One never truly quits!";
                break;
            default:
                LeavingButton();
                break;
        }
        speechBubble.text = dialog;
    }

    public void LeavingButton()
    {
        speechBubble.text = "Need a hand?";
    }

    public void ChangeDialog(int pDialog)
    {
        string dialog = speechBubble.text;
        switch (pDialog)
        {
            case 1:
                dialog = "It's simpel:\nMatch 3 or more gems!";
                break;
            //case 2:
            //    //dialog = "Want me to explain the basics of stealing?";
            //    break;
            //case 3:
            //    dialog = "Wanna regroup in the hideout?";
            //    break;
            //case 4:
            //    dialog = "One never truly quits!";
            //break;
            default:
                LeavingButton();
                break;
        }
        speechBubble.text = dialog;
    }
}
