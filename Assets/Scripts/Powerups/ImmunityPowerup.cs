using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ImmunityPowerup : MonoBehaviour
{
    [SerializeField] private float invincibilityTimer;
    [SerializeField] private Sprite indInv;

    [SerializeField] private Image borderImage;
    private InputAction doubleTapAction;

    private void Awake()
    {
        LevelHandler.powerupInstance = this;
        HandleDoubleTap();

    }

    private void HandleDoubleTap()
    {
        doubleTapAction = new InputAction(
            name: "DoubleTap",
            type: InputActionType.Button);

        doubleTapAction.AddBinding("<Touchscreen>/primaryTouch/press")
            .WithInteraction("MultiTap(tapCount=2)");

        doubleTapAction.performed += StartPowerup;
        doubleTapAction.Enable();
    }
    public void SetImmunity(bool value)
        => LevelHandler.Immune = value;

    public IEnumerator IndiactorAction()
    {
        borderImage.sprite = indInv;
        borderImage.color = Color.white;
        var dt = invincibilityTimer;
        var t = invincibilityTimer;
        SetImmunity(true);

        while (t > 0 && LevelHandler.Immune)
        {
            t -= Time.deltaTime;
            yield return null;
            
            borderImage.color = new Color(1,1,1,Mathf.Lerp(1,0,t / (dt / 2)));
        }

        EndPowerup();
        borderImage.color = Vector4.zero;
    }

    public IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(5);
        LevelHandler.AllowedToPowerup = true;
    }

    public void StartPowerup(InputAction.CallbackContext _)
    {
        print("Double tap!");

        if(LevelHandler.AllowedToPowerup == false)
            return;
        
        StartCoroutine(IndiactorAction());
        LevelHandler.AllowedToPowerup = false;
    }

    public void EndPowerup()
        => StartCoroutine(Cooldown());
}
