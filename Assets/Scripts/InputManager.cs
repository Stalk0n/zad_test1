using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public InputAction spaceAction = new InputAction(binding: "<Keyboard>/space");

    private SphereManager sphereManager;

    void Awake()
    {
        sphereManager = GetComponent<SphereManager>();
    }

    void OnEnable()
    {
        spaceAction.Enable();
        spaceAction.performed += OnSpacePressed;
    }

    void OnDisable()
    {
        spaceAction.performed -= OnSpacePressed;
        spaceAction.Disable();
    }

    void OnSpacePressed(InputAction.CallbackContext ctx)
    {
        sphereManager.OnSpacePressed();
    }
}