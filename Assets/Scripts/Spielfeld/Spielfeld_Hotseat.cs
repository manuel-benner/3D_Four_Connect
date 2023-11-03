using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class Spielfeld_Hotseat : MonoBehaviour
{
    public bool playerTurn;

    public delegate bool onSphereSpawn(string sphereIdentifier);
    public static event onSphereSpawn SphereSpawned;

    public delegate void onNewTurn();
    public static event onNewTurn OnNewTurn;

    public delegate void onWin(Spielfeld_Hotseat.status Winner);
    public static event onWin OnWin;

    public delegate void onDraw();
    public static event onDraw OnDraw;

    public static Spielfeld_Hotseat Instance;
    public enum status
    {
        Player1,
        Player2,
        gameOverWin,
        gameOverDraw
    }

    public status myStatus;

    private int[,,] threeDMatrix;
    public int turnNumber;

    // Start is called before the first frame update
    void Start()
    {
        playerTurn = true;
        threeDMatrix = create3dMatrix();
        myStatus = status.Player1;
    }

    // Add event
    public void Awake()
    {
        SphereSpawned += HandleSphereSpawn;
        if (Instance == null)
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

    public void resetPlayfield()
    {
        threeDMatrix = create3dMatrix();
        turnNumber = 0;
        GameObject[] spawnedSpheres = GameObject.FindGameObjectsWithTag("GespawnteKugel");
        foreach (GameObject sphere in spawnedSpheres)
        {
            Destroy(sphere);
        }
        Instance.myStatus = status.Player1;
    }

    public bool HandleSphereSpawn(string sphereIdentifier)
    {
        string[] coords = sphereIdentifier.Split(',');
        if (Spielfeld_Hotseat.Instance.myStatus == Spielfeld_Hotseat.status.Player1 || Spielfeld_Hotseat.Instance.myStatus == Spielfeld_Hotseat.status.Player2)
        {
            if (coords.Length == 2)
            {
                if (int.TryParse(coords[0], out int x) && int.TryParse(coords[1], out int y))
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (threeDMatrix[x, y, i] == -1)
                        {
                            threeDMatrix[x, y, i] = turnNumber;
                            turnNumber++;
                            if (gameOverByWin())
                            {
                                OnWin?.Invoke(Instance.myStatus);
                                Spielfeld_Hotseat.Instance.myStatus = Spielfeld_Hotseat.status.gameOverWin;
                                
                            }
                            else if (gameOverByDraw())
                            {
                                Spielfeld_Hotseat.Instance.myStatus = Spielfeld_Hotseat.status.gameOverDraw;
                                OnDraw?.Invoke();
                            }
                            // setting new status 
                            if (Spielfeld_Hotseat.Instance.myStatus == status.Player1)
                            {
                                Instance.myStatus = status.Player2;
                                OnNewTurn?.Invoke();
                            }
                            else if (Spielfeld_Hotseat.Instance.myStatus == status.Player2)
                            {
                                Instance.myStatus = status.Player1;
                                OnNewTurn?.Invoke();
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
        }

        return false;
    }

    public status? getWinner()
    {
        if (Instance.myStatus == status.gameOverWin)
        {
            if (turnNumber % 2 == 0) return status.Player1;
            else return status.Player2;
        }
        return null;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private bool gameOverByWin()
    {
        if (CheckHorizontalWin((turnNumber + 1) % 2) || CheckVerticalWin((turnNumber + 1) % 2) || CheckDiagonalWin((turnNumber + 1) % 2))
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
            if ((threeDMatrix[x, 0, 0] % 2 == player) &&
                (threeDMatrix[x, 1, 1] % 2 == player) &&
                (threeDMatrix[x, 2, 2] % 2 == player) &&
                (threeDMatrix[x, 3, 3] % 2 == player))
            {
                return true;
            }
            //From Top - Bottom
            if ((threeDMatrix[x, 0, 3] % 2 == player) &&
                (threeDMatrix[x, 1, 2] % 2 == player) &&
                (threeDMatrix[x, 2, 1] % 2 == player) &&
                (threeDMatrix[x, 3, 0] % 2 == player))
            {
                return true;
            }
        }
        //Check from y=0 site
        for (int y = 0; y < 4; y++)
        {
            //From Bottom - Top
            if ((threeDMatrix[0, y, 0] % 2 == player) &&
                (threeDMatrix[1, y, 1] % 2 == player) &&
                (threeDMatrix[2, y, 2] % 2 == player) &&
                (threeDMatrix[3, y, 3] % 2 == player))
            {
                return true;
            }
            //From Top - Bottom
            if ((threeDMatrix[0, y, 3] % 2 == player) &&
                (threeDMatrix[1, y, 2] % 2 == player) &&
                (threeDMatrix[2, y, 1] % 2 == player) &&
                (threeDMatrix[3, y, 0] % 2 == player))
            {
                return true;
            }
        }
        //Check Corner(0,0) to Corner(3,3) 
        if ((threeDMatrix[0, 0, 0] % 2 == player) &&
            (threeDMatrix[1, 1, 1] % 2 == player) &&
            (threeDMatrix[2, 2, 2] % 2 == player) &&
            (threeDMatrix[3, 3, 3] % 2 == player))
        {
            return true;
        }
        if ((threeDMatrix[0, 0, 3] % 2 == player) &&
            (threeDMatrix[1, 1, 2] % 2 == player) &&
            (threeDMatrix[2, 2, 1] % 2 == player) &&
            (threeDMatrix[3, 3, 0] % 2 == player))
        {
            return true;
        }
        //Check Corner(3,0) to Corner(0,3) 
        if ((threeDMatrix[0, 3, 0] % 2 == player) &&
            (threeDMatrix[1, 2, 1] % 2 == player) &&
            (threeDMatrix[2, 1, 2] % 2 == player) &&
            (threeDMatrix[3, 0, 3] % 2 == player))
        {
            return true;
        }
        if ((threeDMatrix[0, 3, 3] % 2 == player) &&
            (threeDMatrix[1, 2, 2] % 2 == player) &&
            (threeDMatrix[2, 1, 1] % 2 == player) &&
            (threeDMatrix[3, 0, 0] % 2 == player))
        {
            return true;
        }

        //Check flat diagonals (0,3,z) to (3,0,z)
        for (int z = 0; z < 4; z++)
        {
            if ((threeDMatrix[0, 3, z] % 2 == player) &&
                (threeDMatrix[1, 2, z] % 2 == player) &&
                (threeDMatrix[2, 1, z] % 2 == player) &&
                (threeDMatrix[3, 0, z] % 2 == player))
            {
                return true;
            }
        }
        //Check flat diagonals (0,0,z) to (3,3,z)
        for (int z = 0; z < 4; z++)
        {
            if ((threeDMatrix[0, 0, z] % 2 == player) &&
                (threeDMatrix[1, 1, z] % 2 == player) &&
                (threeDMatrix[2, 2, z] % 2 == player) &&
                (threeDMatrix[3, 3, z] % 2 == player))
            {
                return true;
            }
        }
        return false;
    }



    private bool gameOverByDraw()
    {
        if (turnNumber == 63)
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