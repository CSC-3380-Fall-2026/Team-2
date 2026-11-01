using System.Collections;
using UnityEngine;

public class AlternatingPlatforms : MonoBehaviour
{
    [SerializeField] private float platformDuration = 2f;
    [SerializeField] private float overlapDuration = 0.8f;
    [SerializeField] private int blinkCount = 100; // Total flashes before disappearing
    [SerializeField] private GameObject redPlatforms;
    [SerializeField] private GameObject purplePlatforms;

    private bool isRedActive = true;

    void Start()
    {
        if (redPlatforms != null) redPlatforms.SetActive(true);
        if (purplePlatforms != null) purplePlatforms.SetActive(false);

        StartCoroutine(PlatformTimer());
    }

    IEnumerator PlatformTimer()
    {
        while (true)
        {
            // Solo active period
            yield return new WaitForSeconds(platformDuration);

            // Determine which set is arriving and which is about to vanish
            GameObject incoming = isRedActive ? purplePlatforms : redPlatforms;
            GameObject outgoing = isRedActive ? redPlatforms : purplePlatforms;

            // 1. Turn on incoming platforms so both exist at once
            if (incoming != null) incoming.SetActive(true);

            // 2. Flash outgoing platform during the overlap window
            if (outgoing != null)
            {
                yield return StartCoroutine(BlinkRoutine(outgoing, overlapDuration, blinkCount));
                outgoing.SetActive(false);
            }

            // Flip state
            isRedActive = !isRedActive;
        }
    }

    IEnumerator BlinkRoutine(GameObject target, float totalDuration, int flashes)
    {
        Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
        float interval = totalDuration / (flashes * 2f);

        for (int i = 0; i < flashes; i++)
        {
            SetRenderersVisibility(renderers, false);
            yield return new WaitForSeconds(interval);

            SetRenderersVisibility(renderers, true);
            yield return new WaitForSeconds(interval);
        }
    }

    private void SetRenderersVisibility(Renderer[] renderers, bool isVisible)
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null) renderers[i].enabled = isVisible;
        }
    }
}