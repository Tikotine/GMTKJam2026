using UnityEngine;

public class CombatAnimationController : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator=GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayIdle()
    {
        animator.Play("Idle");
    }

    public void PlayCast()
    {
        animator.Play("Cast");
    }

    public void PlayFlinch()
    {
        animator.Play("Flinch");
    }

    public void PlayParry()
    {
        animator.Play("Parry");
    }
}
