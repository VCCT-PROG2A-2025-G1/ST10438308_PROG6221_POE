using System.Media;
using NAudio.Wave;
using System.Threading;
using System.Threading.Tasks;

namespace CyberSecurityChatbot
{
    public static class AudioPlayer
    {
        //--------------------------------AUDIO PLAYER--------------------------------------//
        // This method plays a greeting audio file asynchronously
        public static async Task PlayGreetingAudioAsync()
        {
            string audioFilePath = "chatbotgreetings.wav";

            await Task.Run(() =>
            {
                using (var audioFile = new AudioFileReader(audioFilePath))
                using (var outputDevice = new WaveOutEvent())
                {
                    outputDevice.Init(audioFile);
                    outputDevice.Play();
                    while (outputDevice.PlaybackState == PlaybackState.Playing)
                    {
                        Thread.Sleep(100);
                    }
                }
            });
        }
    }
}
//-----------------------------------------------------------------------------------------------END OF AUDIO PLAYER-------------------------------------------------------------------------//