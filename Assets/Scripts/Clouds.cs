using UnityEngine;
using System.Collections;

public class Clouds : MonoBehaviour
{
    private ParticleSystem m_CloudParticleSystem;


	void Start () 
    {
        m_CloudParticleSystem = GetComponent<ParticleSystem>();

        StartCoroutine("UpdateParticles");
	}
	
    IEnumerator UpdateParticles()
    {
        bool particlesAltered = false;

        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[m_CloudParticleSystem.particleCount];
        
        int num = m_CloudParticleSystem.GetParticles(particles);
        
        for(int i = 0; i < num; i++)
        {
            if(particles[i].position.x > 17)
            {
                particles[i].lifetime = 0;
                particlesAltered = true;
            }
        }
        
        if(particlesAltered)
            m_CloudParticleSystem.SetParticles(particles,num);

        yield return new WaitForSeconds(1.0f);

        StartCoroutine("UpdateParticles");
    }
}
