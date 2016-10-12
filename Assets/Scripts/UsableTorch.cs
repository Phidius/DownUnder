using UnityEngine;
using System.Collections;

public class UsableTorch : Usable
{

    protected Light torchLight;
    protected ParticleSystem[] particleSystems;

    protected override void Start()
    {
        torchLight = gameObject.GetComponentInChildren<Light>();
        particleSystems = gameObject.GetComponentsInChildren<ParticleSystem>();
    }
    public override void Use()
    {
        var enabled = !torchLight.enabled;
        torchLight.enabled = enabled;
        foreach (var particle in particleSystems)
        {
            if (enabled)
            {
                particle.Play();
            }
            else
            {
                particle.Stop();
            }
        }
    }
}
