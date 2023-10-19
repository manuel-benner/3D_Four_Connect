using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Spielfeld : MonoBehaviour
{
    public delegate bool onSphereSpawn(string sphereIdentifier);
    public static event onSphereSpawn SphereSpawned;
    public static Spielfeld Instance;
    public enum Status
    {
        myTurn,
        opponentTurn,
        gameOverWin,
        gameOverDraw,
        newGame
    }

    // Initialize by network
    public Status myStatus;
    public int turnNumber;
    public bool placedSphere;
    public int player;

    private int[,,] threeDMatrix;
       

    // Start is called before the first frame update
    void Start()
    {
        threeDMatrix = create3dMatrix();
        placedSphere = false;
    }

    // Add event
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

    // Remove event
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
                        


                        if (gameOverByWin())
                        {
                            Spielfeld.Instance.myStatus = Spielfeld.Status.gameOverWin;
                            Debug.Log("Game over by win");
                        }
                        else if (gameOverByDraw())
                        {
                            Spielfeld.Instance.myStatus = Spielfeld.Status.gameOverDraw;
                            Debug.Log("Game over by draw");
                        }
                        
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
        if(Spielfeld.Instance.myStatus == Spielfeld.Status.newGame)
        {
            resetPlayfield();
        }
    }

    public void resetPlayfield()
    {
        threeDMatrix = create3dMatrix();
        turnNumber = 0;
        GameObject[] spawnedSpheres = GameObject.FindGameObjectsWithTag("GespawnteKugel");
        foreach (GameObject sphere in spawnedSpheres)
        {
            Destroy(sphere);
        }
        if (NetworkManager.Singleton.IsServer)
        {
            Spielfeld.Instance.myStatus = Spielfeld.Status.myTurn;
        }
        else
        {
            Spielfeld.Instance.myStatus = Spielfeld.Status.opponentTurn;
        }
    }

    public bool gameOverByWin()
    {
        if(CheckHorizontalWin(Spielfeld.Instance.player) || CheckVerticalWin(Spielfeld.Instance.player) || CheckDiagonalWin(Spielfeld.Instance.player))
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
                if (threeDMatrix[x,y,0] != -1)
                {
                    int colour = threeDMatrix[x, y, 0] % 2;
                    if (threeDMatrix[x, y, 1] % 2 == colour &&
                        threeDMatrix[x, y, 2] % 2 == colour &&
                        threeDMatrix[x, y, 3] % 2 == colour)
                    {
                        return true;
                    }
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
                if (threeDMatrix[x,0,z] != -1)
                {
                    int colour = threeDMatrix[x, 0, z] % 2;
                    if (threeDMatrix[x, 1, z] % 2 == colour &&
                        threeDMatrix[x, 2, z] % 2 == colour &&
                        threeDMatrix[x, 3, z] % 2 == colour)
                    {
                        return true;
                    }
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
            if (threeDMatrix[x, 0, 0] != -1)
            {
                int colour = threeDMatrix[x, 0, 0] % 2;
                //From Bottom - Top
                if (threeDMatrix[x, 1, 1] % 2 == colour &&
                    threeDMatrix[x, 2, 2] % 2 == colour &&
                    threeDMatrix[x, 3, 3] % 2 == colour)
                {
                    return true;
                }
            }

            if (threeDMatrix[x, 0, 3] != -1)
            {
                int colour = threeDMatrix[x, 0, 3] % 2;
                //From Top - Bottom
                if (threeDMatrix[x, 1, 2] % 2 == colour &&
                    threeDMatrix[x, 2, 1] % 2 == colour &&
                    threeDMatrix[x, 3, 0] % 2 == colour)
                {
                    return true;
                }
            }
        }

        //Check from y=0 site
        for (int y = 0; y < 4; y++)
        {
            if (threeDMatrix[0, y, 0] != -1)
            {
                int colour = threeDMatrix[0, y, 0] % 2;
                //From Bottom - Top
                if (threeDMatrix[1, y, 1] % 2 == colour &&
                    threeDMatrix[2, y, 2] % 2 == colour &&
                    threeDMatrix[3, y, 3] % 2 == colour)
                {
                    return true;
                }
            }

            if (threeDMatrix[0, y, 3] != -1)
            {
                int colour = threeDMatrix[0, y, 3] % 2;
                //From Top - Bottom
                if (threeDMatrix[1, y, 2] % 2 == colour &&
                    threeDMatrix[2, y, 1] % 2 == colour &&
                    threeDMatrix[3, y, 0] % 2 == colour)
                {
                    return true;
                }
            }
        }

        if (threeDMatrix[0, 0, 0] != -1)
        {
            int colour = threeDMatrix[0, 0, 0] % 2;
            //Check Corner(0,0) to Corner(3,3) 
            if (threeDMatrix[1, 1, 1] % 2 == colour &&
                threeDMatrix[2, 2, 2] % 2 == colour &&
                threeDMatrix[3, 3, 3] % 2 == colour)
            {
                return true;
            }
        }

        if (threeDMatrix[0, 0, 3] != -1)
        {
            int colour = threeDMatrix[0, 0, 3] % 2;
            if (threeDMatrix[1, 1, 2] % 2 == colour &&
                threeDMatrix[2, 2, 1] % 2 == colour &&
                threeDMatrix[3, 3, 0] % 2 == colour)
            {
                return true;
            }
        }

        if (threeDMatrix[0, 3, 0] != -1)
        {
            int colour = threeDMatrix[0, 3, 0] % 2;
            //Check Corner(3,0) to Corner(0,3) 
            if (threeDMatrix[1, 2, 1] % 2 == colour &&
                threeDMatrix[2, 1, 2] % 2 == colour &&
                threeDMatrix[3, 0, 3] % 2 == colour)
            {
                return true;
            }
        }

        if (threeDMatrix[0, 3, 3] != -1)
        {
            int colour = threeDMatrix[0, 3, 3] % 2;
            if (threeDMatrix[1, 2, 2] % 2 == colour &&
                threeDMatrix[2, 1, 1] % 2 == colour &&
                threeDMatrix[3, 0, 0] % 2 == colour)
            {
                return true;
            }
        }


        //Check flat diagonals (0,3,z) to (3,0,z)
        for (int z = 0; z < 4; z++)
        {
            if (threeDMatrix[0, 3, z] != -1)
            {
                int colour = threeDMatrix[0, 3, z] % 2;
                if (threeDMatrix[1, 2, z] % 2 == colour &&
                    threeDMatrix[2, 1, z] % 2 == colour &&
                    threeDMatrix[3, 0, z] % 2 == colour)
                {
                    return true;
                }
            }

        }

        //Check flat diagonals (0,0,z) to (3,3,z)
        for(int z = 0; z < 4; z++)
        {
            if (threeDMatrix[0, 0, z] != -1)
            {
                int colour = threeDMatrix[0, 0, z] % 2;
                if (threeDMatrix[1, 1, z] % 2 == colour &&
                    threeDMatrix[2, 2, z] % 2 == colour &&
                    threeDMatrix[3, 3, z] % 2 == colour)
                {
                    return true;
                }
            }
        }
        return false;
    }



    public bool gameOverByDraw()
    {
        if(turnNumber == 63)
        {
            return true;
        }
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
