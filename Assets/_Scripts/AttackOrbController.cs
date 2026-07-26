using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackOrbController : MonoBehaviour
{
    [Header("Orb References")]
    [SerializeField] private Transform orbAnchor;
    [SerializeField] private GameObject successOrbPrefab;
    [SerializeField] private GameObject perfectOrbPrefab;

    [Header("Orb Formation")]
    [SerializeField] private float radius = 1.2f;
    [SerializeField] private float arcDegrees = 120f;

    [Header("Orb Attack Animation")]
    [Tooltip("How long it takes for an orb to move toward the defender.")]
    [SerializeField] private float orbMoveDuration = 0.5f;

    private readonly List<GameObject> activeOrbs = new();

    public void AddOrb(bool perfect)
    {
        GameObject prefab = perfect ? perfectOrbPrefab : successOrbPrefab;

        GameObject orb = Instantiate(prefab, orbAnchor);

        activeOrbs.Add(orb);

        ArrangeOrbs();
    }

    public IEnumerator RemoveFirstOrb(Transform defender)
    {
        if (activeOrbs.Count == 0)
        {
            yield break;
        }

        if (defender == null)
        {
            Debug.LogWarning("Cannot move attack orb because defender Transform is null.");

            Destroy(activeOrbs[0]);
            activeOrbs.RemoveAt(0);
            ArrangeOrbs();

            yield break;
        }

        GameObject orb = activeOrbs[0];

        if (orb == null)
        {
            activeOrbs.RemoveAt(0);
            ArrangeOrbs();

            yield break;
        }

        AttackOrb attackOrb = orb.GetComponent<AttackOrb>();

        if (attackOrb != null)
        {
            attackOrb.SetFlyingToDefender(true);
        }

        Transform orbTransform = orb.transform;

        Vector3 startPosition = orbTransform.position;
        Vector3 targetPosition = defender.position;

        float timer = 0f;

        while (timer < orbMoveDuration)
        {
            timer += Time.deltaTime;

            float normalizedTime = Mathf.Clamp01(timer / orbMoveDuration);
            float easedTime = EasingFunction.EaseInBack(0f, 1f, normalizedTime);
            orbTransform.position = Vector3.LerpUnclamped(startPosition, targetPosition, easedTime);

            yield return null;
        }

        orbTransform.position = targetPosition;
        Destroy(orb);
        activeOrbs.RemoveAt(0);

        ArrangeOrbs();
    }

    public void Clear()
    {
        foreach (GameObject orb in activeOrbs)
        {
            if (orb != null)
            {
                Destroy(orb);
            }
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
            Vector3 pos = new Vector3(Mathf.Sin(angle) * radius, Mathf.Cos(angle) * radius, 0);
            AttackOrb attackOrb = activeOrbs[i].GetComponent<AttackOrb>();

            if (attackOrb != null)
            {
                attackOrb.SetTarget(pos);
            }
        }
    }
}