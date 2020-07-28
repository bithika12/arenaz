using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ArenaZ
{
    public class ParticleHandler : MonoBehaviour
    {
        public List<ParticleData> ParticleDatas = new List<ParticleData>();

        public void Play(string a_Id, Vector3 a_Position)
        {
            ParticleData t_ParticleData = ParticleDatas.Where(x => x.Id == a_Id).FirstOrDefault();
            if (t_ParticleData != null)
            {
                t_ParticleData.Particle.transform.position = a_Position;
                t_ParticleData.Particle.Play();
            }
        }
    }

    [System.Serializable]
    public class ParticleData
    {
        public string Id;
        public ParticleSystem Particle;
    }
}
