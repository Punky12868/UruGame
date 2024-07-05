using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GroundChangeBruja : MonoBehaviour
{
    [SerializeField] float lifeTime = 5;
    [SerializeField] float speed;
    [SerializeField] float localScale = 1;

    [SerializeField] Ease ease;
    [SerializeField] GameObject center;
    [SerializeField] GameObject[] middleCenter;
    [SerializeField] GameObject[] outGo;

    [SerializeField] Vector3 hitboxOneSize;
    [SerializeField] Vector3 hitboxTwoSize;
    [SerializeField] GameObject fango;
    [SerializeField] GameObject spike;
    [SerializeField] GameObject lava;

    string type;
    float ammount;
    float storedSpeed;
    float timer;
    bool ended;
    bool touchedPlayer;

    private void Awake()
    {
        int randomSelection = Random.Range(0, 3);
        switch (randomSelection)
        {
            case 0:
                type = "Lava";
                break;
            case 1:
                type = "Puas";
                break;
            case 2:
                type = "Fango";
                storedSpeed = FindObjectOfType<PlayerControllerOverhaul>().GetSpeed();
                break;
            default:
                break;
        }

        switch (type)
        {
            case "Lava":
                ammount = 30;
                break;
            case "Puas":
                ammount = 10;
                break;
            case "Fango":
                ammount = 0;
                break;
            default:
                break;
        }


        center.transform.DOMoveY(0.1f, speed).SetEase(ease).onComplete += () =>
        {
            SpawnObject(center.transform);
            foreach (GameObject go in middleCenter)
            {
                go.transform.DOMoveY(0.1f, speed).SetEase(ease).onComplete += () =>
                {
                    SpawnObject(go.transform);
                };
            }
            foreach (GameObject go in outGo)
            {
                go.transform.DOMoveY(0.1f, speed * 1.5f).SetEase(ease).onComplete += () =>
                {
                    SpawnObject(go.transform);
                };
            }
        };
    }

    private void Update()
    {
        if (ended) return;
        CheckCollisions();

        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            center.transform.DOMoveY(-0.5f, speed).SetEase(ease).onComplete += () =>
            {
                foreach (GameObject go in middleCenter)
                {
                    go.transform.DOMoveY(-0.5f, speed).SetEase(ease);
                }
                foreach (GameObject go in outGo)
                {
                    go.transform.DOMoveY(-0.5f, speed * 1.5f).SetEase(ease);
                }
            };
            Invoke("End", 3);

            if (type == "Fango") FindObjectOfType<PlayerControllerOverhaul>().SetSpeed(storedSpeed);

            ended = true;
        }
    }

    void End()
    {
        Destroy(gameObject);
    }

    private void CheckCollisions()
    {
        if (touchedPlayer) return;

        Collider[] hitColliders = Physics.OverlapBox(center.transform.position, hitboxOneSize, Quaternion.identity);
        
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                if (type != "Fango")
                {
                    HurtStates(hitCollider.GetComponent<PlayerControllerOverhaul>());
                    return;
                }
            }
        }

        Collider[] hitCollidersTwo = Physics.OverlapBox(center.transform.position, hitboxTwoSize, Quaternion.identity);
        
        foreach (var hitCollider in hitCollidersTwo)
        {
            if (hitCollider.CompareTag("Player"))
            {
                if (type != "Fango")
                {
                    HurtStates(hitCollider.GetComponent<PlayerControllerOverhaul>());
                    return;
                }
            }
        }
    }

    private void HurtStates(PlayerControllerOverhaul player)
    {
        if (touchedPlayer) return;

        player.TakeDamageProxy(ammount);
        touchedPlayer = true;
    }

    private void SpawnObject(Transform parent)
    {
        switch (type)
        {
            case "Lava":
                GameObject lav = Instantiate(lava, parent);
                lav.transform.localScale = new Vector3(localScale, localScale, localScale);
                break;
            case "Puas":
                GameObject pua = Instantiate(spike, parent);
                pua.transform.localScale = new Vector3(localScale, localScale, localScale);
                break;
            case "Fango":
                GameObject fang = Instantiate(fango, parent);
                fang.transform.localScale = new Vector3(localScale, localScale, localScale);
                break;
            default:
                break;
        }
    }

    private void OnDrawGizmos()
    {
        VisualizeBox.DisplayBox(center.transform.position, hitboxOneSize, Quaternion.identity, UnityEngine.Color.red);
        VisualizeBox.DisplayBox(center.transform.position, hitboxTwoSize, Quaternion.identity, UnityEngine.Color.red);
    }
}
