using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerStates PlayerState;

    public enum PlayerStates
    {
        Idle,
        Shooting
    }

    
}
