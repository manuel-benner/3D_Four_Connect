using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KugelAuswahlFarbeAendern : MonoBehaviour
{
    public GameManager gameManager;

    public Material unhoveredMat;
    public Material hoveredMatPlayer1;
    public Material hoveredMatPlayer2;

    public string sphereIdentifier;

    private Material currentMaterial;
    private Renderer rend;

    // Start is called before the first frame update
    void Start()
    {

        rend = GetComponent<Renderer>();
        currentMaterial = new Material(unhoveredMat);
        rend.material = currentMaterial;
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Swap to opaque material if being hovered
    private void OnMouseEnter()
    {
        if (Spielfeld.Instance.myStatus == Spielfeld.Status.myTurn)
        {
            if (Spielfeld.Instance.turnNumber % 2 == 0)
            {
                currentMaterial = new Material(hoveredMatPlayer1);
            }
            else
            {
                currentMaterial = new Material(hoveredMatPlayer2);
            }
            rend.material = currentMaterial;
        }
    }

    // Swap back to transparent material if not being hovered
    private void OnMouseExit()
    {
        currentMaterial = new Material(unhoveredMat);
        rend.material = currentMaterial;

    }

    private void OnMouseDown()
    {
        if (Spielfeld.Instance.myStatus == Spielfeld.Status.myTurn)
        {
            if (Spielfeld.Instance.turnNumber % 2 == 0)
            {
                // Call callback function in Spielfeld to handle the new sphere
                Spielfeld spielfeld = FindObjectOfType<Spielfeld>();
                if (spielfeld != null)
                {
                    if (spielfeld.HandleSphereSpawn(sphereIdentifier))
                    {
                        gameManager.SpawnBall(transform.position, sphereIdentifier);
                        currentMaterial = new Material(hoveredMatPlayer2);
                        rend.material = currentMaterial;
                    }
                }
            }
            else
            {
                // Call callback function in Spielfeld to handle the new sphere
                Spielfeld spielfeld = FindObjectOfType<Spielfeld>();
                if (spielfeld != null)
                {
                    if (spielfeld.HandleSphereSpawn(sphereIdentifier))
                    {
                        gameManager.SpawnBall(transform.position, sphereIdentifier);
                        currentMaterial = new Material(hoveredMatPlayer1);
                        rend.material = currentMaterial;
                    }
                }
            }
        }
    }
}
