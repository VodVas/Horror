public interface IAudioEffect
{
    void Initialize(int sampleRate, int channels);
    void Process(float[] data, int channels);
    void Reset();
}