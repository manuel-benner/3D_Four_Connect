using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KugelAuswahlFarbeAendern : MonoBehaviour
{
    public Material unhoveredMat;
    public Material hoveredMatPlayer1;
    public Material hoveredMatPlayer2;
    public GameObject prefabToSpawnPlayer1;
    public GameObject prefabToSpawnPlayer2;
    
    public string sphereIdentifier;

    private bool isHovered = false;
    private Material currentMaterial;
    private Renderer rend;

    // TODO: remove this
    private bool player = false;

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
        if (!isHovered)
        {
            isHovered = true;
            if (player)
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
        if (isHovered)
        {
            isHovered = false;
            currentMaterial = new Material(unhoveredMat);
            rend.material = currentMaterial;
        }
    }

    private void OnMouseDown()
    {
        if (player)
        {
            Instantiate(prefabToSpawnPlayer1, transform.position, Quaternion.identity);

            // Call callback function in Spielfeld to handle the new sphere
            Spielfeld spielfeld = FindObjectOfType<Spielfeld>();
            if(spielfeld != null)
            {
                if (spielfeld.HandleSphereSpawn(sphereIdentifier))
                {
                    Instantiate(prefabToSpawnPlayer2, transform.position, Quaternion.identity);
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
                    Instantiate(prefabToSpawnPlayer2, transform.position, Quaternion.identity);
                }
            }
        }
        
    }
}
