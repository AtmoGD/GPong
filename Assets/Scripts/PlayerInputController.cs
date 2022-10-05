using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerInputController : MonoBehaviour
{
    [SerializeField] private Paddle paddle;

    public void OnInputUp(InputAction.CallbackContext _context)
    {
        if (_context.phase != InputActionPhase.Started || paddle == null) return;

        paddle.MoveUp();
    }

    public void OnInputDown(InputAction.CallbackContext _context)
    {
        if (_context.phase != InputActionPhase.Started || paddle == null) return;

        paddle.MoveDown();
    }
}
