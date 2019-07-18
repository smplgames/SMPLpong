using System;
using System.Collections;
using UnityEngine;

public class BallColorCycle : MonoBehaviour
{
    public float Smoothing = 3;
    
    private Color[] colors;
    private int colorIndex;
    
    // Start is called before the first frame update
    void Start()
    {
        colors = ColorPallete.Instance.Colors;
        StartCoroutine(ShiftColor());
    }

    private IEnumerator ShiftColor()
    {
        var meshRenderer = GetComponent<MeshRenderer>();
        var material = meshRenderer.material;

        while (true)
        {
            var color = colors[colorIndex++];
            if (colorIndex >= colors.Length) colorIndex = 0;
            
            var nextColor = colors[colorIndex];
        
            while (Math.Abs(nextColor.r - color.r) > 0.01f 
                   || Math.Abs(nextColor.g - color.g) > 0.01f 
                   || Math.Abs(nextColor.b - color.b) > 0.01f)
            {
                var r = Mathf.SmoothStep(color.r, nextColor.r, Time.deltaTime * Smoothing);
                var g = Mathf.SmoothStep(color.g, nextColor.g, Time.deltaTime * Smoothing);
                var b = Mathf.SmoothStep(color.b, nextColor.b, Time.deltaTime * Smoothing);

                color = new Color(r, g, b);
                material.SetColor("_EmissionColor", color);
                material.SetColor("_Color", color);
            
                yield return null;
            }
        
            yield return null;
        }
    }
}
