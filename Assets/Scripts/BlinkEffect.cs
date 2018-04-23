using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkEffect : MonoBehaviour {
    public float spriteBlinkingTimer = 0.0f;
    public float spriteBlinkingMiniDuration = 0.1f;
    public float spriteBlinkingTotalTimer = 0.0f;
    public float spriteBlinkingTotalDuration = 1.5f;
    public bool startBlinking = false;
    public bool isIndicator;
    private PlayerControl playerControl;
    SpriteRenderer spriteHolder;
    void Awake()
    {
        if (isIndicator)
        {
            playerControl = GameObject.Find("hero").GetComponent<PlayerControl>();
            spriteHolder = GetComponent<SpriteRenderer>();
        }
        else
            spriteHolder = gameObject.transform.Find("body").gameObject.GetComponent<SpriteRenderer>();

    }
    void Update()
    {
        if (startBlinking == true)
        {
            StartBlinkingEffect();
        }
        if (isIndicator)
        {
            spriteBlinkingTotalDuration = playerControl.abilityCooldown;
            if (Time.time > playerControl.abilityStart + playerControl.abilityCooldown)
                GetComponent<SpriteRenderer>().material.color = Color.green;
            else
            {
                if (playerControl.abilityCooldown == 4)
                {
                    if (playerControl.dashToggle)
                    {
                        GetComponent<SpriteRenderer>().material.color = Color.red;
                        startBlinking = true;
                        Invoke("abilityStop", 1);
                    }
                }
                else if (playerControl.abilityCooldown == 15)
                {//infinite shot
                    if (playerControl.checkInfShot())
                    {
                        GetComponent<SpriteRenderer>().material.color = Color.red;
                        startBlinking = true;
                        Invoke("abilityStop", 2.2F);
                    }
                }
                else if (playerControl.abilityCooldown == 7)
                {
                    if (startBlinking == false)
                    {
                        startBlinking = true;
                        abilityStop();
                    }
                }
            }
        }
    }
    private void StartBlinkingEffect()
    {
        spriteBlinkingTotalTimer += Time.deltaTime;
        if (spriteBlinkingTotalTimer >= spriteBlinkingTotalDuration)
        {
            startBlinking = false;
            spriteBlinkingTotalTimer = 0.0f;
            spriteHolder.enabled = true;
            return;
        }

        spriteBlinkingTimer += Time.deltaTime;
        if (spriteBlinkingTimer >= spriteBlinkingMiniDuration)
        {
            spriteBlinkingTimer = 0.0f;
            if (spriteHolder.enabled == true)
            {
                spriteHolder.enabled = false;  //make changes
            }
            else
            {
                spriteHolder.enabled = true;   //make changes
            }
        }
    }
    public void abilityStop()
    {
        GetComponent<SpriteRenderer>().material.color = Color.grey;
    }
}
