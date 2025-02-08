using UnityEngine;
using UnityEngine.UI;

public class ButtonBorderToggle : MonoBehaviour
{
    public GameObject border; // Assign the border GameObject in the Inspector
    public Image GreenImge;

    private void Start()
    {
        if (border != null)
        {
            border.SetActive(false); // Ensure the border is invisible initially
        }
    }

    public void ToggleBorder()
    {
        if (border != null)
        {
            border.SetActive(!border.activeSelf); // Toggle border visibility
        }
    }

    public void EnableImage()
    {
        GreenImge.enabled = true;
    }
}
