using System.Collections;
using UnityEngine;
using ProjectEnums;

public class Inventory : WindowBase {
    public static Inventory instance;


    private void Awake() {
        
    }

    public override void Init(object parameters) {
        if (instance == null)
        {
            instance = this;
            base.Init();
        } else {
            Destroy(gameObject);
        }
    }


    public override void UpdateUI() {
       
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
