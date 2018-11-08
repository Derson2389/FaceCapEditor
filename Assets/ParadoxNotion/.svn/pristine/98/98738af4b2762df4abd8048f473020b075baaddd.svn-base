using System;
using UnityEngine;
using System.Runtime.InteropServices;

public class AudioCapturer
{
    #region Data

    private const int BufferSize = 16;

    private float[] _buffer;

    private float[] _readBuffer;

    private int _bufferIndex;

    private GCHandle _bufferHandle;

    private int _channelCount;

    private object _lockObject = new object();

    #endregion

    #region Public 

    public void Init()
    {
        int bufferLength = 0;
        int bufferCount = 0;
        AudioSettings.GetDSPBufferSize(out bufferLength, out bufferCount);

#if UNITY_5 || UNITY_2017_1_OR_NEWER
        _channelCount = GetNumChannels(AudioSettings.driverCapabilities);
        if (AudioSettings.speakerMode != AudioSpeakerMode.Raw &&
            AudioSettings.speakerMode < AudioSettings.driverCapabilities)
        {
            _channelCount = GetNumChannels(AudioSettings.speakerMode);
        }
        Debug.Log(string.Format("[AudioCapturer] SampleRate: {0}hz SpeakerMode: {1} BestDriverMode: {2} (DSP using {3} buffers of {4} bytes using {5} channels)", AudioSettings.outputSampleRate, AudioSettings.speakerMode.ToString(), AudioSettings.driverCapabilities.ToString(), bufferCount, bufferLength, _channelCount));
#else
        _numChannels = GetNumChannels(AudioSettings.driverCapabilities);
		if (AudioSettings.speakerMode != AudioSpeakerMode.Raw &&
            AudioSettings.speakerMode < AudioSettings.driverCapabilities)
		{
			_numChannels = GetNumChannels(AudioSettings.speakerMode);
		}

        Debug.Log(string.Format("[AVProUnityAudiocapture] SampleRate: {0}hz SpeakerMode: {1} BestDriverMode: {2} (DSP using {3} buffers of {4} bytes using {5} channels)", AudioSettings.outputSampleRate, AudioSettings.speakerMode.ToString(), AudioSettings.driverCapabilities.ToString(), numBuffers, bufferLength, _numChannels));
#endif

        _buffer = new float[bufferLength * _channelCount * bufferCount * BufferSize];
        _readBuffer = new float[bufferLength * _channelCount * bufferCount * BufferSize];
        _bufferIndex = 0;
        _bufferHandle = GCHandle.Alloc(_readBuffer, GCHandleType.Pinned);
    }

    public void Deinit()
    {
        lock (_lockObject)
        {
            _bufferIndex = 0;

            if (_bufferHandle.IsAllocated)
            {
                _bufferHandle.Free();
            }
            _readBuffer = _buffer = null;
        }

        _channelCount = 0;
    }

    public System.IntPtr ReadData(out int length)
    {
        System.IntPtr ptr = IntPtr.Zero;

        lock (_lockObject)
        {
            length = 0;

            if (_buffer != null && _readBuffer != null)
            {
                System.Array.Copy(_buffer, 0, _readBuffer, 0, _bufferIndex);
                length = _bufferIndex;
                _bufferIndex = 0;

                ptr = _bufferHandle.AddrOfPinnedObject();
            }
        }

        return ptr;
    }

    public void FlushBuffer()
    {
        lock (_lockObject)
        {
            _bufferIndex = 0;
        }
    }

    public void OnAudioFilterRead(float[] data, int channels)
    {
        if (_buffer != null)
        {
            // TODO: use double buffering
            lock (_lockObject)
            {
                int length = Mathf.Min(data.Length, _buffer.Length - _bufferIndex);

                //System.Array.Copy(data, 0, _buffer, _bufferIndex, length);
                for (int i = 0; i < length; i++)
                {
                    _buffer[i + _bufferIndex] = data[i];
                }
                _bufferIndex += length;

                if (length < data.Length)
                {
                    Debug.LogWarning("[AudioCapturer] Audio buffer overflow, may cause sync issues.  Disable this component if not recording Unity audio.");
                }
            }
        }
    }

    public int ChannelCount 
    {
        get { return _channelCount; } 
    }

    #endregion

    #region Private

    private static int GetNumChannels(AudioSpeakerMode mode)
    {
        int result = 0;
        switch (mode)
        {
            case AudioSpeakerMode.Raw:
                break;
            case AudioSpeakerMode.Mono:
                result = 1;
                break;
            case AudioSpeakerMode.Stereo:
                result = 2;
                break;
            case AudioSpeakerMode.Quad:
                result = 4;
                break;
            case AudioSpeakerMode.Surround:
                result = 5;
                break;
            case AudioSpeakerMode.Mode5point1:
                result = 6;
                break;
            case AudioSpeakerMode.Mode7point1:
                result = 8;
                break;
            case AudioSpeakerMode.Prologic:
                result = 2;
                break;
        }
        return result;
    }

    #endregion
}
