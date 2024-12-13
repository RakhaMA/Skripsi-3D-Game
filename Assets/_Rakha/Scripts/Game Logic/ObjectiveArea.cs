using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveArea : MonoBehaviour
{
    // make enum for objective type
    public enum ObjectiveType
    {
        Hospital,
        Park,
        BowlingAlley,
    }

    ObjectiveManager objectiveManager;
    public ObjectiveType objectiveType;

    private void Start()
    {
        objectiveManager = GetComponentInParent<ObjectiveManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered " + objectiveType.ToString());
            if (objectiveType == ObjectiveType.Hospital)
            {
                objectiveManager.PlayerInHospital = true;
                objectiveManager.objectiveTextHospital.SetActive(false);
                gameObject.SetActive(false);
            }
            else if (objectiveType == ObjectiveType.Park)
            {
                objectiveManager.PlayerInPark = true;
                objectiveManager.objectiveTextPark.SetActive(false);
                gameObject.SetActive(false);
            }
            else if (objectiveType == ObjectiveType.BowlingAlley)
            {
                objectiveManager.PlayerInBowlingAlley = true;
                objectiveManager.objectiveTextBowlingAlley.SetActive(false);
                gameObject.SetActive(false);
            }
        }
    }
}
