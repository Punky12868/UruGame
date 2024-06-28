using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GlitchedMenu : MonoBehaviour
{
    [SerializeField] private Image[] glitchedSprites;
    [SerializeField] private float timeGlitching;
    [SerializeField] private Vector2 cooldown;
    private float properCooldown;
    private bool isGlitching;
    private bool isCoolingDown = true;
    private float timer;

    private void Update()
    {
        //glitchedSprites[Random.Range(0, glitchedSprites.Length)].material.SetFloat("_IsGlitching", Random.Range(0, 2));
        if (isGlitching) return;

        if (isCoolingDown)
        {
            timer += Time.deltaTime;
            if (timer >= properCooldown)
            {
                isCoolingDown = false;
                timer = 0;
            }
        }
        else StartCoroutine(Glitch());
    }

    private IEnumerator Glitch()
    {
        isGlitching = true;
        int glitchedSprite = Random.Range(0, glitchedSprites.Length);
        glitchedSprites[glitchedSprite].material.SetFloat("_IsGlitching", 1);
        yield return new WaitForSeconds(timeGlitching);
        glitchedSprites[glitchedSprite].material.SetFloat("_IsGlitching", 0);
        properCooldown = Random.Range(cooldown.x, cooldown.y);
        isCoolingDown = true;
        isGlitching = false;
    }
}
