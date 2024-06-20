using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class AbilityCell : MonoBehaviour
{
    [Header("----------")]
    //[SerializeField] private AnimationClip cellOutClip;
    [SerializeField] private GameObject cellPrefab;

    [Header("----------")]
    [SerializeField] private float damageMultiplier = 1;
    [SerializeField] private float appearTime = 0.2f;
    [SerializeField] private float spawnTime;
    [SerializeField] private float hitboxAppearTime = 0.2f;
    [SerializeField] private float spacing = 0.4f;
    [SerializeField] private int maxCells;
    [SerializeField] private float hitCooldown = 0.3f;

    [Header("----------")]
    [SerializeField] private Transform hitboxCenter;
    [SerializeField] private Vector3 hitboxSize = Vector3.one;
    [SerializeField] private Vector3 hitboxOffset = Vector3.zero;
    [SerializeField] private Color hitboxAppearColor = new Color(0, 0, 1, 1);
    [SerializeField] private Color hitboxNormalColor = new Color(0, 1, 0, 1);

    private int cellId;
    private bool isCenter;
    private bool isRight;
    private bool isLeft;
    private Color hitboxColor = new Color(0, 0, 1, 1);
    private Vector3 direction;
    private int damage;
    private float knockback;
    private float lifeTime;
    private float moveTimer;
    private float timer;
    private bool spawnedAnother;
    private bool limitReached;
    private bool lifeTimeReached = true;
    private bool onHitCooldown;
    private List<GameObject> enemies = new List<GameObject>();
    private void ResetLifeTimeReached() { lifeTimeReached = false; }

    private void Awake()
    {
        moveTimer = -0.05f;
        if (damageMultiplier <= 0) damageMultiplier = 1;
        if (cellId == 0)
        {
            if (isLeft || isCenter && isLeft)
            {
                Ray rightRay = new Ray(transform.position, transform.right * spacing);
                if (CheckCell(rightRay)) cellId = GetId(rightRay);
            }

            if (isRight || isCenter && isRight)
            {
                Ray leftRay = new Ray(transform.position, -transform.right * spacing);
                if (CheckCell(leftRay)) cellId = GetId(leftRay);
            }
        }
    }

    public void SetVariables(int damage, float knockback, float lifeTime, Vector3 direction, bool center, bool right, bool left)
    {
        this.damage = damage;
        this.knockback = knockback;
        this.lifeTime = lifeTime;
        this.direction = direction.normalized;
        this.isCenter = center;
        this.isRight = right;
        this.isLeft = left;
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
        /*foreach (var clip in GetComponent<Animator>().runtimeAnimatorController.animationClips)
        { if (clip.name == cellOutClip.name) { GetComponent<Animator>().Play(cellOutClip.name); } }*/

        lifeTimeReached = true;
    }

    private void Update()
    {
        transform.position = new Vector3(transform.position.x, moveTimer, transform.position.z);

        if (transform.position.y < 0 && !lifeTimeReached) moveTimer += Time.deltaTime * appearTime;
        else if (transform.position.y > -0.05 && lifeTimeReached) moveTimer -= Time.deltaTime * appearTime;

        DamageEnemy();
        if (spawnedAnother || limitReached) return;
        if (timer < spawnTime) { timer += Time.deltaTime; return; }
        SpawnCell();
    }

    private void SpawnCell()
    {
        if (cellId > maxCells && maxCells != 0) return;

        Ray forwardRay = new Ray(transform.position, direction * spacing);
        Ray backwardRay = new Ray(transform.position, -direction * spacing);
        Ray rightRay = new Ray(transform.position, transform.right * spacing);
        Ray leftRay = new Ray(transform.position, -transform.right * spacing);

        if (CheckLimitsHit(forwardRay)) limitReached = true;
        if (limitReached) return;

        if (CheckCell(backwardRay))
        {
            if (cellId == 1)
            {
                SpawnCell(forwardRay.origin, forwardRay.direction, true, false, false);
                SpawnCell(rightRay.origin, rightRay.direction, false, true, false);
                SpawnCell(leftRay.origin, leftRay.direction, false, false, true); return;
            }

            if (isCenter && isRight)
            {
                SpawnCell(forwardRay.origin, forwardRay.direction, true, false, false);
                SpawnCell(rightRay.origin, rightRay.direction, false, true, false); return;
            }

            if (isCenter && isLeft)
            {
                SpawnCell(forwardRay.origin, forwardRay.direction, true, false, false);
                SpawnCell(leftRay.origin, leftRay.direction, false, false, true); return;
            }

            if (isCenter) { SpawnCell(forwardRay.origin, forwardRay.direction, true, false, false); return; }
        }
        else
        {
            if (cellId == 0) { SpawnCell(forwardRay.origin, forwardRay.direction, true, false, false); return; }
            if (isRight) { SpawnCell(forwardRay.origin, forwardRay.direction, true, true, false); return; }
            else if (isLeft) { SpawnCell(forwardRay.origin, forwardRay.direction, true, false, true); return; }
        }
        
    }

    private void DamageEnemy()
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
            if (!NullOrCero.isListNullOrCero(enemies)) for (int i = 0; i < enemies.Count; i++) { if (hitCollider.gameObject == enemies[i]) return; }

            if (hitCollider.CompareTag("Enemy"))
            {
                Vector3 enemyPos = hitCollider.transform.position;
                Vector3 directionToEnemy = (enemyPos - transform.position).normalized;
                Vector3 crossProduct = Vector3.Cross(transform.forward, directionToEnemy);
                Vector3 knockbackDirection;

                if (crossProduct.y > 0) knockbackDirection = (transform.forward + transform.right).normalized;
                else knockbackDirection = (transform.forward - transform.right).normalized;

                if (hitCollider.GetComponent<EnemyBase>()) HitEnemy(hitCollider, damage);
                if (hitCollider.GetComponent<BossBase>()) HitBoss(hitCollider, damage);

                enemies.Add(hitCollider.gameObject);
                AbilityCell[] cells = FindObjectsOfType<AbilityCell>();
                foreach (AbilityCell cell in cells) { cell.AddEnemy(hitCollider.gameObject); }

                onHitCooldown = true;
                Invoke("ResetCooldown", hitCooldown);
            }
        }
    }

    private void SpawnCell(Vector3 origin, Vector3 dir, bool isCenter = false, bool isRight = false, bool isLeft = false)
    {
        Vector3 spawnPoint = origin + dir * spacing;
        GameObject cell = Instantiate(cellPrefab, spawnPoint, Quaternion.LookRotation(direction));
        cell.GetComponent<AbilityCell>().SetId(GetNewID());
        cell.GetComponent<AbilityCell>().SetVariables(damage, knockback, lifeTime, direction, isCenter, isRight, isLeft);
        spawnedAnother = true;
    }

    private bool CheckCell(Ray ray)
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, spacing))
        {
            if (hit.collider.tag == "AbilityCell")
            {
                return true;
            }
        }
        return false;
    }

    private int GetId(Ray ray)
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, spacing))
        {
            if (hit.collider.tag == "AbilityCell")
            {
                return hit.collider.GetComponent<AbilityCell>().GetId();
            }
        }
        return 0;
    }

    private bool CheckLimitsHit(Ray ray)
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, spacing))
        {
            if (hit.collider.tag == "Limits" || hit.collider.tag == "Wall")
            {
                return true;
            }
        }
        return false;
    }

    public void ResetCooldown() { onHitCooldown = false; }
    public void SetId(int id) { cellId = id; }
    public int GetId() { return cellId; }
    private int GetNewID() { if (isCenter) return cellId + 1; return cellId; }
    private void HitEnemy(Collider hit, int damage) { hit.GetComponent<EnemyBase>().TakeDamageProxy(damage, knockback, direction); }
    private void HitBoss(Collider hit, int damage) { hit.GetComponent<BossBase>().TakeDamage(damage, knockback, direction); }
    public void SetCellId(int id) { cellId = id; }
    public void AddEnemy(GameObject enemy) { enemies.Add(enemy); }
    private void DrawHitbox() { VisualizeBox.DisplayBox(hitboxCenter.position + hitboxOffset, hitboxSize, Quaternion.LookRotation(direction), hitboxColor); }
    private void OnDrawGizmos() { DrawHitbox(); Debug.DrawRay(transform.position, direction * spacing, Color.blue); }
}
