using System.Numerics;

namespace MySilkProgram;

/// <summary>
/// Static class for generating procedural corn field terrain and corn stalk objects
/// </summary>
public static class CornField
{
    /// <summary>
    /// Creates a ground plane mesh for the corn field
    /// </summary>
    /// <param name="width">Width of the field</param>
    /// <param name="depth">Depth of the field</param>
    /// <param name="subdivisions">Number of subdivisions for more detailed terrain (minimum 1)</param>
    /// <returns>Mesh representing the ground plane</returns>
    public static Mesh CreateGroundPlane(float width = 20.0f, float depth = 20.0f, int subdivisions = 4)
    {
        if (width <= 0) throw new ArgumentException("Width must be positive", nameof(width));
        if (depth <= 0) throw new ArgumentException("Depth must be positive", nameof(depth));
        if (subdivisions < 1) throw new ArgumentException("Subdivisions must be at least 1", nameof(subdivisions));
        
        var vertices = new List<Vertex>();
        var indices = new List<uint>();
        
        float halfWidth = width * 0.5f;
        float halfDepth = depth * 0.5f;
        
        // Generate vertices in a grid
        for (int z = 0; z <= subdivisions; z++)
        {
            for (int x = 0; x <= subdivisions; x++)
            {
                float xPos = -halfWidth + (x / (float)subdivisions) * width;
                float zPos = -halfDepth + (z / (float)subdivisions) * depth;
                float yPos = 0.0f; // Flat ground for now
                
                Vector3 position = new Vector3(xPos, yPos, zPos);
                Vector3 normal = new Vector3(0, 1, 0); // Up-facing normal
                Vector2 texCoords = new Vector2(x / (float)subdivisions, z / (float)subdivisions);
                
                vertices.Add(new Vertex(position, normal, texCoords));
            }
        }
        
        // Generate indices for triangles
        for (int z = 0; z < subdivisions; z++)
        {
            for (int x = 0; x < subdivisions; x++)
            {
                uint topLeft = (uint)(z * (subdivisions + 1) + x);
                uint topRight = (uint)(z * (subdivisions + 1) + x + 1);
                uint bottomLeft = (uint)((z + 1) * (subdivisions + 1) + x);
                uint bottomRight = (uint)((z + 1) * (subdivisions + 1) + x + 1);
                
                // First triangle (top-left, bottom-left, top-right)
                indices.Add(topLeft);
                indices.Add(bottomLeft);
                indices.Add(topRight);
                
                // Second triangle (top-right, bottom-left, bottom-right)
                indices.Add(topRight);
                indices.Add(bottomLeft);
                indices.Add(bottomRight);
            }
        }
        
        Console.WriteLine($"Generated ground plane: {width}x{depth} with {subdivisions}x{subdivisions} subdivisions");
        Console.WriteLine($"Vertices: {vertices.Count}, Triangles: {indices.Count / 3}");
        
        return new Mesh(vertices.ToArray(), indices.ToArray());
    }
    
    /// <summary>
    /// Creates a simple corn stalk mesh
    /// </summary>
    /// <param name="height">Height of the corn stalk</param>
    /// <param name="width">Width of the corn stalk</param>
    /// <returns>Mesh representing a corn stalk</returns>
    public static Mesh CreateCornStalk(float height = 2.5f, float width = 0.3f)
    {
        if (height <= 0) throw new ArgumentException("Height must be positive", nameof(height));
        if (width <= 0) throw new ArgumentException("Width must be positive", nameof(width));
        
        var vertices = new List<Vertex>();
        var indices = new List<uint>();
        
        float halfWidth = width * 0.5f;
        
        // Create a simple cross-shaped corn stalk (two intersecting quads)
        // This gives a 3D appearance while being efficient
        
        // First quad (facing X axis)
        CreateCornStalkQuad(vertices, indices, 
            new Vector3(-halfWidth, 0, 0), new Vector3(halfWidth, 0, 0),
            new Vector3(halfWidth, height, 0), new Vector3(-halfWidth, height, 0),
            new Vector3(0, 0, 1)); // Normal facing forward
        
        // Second quad (facing Z axis, rotated 90 degrees)
        CreateCornStalkQuad(vertices, indices,
            new Vector3(0, 0, -halfWidth), new Vector3(0, 0, halfWidth),
            new Vector3(0, height, halfWidth), new Vector3(0, height, -halfWidth),
            new Vector3(1, 0, 0)); // Normal facing right
        
        Console.WriteLine($"Generated corn stalk: height {height}, width {width}");
        Console.WriteLine($"Vertices: {vertices.Count}, Triangles: {indices.Count / 3}");
        
        return new Mesh(vertices.ToArray(), indices.ToArray());
    }
    
