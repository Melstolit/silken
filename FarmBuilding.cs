using System.Numerics;

namespace MySilkProgram;

/// <summary>
/// Static class for generating procedural farm building geometry
/// </summary>
public static class FarmBuilding
{
    /// <summary>
    /// Creates a simple farm house mesh with a cube base and triangular roof
    /// </summary>
    /// <param name="width">Width of the building</param>
    /// <param name="height">Height of the building walls</param>
    /// <param name="depth">Depth of the building</param>
    /// <param name="roofHeight">Height of the triangular roof</param>
    /// <returns>Mesh representing the farm building</returns>
    public static Mesh CreateFarmHouse(float width = 4.0f, float height = 3.0f, float depth = 4.0f, float roofHeight = 2.0f)
    {
        // Validate input parameters
        if (width <= 0) throw new ArgumentException("Width must be positive", nameof(width));
        if (height <= 0) throw new ArgumentException("Height must be positive", nameof(height));
        if (depth <= 0) throw new ArgumentException("Depth must be positive", nameof(depth));
        if (roofHeight < 0) throw new ArgumentException("Roof height cannot be negative", nameof(roofHeight));
        
        var vertices = new List<Vertex>();
        var indices = new List<uint>();
        
        // Generate cube base for the house
        GenerateCubeBase(vertices, indices, width, height, depth);
        
        // Generate triangular roof
        GenerateTriangularRoof(vertices, indices, width, height, depth, roofHeight);
        
        Console.WriteLine($"Generated farm house with {vertices.Count} vertices and {indices.Count / 3} triangles");
        Console.WriteLine($"Dimensions: {width}x{height}x{depth}, Roof height: {roofHeight}");
        
        return new Mesh(vertices.ToArray(), indices.ToArray());
    }
    
    /// <summary>
    /// Generates the cube base of the farm house
    /// </summary>
    private static void GenerateCubeBase(List<Vertex> vertices, List<uint> indices, float width, float height, float depth)
    {
        float halfWidth = width * 0.5f;
        float halfDepth = depth * 0.5f;
        
        uint baseIndex = (uint)vertices.Count;
        
        // Define the 8 corners of the cube
        Vector3[] corners = new Vector3[]
        {
            // Bottom face (y = 0)
            new Vector3(-halfWidth, 0, -halfDepth), // 0: front-left-bottom
            new Vector3(halfWidth, 0, -halfDepth),  // 1: front-right-bottom
            new Vector3(halfWidth, 0, halfDepth),   // 2: back-right-bottom
            new Vector3(-halfWidth, 0, halfDepth),  // 3: back-left-bottom
            
            // Top face (y = height)
            new Vector3(-halfWidth, height, -halfDepth), // 4: front-left-top
            new Vector3(halfWidth, height, -halfDepth),  // 5: front-right-top
            new Vector3(halfWidth, height, halfDepth),   // 6: back-right-top
            new Vector3(-halfWidth, height, halfDepth)   // 7: back-left-top
        };
        
        // Generate vertices for each face with proper normals and texture coordinates
        
        // Bottom face (y = 0) - normal pointing down
        Vector3 bottomNormal = new Vector3(0, -1, 0);
        vertices.Add(new Vertex(corners[0], bottomNormal, new Vector2(0, 1))); // 0
        vertices.Add(new Vertex(corners[1], bottomNormal, new Vector2(1, 1))); // 1
        vertices.Add(new Vertex(corners[2], bottomNormal, new Vector2(1, 0))); // 2
        vertices.Add(new Vertex(corners[3], bottomNormal, new Vector2(0, 0))); // 3
        
        // Top face (y = height) - normal pointing up
        Vector3 topNormal = new Vector3(0, 1, 0);
        vertices.Add(new Vertex(corners[4], topNormal, new Vector2(0, 0))); // 4
        vertices.Add(new Vertex(corners[5], topNormal, new Vector2(1, 0))); // 5
        vertices.Add(new Vertex(corners[6], topNormal, new Vector2(1, 1))); // 6
        vertices.Add(new Vertex(corners[7], topNormal, new Vector2(0, 1))); // 7
        
        // Front face (z = -halfDepth) - normal pointing forward
        Vector3 frontNormal = new Vector3(0, 0, -1);
        vertices.Add(new Vertex(corners[0], frontNormal, new Vector2(0, 0))); // 8
        vertices.Add(new Vertex(corners[1], frontNormal, new Vector2(1, 0))); // 9
        vertices.Add(new Vertex(corners[5], frontNormal, new Vector2(1, 1))); // 10
        vertices.Add(new Vertex(corners[4], frontNormal, new Vector2(0, 1))); // 11
        
        // Back face (z = halfDepth) - normal pointing backward
        Vector3 backNormal = new Vector3(0, 0, 1);
        vertices.Add(new Vertex(corners[2], backNormal, new Vector2(0, 0))); // 12
        vertices.Add(new Vertex(corners[3], backNormal, new Vector2(1, 0))); // 13
        vertices.Add(new Vertex(corners[7], backNormal, new Vector2(1, 1))); // 14
        vertices.Add(new Vertex(corners[6], backNormal, new Vector2(0, 1))); // 15
        
        // Left face (x = -halfWidth) - normal pointing left
        Vector3 leftNormal = new Vector3(-1, 0, 0);
        vertices.Add(new Vertex(corners[3], leftNormal, new Vector2(0, 0))); // 16
        vertices.Add(new Vertex(corners[0], leftNormal, new Vector2(1, 0))); // 17
        vertices.Add(new Vertex(corners[4], leftNormal, new Vector2(1, 1))); // 18
        vertices.Add(new Vertex(corners[7], leftNormal, new Vector2(0, 1))); // 19
        
        // Right face (x = halfWidth) - normal pointing right
        Vector3 rightNormal = new Vector3(1, 0, 0);
        vertices.Add(new Vertex(corners[1], rightNormal, new Vector2(0, 0))); // 20
        vertices.Add(new Vertex(corners[2], rightNormal, new Vector2(1, 0))); // 21
        vertices.Add(new Vertex(corners[6], rightNormal, new Vector2(1, 1))); // 22
        vertices.Add(new Vertex(corners[5], rightNormal, new Vector2(0, 1))); // 23
        
        // Generate indices for each face (2 triangles per face)
        uint[] faceIndices = new uint[]
        {
            // Bottom face (clockwise when viewed from below)
            baseIndex + 0, baseIndex + 2, baseIndex + 1,
            baseIndex + 0, baseIndex + 3, baseIndex + 2,
            
            // Top face (counter-clockwise when viewed from above)
            baseIndex + 4, baseIndex + 5, baseIndex + 6,
            baseIndex + 4, baseIndex + 6, baseIndex + 7,
            
            // Front face
            baseIndex + 8, baseIndex + 9, baseIndex + 10,
            baseIndex + 8, baseIndex + 10, baseIndex + 11,
            
            // Back face
            baseIndex + 12, baseIndex + 14, baseIndex + 13,
            baseIndex + 12, baseIndex + 15, baseIndex + 14,
            
            // Left face
            baseIndex + 16, baseIndex + 17, baseIndex + 18,
            baseIndex + 16, baseIndex + 18, baseIndex + 19,
            
            // Right face
            baseIndex + 20, baseIndex + 22, baseIndex + 21,
            baseIndex + 20, baseIndex + 23, baseIndex + 22
        };
        
        indices.AddRange(faceIndices);
        
        Console.WriteLine($"Generated cube base: {corners.Length} corners, {vertices.Count - baseIndex} vertices, {faceIndices.Length / 3} triangles");
    }
    
