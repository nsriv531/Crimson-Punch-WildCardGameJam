using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonHelper : MonoBehaviour
{

    private Button _button;
    [HideInInspector]
    public Button button
    {
        get
        {
            if (_button == null) _button = GetComponent<Button>();
            return _button;
        }
    }

    public void Invoke()
    {
        button.onClick.Invoke();
    }
}
