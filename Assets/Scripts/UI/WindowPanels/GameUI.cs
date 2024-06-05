using System.Collections;
using UnityEngine;
using ProjectEnums;

public class GameUI : WindowBase {
    public static GameUI instance;
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

    public override void UpdateUI()
    {

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
