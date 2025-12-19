using TMPro;
using UnityEngine;

public class AmsterdamWins : MonoBehaviour
{
    public TMP_Text winsText; 

    void Start()
    {
        if (winsText != null)
        {
            winsText.text = "Wins: " + GameResult.AmsterdamWins;
        }
    }
}
