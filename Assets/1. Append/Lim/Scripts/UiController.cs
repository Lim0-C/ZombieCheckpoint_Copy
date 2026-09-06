using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiController : MonoBehaviour
{
    [SerializeField] private Image hitPanel;

    public void HitEvent()
    {
        StartCoroutine(EffectHitPanel());
    }
    
    IEnumerator EffectHitPanel()
    {
        hitPanel.gameObject.SetActive(true);
        var tt = 0f;
        while (tt < 2f)
        {
            tt +=Time.deltaTime;
            hitPanel.color = new Color(hitPanel.color.r, hitPanel.color.g, hitPanel.color.b,
                Mathf.Lerp(hitPanel.color.a, 0f, tt));
            yield return null;
        }
        hitPanel.gameObject.SetActive(false);
        hitPanel.color = new Color(hitPanel.color.r, hitPanel.color.g, hitPanel.color.b, 0.5f);
    }

    public void OnClickRetry()
    {
        GameManager.Instance.GameRestart();
    }
}
