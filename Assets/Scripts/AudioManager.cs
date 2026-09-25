using UnityEngine;

/// <summary>
/// Gerenciador central de audio para o jogo Meteor Storm:
/// - Musica de fundo espacial em loop
/// - Efeitos sonoros: tiros de laser, explosoes, impactos, power-ups e game over
/// - Sintese procedural de audio embutida (funciona imediatamente mesmo sem arquivos .wav externos)
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Clips (Opcional - Sintetiza automaticamente se nulos)")]
    [SerializeField] private AudioClip backgroundMusicClip;
    [SerializeField] private AudioClip laserClip;
    [SerializeField] private AudioClip meteorExplosionClip;
    [SerializeField] private AudioClip largeExplosionClip;
    [SerializeField] private AudioClip impactClip;
    [SerializeField] private AudioClip powerUpClip;
    [SerializeField] private AudioClip shieldHitClip;
    [SerializeField] private AudioClip gameOverClip;

    [Header("Configuracoes de Volume")]
    [Range(0f, 1f)] [SerializeField] private float musicVolume = 0.45f;
    [Range(0f, 1f)] [SerializeField] private float sfxVolume = 0.85f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Cria os AudioSources caso nao estejam atribuidos
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
        }
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }

        musicSource.loop = true;
        musicSource.volume = musicVolume;
        sfxSource.volume = sfxVolume;

        // Gera proceduralmente os audios que nao foram atribuidos
        GenerateMissingClips();
    }

    private void Start()
    {
        PlayMusic();
    }

    public void PlayMusic()
    {
        if (musicSource != null && backgroundMusicClip != null)
        {
            musicSource.clip = backgroundMusicClip;
            musicSource.volume = musicVolume;
            musicSource.loop = true;
            if (!musicSource.isPlaying)
            {
                musicSource.Play();
            }
        }
    }

    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    public void PlayLaser()
    {
        PlaySFX(laserClip, 0.7f, Random.Range(0.95f, 1.15f));
    }

    public void PlayExplosion(bool isLarge = false)
    {
        AudioClip clipToPlay = isLarge ? (largeExplosionClip ?? meteorExplosionClip) : meteorExplosionClip;
        PlaySFX(clipToPlay, isLarge ? 1.0f : 0.8f, Random.Range(0.85f, 1.1f));
    }

    public void PlayImpact()
    {
        PlaySFX(impactClip, 0.6f, Random.Range(1.1f, 1.3f));
    }

    public void PlayPowerUp()
    {
        PlaySFX(powerUpClip, 0.9f, 1.0f);
    }

    public void PlayShieldHit()
    {
        PlaySFX(shieldHitClip, 0.85f, Random.Range(1.1f, 1.3f));
    }

    public void PlayGameOver()
    {
        StopMusic();
        PlaySFX(gameOverClip, 1.0f, 1.0f);
    }

    private void PlaySFX(AudioClip clip, float volumeScale = 1f, float pitch = 1f)
    {
        if (sfxSource == null || clip == null) return;
        sfxSource.pitch = pitch;
        sfxSource.PlayOneShot(clip, volumeScale * sfxVolume);
    }

    // -------------------------------------------------------------
    // Sintese Procedural de Audio (Retro Arcade Sci-Fi Synth)
    // -------------------------------------------------------------
    private void GenerateMissingClips()
    {
        int sampleRate = 44100;

        if (laserClip == null)
            laserClip = CreateLaserClip(sampleRate);

        if (meteorExplosionClip == null)
            meteorExplosionClip = CreateExplosionClip(sampleRate, 0.4f, 150f);

        if (largeExplosionClip == null)
            largeExplosionClip = CreateExplosionClip(sampleRate, 0.75f, 90f);

        if (impactClip == null)
            impactClip = CreateImpactClip(sampleRate);

        if (powerUpClip == null)
            powerUpClip = CreatePowerUpClip(sampleRate);

        if (shieldHitClip == null)
            shieldHitClip = CreateShieldClip(sampleRate);

        if (gameOverClip == null)
            gameOverClip = CreateGameOverClip(sampleRate);

        if (backgroundMusicClip == null)
            backgroundMusicClip = CreateAmbientMusicClip(sampleRate);
    }

    private AudioClip CreateLaserClip(int sampleRate)
    {
        float duration = 0.14f;
        int totalSamples = (int)(sampleRate * duration);
        float[] samples = new float[totalSamples];

        for (int i = 0; i < totalSamples; i++)
        {
            float t = (float)i / totalSamples;
            float freq = Mathf.Lerp(880f, 220f, t * t);
            float phase = 2f * Mathf.PI * freq * ((float)i / sampleRate);
            float envelope = 1f - t;
            // Onda quadrada suave com saturacao
            float wave = Mathf.Sign(Mathf.Sin(phase)) * 0.4f + Mathf.Sin(phase * 0.5f) * 0.3f;
            samples[i] = wave * envelope;
        }

        AudioClip clip = AudioClip.Create("Procedural_Laser", totalSamples, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip CreateExplosionClip(int sampleRate, float duration, float baseFreq)
    {
        int totalSamples = (int)(sampleRate * duration);
        float[] samples = new float[totalSamples];
        System.Random rand = new System.Random();

        float filterState = 0f;
        for (int i = 0; i < totalSamples; i++)
        {
            float t = (float)i / totalSamples;
            float envelope = Mathf.Pow(1f - t, 1.8f);
            float whiteNoise = (float)(rand.NextDouble() * 2.0 - 1.0);

            // Filtro passa-baixa simples com frequencia caindo
            float cutoff = Mathf.Lerp(baseFreq * 2.5f, 40f, t) / sampleRate;
            filterState += cutoff * (whiteNoise - filterState);

            float rumble = Mathf.Sin(2f * Mathf.PI * (baseFreq * (1f - t * 0.5f)) * ((float)i / sampleRate)) * 0.5f;
            samples[i] = (filterState * 0.7f + rumble * 0.3f) * envelope;
        }

        AudioClip clip = AudioClip.Create("Procedural_Explosion", totalSamples, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip CreateImpactClip(int sampleRate)
    {
        float duration = 0.08f;
        int totalSamples = (int)(sampleRate * duration);
        float[] samples = new float[totalSamples];

        for (int i = 0; i < totalSamples; i++)
        {
            float t = (float)i / totalSamples;
            float freq = Mathf.Lerp(1200f, 400f, t);
            float phase = 2f * Mathf.PI * freq * ((float)i / sampleRate);
            float envelope = Mathf.Pow(1f - t, 2f);
            samples[i] = Mathf.Sin(phase) * envelope * 0.6f;
        }

        AudioClip clip = AudioClip.Create("Procedural_Impact", totalSamples, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip CreatePowerUpClip(int sampleRate)
    {
        float duration = 0.35f;
        int totalSamples = (int)(sampleRate * duration);
        float[] samples = new float[totalSamples];

        // 3 notas ascendentes (Arpejo espacial)
        for (int i = 0; i < totalSamples; i++)
        {
            float t = (float)i / totalSamples;
            float freq;
            if (t < 0.33f) freq = 523.25f; // C5
            else if (t < 0.66f) freq = 659.25f; // E5
            else freq = 783.99f; // G5

            float phase = 2f * Mathf.PI * freq * ((float)i / sampleRate);
            float envelope = Mathf.Sin(t * Mathf.PI);
            samples[i] = (Mathf.Sin(phase) + 0.5f * Mathf.Sin(phase * 2f)) * envelope * 0.5f;
        }

        AudioClip clip = AudioClip.Create("Procedural_PowerUp", totalSamples, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip CreateShieldClip(int sampleRate)
    {
        float duration = 0.2f;
        int totalSamples = (int)(sampleRate * duration);
        float[] samples = new float[totalSamples];

        for (int i = 0; i < totalSamples; i++)
        {
            float t = (float)i / totalSamples;
            float freq = Mathf.Lerp(300f, 600f, t);
            float phase = 2f * Mathf.PI * freq * ((float)i / sampleRate);
            float envelope = Mathf.Sin(t * Mathf.PI);
            samples[i] = Mathf.Sin(phase) * envelope * 0.5f;
        }

        AudioClip clip = AudioClip.Create("Procedural_Shield", totalSamples, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip CreateGameOverClip(int sampleRate)
    {
        float duration = 1.2f;
        int totalSamples = (int)(sampleRate * duration);
        float[] samples = new float[totalSamples];

        // Notas descendentes tristes
        for (int i = 0; i < totalSamples; i++)
        {
            float t = (float)i / totalSamples;
            float freq;
            if (t < 0.33f) freq = 392.00f; // G4
            else if (t < 0.66f) freq = 349.23f; // F4
            else freq = 261.63f; // C4

            float phase = 2f * Mathf.PI * freq * ((float)i / sampleRate);
            float envelope = (1f - t) * 0.7f;
            samples[i] = (Mathf.Sin(phase) + 0.3f * Mathf.Sin(phase * 0.5f)) * envelope;
        }

        AudioClip clip = AudioClip.Create("Procedural_GameOver", totalSamples, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip CreateAmbientMusicClip(int sampleRate)
    {
        // Loop sintetizado de espaco profundo (8 segundos em loop)
        float duration = 8f;
        int totalSamples = (int)(sampleRate * duration);
        float[] samples = new float[totalSamples];

        float[] droneFreqs = new float[] { 55f, 110f, 164.81f, 220f }; // Acorde A / E espacial
        for (int i = 0; i < totalSamples; i++)
        {
            float timeSec = (float)i / sampleRate;
            float mix = 0f;

            for (int f = 0; f < droneFreqs.Length; f++)
            {
                float freq = droneFreqs[f];
                // LFO suave no pitch e volume
                float lfo = 1f + 0.02f * Mathf.Sin(2f * Mathf.PI * 0.25f * timeSec + f);
                float ampLfo = 0.5f + 0.5f * Mathf.Sin(2f * Mathf.PI * (0.15f + f * 0.05f) * timeSec);
                mix += Mathf.Sin(2f * Mathf.PI * freq * lfo * timeSec) * ampLfo * (0.12f / droneFreqs.Length);
            }

            samples[i] = Mathf.Clamp(mix, -1f, 1f);
        }

        AudioClip clip = AudioClip.Create("Procedural_SpaceMusic", totalSamples, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }
}
