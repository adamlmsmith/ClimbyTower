using UnityEngine;
using System.Collections;

public class BoardHolder : MonoBehaviour
{
    public void LoadBoard()
    {
        UnityEngine.SceneManagement.SceneManager.UnloadScene(k.Scenes.BOARD_SCENE);
        UnityEngine.SceneManagement.SceneManager.LoadScene(k.Scenes.BOARD_SCENE, UnityEngine.SceneManagement.LoadSceneMode.Additive);
    }
}
