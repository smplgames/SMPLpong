using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SingleImageColorCycle : MonoBehaviour
{
    public float CycleSpeed = 2;
    public float CycleNext;
    public float Smoothing = 3;
    
    private float currentIndex;
    
    private void Update()
    {
        CycleNext -= Time.deltaTime;
        if (CycleNext <= 0)
        {
            StopCoroutine(nameof(ShiftColor));
            StartCoroutine(nameof(ShiftColor));
            CycleNext = CycleSpeed;
        }
    }

    private IEnumerator ShiftColor()
    {
        var button = GetComponent<Button>();
        var color =  button.colors.normalColor;
        color = new Color(color.r * 255, color.g * 255, color.b *255, 1);
        
        var nextColor = new Color(Random.Range(50, 255), Random.Range(50, 255), Random.Range(50, 255));
        Debug.Log("Init: " + color + "" + nextColor);
        while (Math.Abs(nextColor.r - color.r) > 1f 
               && Math.Abs(nextColor.g - color.g) > 1f 
               && Math.Abs(nextColor.b - color.b) > 1f)
        {
            var r = Mathf.SmoothStep(color.r, nextColor.r, Time.deltaTime * Smoothing);
            var g = Mathf.SmoothStep(color.g, nextColor.g, Time.deltaTime * Smoothing);
            var b = Mathf.SmoothStep(color.b, nextColor.b, Time.deltaTime * Smoothing);
            
            Debug.Log(color + "" +  nextColor);

            color = new Color(r, g, b);
            var colorBlock = ColorBlock.defaultColorBlock;
            colorBlock.normalColor = new Color(r / 255, g / 255, b / 255);;
            button.colors = colorBlock;

            yield return null;
        }

        yield return null;
    }

    private void CycleShift()
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        var color = spriteRenderer.color;

        var newCol = Col2Int(color);
        newCol = newCol << 1;
        if (newCol <= 29360128) newCol = int.MaxValue;

        Debug.Log(newCol + " oldColor: " + color + " new: " + Int2Col(newCol));
        
        spriteRenderer.color = Int2Col(newCol);
    }
    
    private void CylceSin()
    {
        currentIndex += 0.1f;
        
        float red = Mathf.Sin (0.1f * currentIndex + 3);
        float grn = Mathf.Sin (0.8f * currentIndex + 12);
        float blu = Mathf.Sin (2f * currentIndex + 5);

        var spriteRenderer = GetComponent<SpriteRenderer>();
        var color = spriteRenderer.color;
       
        color.r = red;
        color.g = grn;
        color.b = blu;

        Debug.Log(red + " " + grn + " " + blu + "->" + color);
        
        spriteRenderer.color = color;
    }
    
    public int Col2Int(Color aColor)
    {
        Color32 c = aColor;
        return c.r | (c.g << 8) | (c.b << 16);
    }
  
    public Color Int2Col(int aValue)
    {
        Color32 c = new Color32((byte)aValue,(byte) (aValue >> 8), (byte)(aValue >> 16), 255);
        return c;
    }
}