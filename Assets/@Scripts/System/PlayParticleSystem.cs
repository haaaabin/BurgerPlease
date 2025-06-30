using UnityEngine;

public class PlayParticleSystem : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem _unlockEffect;

    private void Start()
    {
        _unlockEffect.Stop();
    }
    
    public void OnPlayParticleSystem()
    {
        if (_unlockEffect != null)
        {
            _unlockEffect.gameObject.SetActive(true);
            _unlockEffect.Play();
        }
    }
}