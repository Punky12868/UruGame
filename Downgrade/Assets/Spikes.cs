using UnityEngine;

public class Spikes : MonoBehaviour
{
    [Header("----------")]
    [SerializeField] private AnimationClip spikeOutClip;
    [SerializeField] private GameObject spikePrefab;

    [Header("----------")]
    [SerializeField] private float spawnTime;
    [SerializeField] private float hitboxAppearTime = 0.2f;
    [SerializeField] private float spacing;

    [Header("----------")]
    [SerializeField] private Transform hitboxCenter;
    [SerializeField] private Vector3 hitboxSize = Vector3.one;
    [SerializeField] private Vector3 hitboxOffset = Vector3.zero;
    [SerializeField] private Color hitboxAppearColor = new Color(0, 0, 1, 1);
    [SerializeField] private Color hitboxNormalColor = new Color(0, 1, 0, 1);

    private Color hitboxColor;
    private Vector3 direction;
    private float damage;
    private float knockback;
    private float lifeTime;
    private float timer;
    private bool spawnedAnother;
    private bool limitReached;
    private bool lifeTimeReached = true;
    private void ResetLifeTimeReached() { lifeTimeReached = false; }

    public void SetVariables(float damage, float knockback, float lifeTime, Vector3 direction) 
    { 
        this.damage = damage;
        this.knockback = knockback;
        this.lifeTime = lifeTime;
        this.direction = direction.normalized;
        DestroyItself();
        hitboxColor = hitboxAppearColor;
    }

    private void DestroyItself()
    { 
        Destroy(gameObject, lifeTime + 0.3f);
        Invoke("ResetLifeTimeReached", hitboxAppearTime);
        Invoke("EndAnimation", lifeTime);
    }

    private void EndAnimation()
    {
        foreach (var clip in GetComponent<Animator>().runtimeAnimatorController.animationClips)
        { if (clip.name == spikeOutClip.name) { GetComponent<Animator>().Play(spikeOutClip.name); }}

        lifeTimeReached = true;
    }

    private void Update()
    {
        DamagePlayer();
        if (spawnedAnother || limitReached) return;
        if (timer < spawnTime) {timer += Time.deltaTime; return;}
        SpawnSpike();
    }

    private void SpawnSpike()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, direction * spacing); 
        if (Physics.Raycast(ray, out hit, spacing)) 
        {
            if (hit.collider.tag == "Limits") 
            {
                Debug.Log("Limit GameObject: " + hit.transform.name);
                limitReached = true;
            }
        }

        if (limitReached) return;

        Vector3 spawnPoint = ray.origin + ray.direction * spacing;
        GameObject spike = Instantiate(spikePrefab, spawnPoint, Quaternion.identity);
        spike.GetComponent<Spikes>().SetVariables(damage, knockback, lifeTime, direction);

        spawnedAnother = true;
    }

    private void DamagePlayer()
    {
        if (lifeTimeReached)
        {
            hitboxColor = hitboxAppearColor;
            return;
        }

        hitboxColor = hitboxNormalColor;
        Collider[] hitColliders = Physics.OverlapBox(hitboxCenter.position + hitboxOffset, hitboxSize, Quaternion.LookRotation(direction));
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                //hitCollider.GetComponent<PlayerControllerOverhaul>().TakeDamageProxy(damage, knockback, direction);

                /*Vector3 toPlayer = direction.normalized;
                Vector3 knockbackDirection = new Vector3(toPlayer.x, 0, toPlayer.z).normalized;
                hitCollider.GetComponent<PlayerControllerOverhaul>().TakeDamageProxy(damage, knockback, knockbackDirection);*/

                Vector3 playerPosition = hitCollider.transform.position;
                Vector3 directionToPlayer = (playerPosition - transform.position).normalized;
                Vector3 crossProduct = Vector3.Cross(transform.forward, directionToPlayer);
                Vector3 knockbackDirection;

                if (crossProduct.y > 0)
                    knockbackDirection = (transform.forward + transform.right).normalized;
                else
                    knockbackDirection = (transform.forward - transform.right).normalized;

                hitCollider.GetComponent<PlayerControllerOverhaul>().TakeDamageProxy(damage, knockback, knockbackDirection);
            }
        }
    }

    private void DrawHitbox() {VisualizeBox.DisplayBox(hitboxCenter.position + hitboxOffset, hitboxSize, Quaternion.LookRotation(direction), hitboxColor);}
    private void OnDrawGizmos() {DrawHitbox(); Debug.DrawRay(transform.position, direction * spacing, Color.blue); }
}