    /// <summary>
    /// Creates a single quad for the corn stalk
    /// </summary>
    private static void CreateCornStalkQuad(List<Vertex> vertices, List<uint> indices,
        Vector3 bottomLeft, Vector3 bottomRight, Vector3 topRight, Vector3 topLeft, Vector3 normal)
    {
        uint baseIndex = (uint)vertices.Count;
        
        // Add vertices for the quad
        vertices.Add(new Vertex(bottomLeft, normal, new Vector2(0, 0)));
        vertices.Add(new Vertex(bottomRight, normal, new Vector2(1, 0)));
        vertices.Add(new Vertex(topRight, normal, new Vector2(1, 1)));
        vertices.Add(new Vertex(topLeft, normal, new Vector2(0, 1)));
        
        // Add indices for two triangles
        indices.Add(baseIndex + 0); // Bottom-left
        indices.Add(baseIndex + 1); // Bottom-right
        indices.Add(baseIndex + 2); // Top-right
        
        indices.Add(baseIndex + 0); // Bottom-left
        indices.Add(baseIndex + 2); // Top-right
        indices.Add(baseIndex + 3); // Top-left
    }
    
    /// <summary>
    /// Creates multiple corn stalk game objects arranged in a grid pattern
    /// </summary>
    /// <param name="fieldWidth">Width of the corn field</param>
    /// <param name="fieldDepth">Depth of the corn field</param>
    /// <param name="spacing">Spacing between corn stalks</param>
    /// <param name="cornMaterial">Material to apply to corn stalks</param>
    /// <returns>List of corn stalk game objects</returns>
    public static List<GameObject> CreateCornStalks(float fieldWidth = 16.0f, float fieldDepth = 16.0f, 
        float spacing = 2.0f, Material? cornMaterial = null)
    {
        if (fieldWidth <= 0) throw new ArgumentException("Field width must be positive", nameof(fieldWidth));
        if (fieldDepth <= 0) throw new ArgumentException("Field depth must be positive", nameof(fieldDepth));
        if (spacing <= 0) throw new ArgumentException("Spacing must be positive", nameof(spacing));
        
        var cornStalks = new List<GameObject>();
        
        // Create corn material if not provided
        cornMaterial ??= Material.CreateCornMaterial();
        
        // Create a single corn stalk mesh to share among all instances
        var cornStalkMesh = CreateCornStalk();
        
        float halfWidth = fieldWidth * 0.5f;
        float halfDepth = fieldDepth * 0.5f;
        
        int stalksX = (int)(fieldWidth / spacing);
        int stalksZ = (int)(fieldDepth / spacing);
        
        Console.WriteLine($"Creating corn field: {stalksX}x{stalksZ} stalks over {fieldWidth}x{fieldDepth} area");
        
        for (int z = 0; z < stalksZ; z++)
        {
            for (int x = 0; x < stalksX; x++)
            {
                // Calculate position with some randomization for natural look
                float xPos = -halfWidth + (x + 0.5f) * spacing + GetRandomOffset(0.3f);
                float zPos = -halfDepth + (z + 0.5f) * spacing + GetRandomOffset(0.3f);
                float yPos = 0.0f;
                
                Vector3 position = new Vector3(xPos, yPos, zPos);
                
                // Create game object for this corn stalk
                string name = $"CornStalk_{x}_{z}";
                var cornStalk = new GameObject(name, cornStalkMesh, cornMaterial);
                cornStalk.SetPosition(position);
                
                // Add some random rotation for variety
                float randomRotation = GetRandomOffset(MathF.PI * 0.25f); // ±45 degrees
                cornStalk.SetRotation(0, randomRotation, 0);
                
                // Add slight scale variation
                float scaleVariation = 1.0f + GetRandomOffset(0.2f); // ±20%
                cornStalk.SetScale(scaleVariation, scaleVariation, scaleVariation);
                
                cornStalks.Add(cornStalk);
            }
        }
        
        Console.WriteLine($"Created {cornStalks.Count} corn stalks");
        
        return cornStalks;
    }
    
