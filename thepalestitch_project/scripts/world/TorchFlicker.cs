using Godot;

namespace ThePaleStitch.World;

/// <summary>Makes a light breathe like a flame. Attach to an OmniLight3D.</summary>
public partial class TorchFlicker : OmniLight3D
{
    /// <summary>How deep the flame dips, as a fraction of its energy.</summary>
    [Export(PropertyHint.Range, "0,1")] public float Depth = 0.25f;
    [Export] public float Speed = 9f;

    private float _baseEnergy;
    private float _time;
    private FastNoiseLite _noise;

    public override void _Ready()
    {
        _baseEnergy = LightEnergy;
        _noise = new FastNoiseLite { Seed = (int)(GetInstanceId() % 10000), Frequency = 0.35f };
        _time = GD.Randf() * 100f;
    }

    public override void _Process(double delta)
    {
        _time += (float)delta * Speed;
        float n = _noise.GetNoise1D(_time); // -1..1
        LightEnergy = _baseEnergy * (1f - Depth * 0.5f + n * Depth * 0.5f);
    }
}
