using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [SerializeField] private int bossFases;
    private int currentFase = 1;

    public bool NextFase()
    {
        if (currentFase < bossFases)
        {
            Debug.Log("Fase " + currentFase);
            currentFase++;
            Debug.Log("Fase " + currentFase);
            return true;
        }

        return false;
    }
}
