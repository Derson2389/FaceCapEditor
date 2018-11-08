using System;
using System.IO;
using UnityEngine;

namespace Slate
{
    [ExecuteInEditMode]
    public class WAVRecorder : MonoBehaviour
    {
        #region Data

        private FileStream _filestream = null;
        private int _framerate = 24;
        private string _targetFilePath = string.Empty;
        private string _targetFileName = string.Empty;
        private int _channels = 2;
        private int _sampleRate = 48000;
        private const int WAV_HEADER_SIZE = 44; //default for uncompressed wav

        #endregion

        #region Public

        public bool StartRecording()
        {
            _sampleRate = AudioSettings.outputSampleRate;
            _filestream = GenerateAudioFileStream();
            return (_filestream != null);
        }

        public void StopRecording()
        {
            if(_filestream != null)
            {
                WriteHeader(_filestream);
            }
            
            _filestream = null;
        }

        public int FrameRate
        {
            set { _framerate = value; }
            get { return _framerate; }
        }

        public string TargetFilePath
        {
            set { _targetFilePath = value; }
            get { return _targetFilePath; }
        }

        public string TargetFileName
        {
            set { _targetFileName = value; }
            get { return _targetFileName; }
        }

        #endregion

        #region Private

        private string GenerateWAVFilename()
        {
            string projectFolder = System.IO.Path.GetFullPath(System.IO.Path.Combine(Application.dataPath, ".."));
            string fileFolder = System.IO.Path.Combine(projectFolder, _targetFilePath);
            string filename = System.IO.Path.Combine(_targetFilePath, _targetFileName + ".wav");

            return filename;
        }

        private FileStream GenerateAudioFileStream()
        {
            string name = GenerateWAVFilename();
            if (File.Exists(name))
            {
                File.Delete(name);
            }

            FileStream fileStream = new FileStream(name, FileMode.CreateNew);
            byte emptyByte = new byte();

            for (int i = 0; i < WAV_HEADER_SIZE; i++) //preparing the header  
            {
                fileStream.WriteByte(emptyByte);
            }

            return fileStream;
        }

        void OnAudioFilterRead(float[] data, int channels)
        {
            _channels = channels;
            if (_filestream != null)
            {
                ConvertAndWrite(_filestream, data); //audio data is interlaced  
            }
        }

        private void ConvertAndWrite(FileStream fileStream, float[] dataSource)
        {
            try
            {
                Int16[] intData = new Int16[dataSource.Length];
                //converting in 2 steps : float[] to Int16[], //then Int16[] to Byte[]  

                Byte[] bytesData = new Byte[dataSource.Length * 2];
                //bytesData array is twice the size of  
                //dataSource array because a float converted in Int16 is 2 bytes.  

                int rescaleFactor = 32767; //to convert float to Int16  

                for (int i = 0; i < dataSource.Length; i++)
                {
                    intData[i] = (short)(dataSource[i] * rescaleFactor);
                    Byte[] byteArr = new Byte[2];
                    byteArr = BitConverter.GetBytes(intData[i]);
                    byteArr.CopyTo(bytesData, i * 2);
                }

                fileStream.Write(bytesData, 0, bytesData.Length);
            }
            catch (Exception ex)
            {

            }
        }

        private void WriteHeader(FileStream fileStream)
        {
            fileStream.Seek(0, SeekOrigin.Begin);

            Byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
            fileStream.Write(riff, 0, 4);

            Byte[] chunkSize = BitConverter.GetBytes(fileStream.Length - 8);
            fileStream.Write(chunkSize, 0, 4);

            Byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
            fileStream.Write(wave, 0, 4);

            Byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
            fileStream.Write(fmt, 0, 4);

            Byte[] subChunk1 = BitConverter.GetBytes(16);
            fileStream.Write(subChunk1, 0, 4);

            UInt16 one = 1;
            Byte[] audioFormat = BitConverter.GetBytes(one);
            fileStream.Write(audioFormat, 0, 2);

            UInt16 channels = (UInt16)_channels;
            Byte[] numChannels = BitConverter.GetBytes(channels);
            fileStream.Write(numChannels, 0, 2);

            Byte[] sampleRate = BitConverter.GetBytes(_sampleRate);
            fileStream.Write(sampleRate, 0, 4);

            Byte[] byteRate = BitConverter.GetBytes(_sampleRate * channels * 2);// sampleRate * bytesPerSample * number of channels
            fileStream.Write(byteRate, 0, 4);

            UInt16 four = 4;
            Byte[] blockAlign = BitConverter.GetBytes(four);
            fileStream.Write(blockAlign, 0, 2);

            UInt16 sixteen = 16;
            Byte[] bitsPerSample = BitConverter.GetBytes(sixteen);
            fileStream.Write(bitsPerSample, 0, 2);

            Byte[] dataString = System.Text.Encoding.UTF8.GetBytes("data");
            fileStream.Write(dataString, 0, 4);

            Byte[] subChunk2 = BitConverter.GetBytes(fileStream.Length - WAV_HEADER_SIZE);
            fileStream.Write(subChunk2, 0, 4);

            fileStream.Close();
        }

        #endregion
    }
}