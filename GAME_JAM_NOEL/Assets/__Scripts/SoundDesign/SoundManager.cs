using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.InputSystem;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    [Header("0=Lobby; 1=InGame; 2=Win; 3=Lose")]
    public AudioClip[] Music;

    [Header("Enemy Sound")]
    public AudioClip[] enemyDie;
    public AudioClip[] hitEnemy;

    [Header("Player Sound")]
    public AudioClip[] playerDie;
    public AudioClip[] hitPlayer;

    [Header("Source")]
    public AudioMixerGroup mixerSound;
    public AudioSource audioSource;

    int i = 0;

    public static SoundManager instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Problem SoundManager");
            return;
        }

        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        ChangeMusic(0);
    }

    private void Update()
    {
        if (i >= 300)
        {
            PlayEnemyDieSound(this.transform, 0);
            i = 0;
        }
        else if(i <= 300)
        {
            i++;
        }
    }

    public IEnumerator Test() // Test fonction coroutine
    {
        yield return new WaitForSeconds(3f);
        PlayEnemyDieSound(this.transform, 0);
    }


    public void ChangeMusic(int index) // 0=Lobby ; 1=InGame ; 2=Win ; 3=Lose
    {
        switch (index)
        {
            case 0: audioSource.clip = Music[0]; audioSource.loop = true; 
                break;
            case 1: audioSource.clip = Music[1]; audioSource.loop = true;
                break;
            case 2: audioSource.clip = Music[2]; audioSource.loop = false;
                break;
            case 3: audioSource.clip = Music[3]; audioSource.loop = false;
                break;
        }

        audioSource.Play();
    }

    // Create Sounds Effects Public, Call your necessary function with : SoundManager.instance."fonction" and give parameters : your transform (enemy, player, etc..) and your number (check the GDD for the order)
    public void PlayEnemyDieSound(Transform pos, int nbEnemy)
    {
        AudioClip audio = enemyDie[nbEnemy];
        CreateSoundEffect(audio, pos);
    }

    public void PlayHitEnemySound(Transform pos, int nbHitEnemy)
    {
        AudioClip audio = hitEnemy[nbHitEnemy];
        CreateSoundEffect(audio, pos);
    }

    public void PlayPlayerDieSound(Transform pos, int nbPlayer)
    {
        AudioClip audio = playerDie[nbPlayer];
        CreateSoundEffect(audio, pos);
    }

    public void PlayHitPlayerSound(Transform pos, int nbHitPlayer)
    {
        AudioClip audio = hitPlayer[nbHitPlayer];
        CreateSoundEffect(audio, pos);
    }
    // -------------------------------------------------------------------------------------- End Create Sounds Effects Public ------------------------------------------------------------------------------------

    private void CreateSoundEffect(AudioClip audio, Transform pos)
    {
        GameObject objToSpawn = new GameObject("soundDie");
        objToSpawn.AddComponent<AudioSource>();
        objToSpawn.transform.position = pos.position;

        objToSpawn.GetComponent<AudioSource>().outputAudioMixerGroup = mixerSound;
        objToSpawn.GetComponent<AudioSource>().clip = audio;
        objToSpawn.GetComponent<AudioSource>().Play();
        objToSpawn.AddComponent<DeleteSound>();
    }
}
