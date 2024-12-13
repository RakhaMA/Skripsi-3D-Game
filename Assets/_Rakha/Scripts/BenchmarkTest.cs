using System.Collections;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class BenchmarkTest : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    private StarterAssetsInputs playerInput;
    private PlayerInput playerInputComponent;
    private Camera mainCamera;

    private void Awake()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        playerInput = playerTransform.GetComponent<StarterAssetsInputs>();
        playerInputComponent = playerTransform.GetComponent<PlayerInput>();
        mainCamera = Camera.main;
    }

    public void StartBenchmark()
    {
        // Disable all input from player
        playerInputComponent.enabled = false;

        // Disable camera input
        mainCamera.TryGetComponent(out PlayerInput cameraInput);
        if (cameraInput != null)
        {
            cameraInput.enabled = false;
        }

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;

        // Disable camera rotation
        playerInput.cursorInputForLook = false;

        // Reset player position and rotation
        playerTransform.position = Vector3.zero;
        playerTransform.rotation = Quaternion.Euler(0, 0, 0);

        StartCoroutine(Benchmark());
    }

    private IEnumerator Benchmark()
    {
        // 1. Move forward for 10 seconds
        float startTime = Time.time;
        Quaternion initialRotation = playerTransform.rotation; // Store the initial rotation
        while (Time.time - startTime < 10)
        {
            playerInput.move = new Vector2(0, 1); // Forward
            playerTransform.rotation = initialRotation; // Reset the rotation to the initial rotation
            yield return null;
        }
        
        // 2. Move backward for 10 seconds
        startTime = Time.time;
        while (Time.time - startTime < 10)
        {
            playerInput.move = new Vector2(0, -1); // Backward
            playerTransform.rotation = initialRotation; // Reset the rotation to the initial rotation
            yield return null;
        }

        Debug.Log("Benchmark finished");

        // Re-enable player input and camera control
        playerInputComponent.enabled = true;

        var cameraInput = mainCamera.GetComponent<PlayerInput>();
        if (cameraInput != null)
        {
            cameraInput.enabled = true;
        }

        playerInput.cursorInputForLook = true;
    }
}
