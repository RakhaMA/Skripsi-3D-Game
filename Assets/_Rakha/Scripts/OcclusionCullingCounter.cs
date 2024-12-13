using UnityEngine;
using System.Collections.Generic;

public class OcclusionCullingCounter : MonoBehaviour
{
    public LayerMask layerToCheck; // Layer(s) to identify relevant GameObjects
    private List<GameObject> objectsToCheck = new List<GameObject>();
    private int occludedByOcclusionCulling;
    private int lastOccludedCount;
    private Plane[] frustumPlanes;

    void Start()
    {
        // Find all objects in the specified layer(s)
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (((1 << obj.layer) & layerToCheck) != 0) // Check if the object's layer is in the layer mask
            {
                objectsToCheck.Add(obj);
            }
        }

        if (objectsToCheck.Count == 0)
        {
            Debug.LogWarning("No objects found in the specified layer(s).");
        }
    }

    void Update()
    {
        if (objectsToCheck == null || objectsToCheck.Count == 0)
        {
            return; // Exit early if there are no objects to check
        }

        occludedByOcclusionCulling = 0;
        frustumPlanes = GeometryUtility.CalculateFrustumPlanes(Camera.main);

        foreach (GameObject obj in objectsToCheck)
        {
            if (obj == null)
            {
                continue; // Skip any null objects
            }

            Renderer renderer = obj.GetComponent<Renderer>();

            if (renderer == null)
            {
                Debug.LogWarning($"GameObject '{obj.name}' does not have a Renderer component.");
                continue; // Skip objects without a Renderer
            }

            bool isInFrustum = GeometryUtility.TestPlanesAABB(frustumPlanes, renderer.bounds);

            // Count it as occluded only if it's within the frustum and not visible
            if (isInFrustum && !renderer.isVisible)
            {
                occludedByOcclusionCulling++;
            }
        }

        if (occludedByOcclusionCulling != lastOccludedCount)
        {
            Debug.Log("Number of objects occluded by occlusion culling: " + occludedByOcclusionCulling);
            lastOccludedCount = occludedByOcclusionCulling;
        }
    }
}
