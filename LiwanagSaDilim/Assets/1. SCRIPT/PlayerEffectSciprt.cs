using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//---ADDED BY NULL FOR EFFECTS AND UI STUFF---

public enum PowerUpType { None, Blue, Green, Red}
public class PlayerEffectSciprt : MonoBehaviour
{
    PlayerMovements playerMovements;

    [Header("References indicators")]
    public GameObject redIndicator;
    public GameObject blueIndicator;
    public GameObject greenIndicator;

    [Header("References Buttons")]
    public GameObject redButton;
    public GameObject blueButton;
    public GameObject greenButton;

    void Start()
    {
        playerMovements = GetComponent<PlayerMovements>();
    }


    void LateUpdate()
    {
        PlayerUi();
    }
    
    void PlayerUi()
    {
        PowerUpType currentPower = playerMovements.currentPower;

        redIndicator.SetActive(currentPower == PowerUpType.Red);
        blueIndicator.SetActive(currentPower == PowerUpType.Blue);
        greenIndicator.SetActive(currentPower == PowerUpType.Green);

        blueButton.SetActive(currentPower == PowerUpType.Blue);
        greenButton.SetActive(currentPower == PowerUpType.Green);
        redButton.SetActive(currentPower == PowerUpType.Red);
    }

    void AnimationController()
    {
        
    }
}
