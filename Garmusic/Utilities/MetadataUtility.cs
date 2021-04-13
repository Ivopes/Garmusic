using ATL;
using Garmusic.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Garmusic.Utilities
{
    public static class MetadataUtility
    {
        /// <summary>
        /// Fills song object with metadata from stream
        /// </summary>
        /// <param name="song">Song to be filled</param>
        /// <param name="stream">Stream containing loaded audio file</param>
        public static void FillMetadata(Song song, Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);

            Track track = new Track(stream, ".mp3");

            song.Name = track.Title != string.Empty ? track.Title : song.FileName[..song.FileName.LastIndexOf('.')];
            song.LengthSec = track.Duration;
            song.Author = string.IsNullOrWhiteSpace(track.Artist) ? string.Empty : track.Artist;
        }
    }
}
