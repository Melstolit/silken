using System.Collections.Generic;
using System.Numerics;

namespace MySilkProgram;

/// <summary>
/// Manages the scene hierarchy and handles multiple game objects
/// </summary>
public class SceneManager : IDisposable
{
    private readonly List<GameObject> _gameObjects;
    private readonly Dictionary<string, GameObject> _namedObjects;
    private readonly List<Mesh> _meshes;
    private readonly List<Material> _materials;
    private bool _disposed = false;
    
    /// <summary>
    /// Gets the number of game objects in the scene
    /// </summary>
    public int ObjectCount => _gameObjects.Count;
    
    /// <summary>
    /// Gets the number of active game objects in the scene
    /// </summary>
    public int ActiveObjectCount => _gameObjects.Count(obj => obj.IsActive);
    
    /// <summary>
    /// Gets read-only access to all game objects
    /// </summary>
    public IReadOnlyList<GameObject> GameObjects => _gameObjects.AsReadOnly();
    
    /// <summary>
    /// Gets read-only access to all meshes
    /// </summary>
    public IReadOnlyList<Mesh> Meshes => _meshes.AsReadOnly();
    
    /// <summary>
    /// Gets read-only access to all materials
    /// </summary>
    public IReadOnlyList<Material> Materials => _materials.AsReadOnly();
    
    /// <summary>
    /// Creates a new scene manager
    /// </summary>
    public SceneManager()
    {
        _gameObjects = new List<GameObject>();
        _namedObjects = new Dictionary<string, GameObject>();
        _meshes = new List<Mesh>();
        _materials = new List<Material>();
        
        Console.WriteLine("SceneManager initialized");
    }
    
    /// <summary>
    /// Loads the default farm scene
    /// </summary>
    public void LoadScene()
    {
        Console.WriteLine("Loading farm scene...");
        
        try
        {
            // Clear existing scene
            ClearScene();
            
            // Create materials
            CreateSceneMaterials();
            
            // Create meshes (will be implemented in subsequent tasks)
            CreateSceneMeshes();
            
            // Create game objects (will be implemented in subsequent tasks)
            CreateSceneObjects();
            
            Console.WriteLine($"Farm scene loaded successfully with {ObjectCount} objects");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load scene: {ex.Message}");
            throw;
        }
    }
    
    /// <summary>
    /// Creates the materials used in the scene
    /// </summary>
    private void CreateSceneMaterials()
    {
        Console.WriteLine("Creating scene materials...");
        
        // Create wood material for farm building
        var woodMaterial = Material.CreateWoodMaterial();
        _materials.Add(woodMaterial);
        
        // Create grass material for ground
        var grassMaterial = Material.CreateGrassMaterial();
        _materials.Add(grassMaterial);
        
        // Create corn material for corn stalks
        var cornMaterial = Material.CreateCornMaterial();
        _materials.Add(cornMaterial);
        
        Console.WriteLine($"Created {_materials.Count} materials");
    }
    
    /// <summary>
    /// Creates the meshes used in the scene
    /// </summary>
    private void CreateSceneMeshes()
    {
        Console.WriteLine("Creating scene meshes...");
        
        // Create farm building mesh
        var farmHouseMesh = FarmBuilding.CreateFarmHouse();
        _meshes.Add(farmHouseMesh);
        
        // Create corn field ground plane
        var groundMesh = CornField.CreateGroundPlane(30.0f, 30.0f, 6);
        _meshes.Add(groundMesh);
        
        // Create corn stalk mesh
        var cornStalkMesh = CornField.CreateCornStalk();
        _meshes.Add(cornStalkMesh);
        
        Console.WriteLine($"Created {_meshes.Count} meshes (farm building, ground, and corn stalk)");
    }
    
