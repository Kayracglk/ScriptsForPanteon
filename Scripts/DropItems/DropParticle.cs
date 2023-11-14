using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropParticle : MonoBehaviour
{
    [SerializeField] private ParticleSystem m_Particle;

    private void OnEnable()
    {
        m_Particle.Stop();
    }

    private void OnDisable()
    {
        m_Particle.Stop();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Ground"))
        {
            m_Particle.Play();
        }
    }
}
