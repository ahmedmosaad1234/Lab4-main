using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    public static int health = 3;
    public static int lives = 3;
    public static int score = 0;
    public static bool hasHeart = false;

    [Header("UI (assign in the Inspector)")]
    public Image healthBar;              // drag GreenBar here
    public TextMeshProUGUI scoreUI;      // drag Coin Score here

    private float flickerTime = 0f;
    public float flickerDuration = 0.1f;

    private SpriteRenderer sr;

    public bool isImmune = false;
    private float immunityTime = 0f;
    public float immunityDuration = 1.5f;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        if (sr == null)
            Debug.LogError("PlayerStats: no SpriteRenderer found on " + gameObject.name, this);
        if (healthBar == null)
            Debug.LogError("PlayerStats: healthBar is not assigned on " + gameObject.name, this);
        if (scoreUI == null)
            Debug.LogError("PlayerStats: scoreUI is not assigned on " + gameObject.name, this);

        UpdateHealthBar();
    }

    // Call this when starting a new game / restarting after game over,
    // because static values survive scene reloads.
    public static void ResetStats()
    {
        health = 3;
        lives = 3;
        score = 0;
        hasHeart = false;
    }

    void UpdateHealthBar()
    {
        if (healthBar != null)
            healthBar.fillAmount = health / 3f;
    }

    void SpriteFlicker()
    {
        if (sr == null)
            return;

        if (flickerTime < flickerDuration)
        {
            flickerTime = flickerTime + Time.deltaTime;
        }
        else
        {
            sr.enabled = !(sr.enabled);
            flickerTime = 0;
        }
    }

    public void TakeDamage(int damage)
    {
        // Ignore damage while immune (and don't restart the immunity timer)
        if (isImmune)
            return;

        health = health - damage;
        if (health < 0)
            health = 0;

        UpdateHealthBar();

        if (lives > 0 && health == 0)
        {
            LevelManager levelManager = FindObjectOfType<LevelManager>();
            if (levelManager != null)
                levelManager.RespawnPlayer();
            else
                Debug.LogError("PlayerStats: no LevelManager found in the scene", this);

            health = 3;
            UpdateHealthBar();
            lives--;
        }
        else if (lives == 0 && health == 0)
        {
            Debug.Log("Gameover");
            ResetStats();
            SceneManager.LoadScene(3);
            return;
        }

        Debug.Log("Player Health:" + health.ToString());
        Debug.Log("Player Lives:" + lives.ToString());

        // Start immunity only after damage was actually taken
        isImmune = true;
        immunityTime = 0f;
    }

    void Update()
    {
        if (isImmune)
        {
            SpriteFlicker();
            immunityTime = immunityTime + Time.deltaTime;
            if (immunityTime >= immunityDuration)
            {
                isImmune = false;
                if (sr != null)
                    sr.enabled = true;
            }
        }

        if (scoreUI != null)
            scoreUI.text = " " + score;
    }
}