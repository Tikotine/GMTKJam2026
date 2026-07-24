using System.Collections;
using UnityEngine;

public class RandomPlayDelay : MonoBehaviour
{
    private ParticleSystem ps;
    [SerializeField] private float minDelay;
    [SerializeField] private float maxDelay;
    private float delay;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!ps)
        {
            ps = GetComponent<ParticleSystem>();
        }

        StartCoroutine(LoopParticle());
    }

    private IEnumerator LoopParticle()
    {
        RandomizeDelay();

        yield return new WaitForSeconds(delay);

        ps.Play();

        yield return new WaitForSeconds(ps.main.duration);
       
        StartCoroutine(LoopParticle());

        yield break;
    }

    private void RandomizeDelay()
    {
        delay = Random.Range(minDelay, maxDelay);
    }
}
