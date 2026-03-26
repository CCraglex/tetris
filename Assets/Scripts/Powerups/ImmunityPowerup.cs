using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ImmunityPowerup : MonoBehaviour
{
    [SerializeField] private LevelGameplay levelGameplay;
    [SerializeField] private float invincibilityTimer;
    [SerializeField] private Sprite indInv;

    [SerializeField] private Image borderImage;
    private InputAction doubleTapAction;

    private bool powerupCooldown;

    private void Awake()
        => InitInput();

    private void InitInput()
    {
        doubleTapAction = new InputAction(
            name: "DoubleTap",
            type: InputActionType.Button);

        doubleTapAction.AddBinding("<Touchscreen>/primaryTouch/press")
            .WithInteraction("MultiTap(tapCount=2)");

        doubleTapAction.performed += StartPowerup;
        doubleTapAction.Enable();
    }

    public IEnumerator IPowerup()
    {
        yield return StartCoroutine(levelGameplay.ActivatePowerup());
        powerupCooldown = true;

        yield return new WaitForSeconds(invincibilityTimer);
        powerupCooldown = false;
    }

    public void StartPowerup(InputAction.CallbackContext _)
    {
        print("Double tap!");

        if(!SaveStateHandler.HasPowerup() && !powerupCooldown && !levelGameplay.isPowerUpOn)
            StartCoroutine(IPowerup());
    }
}
