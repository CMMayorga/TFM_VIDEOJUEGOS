using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ThirdPersonStatus : MonoBehaviour
{

    public int health = 6;
    public int maxHealth = 6;
    public float lives = 3;
    [SerializeField]
    AudioClip struckSound;
    [SerializeField]
    AudioClip deathSound;

    private LevelStatus levelStateMachine;
    private int remainingItems;

    void Awake()
    {

        levelStateMachine = FindObjectOfType<LevelStatus>();


        remainingItems = levelStateMachine.itemsNeeded;
    }

    public int GetRemainingItems()
    {
        return remainingItems;
    }

    public void ApplyDamage(int damage)
    {
        if (struckSound) AudioSource.PlayClipAtPoint(struckSound, transform.position);

        health -= damage;
        if (health <= 0)
        {
            SendMessage("Die");
        }
    }
    public void AddLife(int powerUp)
    {
        lives += powerUp;
        if (lives > 9) lives = 9;

        health = maxHealth;
    }

    void AddHealth(int powerUp)
    {
        health += powerUp;

        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }
    public void FoundItem(int numFound)
    {
        remainingItems -= numFound;

        if (remainingItems == 0)
        {
            levelStateMachine.UnlockLevelExit();
        }
    }
    public void FalloutDeath()
    {
        Die();
        return;
    }

    IEnumerator Die()
    {

        if (deathSound)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
        }

        lives--;
        health = maxHealth;

        if (lives < 0) SceneManager.LoadScene("GameOver", LoadSceneMode.Single);

        Vector3 respawnPosition = Respawn.currentRespawn.transform.position;
        Camera.main.transform.position = respawnPosition - (transform.forward * 4) + Vector3.up;
        SendMessage("HidePlayer");

        transform.position = respawnPosition + Vector3.up;
        yield return new WaitForSeconds(1.6f);

        SendMessage("ShowPlayer");
        Respawn.currentRespawn.FireEffect();
    }

    public void LevelCompleted()
    {
        levelStateMachine.LevelCompleted();
    }
}