using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class HandleSpaceShipCollision : MonoBehaviour
{

    void OnTriggerEnter(Collider col)
    {
        SceneManager.LoadScene("EndGame", LoadSceneMode.Single);
    }

}