using UnityEngine;
using TMPro;

public class TotalKillsDisplay : MonoBehaviour
{
    public TextMeshProUGUI totalKillsText;

    void Start()
    {
        // If not assigned, try to find a TextMeshProUGUI in the current GameObject
        if (totalKillsText == null)
        {
            totalKillsText = GetComponent<TextMeshProUGUI>();
        }

        Refresh();
    }

    // Call to refresh the displayed total (useful when returning to menu)
    public void Refresh()
    {
        int total = PlayerPrefs.GetInt("totalKills", 0);
        if (totalKillsText != null)
            totalKillsText.SetText($"Total kills: {total}");
        else
            Debug.LogWarning("TotalKillsDisplay: totalKillsText is not assigned.");
    }

    // Optional helper to reset total kills (can be wired to a reset button)
    public void ResetTotalKills()
    {
        PlayerPrefs.DeleteKey("totalKills");
        PlayerPrefs.Save();
        Refresh();
    }
}
