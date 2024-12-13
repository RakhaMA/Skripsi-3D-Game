using UnityEngine;
using System.Collections.Generic;
using System.IO;
using StarterAssets;

public class PlayerMovementRecorder : MonoBehaviour
{
    [System.Serializable]
    public class PlayerMovementData
    {
        public float Time;
        public Vector3 Position;
        public Vector2 MoveInput; // Add this to record movement input
    }

    [System.Serializable]
    public class MovementDataList
    {
        public List<PlayerMovementData> data = new List<PlayerMovementData>();
    }

    public float recordInterval = 0.1f; // How often to record data (in seconds)
    private float nextRecordTime = 0f;

    private List<PlayerMovementData> movementData = new List<PlayerMovementData>();
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
        // Record data at specified intervals
        if (Time.time >= nextRecordTime)
        {
            RecordMovementData();
            nextRecordTime = Time.time + recordInterval;
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            SaveMovementData();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            ClearRecordedData();
        }
    }

    private void RecordMovementData()
    {
        PlayerMovementData data = new PlayerMovementData
        {
            Time = Time.time,
            Position = transform.position,
            MoveInput = inputs.move // Record the input values
        };

        movementData.Add(data);
    }

    public void SaveMovementData()
    {
        MovementDataList dataList = new MovementDataList
        {
            data = movementData
        };

        string filePath = Path.Combine(Application.persistentDataPath, "movementData.json");
        string json = JsonUtility.ToJson(dataList, true);
        File.WriteAllText(filePath, json);

        Debug.Log("Movement data saved to " + filePath);
    }

    public void ClearRecordedData()
    {
        movementData.Clear();
        string filePath = Path.Combine(Application.persistentDataPath, "movementData.json");

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log("Recorded data cleared.");
        }
        else
        {
            Debug.Log("No data file found to delete.");
        }
    }
}
