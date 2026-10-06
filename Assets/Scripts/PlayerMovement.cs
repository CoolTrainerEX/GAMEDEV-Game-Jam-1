using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerSettings settings;

    public bool IsMoving { get; private set; } = false;

    private CharacterController controller;
    private PlayerInput input;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
        input = GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        var motion = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0) * new Vector3(input.Move.x, 0, input.Move.y) * settings.speed * Time.deltaTime;

        controller.Move(motion);

        IsMoving = motion != Vector3.zero;
    }
}
