using Fusion;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private NetworkCharacterController _characterController;
    [SerializeField] private NetworkPrefabRef _ballPrefab;

    [Networked] private TickTimer FireDelay { get; set; }


    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            data.Direction.Normalize(); // Prevent cheating
            _characterController.Move(5f * Runner.DeltaTime * data.Direction);

            if (HasStateAuthority && FireDelay.ExpiredOrNotRunning(Runner))
            {
                if (data.Buttons.IsSet(NetworkInputData.MOUSEBUTTON0))
                {
                    Runner.Spawn(_ballPrefab, transform.position + transform.forward, Quaternion.LookRotation(transform.forward),
                        Object.InputAuthority, (runner, @object) =>
                        {
                            @object.GetComponent<Ball>().Init();
                        });

                    FireDelay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                }
            }
        }
    }
}
