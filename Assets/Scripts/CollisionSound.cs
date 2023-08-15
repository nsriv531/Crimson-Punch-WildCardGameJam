using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// To be placed on a GameObject that is expected to collide with other colliders.
/// </summary>
[ExecuteInEditMode, RequireComponent(typeof(BoxCollider), typeof(SphereCollider), typeof(AudioSource))]
public class CollisionSound : MonoBehaviour
{
    [SerializeField] private ColliderType _type;
    [SerializeField] private Modes _mode = Modes.OnTriggerEnter;
    [SerializeField] private CollisionSound includeSoundsFrom;
    [SerializeField] private List<Sound> _sounds;

    [Header("Debugging")]
    [SerializeField] private bool debug = false;


    [HideInInspector] public List<Sound> sounds { get => _sounds; }

    [HideInInspector] public Modes mode { get => _mode; }

    private BoxCollider _boxCollider;
    [HideInInspector]
    public BoxCollider boxCollider
    {
        get
        {
            if (_boxCollider == null) _boxCollider = GetComponent<BoxCollider>();
            return _boxCollider;
        }
    }
    private SphereCollider _sphereCollider;
    [HideInInspector]
    public SphereCollider sphereCollider
    {
        get
        {
            if (_sphereCollider == null) _sphereCollider = GetComponent<SphereCollider>();
            return _sphereCollider;
        }
    }

    private AudioSource _audioSource;
    [HideInInspector]
    public AudioSource audioSource
    {
        get
        {
            if (_audioSource == null) _audioSource = GetComponent<AudioSource>();
            return _audioSource;
        }
    }




    private CollisionEvents _events;
    [HideInInspector]
    public CollisionEvents events
    {
        get
        {
            if (_events == null) _events = GetComponent<CollisionEvents>();
            return _events;
        }
    }

    [HideInInspector] public ColliderType type { get => _type; }

    private bool justEntered = false;

    public enum ColliderType
    {
        Box,
        Sphere
    }

    [System.Flags]
    public enum Modes
    {
        Nothing = 0,
        OnTriggerEnter = 1 << 0,
        OnTriggerExit = 1 << 1,
        OnTriggerStay = 1 << 2,
        Everything = ~Nothing,
    }


    [System.Serializable]
    public class Sound
    {
        public List<PhysicMaterial> materials;
        [Range(0f, 1f)] public float volume;
        public List<AudioClip> clips;

        public Sound()
        {
            materials = new List<PhysicMaterial>();
            volume = 1f;
            clips = new List<AudioClip>();
        }
    }

    #region Editor
#if UNITY_EDITOR
    void OnValidate()
    {
        if (!Application.isPlaying)
        {
            if (sphereCollider != null && boxCollider != null)
            {
                if (type == ColliderType.Box)
                {
                    boxCollider.enabled = true;
                    sphereCollider.enabled = false;
                    boxCollider.hideFlags &= ~HideFlags.NotEditable;
                    sphereCollider.hideFlags = HideFlags.NotEditable;
                }
                else
                {
                    boxCollider.enabled = false;
                    sphereCollider.enabled = true;

                    boxCollider.hideFlags = HideFlags.NotEditable;
                    sphereCollider.hideFlags &= ~HideFlags.NotEditable;
                }
            }
        }
    }

    void Update()
    {
        boxCollider.isTrigger = true;
        sphereCollider.isTrigger = true;
    }
#endif
    #endregion



    public void PlaySounds(PhysicMaterial material)
    {
        if (debug) Debug.Log("PlaySounds called", gameObject);
        List<Sound> s = new List<Sound>(sounds);
        if (includeSoundsFrom != null)
        {
            foreach (Sound sound in includeSoundsFrom.sounds)
            {
                s.Add(sound);
            }
        }
        foreach (Sound sound in s)
        {
            if (sound.materials.Contains(material))
            {
                audioSource.PlayOneShot(sound.clips.Random(), sound.volume);
            }
        }
    }


    // This is called before OnTriggerStay
    public void OnTriggerEnter(Collider other)
    {
        if (debug) Debug.Log("OnTriggerEnter, mode = " + mode + ", collider = " + other, gameObject);
        if (mode.HasFlag(Modes.OnTriggerEnter)) PlaySounds(other.sharedMaterial);
        justEntered = true;
    }

    public void OnTriggerExit(Collider other)
    {
        if (debug) Debug.Log("OnTriggerExit, mode = " + mode + ", collider = " + other, gameObject);
        if (mode.HasFlag(Modes.OnTriggerExit)) PlaySounds(other.sharedMaterial);
        justEntered = false;
    }

    // This is called after OnTriggerStay
    public void OnTriggerStay(Collider other)
    {
        if (!justEntered) OnTriggerEnter(other);
        if (debug) Debug.Log("OnTriggerStay, mode = " + mode + ", collider = " + other, gameObject);
        if (mode.HasFlag(Modes.OnTriggerStay)) PlaySounds(other.sharedMaterial);
    }
}
