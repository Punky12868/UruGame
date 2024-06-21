using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CooldownSlider : MonoBehaviour
{
    PlayerControllerOverhaul player;
    float cooldown = 0;
    bool usedAbility;

    private void Awake()
    {
        player = FindObjectOfType<PlayerControllerOverhaul>();
        GetComponent<Slider>().maxValue = player.GetAbilityCooldown();
        GetComponent<Slider>().value = player.GetAbilityCooldown();
    }

    private void Update()
    {
        if (player.GetAbilityCooldownStatus())
        {
            GetComponent<Slider>().value = cooldown;

            if (!usedAbility)
            {
                usedAbility = true;
                cooldown = 0;
            }

            if (cooldown < player.GetAbilityCooldown())
            {
                cooldown += Time.deltaTime;
            }
        }
        else
        {
            usedAbility = false;
        }
    }
}
