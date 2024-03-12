using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PowerLine : MonoBehaviour
{
    float m_DelayBetweenZaps = 5.0f;

    public List<AudioClip> m_ElectricityShortZapClip = new List<AudioClip>();
    public AudioSource m_ElectricityShortZapAudio;

    public bool IsAwake { get; set; }

    void Start()
    {
        IsAwake = false;
    }

    void OnEnable()
    {
        StartCoroutine("RandomizeElectrifyLoop");

        if (Random.Range(0, 2) == 1)
            transform.localEulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
        else
            transform.localEulerAngles = Vector3.zero;
    }

    IEnumerator RandomizeElectrifyLoop()
    {
        // Delay for a random time so all the powerlines don't animate in sync.
        yield return new WaitForSeconds(Random.Range(0.0f, m_DelayBetweenZaps));
        StartCoroutine("Electrify");
    }

    IEnumerator Electrify()
    {
        GetComponent<Animator>().SetTrigger("Electrify");
        yield return new WaitForSeconds(m_DelayBetweenZaps);
        StartCoroutine("Electrify");
    }

    void PlayZapBurstSound()
    {
        if (IsAwake)
        {
            m_ElectricityShortZapAudio.clip = m_ElectricityShortZapClip [Random.Range(0, m_ElectricityShortZapClip.Count)];
            m_ElectricityShortZapAudio.Play();
        }
    }

    void StopZapBurstSound()
    {
        m_ElectricityShortZapAudio.Stop();
    }
}
