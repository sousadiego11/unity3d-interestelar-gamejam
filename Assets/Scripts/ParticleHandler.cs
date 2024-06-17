using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParticleHandler : ExtensibleSingleton<ParticleHandler> {
    [SerializeField] List<Particle> particles;
    List<Particle> _instantiated;

    void Start() {
        _instantiated = particles.Select(particle => new Particle{
            prefab = Instantiate(particle.prefab),
            name = particle.name
        }).ToList();
    }
    
    public void Activate(Vector3 hit, Particle.ParticleEnum name) {
        Particle particle = _instantiated.Find(a => a.name == name);
        particle.prefab.transform.position = hit;
        particle.prefab.SetActive(true);
        particle.prefab.GetComponent<ParticleSystem>().Play();
    }

    public void Disable(Particle.ParticleEnum name) {
        Particle particle = _instantiated.Find(a => a.name == name);
        particle.prefab.SetActive(false);
        particle.prefab.GetComponent<ParticleSystem>().Pause();
    }
}


[Serializable]
public struct Particle {
    public GameObject prefab;
    public ParticleEnum name;
    public enum ParticleEnum {
        HackParticle,
        SingleLazerHitParticle
    }
}