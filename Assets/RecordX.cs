using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RecordX : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    List<float> floats = new List<float>();

    // Update is called once per frame
    void FixedUpdate()
    {
        floats.Add(gameObject.transform.position.x);

        if (floats.Count > 100)
        {
            // Convert the float list to a string with space-separated values
            string floatLine = string.Join(" ", floats);

            // Write the line to a file
            WriteToFile(floatLine);

            // Clear the list after writing to file
            floats.Clear();
        }
    }

    void WriteToFile(string line)
    {
        string path = Application.dataPath + "/floatData.txt";

        // Write the string to the file (appending mode)
        using (StreamWriter writer = new StreamWriter(path, true))
        {
            writer.WriteLine(line);
        }

        Debug.Log("Data written to file: " + path);
    }
}
