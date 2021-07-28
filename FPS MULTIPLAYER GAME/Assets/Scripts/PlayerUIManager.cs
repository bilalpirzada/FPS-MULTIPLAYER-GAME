using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{

    private static PlayerUIManager playerUIManager;

    public static PlayerUIManager Instance { get { return playerUIManager; } }


    private void Awake()
    {
        if (playerUIManager != null && playerUIManager != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            playerUIManager = this;
        }
    }




   public GameObject crossHair;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
