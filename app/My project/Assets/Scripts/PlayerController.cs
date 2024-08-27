using UnityEngine;

/// <summary>
/// Class to control the player model actions
/// </summary>
public class PlayerController : MonoBehaviour
{
    private enum PlayerState
    {
        Stationary,
        Walk,
        Run,
        Sprint
    }

    private const float WalkThreshold = 0.5f;
    private const float RunThreshold = 10f;
    private const float SprintThreshold = 20f;

    private PlayerState currentPlayerState = PlayerState.Stationary;
    private bool IsPlayerMoving => currentPlayerState is
        PlayerState.Walk or PlayerState.Run or PlayerState.Sprint;

    //private bool IsPlayerResting => currentPlayerState is
    //    PlayerState.Stationary;

    public void UpdatePlayerState(float moveDistance)
    {
        switch (moveDistance)
        {
            case > SprintThreshold:
                {
                    if (currentPlayerState != PlayerState.Sprint)
                    {
                        currentPlayerState = PlayerState.Sprint;
                    }
                    break;
                }
                
            case > RunThreshold:
                {
                    if (currentPlayerState != PlayerState.Run)
                    {
                        currentPlayerState = PlayerState.Run;
                    }
                    break;   
                }

            case > WalkThreshold:
                {
                    if (currentPlayerState != PlayerState.Walk)
                    {
                        currentPlayerState = PlayerState.Walk;
                    }
                    break;
                }
            default:
                HandleIdleState();
                break;
        }
    }

    private void HandleIdleState()
    {
        if (IsPlayerMoving)
        {
            currentPlayerState = PlayerState.Stationary;
        }
    }
}
