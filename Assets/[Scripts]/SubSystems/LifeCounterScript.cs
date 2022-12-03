using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class LifeCounterScript : MonoBehaviour
{
    [Header("Life Properties")]
    public int value;

    private Image lifeImage;

    // Start is called before the first frame update
    void Start()
    {
        lifeImage = GetComponent<Image>();
        ResetLives();
    }

    // Update is called once per frame
    public void ResetLives()
    {
        value = 3;
        lifeImage.sprite = Resources.Load<Sprite>("Sprites/Life3");
    }

    public void LoseLife()
    {
        value -= 1;
        if (value < 0)
        {
            value = 0;
        }
        lifeImage.sprite = Resources.Load<Sprite>($"Sprites/Life{value}");
    }

    public void GainLife()
    {
        value += 1;
        if (value > 3)
        {
            value = 3;
        }
        lifeImage.sprite = Resources.Load<Sprite>($"Sprites/Life{value}");
    }
}
