using System.IO;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace MixageSon.Utils
{
    public static class YoutubeDL
    {
        public static Stream GetAudio(string url)
        {
            YoutubeClient client = new();

            string id = url.Split('?')[1];

            id = Uri.UnescapeDataString(id);

            StreamManifest streamManifest = client.Videos.Streams.GetManifestAsync(id).GetAwaiter().GetResult();
            IStreamInfo streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
            Stream videoStream = client.Videos.Streams.GetAsync(streamInfo).GetAwaiter().GetResult();

            return videoStream;
        }
    }
}
