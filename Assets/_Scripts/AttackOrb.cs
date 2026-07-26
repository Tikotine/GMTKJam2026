using UnityEngine;

public class AttackOrb : MonoBehaviour
{
    private Vector3 targetPosition;

    private bool isFlyingToDefender;

    public void SetTarget(Vector3 target)
    {
        targetPosition = target;
    }

    public void SetFlyingToDefender(bool flying)
    {
        isFlyingToDefender = flying;
    }

    private void Update()
    {
        if (isFlyingToDefender)
        {
            return;
        }

        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * 8f);
    }
}