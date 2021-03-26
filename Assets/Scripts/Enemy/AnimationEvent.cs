using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvent : MonoBehaviour
{
    [SerializeField]
    AudioSource walkAudio;
    [SerializeField]
    EnemyChase enemy;

    private Transform enemyPos;
    public bool audioEvent = false;

    public void Awake()
    {
        enemyPos = enemy.transform;
    }
    public void SoundWhenAnim()
    {
        if (Vector3.Distance(transform.position, enemyPos.position) <= 2.5f)
        {
            enemy.findTargetSound = true;
            
            audioEvent = true;            
        }
        walkAudio.Play();
    }
}