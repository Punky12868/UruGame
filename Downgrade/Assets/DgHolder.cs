using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DgHolder : MonoBehaviour
{
    [SerializeField] private DowngradeCard[] dgCard;
    [SerializeField] private SelectedDowngrade selectedDowngrade;
    int index;

    private void Awake()
    {
        Invoker.InvokeDelayed(DelayedAwake, 0.2f);
    }

    private void DelayedAwake()
    {
        selectedDowngrade = SimpleSaveLoad.Instance.LoadData<SelectedDowngrade>(FileType.Gameplay, "Downgrade", SelectedDowngrade.None);

        for (int i = 0; i < dgCard.Length; i++)
        {
            if (dgCard[i].selectedDowngrade == selectedDowngrade)
            {
                index = i;
                break;
            }
        }

        LoadDg(index);
    }

    private void LoadDg(int i)
    {
        dgCard[i].CardEffect();
    }
}
