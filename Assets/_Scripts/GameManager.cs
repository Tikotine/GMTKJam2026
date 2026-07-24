using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Character References")]
    public Player playerScript;
    public Enemy enemyScript;

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        playerScript = FindAnyObjectByType<Player>();
        enemyScript = FindAnyObjectByType<Enemy>();       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Let player pick heads or tails
    //roll heads or tails
    //Assign turn order accordingly



}
