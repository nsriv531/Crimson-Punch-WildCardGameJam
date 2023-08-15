using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFade : MonoBehaviour
{
    public bool destroyOnFade = true;
    
    public float fadeDelay = 5f;
    public float fadeTime = 3f;

    private float _time = 0.0f;
    
    private Text _text;
    
    void Start()
    {
        _text = GetComponent<Text>();
    }
    
    void Update()
    {
        _time += Time.deltaTime;
        
        if(_time > fadeDelay + fadeTime) 
        {
            if (destroyOnFade)
            {
                Destroy(gameObject);
            }
        }
        else if (_time > fadeDelay)
        {
            var alpha = Mathf.Clamp01(1.0f - (_time - fadeDelay) / fadeTime);
            _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, alpha);
        }
    }
}
