using UnityEngine;

class FirstScript
{
    [RuntimeInitializeOnLoadMethod]
    static void OnRuntimeMethodLoad ()
    {
        if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == k.Scenes.BOARD_SCENE) 
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(k.Scenes.MASTER_SCENE);
        }
    }
}
