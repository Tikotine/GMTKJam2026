using UnityEngine;
using System.Collections.Generic;

public class PeanutGallery : MonoBehaviour
{
    public List<GameObject> audience = new List<GameObject>();
    public float amplitude;
    public float speed;
    float time;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MoveAudience();
    }

    public void MoveAudience()
    {
        time += Time.deltaTime * speed * 2 * Mathf.PI;
        foreach (GameObject go in audience)
        {
            if (audience.Count == 0) continue;
            float unitOffset = speed * 2 * Mathf.PI / audience.Count;
            for (int i = 0; i < audience.Count; i++)
            {
                var pos = audience[i].transform.localPosition;
                pos.y = amplitude * Mathf.Sin(time + unitOffset * i);
                audience[i].transform.localPosition = pos;
            }
        }
    }
}