    /// <summary>
    /// Generates a triangular roof on top of the cube base
    /// </summary>
    private static void GenerateTriangularRoof(List<Vertex> vertices, List<uint> indices, float width, float height, float depth, float roofHeight)
    {
        float halfWidth = width * 0.5f;
        float halfDepth = depth * 0.5f;
        float roofTop = height + roofHeight;
        
        uint baseIndex = (uint)vertices.Count;
        
        // Define roof vertices
        Vector3[] roofVertices = new Vector3[]
        {
            // Ridge line (top of the roof)
            new Vector3(-halfWidth, roofTop, 0), // 0: left ridge
            new Vector3(halfWidth, roofTop, 0),  // 1: right ridge
            
            // Base of roof (connects to house top)
            new Vector3(-halfWidth, height, -halfDepth), // 2: front-left
            new Vector3(halfWidth, height, -halfDepth),  // 3: front-right
            new Vector3(halfWidth, height, halfDepth),   // 4: back-right
            new Vector3(-halfWidth, height, halfDepth)   // 5: back-left
        };
        
        // Calculate normals for roof faces
        Vector3 frontRoofNormal = CalculateTriangleNormal(roofVertices[2], roofVertices[0], roofVertices[3]);
        Vector3 backRoofNormal = CalculateTriangleNormal(roofVertices[4], roofVertices[1], roofVertices[5]);
        Vector3 leftTriangleNormal = CalculateTriangleNormal(roofVertices[5], roofVertices[0], roofVertices[2]);
        Vector3 rightTriangleNormal = CalculateTriangleNormal(roofVertices[3], roofVertices[1], roofVertices[4]);
        
        // Front roof face
        vertices.Add(new Vertex(roofVertices[2], frontRoofNormal, new Vector2(0, 0))); // 0
        vertices.Add(new Vertex(roofVertices[0], frontRoofNormal, new Vector2(0.5f, 1))); // 1
        vertices.Add(new Vertex(roofVertices[3], frontRoofNormal, new Vector2(1, 0))); // 2
        vertices.Add(new Vertex(roofVertices[1], frontRoofNormal, new Vector2(0.5f, 1))); // 3
        
        // Back roof face
        vertices.Add(new Vertex(roofVertices[4], backRoofNormal, new Vector2(0, 0))); // 4
        vertices.Add(new Vertex(roofVertices[1], backRoofNormal, new Vector2(0.5f, 1))); // 5
        vertices.Add(new Vertex(roofVertices[5], backRoofNormal, new Vector2(1, 0))); // 6
        vertices.Add(new Vertex(roofVertices[0], backRoofNormal, new Vector2(0.5f, 1))); // 7
        
        // Left triangle (gable end)
        vertices.Add(new Vertex(roofVertices[5], leftTriangleNormal, new Vector2(0, 0))); // 8
        vertices.Add(new Vertex(roofVertices[0], leftTriangleNormal, new Vector2(0.5f, 1))); // 9
        vertices.Add(new Vertex(roofVertices[2], leftTriangleNormal, new Vector2(1, 0))); // 10
        
        // Right triangle (gable end)
        vertices.Add(new Vertex(roofVertices[3], rightTriangleNormal, new Vector2(0, 0))); // 11
        vertices.Add(new Vertex(roofVertices[1], rightTriangleNormal, new Vector2(0.5f, 1))); // 12
        vertices.Add(new Vertex(roofVertices[4], rightTriangleNormal, new Vector2(1, 0))); // 13
        
        // Generate indices for roof faces
        uint[] roofIndices = new uint[]
        {
            // Front roof face (2 triangles)
            baseIndex + 0, baseIndex + 1, baseIndex + 2,
            baseIndex + 1, baseIndex + 3, baseIndex + 2,
            
            // Back roof face (2 triangles)
            baseIndex + 4, baseIndex + 5, baseIndex + 6,
            baseIndex + 5, baseIndex + 7, baseIndex + 6,
            
            // Left gable triangle
            baseIndex + 8, baseIndex + 9, baseIndex + 10,
            
            // Right gable triangle
            baseIndex + 11, baseIndex + 12, baseIndex + 13
        };
        
        indices.AddRange(roofIndices);
        
        Console.WriteLine($"Generated triangular roof: {roofVertices.Length} vertices, {roofIndices.Length / 3} triangles");
    }
    
