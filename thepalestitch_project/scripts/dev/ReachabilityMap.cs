using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace ThePaleStitch.Dev;

/// <summary>
/// Which ground can the player reach on foot from a start point? Bakes a
/// navigation mesh sized for the player from the static colliders under
/// <c>root</c>, on a private navigation map, and walks a grid over it.
///
/// Used to enforce the level design rule "no shortcut around the challenge"
/// (design doc §5.1, CR-021): every level states the area it expects.
/// </summary>
public sealed class ReachabilityMap
{
    public const float Cell = 0.5f;

    public float AreaSquareMetres { get; private set; }
    public Image Picture { get; private set; }

    private Rid _map;
    private Rid _region;

    public async Task Build(Node host, Node3D root, Vector3 start, Rect2 bounds)
    {
        var mesh = new NavigationMesh
        {
            GeometryParsedGeometryType = NavigationMesh.ParsedGeometryType.StaticColliders,
            // Slightly thinner than the player (0.35 m): a gap the player can
            // only just squeeze through must still show up.
            AgentRadius = 0.25f,
            AgentHeight = 1.75f,
            AgentMaxClimb = 0.25f,
            CellSize = 0.25f,
            CellHeight = 0.25f,
        };
        var source = new NavigationMeshSourceGeometryData3D();
        NavigationServer3D.ParseSourceGeometryData(mesh, source, root);
        NavigationServer3D.BakeFromSourceGeometryData(mesh, source);

        _map = NavigationServer3D.MapCreate();
        NavigationServer3D.MapSetCellSize(_map, mesh.CellSize);
        NavigationServer3D.MapSetCellHeight(_map, mesh.CellHeight);
        NavigationServer3D.MapSetActive(_map, true);
        _region = NavigationServer3D.RegionCreate();
        NavigationServer3D.RegionSetMap(_region, _map);
        NavigationServer3D.RegionSetNavigationMesh(_region, mesh);
        // The map synchronises on the physics step: wait until the start point is on it.
        for (int i = 0; i < 240 && (NavigationServer3D.MapGetIterationId(_map) == 0 || !OnGround(start, out _)); i++)
            await host.ToSignal(host.GetTree(), SceneTree.SignalName.PhysicsFrame);

        int w = (int)(bounds.Size.X / Cell), h = (int)(bounds.Size.Y / Cell);
        Picture = Image.CreateEmpty(w, h, false, Image.Format.Rgb8);
        int reachable = 0;
        for (int zi = 0; zi < h; zi++)
        for (int xi = 0; xi < w; xi++)
        {
            var p = new Vector3(bounds.Position.X + (xi + 0.5f) * Cell, 0, bounds.Position.Y + (zi + 0.5f) * Cell);
            Color colour = new(0.08f, 0.08f, 0.09f);
            if (OnGround(p, out Vector3 q))
            {
                if (Reaches(start, q)) { colour = new Color(0.85f, 0.8f, 0.6f); reachable++; }
                else colour = new Color(0.35f, 0.15f, 0.15f);
            }
            Picture.SetPixel(xi, zi, colour);
        }
        int sx = (int)((start.X - bounds.Position.X) / Cell), sz = (int)((start.Z - bounds.Position.Y) / Cell);
        for (int dz = -2; dz <= 2; dz++)
        for (int dx = -2; dx <= 2; dx++)
            Picture.SetPixel(Mathf.Clamp(sx + dx, 0, w - 1), Mathf.Clamp(sz + dz, 0, h - 1), new Color(0.2f, 0.6f, 1f));
        Picture.Resize(w * 6, h * 6, Image.Interpolation.Nearest);
        AreaSquareMetres = reachable * Cell * Cell;
    }

    /// <summary>Is the ground point <paramref name="p"/> reachable from <paramref name="start"/>?</summary>
    public bool CanReach(Vector3 start, Vector3 p) => OnGround(p, out Vector3 q) && Reaches(start, q);

    public void Free()
    {
        if (_region.IsValid) NavigationServer3D.FreeRid(_region);
        if (_map.IsValid) NavigationServer3D.FreeRid(_map);
    }

    private bool OnGround(Vector3 p, out Vector3 q)
    {
        q = NavigationServer3D.MapGetClosestPoint(_map, p);
        return new Vector2(q.X - p.X, q.Z - p.Z).Length() < 0.4f && q.Y < 0.8f;
    }

    private bool Reaches(Vector3 start, Vector3 q)
    {
        Vector3[] path = NavigationServer3D.MapGetPath(_map, start, q, true);
        return path.Length > 0 && path[^1].DistanceTo(q) < 0.3f;
    }
}
