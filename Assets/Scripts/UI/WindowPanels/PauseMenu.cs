using System.Collections;
using UnityEngine;
using ProjectEnums;

public class PauseMenu : WindowBase {
    public static PauseMenu instance;


    //public Transform contentParent;
    //public Transform optionsParent;
    //public Transform inventoryParent;
    //public Transform mapParent;

    //[System.NonSerialized]
    //public PauseMenuMode mode;

    private void Awake() {
        
    }

    public override void Init(object parameters) {
        if (instance == null)
        {
            instance = this;
            base.Init();
            //if (parameters is PauseMenuMode)
            //{
            //    mode = (PauseMenuMode)parameters;
            //    if (mode == PauseMenuMode.Options)
            //    {
            //        optionsParent.gameObject.SetActive(true);
            //        inventoryParent.gameObject.SetActive(false);
            //        mapParent.gameObject.SetActive(false);
            //        Debug.Log("options");
            //    }
            //    else if (mode == PauseMenuMode.Inventory)
            //    {
            //        inventoryParent.gameObject.SetActive(true);
            //        optionsParent.gameObject.SetActive(false);
            //        mapParent.gameObject.SetActive(false);
            //        Debug.Log("inventory");
            //    }
            //    else if (mode == PauseMenuMode.Map)
            //    {
            //        mapParent.gameObject.SetActive(true);
            //        optionsParent.gameObject.SetActive(false);
            //        inventoryParent.gameObject.SetActive(false);
            //        Debug.Log("map");
            //    }

            //}
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
