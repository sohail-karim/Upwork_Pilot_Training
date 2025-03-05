using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ButtonBorderToggle : MonoBehaviour
{
    public GameObject border; // Assign the border GameObject in the Inspector
    public Image GreenImge;

    public GameObject[] obj;

    private void Start()
    {
        if (border != null)
        {
            border.SetActive(false); // Ensure the border is invisible initially
        }
    }

    public void ToggleBorder()
    {
        foreach(GameObject o in obj)
        {
            o.GetComponentsInChildren<Image>()[1].enabled = false;
        }
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
