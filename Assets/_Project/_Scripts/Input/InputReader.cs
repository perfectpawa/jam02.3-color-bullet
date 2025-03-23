using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static PlayerInputActions;

public interface IInputReader
{
    Vector2 Direction { get; }
    void EnablePlayerAction();
}

[CreateAssetMenu(fileName = "InputReader", menuName = "New InputReader")]
public class InputReader : ScriptableObject, IInputReader, IPlayerActions
{
    public event UnityAction<Vector2> Move = delegate { };
    public event UnityAction<bool> Fire = delegate { };
    
    public PlayerInputActions inputActions;
    
    public Vector2 Direction => inputActions.Player.Move.ReadValue<Vector2>();
    public bool IsFirePressed => inputActions.Player.Fire.ReadValue<bool>();
    public Vector2 MousePosition => inputActions.Player.Look.ReadValue<Vector2>();
    
    public void EnablePlayerAction()
    {
        if (inputActions == null)
        {
            inputActions = new PlayerInputActions();
            inputActions.Player.SetCallbacks(this);
        }
        inputActions.Enable();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Move?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                Fire?.Invoke(true);
                break;
            case InputActionPhase.Canceled:
                Fire?.Invoke(false);
                break;
            
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        //nothing
    }

    public void OnLook(InputAction.CallbackContext context)
    {
           
    }
}
