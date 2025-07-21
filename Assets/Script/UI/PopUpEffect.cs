using UnityEngine;

public class PopUpEffect : MonoBehaviour
{
    public float fadeDuration = 0.3f;
    public float showDuration = 0.5f; // Thời gian giữ UI sau khi hiện lên

    private CanvasGroup canvasGroup;
    private float timer = 0f;
    private enum FadeState { None, FadingIn, Showing, FadingOut }
    private FadeState currentState = FadeState.None;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    void Update()
    {
        switch (currentState)
        {
            case FadeState.FadingIn:
                timer += Time.deltaTime;
                canvasGroup.alpha = Mathf.Clamp01(timer / fadeDuration);
                if (timer >= fadeDuration)
                {
                    timer = 0f;
                    currentState = FadeState.Showing;
                }
                break;

            case FadeState.Showing:
                timer += Time.deltaTime;
                if (timer >= showDuration)
                {
                    timer = fadeDuration;
                    currentState = FadeState.FadingOut;
                }
                break;

            case FadeState.FadingOut:
                timer -= Time.deltaTime;
                canvasGroup.alpha = Mathf.Clamp01(timer / fadeDuration);
                if (timer <= 0f)
                {
                    currentState = FadeState.None;
                    gameObject.SetActive(false);
                }
                break;
        }
    }

    public void StartFade()
    {
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        gameObject.SetActive(true);
        canvasGroup.alpha = 0f;
        timer = 0f;
        currentState = FadeState.FadingIn;
    }

}