    /// <summary>
    /// Creates the game objects in the scene
    /// </summary>
    private void CreateSceneObjects()
    {
        Console.WriteLine("Creating scene objects...");
        
        // Find materials
        var woodMaterial = _materials.FirstOrDefault(m => m.Name == "Wood");
        var grassMaterial = _materials.FirstOrDefault(m => m.Name == "Grass");
        var cornMaterial = _materials.FirstOrDefault(m => m.Name == "Corn");
        
        // Create farm building object
        if (woodMaterial != null && _meshes.Count > 0)
        {
            var farmBuilding = new GameObject("FarmHouse", _meshes[0], woodMaterial);
            farmBuilding.SetPosition(0, 0, 0); // Center the building
            AddGameObject(farmBuilding);
            Console.WriteLine("Created farm house at center of scene");
        }
        
        // Create ground plane
        if (grassMaterial != null && _meshes.Count > 1)
        {
            var ground = new GameObject("CornFieldGround", _meshes[1], grassMaterial);
            ground.SetPosition(0, -0.05f, 0); // Slightly below origin
            AddGameObject(ground);
            Console.WriteLine("Created corn field ground plane");
        }
        
        // Create corn stalks in a grid pattern
        if (cornMaterial != null && _meshes.Count > 2)
        {
            var cornStalkMesh = _meshes[2];
            
            // Create corn stalks in a grid around the farm house
            float fieldSize = 20.0f;
            float spacing = 2.5f;
            int stalksPerSide = (int)(fieldSize / spacing);
            
            for (int x = -stalksPerSide / 2; x < stalksPerSide / 2; x++)
            {
                for (int z = -stalksPerSide / 2; z < stalksPerSide / 2; z++)
                {
                    // Skip area around the farm house
                    if (Math.Abs(x) <= 1 && Math.Abs(z) <= 1) continue;
                    
                    float xPos = x * spacing + GetRandomOffset(0.5f);
                    float zPos = z * spacing + GetRandomOffset(0.5f);
                    
                    var cornStalk = new GameObject($"CornStalk_{x}_{z}", cornStalkMesh, cornMaterial);
                    cornStalk.SetPosition(xPos, 0, zPos);
                    
                    // Add some variation
                    float randomRotation = GetRandomOffset(MathF.PI * 0.25f);
                    cornStalk.SetRotation(0, randomRotation, 0);
                    
                    float scaleVariation = 1.0f + GetRandomOffset(0.3f);
                    cornStalk.SetScale(scaleVariation, scaleVariation, scaleVariation);
                    
                    AddGameObject(cornStalk);
                }
            }
            
            Console.WriteLine($"Created corn field with {_gameObjects.Count - 2} corn stalks");
        }
        
        Console.WriteLine($"Created {_gameObjects.Count} total scene objects");
    }
    
    /// <summary>
    /// Gets a random offset for natural variation
    /// </summary>
    private static float GetRandomOffset(float maxOffset)
    {
        return (Random.Shared.NextSingle() - 0.5f) * 2.0f * maxOffset;
    }
    
    /// <summary>
    /// Adds a game object to the scene
    /// </summary>
    /// <param name="gameObject">Game object to add</param>
    public void AddGameObject(GameObject gameObject)
    {
        if (gameObject == null)
            throw new ArgumentNullException(nameof(gameObject));
            
        if (_gameObjects.Contains(gameObject))
        {
            Console.WriteLine($"Warning: GameObject '{gameObject.Name}' is already in the scene");
            return;
        }
        
        _gameObjects.Add(gameObject);
        
        // Add to named lookup if name is unique
        if (!_namedObjects.ContainsKey(gameObject.Name))
        {
            _namedObjects[gameObject.Name] = gameObject;
        }
        else
        {
            Console.WriteLine($"Warning: GameObject name '{gameObject.Name}' is not unique");
        }
        
        Console.WriteLine($"Added GameObject '{gameObject.Name}' to scene (total: {_gameObjects.Count})");
    }
    
