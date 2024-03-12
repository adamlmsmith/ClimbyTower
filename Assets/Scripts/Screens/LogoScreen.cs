using UnityEngine;
using System.Collections;

public class LogoScreen : MonoBehaviour
{
    void Awake()
    {
        NotificationCenter.DefaultCenter().AddObserver(gameObject, "ValidInput");
    }

    void OnEnable()
    {
        StartCoroutine("LoadNextBoard");
        StartCoroutine("ScreenTimeout");

        if(!SoundManager.instance.musicSource.isPlaying)
            SoundManager.instance.musicSource.Play();
    }

    IEnumerator LoadNextBoard()
    {
        UnityEngine.SceneManagement.SceneManager.UnloadScene(k.Scenes.BOARD_SCENE);
        UnityEngine.SceneManagement.SceneManager.LoadScene(k.Scenes.BOARD_SCENE, UnityEngine.SceneManagement.LoadSceneMode.Additive);
        
        GameVariables.instance.BoardHolder.gameObject.SetActive(false);
        GameVariables.instance.HUD.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        NotificationCenter.DefaultCenter().PostNotification(null, "StartNewGame");
    }

    IEnumerator ScreenTimeout()
    {
        yield return new WaitForSeconds(2.0f);

        GameVariables.instance.StartScreen.gameObject.SetActive(true);
        GetComponent<Animator>().SetTrigger("FadePanel");
    }
	
    void ValidInput()
    {
        GetComponent<Animator>().SetBool("On", false);
    }
}
