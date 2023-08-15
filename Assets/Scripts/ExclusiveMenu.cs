using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExclusiveMenu : MonoBehaviour
{
    public bool autoresolveMenus = true;
    public GameObject[] menus;
    
    private int currentlyOpenMenu = -1;

    public void Init()
    {
        // Add all children to the menus array
        if (autoresolveMenus)
        {
            menus = new GameObject[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                menus[i] = transform.GetChild(i).gameObject;
            }
        }
        
        CloseAll();
        Open(currentlyOpenMenu);
    }
    
    public void CloseAll()
    {
        currentlyOpenMenu = -1;
        foreach (var menu in menus)
        {
            menu.SetActive(false);
        }
    }
    
    public void Open(int index)
    {
        if (index == currentlyOpenMenu) return;
        
        GetCurrentlyOpenMenu()?.SetActive(false);
        currentlyOpenMenu = index;
        GetCurrentlyOpenMenu().SetActive(true);
    }

    public void Open(string name)
    {
        var currentlyOpenMenu = GetCurrentlyOpenMenu();
        if (currentlyOpenMenu && name == currentlyOpenMenu.name) return;
        Open(GetMenu(name));
        Debug.Log("Opened menu: " + GetCurrentlyOpenMenu().name);
    }
    
    private int GetMenu(string name)
    {
        for (int i = 0; i < menus.Length; i++)
        {
            if (menus[i].name == name)
                return i;
        }

        throw new System.Exception("Menu not found: " + name);
    }
    
    private GameObject GetMenu(int index)
    {
        return index == -1? null : menus[index];
    }
    
    public GameObject GetCurrentlyOpenMenu()
    {
        return currentlyOpenMenu == -1? null : menus[currentlyOpenMenu];
    }
}