    /// <summary>
    /// Removes a game object from the scene
    /// </summary>
    /// <param name="gameObject">Game object to remove</param>
    /// <returns>True if the object was removed</returns>
    public bool RemoveGameObject(GameObject gameObject)
    {
        if (gameObject == null) return false;
        
        bool removed = _gameObjects.Remove(gameObject);
        if (removed)
        {
            _namedObjects.Remove(gameObject.Name);
            Console.WriteLine($"Removed GameObject '{gameObject.Name}' from scene (remaining: {_gameObjects.Count})");
        }
        
        return removed;
    }
    
    /// <summary>
    /// Finds a game object by name
    /// </summary>
    /// <param name="name">Name of the game object</param>
    /// <returns>Game object if found, null otherwise</returns>
    public GameObject? FindGameObject(string name)
    {
        return _namedObjects.TryGetValue(name, out GameObject? obj) ? obj : null;
    }
    
    /// <summary>
    /// Gets all game objects with the specified name
    /// </summary>
    /// <param name="name">Name to search for</param>
    /// <returns>List of matching game objects</returns>
    public List<GameObject> FindGameObjects(string name)
    {
        return _gameObjects.Where(obj => obj.Name == name).ToList();
    }
    
    /// <summary>
    /// Gets all active game objects
    /// </summary>
    /// <returns>List of active game objects</returns>
    public List<GameObject> GetActiveGameObjects()
    {
        return _gameObjects.Where(obj => obj.IsActive).ToList();
    }
    
    /// <summary>
    /// Gets all game objects that are valid for rendering
    /// </summary>
    /// <returns>List of renderable game objects</returns>
    public List<GameObject> GetRenderableGameObjects()
    {
        return _gameObjects.Where(obj => obj.IsValidForRendering()).ToList();
    }
    
    /// <summary>
    /// Updates all active game objects in the scene
    /// </summary>
    /// <param name="deltaTime">Time elapsed since last update in seconds</param>
    public void Update(float deltaTime)
    {
        if (_disposed) return;
        
        // Update all active game objects
        foreach (var gameObject in _gameObjects)
        {
            if (gameObject.IsActive)
            {
                gameObject.Update(deltaTime);
            }
        }
    }
    
    /// <summary>
    /// Renders all renderable game objects in the scene with directional lighting
    /// </summary>
    /// <param name="renderer">DirectX renderer</param>
    /// <param name="cameraPosition">Current camera position for lighting calculations</param>
    public void Render(object? renderer = null, Vector3? cameraPosition = null)
    {
        if (_disposed) return;
        
        var renderableObjects = GetRenderableGameObjects();
        
        Console.WriteLine($"Rendering scene with {renderableObjects.Count} objects using directional lighting");
        
        // Set up directional lighting and fog if renderer supports it
        if (renderer is DirectXRenderer dxRenderer)
        {
            // Enable depth testing for proper 3D rendering
            dxRenderer.EnableDepthTesting(true);
            
            // Configure directional light (sunlight from above-right)
            var lightDirection = new Vector3(-0.5f, -1.0f, -0.3f); // Light coming from above-right
            var lightColor = new Vector3(1.0f, 0.95f, 0.8f); // Warm sunlight color
            dxRenderer.SetDirectionalLight(lightDirection, lightColor, 1.0f);
            
            // Set ambient lighting for outdoor scene
            dxRenderer.SetAmbientLight(0.3f); // Higher ambient for outdoor daylight
            
            // Set specular strength
            dxRenderer.SetSpecularStrength(0.4f);
            
            // Configure atmospheric fog for depth perception
            var fogColor = new Vector3(0.7f, 0.8f, 0.9f); // Light blue-gray atmospheric fog
            dxRenderer.SetFogParameters(true, 15.0f, 45.0f, fogColor);
            
            // Update camera position for specular calculations and fog
            if (cameraPosition.HasValue)
            {
                dxRenderer.UpdateCameraPosition(cameraPosition.Value);
            }
            
            Console.WriteLine("Directional lighting and fog configured:");
            Console.WriteLine($"  - Light direction: ({lightDirection.X:F2}, {lightDirection.Y:F2}, {lightDirection.Z:F2})");
            Console.WriteLine($"  - Light color: warm sunlight ({lightColor.X:F2}, {lightColor.Y:F2}, {lightColor.Z:F2})");
            Console.WriteLine("  - Ambient: 0.30 (outdoor daylight)");
            Console.WriteLine("  - Specular: 0.40 (moderate reflections)");
            Console.WriteLine("  - Depth testing: ENABLED");
            Console.WriteLine("  - Fog: ENABLED (15.0 to 45.0 units, atmospheric blue-gray)");
        }
        
        foreach (var gameObject in renderableObjects)
        {
            gameObject.Render(renderer);
        }
        
        Console.WriteLine("Scene rendering complete with directional lighting applied");
    }
    
