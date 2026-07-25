using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TransformBillboard : MonoBehaviour
{
    public bool flipped = false;
    void UpdateBillboard()
    {
        transform.LookAt(transform.position + (flipped ? -Camera.main.transform.forward : Camera.main.transform.forward));
    }
    private void LateUpdate()
    {
        UpdateBillboard();
    }
    private void OnGUI()
    {
        UpdateBillboard();
    }
}
