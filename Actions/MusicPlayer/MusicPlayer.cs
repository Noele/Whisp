﻿using System.Diagnostics;
using NAudio.Wave;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Videos.Streams;

namespace Whisp.Actions.MusicPlayer;

public class MusicPlayer
{
    
    private YoutubeClient _youtubeClient;
    private WaveOutEvent _waveOutEvent;
    private bool _shouldStop = false;
    private Speaker _speaker;
    public MusicPlayer(Speaker speaker)
    {
        this._youtubeClient = new YoutubeClient();
        this._waveOutEvent = new WaveOutEvent();
        this._speaker = speaker;
    }

    public async Task Play(string searchQuery)
    {
        var searchResults = await this._youtubeClient.Search.GetVideosAsync(searchQuery);
        var video = searchResults.FirstOrDefault();

        if (video == null)
        {
            Console.WriteLine("No video found for the search query.");
            await this._speaker.Speak("No video found for the search query.");
            return;
        }

        Console.WriteLine($"Found video: {video.Title}");

        var streamManifest = await this._youtubeClient.Videos.Streams.GetManifestAsync(video.Id);
        var audioStreamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();

        if (audioStreamInfo == null)
        {
            Console.WriteLine("No audio stream found.");
            await this._speaker.Speak("There was an error trying to find that song");
            return;
        }

        await this._speaker.Speak($"Now playing {video.Title} by {video.Author}");

        // Save the audio stream to a temporary file
        var tempFilePath = Path.GetTempFileName();
        await using (var fileStream = File.Create(tempFilePath))
        {
            await this._youtubeClient.Videos.Streams.CopyToAsync(audioStreamInfo, fileStream);
        }

        // Play the audio
        using var reader = new MediaFoundationReader(tempFilePath);
        this._waveOutEvent.Init(reader);
        this._waveOutEvent.Play();

        // Wait for playback to finish
        while (this._waveOutEvent.PlaybackState == PlaybackState.Playing && !this._shouldStop)
        {
            await Task.Delay(250);
        }

        this._waveOutEvent.Stop();
        this._shouldStop = false;

        // Clean up the temporary file
        File.Delete(tempFilePath);
    }

    public async Task Stop()
    {
        if(this._waveOutEvent.PlaybackState == PlaybackState.Playing) {
            this._shouldStop = true;
            await this._speaker.Speak("Stopping playback");
        } else
        {
            await this._speaker.Speak("Nothing is currently playing");
        }
    }
}