    /// <summary>
    /// Calculates the normal vector for a triangle defined by three vertices
    /// </summary>
    private static Vector3 CalculateTriangleNormal(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        Vector3 edge1 = v2 - v1;
        Vector3 edge2 = v3 - v1;
        Vector3 normal = Vector3.Cross(edge1, edge2);
        return Vector3.Normalize(normal);
    }
    
    /// <summary>
    /// Creates a simple barn mesh with a larger rectangular base
    /// </summary>
    /// <param name="width">Width of the barn</param>
    /// <param name="height">Height of the barn walls</param>
    /// <param name="depth">Depth of the barn</param>
    /// <param name="roofHeight">Height of the barn roof</param>
    /// <returns>Mesh representing the barn</returns>
    public static Mesh CreateBarn(float width = 6.0f, float height = 4.0f, float depth = 8.0f, float roofHeight = 3.0f)
    {
        Console.WriteLine($"Creating barn with dimensions: {width}x{height}x{depth}, roof height: {roofHeight}");
        
        // Use the same generation logic as the farm house but with different proportions
        return CreateFarmHouse(width, height, depth, roofHeight);
    }
    
    /// <summary>
    /// Creates a small shed mesh
    /// </summary>
    /// <param name="width">Width of the shed</param>
    /// <param name="height">Height of the shed walls</param>
    /// <param name="depth">Depth of the shed</param>
    /// <param name="roofHeight">Height of the shed roof</param>
    /// <returns>Mesh representing the shed</returns>
    public static Mesh CreateShed(float width = 2.0f, float height = 2.0f, float depth = 3.0f, float roofHeight = 1.0f)
    {
        Console.WriteLine($"Creating shed with dimensions: {width}x{height}x{depth}, roof height: {roofHeight}");
        
        // Use the same generation logic as the farm house but with smaller proportions
        return CreateFarmHouse(width, height, depth, roofHeight);
    }
    
    /// <summary>
    /// Creates a complete farm building GameObject with appropriate material
    /// </summary>
    /// <param name="name">Name of the game object</param>
    /// <param name="buildingType">Type of building to create</param>
    /// <param name="position">Position in the scene</param>
    /// <returns>GameObject representing the farm building</returns>
    public static GameObject CreateFarmBuildingObject(string name, FarmBuildingType buildingType, Vector3 position)
    {
        Mesh mesh = buildingType switch
        {
            FarmBuildingType.House => CreateFarmHouse(),
            FarmBuildingType.Barn => CreateBarn(),
            FarmBuildingType.Shed => CreateShed(),
            _ => CreateFarmHouse()
        };
        
        // Create wood material for the building
        var material = Material.CreateWoodMaterial();
        
        // Create the game object
        var gameObject = new GameObject(name, mesh, material);
        gameObject.SetPosition(position);
        
        Console.WriteLine($"Created {buildingType} GameObject '{name}' at position {position}");
        
        return gameObject;
    }
}

/// <summary>
/// Types of farm buildings that can be generated
/// </summary>
public enum FarmBuildingType
{
    House,
    Barn,
    Shed
}