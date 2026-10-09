using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerSettings settings;

    public bool IsMoving { get; private set; } = false;
    public bool IsSprinting { get; private set; } = false;
    public float CurrentStamina { get; private set; }

    public UnityEvent<float, float> OnStaminaChanged = new UnityEvent<float, float>();

    private CharacterController controller;
    private PlayerInput input;
    private float regenTimer = 0f;
    private bool isExhausted = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
        input = GetComponent<PlayerInput>();

        CurrentStamina = settings.maxStamina;
        OnStaminaChanged?.Invoke(CurrentStamina, settings.maxStamina);
    }

    // Update is called once per frame
    void Update()
    {
        IsMoving = input.Move != Vector2.zero;

        if (isExhausted && CurrentStamina >= settings.minStaminaToSprint)
        {
            isExhausted = false;
        }

        bool canSprint = input.IsSprinting && IsMoving && !isExhausted && CurrentStamina > 0f;

        float currentSpeed = settings.speed;

        if (canSprint)
        {
            IsSprinting = true;
            currentSpeed *= settings.sprintMultiplier;

            CurrentStamina = Mathf.Max(0f, CurrentStamina - settings.staminaDrainRate * Time.deltaTime);
            regenTimer = settings.regenDelay; 

            if (CurrentStamina <= 0f)
            {
                isExhausted = true;
            }

            OnStaminaChanged?.Invoke(CurrentStamina, settings.maxStamina);
        }
        else
        {
            IsSprinting = false;

            if (regenTimer > 0f)
            {
                regenTimer -= Time.deltaTime;
            }
            else if (CurrentStamina < settings.maxStamina)
            {
                CurrentStamina = Mathf.Min(settings.maxStamina, CurrentStamina + settings.staminaRegenRate * Time.deltaTime);
                OnStaminaChanged?.Invoke(CurrentStamina, settings.maxStamina);
            }
        }

        var motion = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0) * new Vector3(input.Move.x, 0, input.Move.y) * currentSpeed * Time.deltaTime;

        controller.Move(motion);

        IsMoving = motion != Vector3.zero;
    }
}
