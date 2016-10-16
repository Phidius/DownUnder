using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    private Image background;
    private Image image;

	// Use this for initialization
	void Awake ()
	{
        foreach (var component in GetComponentsInChildren<Image>())
        {
            if (component.name == "Background")
            {
                background = component;
            }
            if (component.name == "Image")
            {
                image = component;
            }
        }
    }

    public void SetBackground(Color color, bool highlighted)
    {
        color.a = (highlighted) ? .9f : 0f;
        background.color = color;
    }

    public void SetIcon(Sprite sprite)
    {
        image.sprite = sprite;
    }
}
