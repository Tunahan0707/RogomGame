using System.Collections;
using UnityEngine;

public class EnemyAlgoritmController : MonoBehaviour
{
    private EnemyDisplay display;
    private EnemyManager enemyManager;

    private int currentPlan;
    private int damage;
    private int block;
    public int strenght { get; private set; }
    public int resistance { get; private set; }
    private FightData loadedData => FightDataHolder.Instance.fightData;

    private void OnEnable()
    {
        EnemyManager.OnEnemySelected += Equalize;
    }
    private void OnDisable()
    {
        EnemyManager.OnEnemySelected -= Equalize;
    }

    private void Equalize(EnemysSO sO)
    {
        display = gameObject.GetComponent<EnemyDisplay>();
        enemyManager = FindAnyObjectByType<EnemyManager>();
        damage = display.enemyData.damage;
        block = display.enemyData.addingShield;
        if (loadedData.enemysStrenght == 96963169)
        {
            strenght = display.enemyData.baseStrenght;
            resistance = display.enemyData.baseResistance;
        }
        else
        {
            strenght = loadedData.enemysStrenght;
            resistance = loadedData.enemysResistance;
        }
    }

    /// <summary>
    /// Yeni plan oluşturur (rastgele veya sıralı)
    /// </summary>
    public void DecideNextPlan()
    {
        float rand = Random.value;
        if (rand < 0.4f)
            currentPlan = 0;
        else if (rand < 0.7f)
            currentPlan = 1;
        else if (rand < 0.9f)
            currentPlan = 2;
        else
            currentPlan = 3;

        ShowEnemyIntent(currentPlan);
    }

    /// <summary>
    /// UI'da niyeti gösterir
    /// </summary>
    private void ShowEnemyIntent(int plan)
    {
        switch (plan)
        {
            case 0:
                display.ShowIntent("Attack", "x", (damage + strenght));
                break;
            case 1:
                display.ShowIntent("Defend", "t", (block + resistance));
                break;
            case 2:
                display.ShowIntent("Buff", "++", 0);
                break;
            case 3:
                display.ShowIntent("Debuff", "--", 0);
                break;
        }
    }
    public void UpdateCurrentPlan()
    {
        ShowEnemyIntent(currentPlan);
    }

    /// <summary>
    /// Düşman planını uygular (oyuncu turu bitince çağrılır)
    /// </summary>
    public IEnumerator ExecuteCurrentPlan()
    {
        yield return new WaitForSeconds(1f);

        switch (currentPlan)
        {
            case 0:
                PlayerManager.Instance.TakeDamage(damage + strenght);
                break;

            case 1:
                enemyManager.AddShield(block + resistance);
                break;

            case 2:
                int x;
                x = Random.Range(0, 2);
                switch (x)
                {
                    case 0:
                        strenght += display.enemyData.addingStrenght;
                        break;
                    case 1:
                        resistance += display.enemyData.addingResistnace;
                        break;
                }
                break;
            case 3:
                int y;
                int z;
                y = Random.Range(0, 2);
                switch (y)
                {
                    case 0:
                        z = Random.Range(0, 2);
                        DebuffStrenght(z);
                        break;
                    case 1:
                        z = Random.Range(0, 2);
                        DebuffStrenght(z);
                        break;
                }
                break;
        }
        if (strenght > 0)
            strenght -= 1;
        else if (strenght < 0)
            strenght += 1;
        if (resistance > 0)
            resistance -= 1;
        else if (resistance < 0)
            resistance += 1;
        DecideNextPlan(); // yeni plan belirle
    }
    public void DebuffStrenght(int debuff)
    {
        strenght -= debuff;
    }
    public void DebuffResistance(int debuff)
    {
        resistance -= debuff;
    }
}
