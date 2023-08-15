using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayermodelInfo : MonoBehaviour
{
    public Animation animationComponent;
    public string deathAnimationName;
    public GameObject bloodParticleSystem;
    public AudioClip deathSound;

    public void PlayDeathAnimation()
    {
        animationComponent.Play(deathAnimationName);
    }
    
    public void PlayBloodParticleSystem()
    {
        bloodParticleSystem.SetActive(true);
    }
}
