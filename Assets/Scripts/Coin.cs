using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour
{
    public GameObject m_StarParticles;

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.GetComponent<Climber>() != null)
        {
            GameVariables.instance.PlayerManager.CurrentPlayer.Coins++;
            Destroy(gameObject);

            GameObject.Instantiate(m_StarParticles, transform.position, Quaternion.identity);
            DualPistolasAchievementManager.GetInstance().IncrementProgress(DualPistolasAchievementManager.DualPistolasAchievementId.COIN_COLLECTOR, 1);
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if(collider.name == "GameCamera")
        {
            Destroy (gameObject);
        }
    }
}