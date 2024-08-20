using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using ProjectEnums;

public class WindowBase : MonoBehaviour {

    public WindowPanel window;
    public bool addToEscapeStack;
    public bool persistOverSceneLoad = false;
    public Animator panelAnimator;

    protected Coroutine openingRoutine = null;
    protected GraphicRaycaster graphicRaycaster;

    public virtual void Init(object parameters = null)
    {
        if (addToEscapeStack == true)
        {
            WindowManager.instance.escapeableWindowStack.Push(window);
        }
        graphicRaycaster = GetComponent<GraphicRaycaster>();
    }

    public virtual void ReInit(object parameters = null)
    {

    }

    public virtual void UpdateUI()
    {

    }

    protected virtual void OpeningAnimationFinished()
    {

    }

    protected virtual void Closing()
    {

    }

    protected virtual void Destroying()
    {

    }

    public void Show()
    {
        if (panelAnimator != null)
        {
            openingRoutine = StartCoroutine(OpeningRoutine());
        }
        else
        {
            OpeningAnimationFinished();
        }
    }

    public void Close()
    {
        Closing();
        if (panelAnimator != null)
        {
            StartCoroutine(ClosingRoutine());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        StopAllCoroutines();
    }

    virtual protected IEnumerator OpeningRoutine()
    {
        panelAnimator.speed = 1F / Gval.panelAnimationDuration;
        panelAnimator.Play("PanelOpen", -1, 0F);
        panelAnimator.Update(0F);
        float normalizedTime = 0F;
        AnimatorStateInfo stateInfo = panelAnimator.GetCurrentAnimatorStateInfo(0);
        while (normalizedTime < 1F && stateInfo.IsName("PanelOpen") == true)
        {
            yield return null;
            stateInfo = panelAnimator.GetCurrentAnimatorStateInfo(0);
            normalizedTime = stateInfo.normalizedTime;
        }
        OpeningAnimationFinished();
    }

    private IEnumerator ClosingRoutine()
    {
        panelAnimator.speed = 1F / Gval.panelAnimationDuration;
        panelAnimator.Play("PanelClose", -1, 0F);
        panelAnimator.Update(0F);
        float normalizedTime = 0F;
        AnimatorStateInfo stateInfo = panelAnimator.GetCurrentAnimatorStateInfo(0);
        while (normalizedTime < 1F && stateInfo.IsName("PanelClose") == true)
        {
            yield return null;
            stateInfo = panelAnimator.GetCurrentAnimatorStateInfo(0);
            normalizedTime = stateInfo.normalizedTime;
        }
        Destroy(gameObject);
    }
}
