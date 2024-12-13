using UnityEngine;
using System.Collections.Generic;
using System.IO;
using StarterAssets;

public class MovementPlaybackController : MonoBehaviour
{
    [System.Serializable]
    public class PlayerMovementData
    {
        public float Time;
        public Vector3 Position;
        public Vector2 MoveInput; // Add this to simulate movement input
    }

    [System.Serializable]
    public class MovementDataList
    {
        public List<PlayerMovementData> data = new List<PlayerMovementData>();
    }

    public float playbackSpeed = 1f;
    private List<PlayerMovementData> movementData = new List<PlayerMovementData>();
    private int currentIndex = 0;
    private float playbackTime = 0f;
    private bool isBenchmarking = false;

    private ThirdPersonController thirdPersonController;
    private StarterAssetsInputs inputs; // Assuming this is the input script used by ThirdPersonController

    void Start()
    {
        thirdPersonController = GetComponent<ThirdPersonController>();
        inputs = GetComponent<StarterAssetsInputs>();
        // Ensure the ThirdPersonController and inputs are assigned correctly
    }

    void Update()
    {
        if (isBenchmarking)
        {
            playbackTime += Time.deltaTime * playbackSpeed;

            if (currentIndex < movementData.Count && playbackTime >= movementData[currentIndex].Time)
            {
                // Set target position and move input
                Vector3 targetPosition = movementData[currentIndex].Position;
                Vector2 moveInput = movementData[currentIndex].MoveInput;

                // Simulate input to drive movement
                inputs.move = moveInput;

                // Move the character towards the target position
                // Use a method to simulate player movement
                SimulateMovement(targetPosition);

                currentIndex++;
            }

            if (currentIndex >= movementData.Count)
            {
                StopBenchmark();
            }
        }
    }

    private void SimulateMovement(Vector3 targetPosition)
    {
        // Apply movement simulation
        Vector3 direction = (targetPosition - transform.position).normalized;
        inputs.move = new Vector2(direction.x, direction.z); // Update movement input
        thirdPersonController.Update(); // Force update to apply new input
    }

    public void StartBenchmark()
    {
        Debug.Log("Starting benchmark...");
        LoadMovementData();
        isBenchmarking = true;
        thirdPersonController.enabled = false; // Disable ThirdPersonController to prevent manual control
        inputs.enabled = false; // Disable input processing
    }

    public void StopBenchmark()
    {
        Debug.Log("Benchmark finished");
        isBenchmarking = false;
        thirdPersonController.enabled = true; // Re-enable ThirdPersonController
        inputs.enabled = true; // Re-enable input processing
    }

    void LoadMovementData()
    {
        Debug.Log("Loading movement data...");
        string filePath = Path.Combine(Application.persistentDataPath, "movementData.json");

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            MovementDataList dataList = JsonUtility.FromJson<MovementDataList>(json);
            movementData = dataList.data;
            currentIndex = 0;
            playbackTime = 0f;
            Debug.Log("Movement data loaded successfully!");
        }
        else
        {
            Debug.LogError("Movement data file not found!");
        }
    }
}
