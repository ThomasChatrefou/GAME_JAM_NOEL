using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public GameObject []typeEnemy;
    public Vector3 []spawner;

    #region
    private float nbEnemyWave;
    private int nbEnemyInvoke;
    public int nbEnemyKill;
    #endregion

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
        WaveSystem();
    }

    // WaveSystem ------------------------------------------------------------------------------------------------------
    private void WaveSystem()
    {
        if(nbEnemyInvoke != nbEnemyWave)
        {
            SpawnWave();
        }
        else if (nbEnemyKill == nbEnemyWave)
        {
            EndWave();
        }
    }

    private void StartWave()
    {
        nbEnemyWave = Mathf.Round(nbEnemyWave * 1.5f);
        nbEnemyInvoke = 0;
        nbEnemyKill = 0;
    }

    private IEnumerator SpawnWave()
    {
        while(nbEnemyInvoke < nbEnemyWave)
        {
            int number = Random.Range(0,3);
            int spawn = Random.Range(0,5);

            Instantiate(typeEnemy[number], spawner[spawn], new Quaternion());

            yield return new WaitForSeconds(0.2f);
        }
    }

    private IEnumerator EndWave()
    {
        RespawnPlayer();
        yield return new WaitForSeconds(2.0f);
        StartWave();
    }
    // EndWaveSystem ---------------------------------------------------------------------------------------------------

    private void RespawnPlayer()
    {

    }
}
