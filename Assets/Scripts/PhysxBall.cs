using Fusion;
using UnityEngine;

public class PhysxBall : NetworkBehaviour
{
    [Networked] private TickTimer Life { get; set; }

    public void Init(Vector3 forward)
    {
        Life = TickTimer.CreateFromSeconds(Runner, 5f);
        GetComponent<Rigidbody>().linearVelocity = forward;
    }

    public override void FixedUpdateNetwork()
    {
        if (Life.Expired(Runner))
        {
            Runner.Despawn(Object);
        }
    }
}
