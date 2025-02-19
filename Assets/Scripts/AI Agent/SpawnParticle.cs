using UnityEngine;

public class SpawnParticle : MonoBehaviour
{
    public void SpawnParticles(ParticleSystem particles) {
        Instantiate(particles, transform.position, Quaternion.identity);
    }
}
