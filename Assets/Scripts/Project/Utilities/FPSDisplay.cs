using System.Collections;
using UnityEngine;

namespace Project.Utilities
{
    // Code adapted from: https://gist.github.com/mstevenson/5103365
    public class FPSDisplay : MonoBehaviour
    {
        private float count;
    
        private IEnumerator Start()
        {
            GUI.depth = 2;
            while (true)
            {
                count = 1f / Time.unscaledDeltaTime;
                yield return new WaitForSeconds(0.1f);
            }
        }
    
        private void OnGUI()
        {
            var width = 85;
            Rect location = new Rect(Screen.width - (width) + 5, 5, width, 25);
            string text = $"FPS: {Mathf.Round(count)}";
            Texture black = Texture2D.linearGrayTexture;
            GUI.DrawTexture(location, black, ScaleMode.StretchToFill);
            GUI.color = Color.black;
            GUI.skin.label.fontSize = 18;
            GUI.Label(location, text);
        }
    }
}