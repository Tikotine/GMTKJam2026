using UnityEngine;

public class AttackOrb : MonoBehaviour
{
    private Vector3 targetPosition;

    public void SetTarget(Vector3 target)
    {
        targetPosition = target;
    }

    private void Update()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition,targetPosition,Time.deltaTime * 8f);
    }
}