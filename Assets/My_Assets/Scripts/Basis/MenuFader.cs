using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuRetroFader : MonoBehaviour
{
    [Header("UI Overlays")]
    public RectTransform blackOverlay;      // intro overlay, shrinks away
    public Image transitionOverlay; // starts semi-transparent, fades fully on Start

    [Header("Animated UI")]
    public RectTransform titleImage;        // your Title PNG
    public RectTransform playImage;         // your Play PNG

    [Header("Audio & Scene")]
    public AudioSource  bgMusic;
    public string levelToLoad = "Level1";

    [SerializeField] private AudioSource musicSource; // Reference to the AudioSource
    private float originalVolume;

    Vector2 overlaySize;

    void Awake()
    {
        // cache overlay size
        overlaySize = blackOverlay.sizeDelta;

        // size PNG rects to their native sprite size…
        var ti = titleImage.GetComponent<Image>().sprite.rect;
        titleImage.sizeDelta = new Vector2(ti.width, ti.height);
        var pi = playImage .GetComponent<Image>().sprite.rect;
        playImage .sizeDelta = new Vector2(pi.width, pi.height);

        // …but start them “scaled down” to zero
        titleImage.localScale = Vector3.zero;
        playImage .localScale = Vector3.zero;

        // Store the original volume and set it to zero
        if (musicSource != null)
        {
            originalVolume = musicSource.volume;
            musicSource.volume = 0f;
        }
    }

    void Start()
    {
        StartCoroutine(MenuSequence());
    }

    IEnumerator MenuSequence()
    {        
        yield return new WaitForSeconds(10f);

        // Gradually increase the music volume
        if (musicSource != null)
        {
            yield return StartCoroutine(FadeInMusic(2f)); // Adjust fade duration as needed
        }

        // 1) intro: shrink the full-screen black overlay away
        yield return ShrinkHeight(blackOverlay, overlaySize.y, 0f, 1f);

        yield return new WaitForSeconds(0.5f);

        // 2) animate Title with squash & stretch
        yield return AnimateRetro(titleImage, 1f);

        yield return new WaitForSeconds(0.5f);

        // 3) animate Play button the same way
        yield return AnimateRetro(playImage, 1f);
    }

    public void StartGame()
    {
        StartCoroutine(PlayAndLoad());
    }

    IEnumerator PlayAndLoad()
    {
        // fade in the transition overlay to solid
        yield return FadeOverlay(transitionOverlay, transitionOverlay.color.a, 1f, 1f);

        // fade out music
        float startVol = bgMusic.volume, t=0f;
        while(t < 1f)
        {
            t += Time.deltaTime;
            bgMusic.volume = Mathf.Lerp(startVol, 0f, t);
            yield return null;
        }

        // load your next scene
        SceneManager.LoadScene(levelToLoad);
    }

    IEnumerator ShrinkHeight(RectTransform rt, float fromH, float toH, float dur)
    {
        float elapsed = 0f;
        Vector2 s = rt.sizeDelta;
        while(elapsed < dur)
        {
            elapsed += Time.deltaTime;
            s.y = Mathf.Lerp(fromH, toH, elapsed/dur);
            rt.sizeDelta = s;
            yield return null;
        }
        s.y = toH;
        rt.sizeDelta = s;
    }

    IEnumerator FadeOverlay(Image img, float fromA, float toA, float dur)
    {
        float elapsed = 0f;
        Color c = img.color;
        while(elapsed < dur)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(fromA, toA, elapsed/dur);
            img.color = c;
            yield return null;
        }
        c.a = toA;
        img.color = c;
    }

    /// <summary>
    /// Two‐phase overshoot (x-overshoot, y-squash → settle back) with smooth easing.
    /// </summary>
    IEnumerator AnimateRetro(RectTransform rt, float duration)
    {
        float phase1 = duration * 0.6f;    // blow past target
        float phase2 = duration * 0.4f;    // settle back
        float elapsed = 0f;

        // how far past normal? (120% width, 80% height)
        float overX = 1.2f, overY = 0.8f;

        // PHASE 1: 0 → overshoot
        while(elapsed < phase1)
        {
            elapsed += Time.deltaTime;
            float p = elapsed/phase1;
            float x = Mathf.SmoothStep(0f, overX, p);
            float y = Mathf.SmoothStep(0f, overY, p);
            rt.localScale = new Vector3(x, y, 1f);
            yield return null;
        }

        // PHASE 2: overshoot → settle to normal (1,1,1)
        elapsed = 0f;
        while(elapsed < phase2)
        {
            elapsed += Time.deltaTime;
            float p = elapsed/phase2;
            float x = Mathf.SmoothStep(overX, 1f, p);
            float y = Mathf.SmoothStep(overY, 1f, p);
            rt.localScale = new Vector3(x, y, 1f);
            yield return null;
        }

        rt.localScale = Vector3.one;
    }

    IEnumerator FadeInMusic(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0f, originalVolume, elapsed / duration);
            yield return null;
        }
        musicSource.volume = originalVolume;
    }
}
