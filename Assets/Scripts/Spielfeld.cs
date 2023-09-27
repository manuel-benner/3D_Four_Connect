using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spielfeld : MonoBehaviour
{
    public bool playerTurn;

    public delegate bool onSphereSpawn(string sphereIdentifier);
    public static event onSphereSpawn SphereSpawned;

    private int turnNumber;
    private int[,,] threeDMatrix;

    // Start is called before the first frame update
    void Start()
    {
        playerTurn = true;
        threeDMatrix = create3dMatrix();
        turnNumber = 0;
    }

    // Add trigger
    public void Awake()
    {
        SphereSpawned += HandleSphereSpawn;
    }

    // Destroy trigger
    public void OnDestroy()
    {
        SphereSpawned -= HandleSphereSpawn;
    }

    public bool HandleSphereSpawn(string sphereIdentifier)
    {
        string[] coords = sphereIdentifier.Split(',');

        if (coords.Length == 2 )
        {
            if (int.TryParse(coords[0], out int x) && int.TryParse(coords[1], out int y))
            {
                for (int i = 0; i < 4; i++)
                {
                    if (threeDMatrix[x,y,i] == -1)
                    {
                        threeDMatrix[x, y, i] = turnNumber;
                        turnNumber++;
                        return true;
                    }
                }
            }
            else
            {
                Debug.LogError("Invalid string, could not parse to int. input string: " + sphereIdentifier);
            }
        }
        else
        {
            Debug.LogError("Invalid string, length after split not 2. input string: " + sphereIdentifier);
        }
        return false;
}

    // Update is called once per frame
    void Update()
    {
        if (gameOverByWin())
        {

        }
        else if (gameOverByWin() )
        {

        }
    }

    private bool gameOverByWin()
    {
        return false;
    }

    private bool gameOverByDraw()
    {
        return false;
    }


    // Create empty 3D matrix 4 by 4 by 4
    private int[,,] create3dMatrix()
    {
        int[,,] matrix = new int[4, 4, 4];

        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                for (int z = 0; z < 4; z++)
                {
                    matrix[x, y, z] = -1;
                }
            }
        }
        return matrix;
    }


}
