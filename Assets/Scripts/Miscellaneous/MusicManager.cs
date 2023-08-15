using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MusicManager : MonoBehaviour
{
    [SerializeField] private bool _playOnEnable = true;
    [SerializeField] private bool _canSelectPreviouslyPlayedSong = false;
    [SerializeField, Min(0f)] private float _minTimeBetweenSongs;
    [SerializeField, Min(0f)] private float _maxTimeBetweenSongs;

    [Header("Debugging")]
    [ReadOnly, SerializeField] private AudioClip currentSong;
    [ReadOnly, SerializeField] private float songDuration = 0f;
    [ReadOnly, SerializeField] private bool playingSong = false;
    [ReadOnly, SerializeField] private float timer = 0f;
    [ReadOnly, SerializeField] private float timeLimit = 0f;
    [ReadOnly, SerializeField] private bool waitingToPlayNewSong = false;

    private AudioSource previouslyPlayed;

    private List<AudioSource> _sources;
    [HideInInspector] public List<AudioSource> sources
    {
        get
        {
            if (_sources == null) _sources = new List<AudioSource>(GetComponentsInChildren<AudioSource>(true));
            return _sources;
        }
    }


    void OnEnable()
    {
        foreach (AudioSource source in sources)
        {
            source.gameObject.SetActive(false);
            source.playOnAwake = true;
        }
        if (_playOnEnable) PlayNewSong();
    }


    // Update is called once per frame
    void Update()
    {
        if (playingSong)
        {
            timer = Mathf.Min(timer + Time.deltaTime, songDuration);
            if (timer == songDuration)
            {
                timeLimit = Random.Range(_minTimeBetweenSongs, _maxTimeBetweenSongs);
                timer = 0f;
                waitingToPlayNewSong = true;
                playingSong = false;
                previouslyPlayed.gameObject.SetActive(false);
            }
        }
        else if (waitingToPlayNewSong)
        {
            timer = Mathf.Min(timer + Time.deltaTime, timeLimit);
            if (timer == timeLimit)
            {
                PlayNewSong();
            }
        }
    }


    private AudioSource ChooseRandomSong()
    {
        List<AudioSource> available = new List<AudioSource>(sources);

        if (previouslyPlayed != null && !_canSelectPreviouslyPlayedSong)
        {
            available.Remove(previouslyPlayed);
        }

        if (available.Count == 0) throw new System.Exception("Ran out of music");

        return available.Random();
    }

    public void Stop()
    {
        previouslyPlayed.Stop();
        previouslyPlayed.gameObject.SetActive(false);
    }


    public void PlayNewSong()
    {
        if (playingSong) Stop();

        AudioSource song = ChooseRandomSong();
        previouslyPlayed = song;
        currentSong = song.clip;
        songDuration = currentSong.length;
        timer = 0f;
        song.gameObject.SetActive(true);
        playingSong = true;
        waitingToPlayNewSong = false;
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(MusicManager))]
public class MusicManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        bool wasEnabled = GUI.enabled;

        GUI.enabled = Application.isPlaying;

        MusicManager script = target as MusicManager;
        if (GUILayout.Button("Play Next")) script.PlayNewSong();

        GUI.enabled = wasEnabled;
    }
}
#endif