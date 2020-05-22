using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the player relevant ingame UI with health, abilites and resources
/// </summary>
public class UI_InGame : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Ref to the Health Petals; [0] = 1 life, [5] = 6 lives")]
    private HP_Petal[] petalsForHP;
    [SerializeField]
    [Tooltip("Image for the ability UI")]
    private Image abilityIconOne;
    [SerializeField]
    [Tooltip("Image for the ability UI")]
    private Image abilityIconTwo;
    [SerializeField]
    private Text resOnPlayer;
    [SerializeField]
    private Text resInBase;
    [SerializeField]
    private Text playerName;
    //[SerializeField]
    //private NameGenerator nameGen;

    private void Start()
    {
        //nameGen = GetComponent<NameGenerator>();
    }

    //HACK for testing purposes
    int testMaxHP = 6;    
    public void TestHP(bool lose)
    {
        if (lose)
        {
            if (testMaxHP == 0)
                print("no HP left");
            else
            {
                ChangeHP(testMaxHP, testMaxHP - 1);
                testMaxHP -= 1;
            }
        }
        else
        {
            if (testMaxHP == 6)
                print("full hp reached");
            else
            {
                ChangeHP(testMaxHP, testMaxHP + 1);
                testMaxHP += 1;
            }
        }
    }

    public void TestNameGen()
    {
        //playerName.text = nameGen.NewBugName();
    }

    /// <summary>
    /// Changes HP UI according to health loss/gain
    /// Currently assumes that you lose only one petal per call
    /// </summary>
    private void ChangeHP(int prevHP, int newHP)
    {      
        //lost health
        if (prevHP > newHP)
        {
            if (prevHP-newHP == 1)
                petalsForHP[newHP].Fall();
        }
        //regained health
        else if (prevHP < newHP)
        {
            int gained = newHP - prevHP;
            for(int i = 0; i < gained; i++)
            {
                petalsForHP[prevHP+i].Regain();
            }
        }
        //else: no hp change -> no ui change
    }

    private void ChangePlayerRessources(int newResCount)
    {
        resOnPlayer.text = newResCount.ToString();
    }

    private void ChangeBaseRessources(int newResCount)
    {
        resInBase.text = newResCount.ToString();
    }

    /// <summary>
    /// Changes Ability Icon 1 if first is true, else changes Ability Icon 2
    /// </summary>
    /// <param name="first"></param>
    private void SetAbilityIcons(Sprite abilityIcon, bool first)
    {
        if (first)
            abilityIconOne.sprite = abilityIcon;
        else
            abilityIconTwo.sprite = abilityIcon;
    }
}
