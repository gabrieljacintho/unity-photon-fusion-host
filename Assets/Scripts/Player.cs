using Fusion;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private NetworkCharacterController _characterController;
    [SerializeField] private NetworkPrefabRef _ballPrefab;
    [SerializeField] private PhysxBall _physxBallPrefab;
    
    private ChangeDetector _changeDetector;
    private Material _material;

    [Networked] private TickTimer FireDelay { get; set; }
    [Networked] private bool SpawnedProjectile { get; set; }


    private void Awake()
    {
        _material = GetComponentInChildren<Renderer>().material;
    }

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

                    SpawnedProjectile = !SpawnedProjectile;
                }
                else if (data.Buttons.IsSet(NetworkInputData.MOUSEBUTTON1))
                {
                    FireDelay = TickTimer.CreateFromSeconds(Runner, 0.5f);

                    Runner.Spawn(_physxBallPrefab, transform.position + transform.forward, Quaternion.LookRotation(transform.forward),
                        Object.InputAuthority, (runner, @object) =>
                        {
                            @object.GetComponent<PhysxBall>().Init(transform.forward * 10f);
                        });

                    SpawnedProjectile = !SpawnedProjectile;
                }
            }
        }
    }

    public override void Spawned()
    {
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }

    /* 
     * This code iterates over all changes that happened to the Networked Properties since the last call to the ChangeDetector.
     * So in this case since the last Render. If a change in the color value is detected on any client the MeshRenderer is updated.
     * 
     * The primary benefit of using change callbacks over common messages like RPCs is that the callback is executed immediately
     * after the tick in which the value is changed, where as an RPC may arrive later when the game is in an completely different state.
     * 
     * The color should be updated in Render() as a linear interpolation from the current color towards blue.
     * This is done in Render() rather than Update() because it is guaranteed to run after FixedUpdateNetwork()
     * and it uses Time.deltaTime rather than Runner.DeltaTime because it is running in Unity's render loop and not as part of the Fusion simulation.
     */
    public override void Render()
    {
        foreach (var change in _changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(SpawnedProjectile):
                    _material.color = Color.white;
                    break;
            }
        }

        _material.color = Color.Lerp(_material.color, Color.blue, Time.deltaTime);
    }
}
