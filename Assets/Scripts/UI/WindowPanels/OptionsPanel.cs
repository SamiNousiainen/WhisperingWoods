using System.Collections;
using UnityEngine;
using ProjectEnums;
using System;
using UnityEngine.SocialPlatforms.Impl;

public class OptionsPanel : WindowBase {
    public static OptionsPanel instance;

	public Transform AudioSettings;
	public Transform Controls;

   
    private void Awake() {
        
    }

    public override void Init(object parameters) {
        if (instance == null)
        {
            instance = this;
            base.Init();
			ChangeTabs(0);
        } else {
            Destroy(gameObject);
		}
    }

	public void ChangeTabs(int index) {
		if (index == 0) {
			AudioSettings.gameObject.SetActive(true);
			Controls.gameObject.SetActive(false);
		}
		else if (index == 1) {
			AudioSettings.gameObject.SetActive(false);
			Controls.gameObject.SetActive(true);
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
