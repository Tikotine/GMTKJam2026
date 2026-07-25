using System.Runtime.CompilerServices;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Character References")]
    public Player playerScript;
    public Enemy enemyScript;

    [Header("Coin Toss")]
    [SerializeField] private int coin;

    public enum TurnState
    { 
        PLAYER_TURN,
        ENEMY_TURN,
        GAME_OVER
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        playerScript = FindAnyObjectByType<Player>();
        enemyScript = FindAnyObjectByType<Enemy>();       
    }
    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Let player pick heads or tails
    //roll heads or tails
    //Assign turn order accordingly

    private void CoinToss()
    {
        coin = Random.Range(0, 2);

        if (coin == 0)
        { 
            //Player starts first           
        }

        if (coin == 1) 
        {
            //Enemy Starts First            
        }
    }

}
