using FishNet.Connection;
using FishNet.Object;
using pt_player_3d.Scripts;
using pt_player_3d.Scripts.Movement;
using pt_player_3d.Scripts.Rotation;
using UnityEngine;

public class LocalPlayerController : NetworkBehaviour
{
    [SerializeField] 
    private GameObject[] localPlayerOnlyObjects;
    
    private StandardMovementSystem _movementSystem;
    private JumpingSystem _jumpingSystem;
    private RotationSystem _rotationSystem;
    private ItemSystem _itemSystem;

    private void Awake()
    {
        _movementSystem = GetComponentInChildren<StandardMovementSystem>();
        _jumpingSystem = GetComponentInChildren<JumpingSystem>();
        _rotationSystem = GetComponentInChildren<RotationSystem>();
        _itemSystem = GetComponentInChildren<ItemSystem>();
    }

    public override void OnOwnershipClient(NetworkConnection prevOwner)
    {
        foreach (GameObject obj in localPlayerOnlyObjects)
            obj.SetActive(IsOwner);
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    private void Update()
    {
        if (IsOwner)
        {
            _movementSystem.ApplyMovementInput(new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized);
            _movementSystem.Tick(Time.deltaTime);

            _jumpingSystem.IsJumpHeld = Input.GetKey(KeyCode.Space);
            _jumpingSystem.Tick(Time.deltaTime);
            
            _rotationSystem.ApplyRotationInput(-Input.GetAxisRaw("Mouse Y"), Input.GetAxisRaw("Mouse X"));
            
            if (Input.GetKeyDown(KeyCode.Alpha1)) _itemSystem.RpcApplySelectItemInput(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) _itemSystem.RpcApplySelectItemInput(1);
            if (Input.GetKeyDown(KeyCode.Alpha3)) _itemSystem.RpcApplySelectItemInput(2);
            if (Input.GetKeyDown(KeyCode.Mouse0)) _itemSystem.RpcApplyFireInput();
        }
    }
}