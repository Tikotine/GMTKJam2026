using System.Collections.Generic;
using UnityEngine;

public class AttackOrbController : MonoBehaviour
{
    [SerializeField] private Transform orbAnchor;

    [SerializeField] private GameObject successOrbPrefab;
    [SerializeField] private GameObject perfectOrbPrefab;

    [SerializeField] private float radius = 1.2f;
    [SerializeField] private float arcDegrees = 120f;

    private readonly List<GameObject> activeOrbs = new();

    public void AddOrb(bool perfect)
    {
        GameObject prefab = perfect ? perfectOrbPrefab : successOrbPrefab;

        GameObject orb = Instantiate(prefab, orbAnchor);

        activeOrbs.Add(orb);

        ArrangeOrbs();
    }

    public void RemoveFirstOrb()
    {
        if (activeOrbs.Count == 0)
        {
            return;
        }
            
        Destroy(activeOrbs[0]);

        activeOrbs.RemoveAt(0);

        ArrangeOrbs();
    }

    public void Clear()
    {
        foreach (GameObject orb in activeOrbs)
        {
            Destroy(orb);
        }
            
        activeOrbs.Clear();
    }

    private void ArrangeOrbs()
    {
        if (activeOrbs.Count == 0)
        {
            return;
        }
            

        if (activeOrbs.Count == 1)
        {
            activeOrbs[0].transform.localPosition = Vector3.up * radius;
            return;
        }

        float start = -arcDegrees * 0.5f;
        float step = arcDegrees / (activeOrbs.Count - 1);

        for (int i = 0; i < activeOrbs.Count; i++)
        {
            float angle = (start + step * i) * Mathf.Deg2Rad;

            Vector3 pos = new Vector3(
                Mathf.Sin(angle) * radius,
                Mathf.Cos(angle) * radius,
                0);

            activeOrbs[i].GetComponent<AttackOrb>().SetTarget(pos);
        }
    }
}