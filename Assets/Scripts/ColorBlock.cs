using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ColorBlock : MonoBehaviour
{
    #region Enum
public enum ColorType
{
    YELLOW,
    PURPLE,
    RED,
    BLUE,
    GREEN,
    PINK,
    ANY,
    COUNT
}
#endregion
    #region References
    [System.Serializable]
    public struct ColorSprite
    {
        public ColorType color;
        public Sprite sprite;
    }
    //which has keys of Color sprite and values of game object  
    private Dictionary<ColorType, Sprite> colorSpriteDictionary;
    //define array the struct that we can edit in the inspector
    public ColorSprite[] colorSprites;
    private ColorType color;
    private SpriteRenderer sprite;
    #endregion

    #region Proproties

    public ColorType Color
    {
        get { return color; }
        set { SetColor(value); }
    }
    public int NumColors
    {
        get { return colorSprites.Length; }
    }
    #endregion
    private void Awake()
    {
        sprite = transform.Find("block").GetComponent<SpriteRenderer>();

        colorSpriteDictionary = new Dictionary<ColorType, Sprite>();

        for (int i = 0; i < colorSprites.Length; i++)
        {
            if (!colorSpriteDictionary.ContainsKey(colorSprites[i].color))
            {
                //check Dictionary not null with contain key, if it is null the Dictionary add a new key value to Dictionanry.
                colorSpriteDictionary.Add(colorSprites[i].color, colorSprites[i].sprite);
            }
        }
    }

    public void SetColor(ColorType newColor)
    {
        color = newColor;

        if (colorSpriteDictionary.ContainsKey(newColor))
        {
            sprite.sprite = colorSpriteDictionary[newColor];  
        }
    }
}
