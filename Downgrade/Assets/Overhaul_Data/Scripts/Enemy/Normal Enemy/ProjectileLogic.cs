using Rewired.Data.Mapping;
using UnityEngine;

public class ProjectileLogic : MonoBehaviour
{
    [SerializeField] private bool invertSprite = false;
    [SerializeField] private bool faceDir = false;
    [SerializeField] private bool destroyOnLimits = false;
    [SerializeField] private float mortarHeightLimit = 5;
    [SerializeField] private float mortarGroundLimit = 0.2f;
    [SerializeField] private Vector3 parryDetectionSize = new Vector3(0.5f, 0.5f, 0.5f);
    [SerializeField] private Color parryDetectionColor = new Color(1, 0, 0, 1);
    private float travelSpeed;
    private float damage;
    private float knockbackForce;
    private bool canBeParried;
    private bool isParried;
    private GameObject originalEnemy;
    private Vector3 direction;
    private AudioClip[] parrySounds;
    private Transform child;
    private bool isFromMortar;

    public void SetVariables(float speed, float dmg, float lftime, float knckbck, bool parry, Vector3 dir, AudioClip[] prrySnd, GameObject orgnlEnemy, bool fromMortar = false, bool spawnedMortar = false)
    {
        travelSpeed = speed;
        damage = dmg;
        //direction = dir;
        transform.rotation = Quaternion.identity;
        direction = (GameObject.FindGameObjectWithTag("Player").transform.position - transform.position).normalized;
        canBeParried = parry;
        knockbackForce = knckbck;
        parrySounds = prrySnd;
        originalEnemy = orgnlEnemy;
        isFromMortar = fromMortar;
        if (isFromMortar)
        {
            direction = spawnedMortar ? Vector3.down.normalized : Vector3.up.normalized;
        }

        if (spawnedMortar)
        {
            Transform target = GameObject.FindGameObjectWithTag("Player").transform;
            transform.position = new Vector3(target.position.x, transform.position.y, target.position.z);
        }

        child = GetComponentInChildren<SpriteRenderer>().transform;

        Destroy(gameObject, lftime);
    }

    private void Update()
    {
        if (GameManager.Instance.IsGamePaused())
            return;

        transform.Translate(direction * travelSpeed * Time.deltaTime);
        CheckForPlayerParry();

        if (GetComponent<SpriteRenderer>()) { if (direction.x > 0 && !GetComponent<SpriteRenderer>().flipX); }
        else { if (direction.x > 0 && !GetComponentInChildren<SpriteRenderer>().flipX) GetComponentInChildren<SpriteRenderer>().flipX = invertSprite ? true : false; }

        if (faceDir) child.rotation = Quaternion.FromToRotation(Vector3.right, direction);
        else child.rotation = Quaternion.LookRotation(Camera.main.transform.forward, Camera.main.transform.up);

        if (isFromMortar)
        {
            if (transform.position.y >= mortarHeightLimit)
            {
                SpawnAnotherProjectile();
                Destroy(gameObject);
            }

            if (transform.position.y <= mortarGroundLimit)
            {
                Destroy(gameObject);
                //ChangeGround
            }
        }
    }

    private void SpawnAnotherProjectile()
    {
        GameObject prjctl = Instantiate(gameObject, transform.position, transform.rotation);
        prjctl.GetComponent<ProjectileLogic>().SetVariables(travelSpeed, damage, 5, knockbackForce, false, direction, parrySounds, originalEnemy, true, true);
    }

    private void CheckForPlayerParry()
    {
        Collider[] hitColliders = Physics.OverlapBox(transform.position, parryDetectionSize, Quaternion.identity);

        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                PlayerControllerOverhaul player = hitCollider.GetComponent<PlayerControllerOverhaul>();

                if (player.GetPlayerState() == "Parry" && canBeParried)
                {
                    if (player.GetParryAvailable(transform))
                    {
                        direction *= -1;
                        isParried = true;
                        originalEnemy.GetComponent<EnemyBase>().PlaySoundProxy(parrySounds);
                        player.GetParryRewardProxy(EnemyType.None);
                        Vector2 vectorDamage = player.GetDamage();
                        damage = Random.Range(vectorDamage.x, vectorDamage.y) * 2;
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isParried)
        {
            if (!other.GetComponent<PlayerControllerOverhaul>().GetImmunity())
            {
                other.GetComponent<PlayerControllerOverhaul>().TakeDamageProxy(damage, knockbackForce, -direction);
                Destroy(gameObject);
            }
        }

        if (other.CompareTag("Enemy") && isParried)
        {
            other.GetComponent<EnemyBase>().TakeDamageProxy(damage);
            Destroy(gameObject);
        }

        if (other.CompareTag("Wall") || other.CompareTag("Limits"))
        {
            if(destroyOnLimits) Destroy(gameObject);
        }
    }

    private void DrawParryDetectionHitbox(Vector3 size, Color color)
    {
        VisualizeBox.DisplayBox(transform.position, size, transform.rotation, color);
    }

    private void OnDrawGizmos()
    {
        DrawParryDetectionHitbox(parryDetectionSize, parryDetectionColor);
    }
}
