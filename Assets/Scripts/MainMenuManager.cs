using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;

    [Header("Toast Notification")]
    [SerializeField] private GameObject toastPanel;
    [SerializeField] private TextMeshProUGUI toastText;
    [SerializeField] private float toastDuration = 2f;
    [SerializeField] private AnimationCurve fadeInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private AnimationCurve fadeOutCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    private CanvasGroup toastCanvasGroup;
    private Coroutine activeToastCoroutine;

    private void Awake()
    {
        // Get or add CanvasGroup to toast panel
        toastCanvasGroup = toastPanel.GetComponent<CanvasGroup>();
        if (toastCanvasGroup == null)
            toastCanvasGroup = toastPanel.AddComponent<CanvasGroup>();
        
        // Initially hide the toast
        toastPanel.SetActive(false);
    }

    private void Start()
    {
        // Connect button clicks
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayGameClicked);
        
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettingsClicked);
        
        if (exitButton != null)
            exitButton.onClick.AddListener(OnExitClicked);
    }

    public void OnPlayGameClicked()
    {
        ShowToast("Starting game...");
        // Add your game start logic here
    }

    public void OnSettingsClicked()
    {
        ShowToast("Opening settings...");
        // Add your settings logic here
    }

    public void OnExitClicked()
    {
        ShowToast("Exiting game...");
        // Add a small delay before actually quitting
        StartCoroutine(DelayedQuit());
    }

    private IEnumerator DelayedQuit()
    {
        yield return new WaitForSeconds(1f);
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void ShowToast(string message)
    {
        // If there's an active toast, stop it
        if (activeToastCoroutine != null)
            StopCoroutine(activeToastCoroutine);

        // Start new toast
        activeToastCoroutine = StartCoroutine(ShowToastCoroutine(message));
    }

    private IEnumerator ShowToastCoroutine(string message)
    {
        // Set message and show panel
        toastText.text = message;
        toastPanel.SetActive(true);

        // Fade in
        float elapsedTime = 0f;
        float fadeDuration = 0.5f;
        
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / fadeDuration;
            toastCanvasGroup.alpha = fadeInCurve.Evaluate(normalizedTime);
            yield return null;
        }

        // Wait for duration
        yield return new WaitForSeconds(toastDuration);

        // Fade out
        elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / fadeDuration;
            toastCanvasGroup.alpha = fadeOutCurve.Evaluate(normalizedTime);
            yield return null;
        }

        // Hide panel
        toastPanel.SetActive(false);
        activeToastCoroutine = null;
    }
} 