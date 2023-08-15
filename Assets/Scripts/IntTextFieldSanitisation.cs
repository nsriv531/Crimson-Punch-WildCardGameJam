using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntTextFieldSanitisation : MonoBehaviour
{
    public int defaultValue;
    public int minValue;
    public int maxValue;
    
    private InputField _inputField;

    void Start()
    {
        _inputField = GetComponent<InputField>();
    }

    public void Validate()
    {
        var text = _inputField.text.Trim();
        
        if(int.TryParse(text, out int value))
        {
            if (value < minValue)
            {
                value = minValue;
            }
            else if (value > maxValue)
            {
                value = maxValue;
            }
        }
        else
        {
            value = defaultValue;
        }

        _inputField.text = value.ToString();
    }
}
