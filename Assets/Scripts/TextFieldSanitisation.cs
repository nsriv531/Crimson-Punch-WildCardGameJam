using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextFieldSanitisation : MonoBehaviour
{
    public enum CharacterRestrictMode
    {
        Strict,
        Loose, 
        None
    }

    public CharacterRestrictMode characterRestrictMode = CharacterRestrictMode.Strict;
    public int characterLimit = 20;
    
    private InputField _inputField;
    
    private const string AllowedCharactersStrict = "_abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private const string AllowedCharactersLoose = " ~!@#$%^&*()_+`-={}|[]\\;':\",./<>?abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    void Start()
    {
        _inputField = GetComponent<InputField>();
    }

    public void Validate()
    {
        var text = _inputField.text.Trim();

        string value;
        if (characterRestrictMode == CharacterRestrictMode.None)
        {
            value = text.Substring(0, characterLimit);
        }
        else
        {
            value = "";
            
            var allowedCharacters = characterRestrictMode == CharacterRestrictMode.Strict
                ? AllowedCharactersStrict
                : AllowedCharactersLoose;
            
            for(int i = 0; i < Mathf.Min(text.Length, characterLimit); i++)
            {
                if (allowedCharacters.Contains(text[i].ToString()))
                {
                    value += text[i];
                }
            }
        }

        _inputField.text = value.Trim();
    }
}
