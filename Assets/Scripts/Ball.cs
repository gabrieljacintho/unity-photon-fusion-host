using Fusion;

public class Ball : NetworkBehaviour
{
    [Networked] private TickTimer Life { get; set; }


    public void Init()
    {
        Life = TickTimer.CreateFromSeconds(Runner, 5f);
    }

    public override void FixedUpdateNetwork()
    {
        if (Life.Expired(Runner))
        {
            Runner.Despawn(Object);
            return;
        }

        // Runner.DeltaTime corresponding to the time between ticks
        transform.position += 5f * Runner.DeltaTime * transform.forward;
    }
}
