using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;

public class SceneManager : MonoBehaviour
{
    [SerializeField] private string mainMenuName = "MainMenu";

    private const LoadSceneMode defaultLoadSceneMode = LoadSceneMode.Single;

    public void LoadScene(string name, LoadSceneMode mode = defaultLoadSceneMode)
    {
        if (NetworkManager.Singleton != null)
        {
            if (!NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsHost)
                throw new System.Exception("Only servers or hosts can load scenes using the NetworkManager");

            // This should only be used if the NetworkManager is not shutdown, but I'm not sure how to check that
            NetworkManager.Singleton.SceneManager.LoadScene(name, mode);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(name, mode);
        }
    }


    public void LoadScene(string name) => LoadScene(name, defaultLoadSceneMode);

    /*
    public void LoadMainMenu()
    {
        foreach (Character character in FindObjectsOfType<Character>())
        {
            if (character.IsOwner)
            {
                Debug.Log(character);
            }
        }
        LoadScene(mainMenuName);
    }
    */
}
