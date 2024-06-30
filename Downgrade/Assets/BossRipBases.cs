using UnityEngine;
using DG.Tweening;

public class BossRipBases : MonoBehaviour
{
    [SerializeField] Vector2 posOffset;
    [SerializeField] float moveTime;
    [SerializeField] GameObject bigLimit;
    Transform player;
    BossRipBases[] bases;
    bool moveOut;
    bool isMoving;

    private void Awake() { bases = FindObjectsOfType<BossRipBases>(); }

    /*private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!moveOut) { MoveTowards(); return; }
            MoveInwards();
        }
    }*/

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (player == null) { foreach (var b in bases) { if (b.HasPlayer()) { b.RemovePlayer(); } } }

            player = other.transform;
            player.SetParent(transform);
        }
    }

    public void RemovePlayer()
    {
        player.SetParent(null);
        player = null;
    }

    public bool HasPlayer() { return player != null; }

    public void MoveTowards()
    {
        moveOut = true;
        bigLimit.SetActive(false);
        transform.GetChild(0).gameObject.SetActive(true);
        StunPlayer(true);
        Vector3 newPos = new Vector3(posOffset.x, transform.position.y, posOffset.y);
        transform.DOMove(transform.position + newPos, moveTime).onComplete += () => StunPlayer(false);
        GameManager.Instance.CameraShake(moveTime, 1.5f, 1.5f, true);
    }

    public void MoveInwards()
    {
        moveOut = false;
        transform.GetChild(0).gameObject.SetActive(false);
        StunPlayer(true);
        Vector3 newPos = new Vector3(posOffset.x, transform.position.y, posOffset.y);
        transform.DOMove(transform.position - newPos, moveTime).onComplete += () => StunPlayer(false, true);
        GameManager.Instance.CameraShake(moveTime, 1.5f, 1.5f, true);
    }

    private void StunPlayer(bool value, bool bigLimit = false)
    {
        FindObjectOfType<PlayerControllerOverhaul>().SetStunStatus(value);
        this.bigLimit.SetActive(bigLimit);
        isMoving = value;
    }

    public bool GetIsMoving() { return isMoving; }
}
