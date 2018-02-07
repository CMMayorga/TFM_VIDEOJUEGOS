using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelStatus : MonoBehaviour
{

    [SerializeField]
    GameObject exitGateway;
    [SerializeField]
    GameObject levelGoal;
    [SerializeField]
    AudioClip unlockedSound;
    [SerializeField]
    AudioClip levelCompleteSound;
    [SerializeField]
    GameObject mainCamera;
    [SerializeField]
    GameObject unlockedCamera;
    [SerializeField]
    GameObject levelCompletedCamera;
    [SerializeField]
    public int itemsNeeded = 20;
    private GameObject playerLink;

    GameHUD gamehud;

    void Awake()
    {
        gamehud = FindObjectOfType<GameHUD>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        
        playerLink = GameObject.Find("Player");
    }

    public IEnumerator UnlockLevelExit()
    {
        mainCamera.GetComponent<AudioListener>().enabled = false;
        unlockedCamera.SetActive(true);
        unlockedCamera.GetComponent<AudioListener>().enabled = true;
        exitGateway.GetComponent<AudioSource>().Stop();

        if (unlockedSound)
        {
            PlayAudioClip(unlockedSound, transform.position, 1.0f);
        }

        yield return new WaitForSeconds(1);
        exitGateway.SetActive(false);
        yield return new WaitForSeconds(0.2f);
        exitGateway.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        exitGateway.SetActive(false);
        levelGoal.GetComponent<MeshCollider>().isTrigger = true;
        yield return new WaitForSeconds(4);

        unlockedCamera.SetActive(false);
        unlockedCamera.GetComponent<AudioListener>().enabled = false;
        mainCamera.GetComponent<AudioListener>().enabled = true;
    }

    public void LevelCompleted()
    {
        mainCamera.GetComponent<AudioListener>().enabled = false;
        levelCompletedCamera.SetActive(true);
        levelCompletedCamera.GetComponent<AudioListener>().enabled = true;
        playerLink.GetComponent<ThirdPersonController>().SendMessage("HidePlayer");
        playerLink.transform.position += Vector3.up * 500.0f;

        if (levelCompleteSound)
        {
            PlayAudioClip(levelCompleteSound, levelGoal.transform.position, 1.0f);
        }

        levelGoal.GetComponent<Animation>().Play();
        SceneManager.LoadScene("GameOver", LoadSceneMode.Single);
    }

    AudioSource PlayAudioClip(AudioClip clip, Vector3 position, float volume)
    {
        GameObject go = new GameObject("One shot audio");
        go.transform.position = position;
        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = clip;
        source.spatialBlend = 0;
        source.volume = volume;
        source.Play();
        Destroy(go, clip.length);
        return source;
    }
}