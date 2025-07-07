using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
    }

    public Sound[] sounds;

    private AudioSource _oneShotSource;
    private AudioSource _loopSource;
    private Dictionary<string, Sound> _soundDict = new Dictionary<string, Sound>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        // Источник для однократных звуков
        _oneShotSource = gameObject.AddComponent<AudioSource>();
        _oneShotSource.playOnAwake = false;

        // Источник для постоянных звуков
        _loopSource = gameObject.AddComponent<AudioSource>();
        _loopSource.playOnAwake = false;
        _loopSource.loop = true;

        foreach (var s in sounds)
            _soundDict[s.name] = s;
    }

    //Просто PlayOneShot
    public void Play(string name)
    {
        if (!_soundDict.ContainsKey(name)) return;
        var s = _soundDict[name];
        _oneShotSource.pitch = Random.Range(0.95f, 1.05f);
        _oneShotSource.volume = s.volume;
        _oneShotSource.PlayOneShot(s.clip);
    }

    //loop
    public void PlayLoop(string name)
    {
        if (!_soundDict.ContainsKey(name)) return;
        var s = _soundDict[name];
        if (_loopSource.isPlaying && _loopSource.clip == s.clip) return;
        _loopSource.clip = s.clip;
        _loopSource.volume = s.volume;
        _loopSource.pitch = 1f;
        _loopSource.Play();
    }

    public void StopLoop()
    {
        if (_loopSource.isPlaying)
            _loopSource.Stop();
    }

    public void PlaySequence(string name, int count, float interval = 0.05f)
    {
        if (!_soundDict.ContainsKey(name)) return;
        StopAllCoroutines();
        StartCoroutine(PlaySequenceCoroutine(_soundDict[name], count, interval));
    }

    private IEnumerator PlaySequenceCoroutine(Sound sound, int count, float interval)
    {
        for (int i = 0; i < count; i++)
        {
            _oneShotSource.pitch = Random.Range(0.95f, 1.05f);
            _oneShotSource.volume = sound.volume;
            _oneShotSource.PlayOneShot(sound.clip);
            yield return new WaitForSeconds(interval);
        }
    }
}
