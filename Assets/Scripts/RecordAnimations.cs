using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class MixamoRigLoopRecorder : MonoBehaviour
{
    public bool Read = false;

    private List<string> boneNames = new List<string>();

    // Store up to 299 frames of local positions and rotations
    private List<List<Vector3>> recordedLocalPositions = new List<List<Vector3>>();
    private List<List<Quaternion>> recordedLocalRotations = new List<List<Quaternion>>();

    private Animator animator;
    private int maxFrameCount = 299;

    void Start()
    {
        animator = GetComponent<Animator>();
        foreach (Transform child in transform.GetComponentsInChildren<Transform>())
        {
            if (child.name.Contains("mixamorig"))
            {
                boneNames.Add(child.name);
            }
        }
        if (Read)
        {
            animator.enabled = false;
            LoadFromFile();
        }
    }

    void FixedUpdate()
    {
        if (Read)
        {
            if (maxFrameCount > 0 && recordedLocalPositions.Count > 0)
            {
                int currentFrame = Time.frameCount % recordedLocalPositions.Count;

                for (int i = 0; i < boneNames.Count; i++)
                {
                    GameObject boneObject = FindChildByName(transform, boneNames[i])?.gameObject;
                    if (boneObject != null)
                    {
                        boneObject.transform.localPosition = recordedLocalPositions[currentFrame][i];
                        boneObject.transform.localRotation = recordedLocalRotations[currentFrame][i];
                    }
                }
            }
        }
        else
        {
            if (recordedLocalPositions.Count >= maxFrameCount)
                return;

            // Store current frame's data
            List<Vector3> currentLocalPositions = new List<Vector3>();
            List<Quaternion> currentLocalRotations = new List<Quaternion>();

            foreach (string boneName in boneNames)
            {
                GameObject boneObject = FindChildByName(transform, boneName)?.gameObject;
                if (boneObject != null)
                {
                    currentLocalPositions.Add(boneObject.transform.localPosition);
                    currentLocalRotations.Add(boneObject.transform.localRotation);
                }
            }

            // Save current frame data
            recordedLocalPositions.Add(currentLocalPositions);
            recordedLocalRotations.Add(currentLocalRotations);

            // Save to file when the limit is reached
            if (recordedLocalPositions.Count == maxFrameCount)
            {
                SaveToFile();
            }
        }
    }

    private Transform FindChildByName(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
            {
                return child;
            }
            Transform found = FindChildByName(child, name);
            if (found != null)
            {
                return found;
            }
        }
        return null;
    }

    private void SaveToFile()
    {
        string filePath = Path.Combine(Application.dataPath, "MixamoRigRecordedData.txt");
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            for (int frameIndex = 0; frameIndex < recordedLocalPositions.Count; frameIndex++)
            {
                writer.WriteLine($"Frame {frameIndex + 1}:");
                for (int i = 0; i < boneNames.Count; i++)
                {
                    Vector3 position = recordedLocalPositions[frameIndex][i];
                    Quaternion rotation = recordedLocalRotations[frameIndex][i];

                    writer.WriteLine($"  Bone: {boneNames[i]} | Position: ({position.x:F6}, {position.y:F6}, {position.z:F6}) | Rotation: ({rotation.x:F6}, {rotation.y:F6}, {rotation.z:F6}, {rotation.w:F6})");
                }
            }
        }
        Debug.Log($"Recorded 299 frames. Data saved to {filePath}");
    }

    private void LoadFromFile()
    {
        string filePath = Path.Combine(Application.dataPath, "MixamoRigRecordedData.txt");

        if (File.Exists(filePath))
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                List<Vector3> framePositions = null;
                List<Quaternion> frameRotations = null;

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("Frame"))
                    {
                        if (framePositions != null && frameRotations != null)
                        {
                            recordedLocalPositions.Add(framePositions);
                            recordedLocalRotations.Add(frameRotations);
                        }

                        framePositions = new List<Vector3>();
                        frameRotations = new List<Quaternion>();
                    }
                    else
                    {
                        string[] parts = line.Split('|');
                        if (parts.Length == 3)
                        {
                            string positionPart = parts[1].Trim();
                            string rotationPart = parts[2].Trim();

                            Vector3 position = ParseVector3(positionPart);
                            Quaternion rotation = ParseQuaternion(rotationPart);

                            framePositions.Add(position);
                            frameRotations.Add(rotation);
                        }
                    }
                }

                if (framePositions != null && frameRotations != null)
                {
                    recordedLocalPositions.Add(framePositions);
                    recordedLocalRotations.Add(frameRotations);
                }
            }
        }
        else
        {
            Debug.LogError($"File not found: {filePath}");
        }
    }

    private Vector3 ParseVector3(string text)
    {
        text = text.Replace("Position: (", "").Replace(")", "");
        string[] parts = text.Split(',');
        return new Vector3(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]));
    }

    private Quaternion ParseQuaternion(string text)
    {
        text = text.Replace("Rotation: (", "").Replace(")", "");
        string[] parts = text.Split(',');
        return new Quaternion(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]));
    }
}
