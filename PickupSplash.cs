using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickupSplash : MonoBehaviour
{
    public static PickupSplash Instance;

    [Header("UI Reference")]
    [SerializeField] private Canvas splashCanvas;
    [SerializeField] private Image backgroundRenderer;
    [SerializeField] private Image itemRenderer;

    [Header("Animations")]
    [SerializeField] private AnimationData yellowBackgroundAnimation;
    [SerializeField] private AnimationData[] itemPickupAnimations;

    private Coroutine currentSplash;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Duplicate PickupSplash detected. Destroying duplicate");
            Destroy(gameObject);
        }
            
        splashCanvas.enabled = false;
    }

    public void PlayItemSplash(int itemIndex)
    {
        if (itemIndex < 0 || itemIndex >= itemPickupAnimations.Length)
        {
            Debug.LogWarning("Invalid item index for pickup splash!");
            return;
        }

        PlaySplash(yellowBackgroundAnimation, itemPickupAnimations[itemIndex]);
    }

    public void PlaySplash(AnimationData backgroundAnimation, AnimationData itemAnimation)
    {
        if (currentSplash != null)
            StopCoroutine(currentSplash);

        currentSplash = StartCoroutine(PlaySplashCoroutine(backgroundAnimation, itemAnimation));
    }

    private IEnumerator PlaySplashCoroutine(AnimationData backgroundAnimation, AnimationData itemAnimation)
    {
        splashCanvas.enabled = true;

        backgroundRenderer.color = Color.white;
        itemRenderer.color = Color.white;

        int maxFrames = Mathf.Max(backgroundAnimation.sprites.Length, itemAnimation.sprites.Length);

        float waitTime = Mathf.Min(backgroundAnimation.frameOfGap * AnimationData.targetFrameTime, itemAnimation.frameOfGap * AnimationData.targetFrameTime);

        for (int i = 0; i < maxFrames; i++)
        {
            if (i < backgroundAnimation.sprites.Length)
            {
                backgroundRenderer.sprite = backgroundAnimation.sprites[i];
            }

            if (i < itemAnimation.sprites.Length)
            {
                itemRenderer.sprite = itemAnimation.sprites[i];
            }

            yield return new WaitForSeconds(waitTime);
        }

        StartCoroutine(FadeOutSplash(0.10f));
        currentSplash = null;
    }

    private IEnumerator FadeOutSplash(float duration)
    {
        float elapsed = 0f;

        Color bgColor = backgroundRenderer.color;
        Color itemColor = itemRenderer.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);

            backgroundRenderer.color = new Color(bgColor.r, bgColor.g, bgColor.b, alpha);
            itemRenderer.color = new Color(itemColor.r, itemColor.g, itemColor.b, alpha);

            yield return null;
        }

        backgroundRenderer.color = new Color(bgColor.r, bgColor.g, bgColor.b, 0f);
        itemRenderer.color = new Color(itemColor.r, itemColor.g, itemColor.b, 0f);

        splashCanvas.enabled = false;
    }
}