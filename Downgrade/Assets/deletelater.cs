using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deletelater : MonoBehaviour
{
    [SerializeField] Transform player, higestPoint, lowestPoint;
    [SerializeField] float multiply;
    [SerializeField] bool invertValue;

    private void Update()
    {
        var expo = GetExpo();
    }

    public float GetExpo()
    {
        float value = Mathf.InverseLerp(lowestPoint.position.y, higestPoint.position.y, player.position.y);
        float scaledValue = value * multiply;
        scaledValue = invertValue ? -1 * scaledValue : scaledValue;
        return scaledValue;
    }
}