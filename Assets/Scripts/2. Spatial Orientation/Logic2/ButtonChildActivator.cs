using UnityEngine;

public class ButtonChildActivator : MonoBehaviour
{
    public void ActivateMyBorder()
    {
        Transform parent = transform.parent;
        Debug.Log("Parent: " + parent.name);

        foreach (Transform button in parent)
        {
            if (button.childCount > 1)
            {
                button.GetChild(1).gameObject.SetActive(false);
            }
        }

        if (transform.childCount > 1)
        {
            transform.GetChild(1).gameObject.SetActive(true);
        }
    }
}
