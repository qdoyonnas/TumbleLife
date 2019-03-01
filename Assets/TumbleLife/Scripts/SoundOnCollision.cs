using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource), typeof(Rigidbody))]
public class SoundOnCollision : MonoBehaviour
{
    public AudioClip collisionSound;
    public float minimumVelocity = 0.5f;
    public bool velocityAffectsVolume = true;
    public float pitchVariation = 0.4f;

    AudioSource audioSource;
    Rigidbody rigidbody;

	// Use this for initialization
	void Start ()
    {
		audioSource = GetComponent<AudioSource>();
        rigidbody = GetComponent<Rigidbody>();

        audioSource.clip = collisionSound;
	}
    
    private void OnCollisionEnter(Collision collision)
    {
        if (rigidbody.velocity.magnitude >= minimumVelocity)
        {
            audioSource.volume = rigidbody.velocity.magnitude / (minimumVelocity * 4);
            audioSource.pitch = Random.Range(1 - pitchVariation, 1 + pitchVariation);

            audioSource.PlayOneShot(audioSource.clip);
        }
    }
}
