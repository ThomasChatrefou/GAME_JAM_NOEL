using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public GameplayManager instance;
    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogWarning("GameplayManager already exists");
            return;
        }

        instance = this;
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
