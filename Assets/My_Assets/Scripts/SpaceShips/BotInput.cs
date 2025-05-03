using System.Collections;
using UnityEngine;
using System.IO;

public class BotInput : MonoBehaviour
{
    private Track track;
    private BestLapData lapData;

    void Start()
    {
        track = GetComponent<Track>();

        // Adjust the path to match the new directory structure
        string path = Application.dataPath + "/My_Assets/Scripts/Runs/best_lap.json";
        
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            lapData = JsonUtility.FromJson<BestLapData>(json);

            if (lapData.segmentTimes.Length != track.route.Count)
            {
                Debug.LogError("Mismatch between route points and saved lap data.");
                return;
            }

            StartCoroutine(ReplayLap());
        }
        else
        {
            Debug.Log(path);
            Debug.LogError("No lap data available for bot replay!");
        }
    }

    IEnumerator ReplayLap()
    {
        int index = 0;

        while (true) // Loop infinitely or change condition for just 1 lap
        {
            Vector3 start = transform.position;
            Vector3 end = track.route[(index + 1) % track.route.Count].position;
            float duration = lapData.segmentTimes[index];

            float timer = 0f;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                transform.position = Vector3.Lerp(start, end, timer / duration);

                // Rotate smoothly
                Vector3 direction = (end - transform.position).normalized;
                direction.y = 0;
                if (direction != Vector3.zero)
                {
                    Quaternion lookRot = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 5f);
                }

                yield return null;
            }

            index = (index + 1) % lapData.segmentTimes.Length;
        }
    }
}
