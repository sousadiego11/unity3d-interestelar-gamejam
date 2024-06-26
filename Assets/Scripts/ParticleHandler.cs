using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AGOUtils;

public class ParticleHandler : ExtensibleSingleton<ParticleHandler> {
    [SerializeField] List<Particle> particles;
    List<Particle> _instantiated;

    void Start() {
        _instantiated = particles.Select(particle => new Particle{
            prefab = Instantiate(particle.prefab),
            name = particle.name
        }).ToList();
    }
    
    public void Activate(Vector3 position, Particle.ParticleEnum name) {
        Particle particle = _instantiated.Find(a => a.name == name);
        ParticleSystem ps = particle.prefab.GetComponent<ParticleSystem>();
        if (!ps.isPlaying) {
            particle.prefab.transform.position = position;
            particle.prefab.SetActive(true);
            ps.Play();
        }
    }

    public void Disable(Particle.ParticleEnum name) {
        Particle particle = _instantiated.Find(a => a.name == name);
        ParticleSystem ps = particle.prefab.GetComponent<ParticleSystem>();
        if (ps.isPlaying) {
            particle.prefab.SetActive(false);
            ps.Pause();
        }
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