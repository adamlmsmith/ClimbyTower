using UnityEngine;
using System.Collections;

public class ProximityMine : MonoBehaviour
{
    float m_CountdownTime = 3.0f;

    public GameObject m_ExplosionPrefab;
    public GameObject m_WarningLight;

    [Header("Audio")]
    public AudioSource m_TickingAudio;

    public void TriggerExplosion()
    {
        NotificationCenter.DefaultCenter().PostNotification(gameObject, "PlayerTriggeredMine");
        StartCoroutine("DoCountdown");
    }
	
    IEnumerator DoCountdown()
    {
        GetComponent<Animator>().SetTrigger("Countdown");
        m_TickingAudio.pitch = Random.Range(0.95f, 1.05f);
        m_TickingAudio.Play();

        yield return new WaitForSeconds(m_CountdownTime);

        m_TickingAudio.Stop();
        Explode();
    }

    void Explode()
    {
        if (m_ExplosionPrefab != null)
        {
            GameObject explosionObject = GameObject.Instantiate(m_ExplosionPrefab);
            explosionObject.transform.position = gameObject.transform.position;
            explosionObject.transform.SetParent(GameVariables.instance.Board.transform);
        }

        Destroy(gameObject);

        NotificationCenter.DefaultCenter().PostNotification(gameObject, "ProximityMineExploded");
    }

    public void SetSortingOrder(int sortingOrder)
    {
        GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;
        transform.Find("ClockHand").GetComponent<SpriteRenderer>().sortingOrder = sortingOrder + 1;
    }
}
