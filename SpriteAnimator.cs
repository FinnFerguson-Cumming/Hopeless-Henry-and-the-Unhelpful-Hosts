using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimator : MonoBehaviour
{
    public SpriteRenderer mySpriteRenderer;
    public AnimationData baseAnimation;

    private Coroutine currentAnimation;
    private GameManager gameManager;

    public System.Action<AnimationData> onAnimationComplete;
    public System.Action<AnimationData> onNearlyComplete;
    public System.Action<AnimationData, int> onSpecificFrame;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public void Start()
    {
        if (baseAnimation != null)
        {
            PlayAnimation(baseAnimation);
        }
    }

    public void PlayAnimation(AnimationData data)
    {
        if (!isActiveAndEnabled)
            return;

        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
            currentAnimation = null;
        }
        //onAnimationComplete = null;
            
        currentAnimation = StartCoroutine(PlayAnimationCoroutine(data));
    }

    public IEnumerator PlayAnimationCoroutine(AnimationData data)
    {
        if (data == null)
            data = baseAnimation;

        int spritesAmount = data.sprites.Length;
        int soundsAmount = data.sounds.Length;
        int i = 0;

        float waitTime = data.frameOfGap * AnimationData.targetFrameTime;

        if (!data.loop)
        {
            //single animations
            for (i = 0; i < spritesAmount; i++)
            {
                if (i < soundsAmount)
                {
                    gameManager.PlaySound(data.sounds[i]);
                }

                mySpriteRenderer.sprite = data.sprites[i];

                onSpecificFrame?.Invoke(data, i);

                yield return new WaitForSeconds(waitTime);

                if (i == spritesAmount - 2)
                {
                    onNearlyComplete?.Invoke(data);
                    onNearlyComplete = null;
                }
            }

            mySpriteRenderer.sprite = data.sprites[spritesAmount - 1];
            onAnimationComplete?.Invoke(data);
            onAnimationComplete = null;
            yield break;
        }
        else
        {
            //looping animations
            while (true)
            {
                for (i = 0; i < spritesAmount; i++)
                {
                    if (i < soundsAmount)
                    {
                        gameManager.PlaySound(data.sounds[i]);
                    }

                    mySpriteRenderer.sprite = data.sprites[i];
                    yield return new WaitForSeconds(waitTime);
                }
            }
        }
    }

    public void SetBaseAnimation(AnimationData anim)
    {
        baseAnimation = anim;
        PlayAnimation(baseAnimation);
    }

    private void OnDisable()
    {
        //stops any animations when disabled
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
            currentAnimation = null;
        }
    }
}