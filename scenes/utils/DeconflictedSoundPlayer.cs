using Godot;
using System;

public static class DeconflictedSoundPlayer
{
    public static AudioStreamPlayer PlayUnconflictingSound(AudioStream stream, Node owner, float volume = 0f, float pitch = 1.0f)
    {
        var audioScript = GD.Load<GDScript>("res://scenes/utils/sfx_deconflicter/deconflicted_audio_stream_player.gd");
        var audio = (AudioStreamPlayer)audioScript.New();

        audio.PitchScale = pitch;
        audio.VolumeDb = volume;
        audio.Bus = "SFX";
        audio.Stream = stream;

        owner.CallDeferred("add_child", audio);
        
        audio.Call("play");    

        return audio; 
    }
}