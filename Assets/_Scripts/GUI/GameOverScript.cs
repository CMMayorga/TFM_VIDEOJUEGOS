using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameOverScript : MonoBehaviour
{

    public void End()
    {
        SceneManager.LoadScene("Start", LoadSceneMode.Single);
    }

    public void Exit()
    {
        Application.Quit();
    }
}