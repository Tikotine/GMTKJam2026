using UnityEngine;
using Unity.Cinemachine;

public class CombatCameraManager : MonoBehaviour
{
    public enum CameraState
    {
        DiceAssignment,
        PlayerCombat,
        EnemyCombat
    }

    [Header("Cinemachine Cameras")]
    [SerializeField] private CinemachineCamera camera1_DiceAssignment;
    [SerializeField] private CinemachineCamera camera2_PlayerCombat;
    [SerializeField] private CinemachineCamera camera3_EnemyCombat;

    [Header("Camera Priorities")]
    [SerializeField] private int activePriority = 20;
    [SerializeField] private int inactivePriority = 10;

    [Header("References")]
    [SerializeField] private GameManager gameManager;

    private CameraState currentCameraState;

    private void Awake()
    {
        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<GameManager>();
        }
    }

    private void Start()
    {
        SetCamera(CameraState.DiceAssignment);
    }

    public void SetCamera(CameraState newCameraState)
    {
        currentCameraState = newCameraState;

        SetAllCamerasToInactive();

        switch (newCameraState)
        {
            case CameraState.DiceAssignment:
                camera1_DiceAssignment.Priority = activePriority;
                Debug.Log("Camera switched to: Dice Assignment Camera");
                break;

            case CameraState.PlayerCombat:
                camera2_PlayerCombat.Priority = activePriority;
                Debug.Log("Camera switched to: Player Combat Camera");
                break;

            case CameraState.EnemyCombat:
                camera3_EnemyCombat.Priority = activePriority;
                Debug.Log("Camera switched to: Enemy Combat Camera");
                break;
        }
    }

    private void SetAllCamerasToInactive()
    {
        camera1_DiceAssignment.Priority = inactivePriority;
        camera2_PlayerCombat.Priority = inactivePriority;
        camera3_EnemyCombat.Priority = inactivePriority;
    }

    public void SetDiceAssignmentCamera()
    {
        SetCamera(CameraState.DiceAssignment);
    }

    public void SetPlayerCombatCamera()
    {
        SetCamera(CameraState.PlayerCombat);
    }

    public void SetEnemyCombatCamera()
    {
        SetCamera(CameraState.EnemyCombat);
    }

    public void SetCombatCameraBasedOnAttacker()
    {
        if (gameManager == null)
        {
            Debug.LogWarning("GameManager reference is missing.");
            return;
        }

        if (gameManager.IsPlayerAttacking())
        {
            SetPlayerCombatCamera();
        }
        else
        {
            SetEnemyCombatCamera();
        }
    }

    public CameraState GetCurrentCameraState()
    {
        return currentCameraState;
    }
}