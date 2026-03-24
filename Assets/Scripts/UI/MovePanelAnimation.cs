using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovePanelAnimation : MonoBehaviour
{
    public class Transition
    {
        public CanvasGroup panelIn;
        public CanvasGroup panelOut;
        public Transition(CanvasGroup panelIn,CanvasGroup panelOut)
        {
            this.panelIn = panelIn;
            this.panelOut = panelOut;
        }
    }
    Transition currentTransition;
    GameObject currentBlocker;
    public void MovePanel(Transition transition)
    {
        if(currentTransition != null)
        {
            StopAllCoroutines();
            EndTransition();
        }
        if(transition.panelIn == transition.panelOut)
            return;
        currentTransition = transition;
        if(currentTransition.panelIn != null)
            FadeIn(currentTransition.panelIn);
        if(currentTransition.panelOut != null)
            FadeOut(currentTransition.panelOut);
        currentBlocker = CreateBlocker();
    }
    void EndTransition()
    {
        if(currentTransition == null)
            return;
        if(currentTransition.panelIn != null)
        {
            currentTransition.panelIn.alpha = 1;
            currentTransition.panelIn.gameObject.SetActive(true);
        }
        if(currentTransition.panelOut != null)
        {
            currentTransition.panelOut.alpha = 0;
            currentTransition.panelOut.gameObject.SetActive(false);   
        }
        currentTransition = null;
        Destroy(currentBlocker);
    }
    IEnumerator fadePanel(bool fadeIn, CanvasGroup panel)
    {
        if (fadeIn)
        {
            panel.gameObject.SetActive(true);
            panel.alpha = 0;
            while(panel.alpha < 1)
            {
                panel.alpha += Time.unscaledDeltaTime * 5;
                yield return null;
            }
            EndTransition();
        }
        else
        {
            panel.alpha = 1;
            while(panel.alpha > 0)
            {
                panel.alpha -= Time.unscaledDeltaTime * 5;
                yield return null;
            }
            panel.gameObject.SetActive(false);
            EndTransition();
        }
    }
    void FadeIn(CanvasGroup panel)=>StartCoroutine(fadePanel(true,panel));
    void FadeOut(CanvasGroup panel)=>StartCoroutine(fadePanel(false,panel));
    GameObject CreateBlocker()
    {
        GameObject blocker = new GameObject("Blocker");
        RectTransform blockerRect = blocker.AddComponent<RectTransform>();
        blockerRect.SetParent(transform, false);
        blockerRect.anchorMin = Vector3.zero;
        blockerRect.anchorMax = Vector3.one;
        blockerRect.sizeDelta = Vector2.zero;
        Image blockerImage = blocker.AddComponent<Image>();
        blockerImage.color = Color.clear;
        return blocker;
    }
}
