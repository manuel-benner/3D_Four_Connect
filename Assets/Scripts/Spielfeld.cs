using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spielfeld : MonoBehaviour
{
    public bool playerTurn;

    public delegate bool onSphereSpawn(string sphereIdentifier);
    public static event onSphereSpawn SphereSpawned;
    public static Spielfeld Instance;

    private int[,,] threeDMatrix;
    public int turnNumber;

    // Start is called before the first frame update
    void Start()
    {
        playerTurn = true;
        threeDMatrix = create3dMatrix();
    }

    // Add trigger
    public void Awake()
    {
        SphereSpawned += HandleSphereSpawn;
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
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
        if(CheckHorizontalWin((turnNumber + 1) % 2) || CheckVerticalWin((turnNumber + 1) % 2) || CheckDiagonalWin((turnNumber + 1) % 2))
        {
            return true;
        }
        return false;
    }


    private bool CheckHorizontalWin(int player)
    {
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                if (threeDMatrix[x, y, 0] % 2 == player &&
                    threeDMatrix[x, y, 1] % 2 == player &&
                    threeDMatrix[x, y, 2] % 2 == player &&
                    threeDMatrix[x, y, 3] % 2 == player)
                {
                    return true;
                }
            }
        }
        return false;
    }


    bool CheckVerticalWin(int player)
    {
        for (int x = 0; x < 4; x++)
        {
            for (int z = 0; z < 4; z++)
            {
                if (threeDMatrix[x, 0, z] % 2 == player &&
                    threeDMatrix[x, 1, z] % 2 == player &&
                    threeDMatrix[x, 2, z] % 2 == player &&
                    threeDMatrix[x, 3, z] % 2 == player)
                {
                    return true;
                }
            }
        }
        return false;
    }


    private bool CheckDiagonalWin(int player)
    {
        //Check from x=0 site
        for (int x = 0; x < 4; x++)
        {
            //From Bottom - Top
            if ((threeDMatrix[x, 0, 0] == player) &&
                (threeDMatrix[x, 1, 1] == player) &&
                (threeDMatrix[x, 2, 2] == player) &&
                (threeDMatrix[x, 3, 3] == player))
            {
                return true;
            }
            //From Top - Bottom
            if ((threeDMatrix[x, 0, 3] == player) &&
                (threeDMatrix[x, 1, 2] == player) &&
                (threeDMatrix[x, 2, 1] == player) &&
                (threeDMatrix[x, 3, 0] == player))
            {
                return true;
            }
        }
        //Check from y=0 site
        for (int y = 0; y < 4; y++)
        {
            //From Bottom - Top
            if ((threeDMatrix[0, y, 0] == player) &&
                (threeDMatrix[1, y, 1] == player) &&
                (threeDMatrix[2, y, 2] == player) &&
                (threeDMatrix[3, y, 3] == player))
            {
                return true;
            }
            //From Top - Bottom
            if ((threeDMatrix[0, y, 3] == player) &&
                (threeDMatrix[1, y, 2] == player) &&
                (threeDMatrix[2, y, 1] == player) &&
                (threeDMatrix[3, y, 0] == player))
            {
                return true;
            }
        }
        //Check Corner(0,0) to Corner(3,3) 
        if ((threeDMatrix[0, 0, 0] == player) &&
            (threeDMatrix[1, 1, 1] == player) &&
            (threeDMatrix[2, 2, 2] == player) &&
            (threeDMatrix[3, 3, 3] == player))
        {
            return true;
        }
        if ((threeDMatrix[0, 0, 3] == player) &&
            (threeDMatrix[1, 1, 2] == player) &&
            (threeDMatrix[2, 2, 1] == player) &&
            (threeDMatrix[3, 3, 0] == player))
        {
            return true;
        }
        //Check Corner(3,0) to Corner(0,3) 
        if ((threeDMatrix[0, 3, 0] == player) &&
            (threeDMatrix[1, 2, 1] == player) &&
            (threeDMatrix[2, 1, 2] == player) &&
            (threeDMatrix[3, 0, 3] == player))
        {
            return true;
        }
        if ((threeDMatrix[0, 3, 3] == player) &&
            (threeDMatrix[1, 2, 2] == player) &&
            (threeDMatrix[2, 1, 1] == player) &&
            (threeDMatrix[3, 0, 0] == player))
        {
            return true;
        }
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
