using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mountains : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> mountainPrefabs;

    [SerializeField]
    private List<GameObject> mountains;

    [SerializeField]
    private float scaleWidth, scaleHeight, distance;


    [ContextMenu("CreateMountains")]
    private void createMountains()
    {
        foreach (var mountain in mountains)
        {
            Destroy(mountain);
        }

        mountains = new List<GameObject>();


        int prefabIndex = 0;
        float angle = 0f;

        for(int i = 0; i < 8; i++)
        {
            mountains.Add(Instantiate(mountainPrefabs[prefabIndex], transform));

            mountains[i].transform.Rotate(new Vector3(0f, angle, 0f));

            angle += 45f;

            prefabIndex++;

            if (prefabIndex > mountainPrefabs.Count - 1)
                prefabIndex = 0;
        }
    }


    [ContextMenu("SetMountainScale")]
    private void SetMountainScale()
    {
        foreach(var mt in mountains)
        {
            mt.transform.localScale = new Vector3(scaleWidth, scaleHeight, 1f);
            float spriteLength = mountains[0].GetComponent<SpriteRenderer>().bounds.size.x;
            distance = spriteLength / 2f + (Mathf.Sqrt(2) / 2) * spriteLength;
            mt.transform.position = distance * mt.transform.forward;
        }
    }

    private void OnValidate()
    {
        if(mountains.Count == 8)
        {
            SetMountainScale();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