    /// <summary>
    /// Clears all objects from the scene
    /// </summary>
    public void ClearScene()
    {
        Console.WriteLine("Clearing scene...");
        
        // Dispose game objects
        foreach (var gameObject in _gameObjects)
        {
            gameObject.Dispose();
        }
        _gameObjects.Clear();
        _namedObjects.Clear();
        
        // Dispose meshes
        foreach (var mesh in _meshes)
        {
            mesh.Dispose();
        }
        _meshes.Clear();
        
        // Dispose materials
        foreach (var material in _materials)
        {
            material.Dispose();
        }
        _materials.Clear();
        
        Console.WriteLine("Scene cleared");
    }
    
    /// <summary>
    /// Gets statistics about the scene
    /// </summary>
    /// <returns>Scene statistics</returns>
    public SceneStatistics GetStatistics()
    {
        int totalVertices = 0;
        int totalTriangles = 0;
        
        foreach (var gameObject in _gameObjects)
        {
            if (gameObject.Mesh != null)
            {
                totalVertices += gameObject.Mesh.VertexCount;
                totalTriangles += gameObject.Mesh.TriangleCount;
            }
        }
        
        return new SceneStatistics
        {
            TotalObjects = _gameObjects.Count,
            ActiveObjects = ActiveObjectCount,
            RenderableObjects = GetRenderableGameObjects().Count,
            TotalMeshes = _meshes.Count,
            TotalMaterials = _materials.Count,
            TotalVertices = totalVertices,
            TotalTriangles = totalTriangles
        };
    }
    
    /// <summary>
    /// Prints scene statistics to console
    /// </summary>
    public void PrintStatistics()
    {
        var stats = GetStatistics();
        
        Console.WriteLine("=== Scene Statistics ===");
        Console.WriteLine($"Total Objects: {stats.TotalObjects}");
        Console.WriteLine($"Active Objects: {stats.ActiveObjects}");
        Console.WriteLine($"Renderable Objects: {stats.RenderableObjects}");
        Console.WriteLine($"Total Meshes: {stats.TotalMeshes}");
        Console.WriteLine($"Total Materials: {stats.TotalMaterials}");
        Console.WriteLine($"Total Vertices: {stats.TotalVertices}");
        Console.WriteLine($"Total Triangles: {stats.TotalTriangles}");
        Console.WriteLine("========================");
    }
    
    public void Dispose()
    {
        if (_disposed) return;
        
        Console.WriteLine("Disposing SceneManager...");
        
        ClearScene();
        
        _disposed = true;
        Console.WriteLine("SceneManager disposed successfully");
    }
}

/// <summary>
/// Statistics about the scene
/// </summary>
public struct SceneStatistics
{
    public int TotalObjects;
    public int ActiveObjects;
    public int RenderableObjects;
    public int TotalMeshes;
    public int TotalMaterials;
    public int TotalVertices;
    public int TotalTriangles;
}