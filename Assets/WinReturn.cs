using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinReturn : MonoBehaviour
{
    [Header("Fade Settings")]
    public GameObject blackPanel;
    public float fadeDuration = 2f;
    public float delayBeforeScene = 1f;
    public int sceneIndex = 0;

    public void BackToLobby()
    {
        StartCoroutine(FadeAndLoad());
    }

    private IEnumerator FadeAndLoad()
    {
        if (blackPanel != null)
            blackPanel.SetActive(true);

        CanvasGroup canvasGroup = blackPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = blackPanel.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;

        yield return new WaitForSeconds(delayBeforeScene);

        GameResult.AmsterdamWins++;
        WinSave.SaveWins(GameResult.AmsterdamWins);

        SceneManager.LoadSceneAsync(sceneIndex);
    }


}
