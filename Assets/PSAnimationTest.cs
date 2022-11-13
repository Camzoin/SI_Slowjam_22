using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSAnimationTest : MonoBehaviour
{
    public List<ParticleSystem> particleSystems;

    public float crmbleTime = 5f;

    public float startSpeed = 10f;

    public float maxDist = 10f;

    public Transform centerpoint;

    public Transform camTransform;



    private bool hasStarted = false;

    private float particlespawnCurTime = 0;

    private List<bool> hasShot0 = new List<bool>(750);

    private List<bool> hasShot1 = new List<bool>(750);

    private List<bool> hasShot2 = new List<bool>(750);

    private bool particlesUpdating = false;

    private int particleCount = 1000;

    public void FixedUpdate()
    {
        if (hasStarted == true)
        {
            for (int r = 0; r < particleSystems.Count; r++)
            {
            
                particleCount = particleSystems[r].particleCount;

                var particles = new ParticleSystem.Particle[particleCount];
                var currentAmount = particleSystems[r].GetParticles(particles);

                if (particlespawnCurTime > crmbleTime)
                {
                    particlespawnCurTime = 0;

                    particlesUpdating = false;
                }

                if (particlesUpdating == true)
                {
                    particlespawnCurTime += Time.fixedDeltaTime;

                    // Change only the particles that are alive
                    for (int i = 0; i < currentAmount; i++)
                    {
                        float dist;

                        //Use my trasform
                        dist = Vector3.Distance(particles[i].position, centerpoint.position - new Vector3(centerpoint.up.normalized.x * 10, centerpoint.up.normalized.y * 10, centerpoint.up.normalized.z * 10));

                        float goodDist = 1 - dist;

                        if (r == 0)
                        {
                            if (dist < Mathf.Lerp(0, maxDist, particlespawnCurTime / crmbleTime) && hasShot0[i] == false)
                            {
                                hasShot0[i] = true;
                            }
                        }
                        else if (r == 1)
                        {
                            if (dist < Mathf.Lerp(0, maxDist, particlespawnCurTime / crmbleTime) && hasShot1[i] == false)
                            {
                                hasShot1[i] = true;
                            }

                        }
                        else if (r == 2)
                        {
                            if (dist < Mathf.Lerp(0, maxDist, particlespawnCurTime / crmbleTime) && hasShot2[i] == false)
                            {
                                hasShot2[i] = true;
                            }
                        }
                    }
                }

                for (int i = 0; i < currentAmount; i++)
                {
                    if (r == 0)
                    {
                        Vector3 vel;

                        if (hasShot0[i] == true)
                        {
                            vel = particles[i].velocity;

                            //(8.1f * Time.fixedDeltaTime)

                            vel = vel - centerpoint.up.normalized;

                            particles[i].velocity = vel;

                            vel = particles[i].velocity;
                        }
                    }
                    else if (r == 1)
                    {
                        Vector3 vel;

                        if (hasShot1[i] == true)
                        {
                            vel = particles[i].velocity;

                            //(8.1f * Time.fixedDeltaTime)

                            vel = vel - centerpoint.up.normalized;

                            particles[i].velocity = vel;

                            vel = particles[i].velocity;
                        }
                    }

                    else if (r == 2)
                    {
                        Vector3 vel;

                        if (hasShot2[i] == true)
                        {
                            vel = particles[i].velocity;

                            //(8.1f * Time.fixedDeltaTime)

                            vel = vel - centerpoint.up.normalized;

                            particles[i].velocity = vel;

                            vel = particles[i].velocity;
                        }
                    }
                }

                particleSystems[r].SetParticles(particles, currentAmount);
            }  
        }
    }

    public void Crumble()
    {
        hasStarted = true;

        hasShot0 = new List<bool>();

        hasShot1 = new List<bool>();

        hasShot2 = new List<bool>();

        foreach (ParticleSystem ps in particleSystems)
        {
            ParticleSystem.MainModule _main = ps.main;
        }

        particlesUpdating = true;

        for (int i = 0; i < particleCount; i++)
        {
            hasShot0.Add(false);

            hasShot1.Add(false);

            hasShot2.Add(false);
        }
    }
}