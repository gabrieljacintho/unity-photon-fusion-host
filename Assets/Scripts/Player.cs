using Fusion;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private NetworkCharacterController _characterController;
    [SerializeField] private NetworkPrefabRef _ballPrefab;
    [SerializeField] private PhysxBall _physxBallPrefab;

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
                    FireDelay = TickTimer.CreateFromSeconds(Runner, 0.5f);

                    Runner.Spawn(_ballPrefab, transform.position + transform.forward, Quaternion.LookRotation(transform.forward),
                        Object.InputAuthority, (runner, @object) =>
                        {
                            @object.GetComponent<Ball>().Init();
                        });

                }
                else if (data.Buttons.IsSet(NetworkInputData.MOUSEBUTTON1))
                {
                    FireDelay = TickTimer.CreateFromSeconds(Runner, 0.5f);

                    Runner.Spawn(_physxBallPrefab, transform.position + transform.forward, Quaternion.LookRotation(transform.forward),
                        Object.InputAuthority, (runner, @object) =>
                        {
                            @object.GetComponent<PhysxBall>().Init(transform.forward * 10f);
                        });
                }
            }
        }
    }
}
