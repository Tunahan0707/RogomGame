using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static event Action OnEnemyDied;
    [Header("Enemy Settings")]
    public EnemysSO currentEnemy;
    [Header("Enemy Prefab")]
    public GameObject enemyPrefab;
    public Transform spawnPoint; // Sahnede düşmanın çıkacağı nokta
    private GameObject spawnedEnemyGO;
    [SerializeField] private EnemiesDataBase enemiesDataBase;


    [HideInInspector] public int currentHealth;

    private void Awake()
    {
        if (currentEnemy != null)
            currentHealth = currentEnemy.health;
    }

    private void SpawnEnemy()
    {
        // Eğer zaten sahnede bir düşman varsa sil
        if (spawnedEnemyGO != null)
            Destroy(spawnedEnemyGO);

        // Prefabı sahnede oluştur
        spawnedEnemyGO = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

        // Prefab üzerine ismi ve görseli set edelim (opsiyonel)
        var sr = spawnedEnemyGO.GetComponent<SpriteRenderer>();
        if (sr != null && currentEnemy.artwork != null)
            sr.sprite = currentEnemy.artwork;

        // Prefab üzerinde EnemyManager referansını set et
        if (spawnedEnemyGO.TryGetComponent<EnemyManager>(out var em))
        {
            em.currentEnemy = currentEnemy;
            em.currentHealth = currentHealth;
        }
        spawnedEnemyGO.transform.SetParent(spawnPoint);
    }

    public void SelectEnemy(EnemyType type)
    {
        currentEnemy = enemiesDataBase.GetEnemyByType(type);

        Debug.Log($"Selected Enemy: {currentEnemy.enemyName} (HP: {currentHealth})");
        SpawnEnemy();
    }


    // Hasar alma fonksiyonu
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log($"{currentEnemy.enemyName} took {amount} damage! Remaining HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public string GetEnemyID()
    {
        return currentEnemy != null ? currentEnemy.enemyID : "0";
    }
    private void Die()
    {
        OnEnemyDied?.Invoke();
        currentEnemy = null;
    }
}
