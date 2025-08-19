using Fusion;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private NetworkCharacterController _characterController;


    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            data.Direction.Normalize(); // Prevent cheating
            _characterController.Move(data.Direction * 5f * Runner.DeltaTime);
        }
    }
}
