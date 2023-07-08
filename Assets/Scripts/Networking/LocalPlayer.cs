using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;


public static class LocalPlayer
{
    public static UnityEvent onGameObjectSet = new UnityEvent();

    private static GameObject _gameObject;
    public static GameObject gameObject
    {
        get
        {
            if (_gameObject == null)
            {
                if (NetworkManager.Singleton == null) throw new NetworkManagerNotFoundException();
                if (NetworkManager.Singleton.LocalClient == null) throw new LocalClientNotFoundException();
                if (NetworkManager.Singleton.LocalClient.PlayerObject == null) throw new LocalPlayerNotFoundException();
                _gameObject = NetworkManager.Singleton.LocalClient.PlayerObject.gameObject;
                onGameObjectSet.Invoke();
            }
            return _gameObject;
        }
    }

    /*
    private static Camera _camera;
    public static Camera camera
    {
        get
        {
            if (_camera == null)
            {
                PlayerInput playerInput = gameObject.GetComponent<PlayerInput>();
                if (playerInput != null) _camera = playerInput.camera;
                else throw new System.Exception("Failed to find the PlayerInput on LocalPlayer");
            }
            return _camera;
        }
    }

    private static AudioListener _audioListener;
    public static AudioListener audioListener
    {
        get
        {
            if (_audioListener == null)
            {
                List<AudioListener> listeners = new List<AudioListener>(gameObject.GetComponentsInChildren<AudioListener>());
                if (listeners.Count > 1) throw new System.Exception("Found more than 1 AudioListeners on Player " + gameObject);
                if (listeners.Count == 1) _audioListener = listeners[0];
            }
            return _audioListener;
        }
    }
    

    private static Character _character;
    public static Character character
    {
        get
        {
            if (_character == null) _character = gameObject.GetComponent<Character>();
            return _character;
        }
    }

    public static bool IsOwner(Transform t) => ReferenceEquals(t.root, character.transform.root);
    */



    public class NetworkManagerNotFoundException : Exception
    {
        public NetworkManagerNotFoundException() { }
        public NetworkManagerNotFoundException(string message) : base(message) { }
        public NetworkManagerNotFoundException(string message, Exception inner) : base(message, inner) { }
    }

    public class LocalClientNotFoundException : Exception
    {
        public LocalClientNotFoundException() { }
        public LocalClientNotFoundException(string message) : base(message) { }
        public LocalClientNotFoundException(string message, Exception inner) : base(message, inner) { }
    }

    public class LocalPlayerNotFoundException : Exception
    {
        public LocalPlayerNotFoundException() { }
        public LocalPlayerNotFoundException(string message) : base(message) { }
        public LocalPlayerNotFoundException(string message, Exception inner) : base(message, inner) { }
    }
}


