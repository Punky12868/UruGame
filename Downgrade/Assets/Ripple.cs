using UnityEngine;

public class Ripple : MonoBehaviour
{
    [SerializeField] private bool isPlayer;
    [SerializeField] private int rateOverTimeWhileMoving = 10;
    private PlayerControllerOverhaul player;
    private ParticleSystem.EmissionModule ripple;

    private void Awake() 
    { 
        ripple = GetComponent<ParticleSystem>().emission;
        if (isPlayer) { player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControllerOverhaul>(); return; }
        if (!GetComponentInParent<EnemyBase>().GetIsUsingRipple()) { gameObject.SetActive(false); }
    }

    private void Update() { ChangePos(); ChangeRateOverTime(); }

    private void ChangeRateOverTime() 
    {
        if (isPlayer) { ripple.rateOverTime = player.IsPlayerMoving() ? rateOverTimeWhileMoving : 1; return; }
        ripple.rateOverTime = GetComponentInParent<EnemyBase>().GetIsMoving() ? rateOverTimeWhileMoving : 1;
    }

    private void ChangePos()
    {
        if (!isPlayer) return;

        Vector3 pos = player.IsPlayerMoving() ?
            new Vector3(player.transform.GetChild(0).position.x, transform.position.y, player.transform.GetChild(0).position.z)
          : new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);

        transform.position = pos;
    }
}
