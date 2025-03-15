using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonBorderToggle : MonoBehaviour
{
    [SerializeField] private List<GameObject> buttons = new List<GameObject>();
    [SerializeField] private GameObject ClickedButton;

    private void OnEnable()
    {
        // Optional: Call UpdateButtonsArray() if buttons are instantiated at runtime
        Debug.Log("Enabled...");
       Invoke("UpdateButtonsArray" , 1f);
    }

    private void OnDisable()
    {
        Debug.Log("OnDisabled...");

        DestroyAllButtons();
    }

    public void EnableImage()
    {
        if (ClickedButton != null)
        {
            ClickedButton.transform.GetChild(0).GetComponent<Image>().enabled = true;
        }
    }

    public void DestroyAllButtons()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        buttons.Clear();
    }

    public void UpdateButtonsArray()
    {
        buttons.Clear();
        //   DestroyAllButtons();
        //   buttons = new List<GameObject>();

        for (int i = 0; i < 4; i++)
        {
            GameObject button = transform.GetChild(i).gameObject;
            buttons.Add(button); // Add button to list

            Button btn = button.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() => OnButtonClicked(button));
            }

            // Disable all borders initially
            button.transform.GetChild(1).gameObject.SetActive(false);
        }

       
        ClickedButton = null; // Reset ClickedButton
    }

    private void OnButtonClicked(GameObject clickedButton)
    {

        Debug.Log("button Clicked...");
      if (buttons == null || buttons.Count == 0) return; // Prevent errors if empty
   
      ClickedButton = clickedButton;
   
      // Disable all borders
      foreach (GameObject btn in buttons)
      {
          btn.transform.GetChild(1).gameObject.SetActive(false);
      }
   
      // Enable border for the clicked button
      clickedButton.transform.GetChild(1).gameObject.SetActive(true);
    }
}
