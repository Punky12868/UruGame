using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutOutObject : MonoBehaviour
{
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private float cutoutSize = 0.1f, falloffSize = 0.05f;
    private Transform target;
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();

        Transform obj = FindObjectOfType<CinemachineVirtualCamera>().transform;
        //obj.position = new Vector3(obj.position.x, 3.37f, obj.position.z + 2);
    }

    public void AddTarget(Transform target)
    {
        this.target = target;
    }

    private void Update()
    {
        if (target == null)
            return;

        Vector2 cutoutPos = mainCamera.WorldToViewportPoint(target.position);
        cutoutPos.y /= (Screen.width / Screen.height);

        Vector3 offset = target.position - transform.position;
        RaycastHit[] hitObjs = Physics.RaycastAll(transform.position, offset, offset.magnitude, wallMask);

        for (int i = 0; i < hitObjs.Length; i++)
        {
            Material[] materials = hitObjs[i].transform.GetComponent<Renderer>().materials;

            for (int j = 0; j < materials.Length; j++)
            {
                materials[j].SetVector("_CutOut_Pos", cutoutPos);
                materials[j].SetFloat("_CutOut_SIze", cutoutSize);
                materials[j].SetFloat("_FallOff_Size", falloffSize);
            }
        }
    }
}
