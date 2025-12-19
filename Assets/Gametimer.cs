using UnityEngine;
using TMPro;
using System.Collections;

public class Gametimer : MonoBehaviour
{
    public static Gametimer Instance;

    public TextMeshProUGUI timeText;

    public GameObject player;
    public GameObject firstCanvas;
    public GameObject secondCanvas;
    public float canvasSwitchDelay = 5f;

    private int seconds;
    private int minutes;
    private int hours;
    private float timer;

    private bool canvasSwitched;

    public int Hours => hours;
    public int Minutes => minutes;
    public int Seconds => seconds;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= 1f)
        {
            timer -= 1f;
            minutes++;
            if (minutes >= 60)
            {
                minutes = 0;
                hours++;
            }
            if (seconds >= 60)
            {
                seconds = 0;
                minutes++;

                
            }

            if (timeText != null)
                timeText.text = $"{hours:00}:{minutes:00}:{seconds:00}";

            if (hours >= 6 && !canvasSwitched)
            {
                canvasSwitched = true;
                StartCoroutine(SwapCanvases());
            }
        }
    }

    private IEnumerator SwapCanvases()
    {
        
        CharacterController controller = player.GetComponent<CharacterController>();
        if (controller != null)
            controller.enabled = false;

        
        MonoBehaviour[] scripts = player.GetComponents<MonoBehaviour>();
        foreach (var script in scripts)
        {
            if (script != this) 
                script.enabled = false;
        }

        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (secondCanvas != null)
            secondCanvas.SetActive(true);

        if (firstCanvas != null)
            firstCanvas.SetActive(false);

        yield return new WaitForSeconds(canvasSwitchDelay);

        if (firstCanvas != null)
            firstCanvas.SetActive(true);
    }
}
