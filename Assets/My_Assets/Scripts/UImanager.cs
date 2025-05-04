using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UImanager : MonoBehaviour
{
    [Header("Player")]
    public GameObject player;               // player prefab

    [Header("UI Overlays")]
    public RectTransform blackOverlay;      // intro overlay, shrinks away

    [Header("Animated UI")]
    public RectTransform mercury;           // the bar we grow/shrink

    public string levelToLoad = "Level1";

    private Vector2 overlaySize;
    private float    _fullHeight;

    void Awake()
    {
        // 1) Cache overlay size for your intro shrink
        overlaySize = blackOverlay.sizeDelta;

        // 2) Pull the sprite’s native size
        if (mercury == null)
        {
            Debug.LogError("Mercury Transform is not assigned!");
            return;
        }
        Rect spriteRect = mercury.GetComponent<Image>().sprite.rect;
        float nativeWidth  = spriteRect.width;
        _fullHeight       = spriteRect.height;

        // 3) Make it “empty” at start by zeroing its height
        mercury.sizeDelta = new Vector2(nativeWidth, 0f);
    }

    void Start()
    {
        StartCoroutine(MenuSequence());
    }

    IEnumerator MenuSequence()
    {
        yield return new WaitForSeconds(0.5f);

        // shrink the intro black overlay away
        yield return ShrinkHeight(blackOverlay, overlaySize.y, 0f, 1f);
    }

    void Update()
    {
        // normalize speed 0→1
        float speed = Mathf.Clamp01(player.GetComponent<Movement>().speed);

        // compute new height
        float h = _fullHeight * speed;

        // apply it (width stays the same)
        mercury.sizeDelta = new Vector2(mercury.sizeDelta.x, h);
    }

    IEnumerator ShrinkHeight(RectTransform rt, float fromH, float toH, float dur)
    {
        float elapsed = 0f;
        Vector2 s     = rt.sizeDelta;

        while (elapsed < dur)
        {
            elapsed += Time.deltaTime;
            s.y = Mathf.Lerp(fromH, toH, elapsed / dur);
            rt.sizeDelta = s;
            yield return null;
        }

        s.y = toH;
        rt.sizeDelta = s;
    }

    public void EndRace()
    {
        Debug.Log("EndRace called!!!!!!!!!");
        if (string.IsNullOrEmpty(levelToLoad))
        {
            Debug.LogError("Scene name is not assigned in the Inspector!");
            return;
        }

        // Start the coroutine to handle the delay
        StartCoroutine(EndRaceCoroutine(levelToLoad));
    }

    private IEnumerator EndRaceCoroutine(string sceneName)
{
    Debug.Log("EndRaceCoroutine called with scene name: " + sceneName);

    // 1) wait a beat
    yield return new WaitForSeconds(2f);

    // 2) animate overlay from zero → full height
    float animationDuration = 1f;
    float elapsedTime       = 0f;

    // use the original size you saved in Awake()
    Vector2 initialSize = new Vector2(overlaySize.x, 0f);
    Vector2 targetSize  = overlaySize;

    // make sure it's visible
    blackOverlay.gameObject.SetActive(true);
    blackOverlay.sizeDelta = initialSize;

    while (elapsedTime < animationDuration)
    {
        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / animationDuration);
        blackOverlay.sizeDelta = Vector2.Lerp(initialSize, targetSize, t);
        yield return null;
    }

    // snap to the exact final size
    blackOverlay.sizeDelta = targetSize;

    // 3) load the scene
    SceneManager.LoadScene(sceneName);
}
}
