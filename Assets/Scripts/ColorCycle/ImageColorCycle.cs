using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ColorCycle
{
    public class ImageColorCycle : MonoBehaviour
    {
        public float CycleSpeed = 2;
        public float CycleNext;
        public float Smoothing = 3;
        public List<GameObject> GameObjects;
        public List<Color> ColorPallete;
    
        private int currentIndex;

        private void Update()
        {
            CycleNext -= Time.deltaTime;
            if (CycleNext <= 0)
            {
                ShiftColorMultiple();
                CycleNext = CycleSpeed;
            }
        }

        private void ShiftColorMultiple()
        {
            StopAllCoroutines();

            var nextColor = ColorPallete[currentIndex++];
            if (currentIndex >= ColorPallete.Count) currentIndex = 0;
            nextColor = new Color(nextColor.r * 255, nextColor.g * 255, nextColor.b *255, 1);
            for (int i = 0; i < GameObjects.Count; i++)
            {
                StartCoroutine(ShiftColor(GameObjects[i], nextColor));
            }
        }
    
        private IEnumerator ShiftColor(GameObject go, Color nextColor)
        {
            var image = go.GetComponent<Image>();
            var color = image.color;
            color = new Color(color.r * 255, color.g * 255, color.b *255, 1);
        
            while (Math.Abs(nextColor.r - color.r) > 1f 
                   || Math.Abs(nextColor.g - color.g) > 1f 
                   || Math.Abs(nextColor.b - color.b) > 1f)
            {
                var r = Mathf.SmoothStep(color.r, nextColor.r, Time.deltaTime * Smoothing);
                var g = Mathf.SmoothStep(color.g, nextColor.g, Time.deltaTime * Smoothing);
                var b = Mathf.SmoothStep(color.b, nextColor.b, Time.deltaTime * Smoothing);
            
            
                color = new Color(r, g, b);
                image.color = new Color(r / 255, g / 255, b / 255);

                yield return null;
            }

            yield return null;
        }
    }
}