    /// <summary>
    /// Gets a random offset value for natural variation
    /// </summary>
    /// <param name="maxOffset">Maximum offset in either direction</param>
    /// <returns>Random offset between -maxOffset and +maxOffset</returns>
    private static float GetRandomOffset(float maxOffset)
    {
        return (Random.Shared.NextSingle() - 0.5f) * 2.0f * maxOffset;
    }
    
    /// <summary>
    /// Creates a complete corn field scene with ground plane and corn stalks
    /// </summary>
    /// <param name="fieldWidth">Width of the field</param>
    /// <param name="fieldDepth">Depth of the field</param>
    /// <param name="cornSpacing">Spacing between corn stalks</param>
    /// <returns>Tuple containing ground plane and list of corn stalks</returns>
    public static (GameObject groundPlane, List<GameObject> cornStalks) CreateCornFieldScene(
        float fieldWidth = 20.0f, float fieldDepth = 20.0f, float cornSpacing = 2.0f)
    {
        Console.WriteLine($"Creating complete corn field scene: {fieldWidth}x{fieldDepth}");
        
        // Create materials
        var grassMaterial = Material.CreateGrassMaterial();
        var cornMaterial = Material.CreateCornMaterial();
        
        // Create ground plane
        var groundMesh = CreateGroundPlane(fieldWidth, fieldDepth, 8);
        var groundPlane = new GameObject("CornFieldGround", groundMesh, grassMaterial);
        groundPlane.SetPosition(0, -0.05f, 0); // Slightly below origin
        
        // Create corn stalks
        var cornStalks = CreateCornStalks(fieldWidth * 0.8f, fieldDepth * 0.8f, cornSpacing, cornMaterial);
        
        Console.WriteLine("Corn field scene created successfully");
        
        return (groundPlane, cornStalks);
    }
    
    /// <summary>
    /// Creates a simple dirt path mesh through the corn field
    /// </summary>
    /// <param name="pathWidth">Width of the path</param>
    /// <param name="pathLength">Length of the path</param>
    /// <returns>Mesh representing the dirt path</returns>
    public static Mesh CreateDirtPath(float pathWidth = 1.5f, float pathLength = 20.0f)
    {
        if (pathWidth <= 0) throw new ArgumentException("Path width must be positive", nameof(pathWidth));
        if (pathLength <= 0) throw new ArgumentException("Path length must be positive", nameof(pathLength));
        
        var vertices = new List<Vertex>();
        var indices = new List<uint>();
        
        float halfWidth = pathWidth * 0.5f;
        float halfLength = pathLength * 0.5f;
        
        Vector3 normal = new Vector3(0, 1, 0); // Up-facing normal
        
        // Create a simple rectangular path
        vertices.Add(new Vertex(new Vector3(-halfWidth, 0.01f, -halfLength), normal, new Vector2(0, 0)));
        vertices.Add(new Vertex(new Vector3(halfWidth, 0.01f, -halfLength), normal, new Vector2(1, 0)));
        vertices.Add(new Vertex(new Vector3(halfWidth, 0.01f, halfLength), normal, new Vector2(1, 1)));
        vertices.Add(new Vertex(new Vector3(-halfWidth, 0.01f, halfLength), normal, new Vector2(0, 1)));
        
        // Two triangles for the path
        indices.AddRange(new uint[] { 0, 1, 2, 0, 2, 3 });
        
        Console.WriteLine($"Generated dirt path: {pathWidth}x{pathLength}");
        
        return new Mesh(vertices.ToArray(), indices.ToArray());
    }
}