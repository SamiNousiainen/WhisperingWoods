using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using ProjectEnums;
using TMPro;

public class GameUI : WindowBase {
    public static GameUI instance;

    //public TextMeshProUGUI playerHealth;
    public GameObject playerHpBar;
    public Slider playerHpSlider;
    public Image playerHpBarFill;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public override void Init(object parameters) {
        base.Init();
    }

    public void Update() {
        UpdateUI();
    }

    public override void UpdateUI() {
        PlayerHealth();
    }

    private void PlayerHealth() {
        if (Player.instance != null) {
            //playerHealth.enabled = true;
            playerHpSlider.enabled = true;
            playerHpBar.SetActive(true);
            playerHpSlider.value = Player.instance.playerCurrentHealth;
        }
    }

    protected override void OpeningAnimationFinished()
    {

    }

    protected override void Closing()
    {

    }

    protected override void Destroying()
    {

    }

}
