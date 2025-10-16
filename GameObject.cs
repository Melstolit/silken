using System.Numerics;

namespace MySilkProgram;

/// <summary>
/// Represents a game object with transform, mesh, and material components
/// </summary>
public class GameObject : IDisposable
{
    private bool _disposed = false;
    private Matrix4x4? _cachedModelMatrix = null;
    private bool _transformDirty = true;
    
    /// <summary>
    /// Position of the game object in world space
    /// </summary>
    public Vector3 Position { get; set; }
    
    /// <summary>
    /// Rotation of the game object in radians (pitch, yaw, roll)
    /// </summary>
    public Vector3 Rotation { get; set; }
    
    /// <summary>
    /// Scale of the game object
    /// </summary>
    public Vector3 Scale { get; set; }
    
    /// <summary>
    /// Mesh component for rendering
    /// </summary>
    public Mesh? Mesh { get; set; }
    
    /// <summary>
    /// Material component for appearance
    /// </summary>
    public Material? Material { get; set; }
    
    /// <summary>
    /// Name of this game object
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Whether this game object is active and should be updated/rendered
    /// </summary>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// Creates a new game object with default transform
    /// </summary>
    /// <param name="name">Name of the game object</param>
    public GameObject(string name = "GameObject")
    {
        Name = name;
        Position = Vector3.Zero;
        Rotation = Vector3.Zero;
        Scale = Vector3.One;
        IsActive = true;
    }
    
    /// <summary>
    /// Creates a new game object with mesh and material
    /// </summary>
    /// <param name="name">Name of the game object</param>
    /// <param name="mesh">Mesh component</param>
    /// <param name="material">Material component</param>
    public GameObject(string name, Mesh mesh, Material material) : this(name)
    {
        Mesh = mesh;
        Material = material;
    }
    
    /// <summary>
    /// Sets the position of the game object
    /// </summary>
    /// <param name="position">New position</param>
    public void SetPosition(Vector3 position)
    {
        Position = position;
        _transformDirty = true;
    }
    
    /// <summary>
    /// Sets the position of the game object
    /// </summary>
    /// <param name="x">X coordinate</param>
    /// <param name="y">Y coordinate</param>
    /// <param name="z">Z coordinate</param>
    public void SetPosition(float x, float y, float z)
    {
        SetPosition(new Vector3(x, y, z));
    }
    
    /// <summary>
    /// Sets the rotation of the game object
    /// </summary>
    /// <param name="rotation">New rotation in radians</param>
    public void SetRotation(Vector3 rotation)
    {
        Rotation = rotation;
        _transformDirty = true;
    }
    
    /// <summary>
    /// Sets the rotation of the game object
    /// </summary>
    /// <param name="pitch">Pitch rotation in radians</param>
    /// <param name="yaw">Yaw rotation in radians</param>
    /// <param name="roll">Roll rotation in radians</param>
    public void SetRotation(float pitch, float yaw, float roll)
    {
        SetRotation(new Vector3(pitch, yaw, roll));
    }
    
    /// <summary>
    /// Sets the scale of the game object
    /// </summary>
    /// <param name="scale">New scale</param>
    public void SetScale(Vector3 scale)
    {
        Scale = scale;
        _transformDirty = true;
    }
    
    /// <summary>
    /// Sets the scale of the game object uniformly
    /// </summary>
    /// <param name="scale">Uniform scale factor</param>
    public void SetScale(float scale)
    {
        SetScale(new Vector3(scale, scale, scale));
    }
    
    /// <summary>
    /// Sets the scale of the game object
    /// </summary>
    /// <param name="x">X scale</param>
    /// <param name="y">Y scale</param>
    /// <param name="z">Z scale</param>
    public void SetScale(float x, float y, float z)
    {
        SetScale(new Vector3(x, y, z));
    }
    
    /// <summary>
    /// Translates the game object by the specified offset
    /// </summary>
    /// <param name="offset">Translation offset</param>
    public void Translate(Vector3 offset)
    {
        Position += offset;
        _transformDirty = true;
    }
    
    /// <summary>
    /// Rotates the game object by the specified angles
    /// </summary>
    /// <param name="rotation">Rotation offset in radians</param>
    public void Rotate(Vector3 rotation)
    {
        Rotation += rotation;
        _transformDirty = true;
    }
    
    /// <summary>
    /// Gets the model matrix for this game object (world transformation)
    /// </summary>
    /// <returns>Model matrix combining scale, rotation, and translation</returns>
    public Matrix4x4 GetModelMatrix()
    {
        if (!_transformDirty && _cachedModelMatrix.HasValue)
        {
            return _cachedModelMatrix.Value;
        }
        
        // Create transformation matrices
        Matrix4x4 scaleMatrix = Matrix4x4.CreateScale(Scale);
        Matrix4x4 rotationMatrix = Matrix4x4.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
        Matrix4x4 translationMatrix = Matrix4x4.CreateTranslation(Position);
        
        // Combine transformations: Scale * Rotation * Translation
        Matrix4x4 modelMatrix = scaleMatrix * rotationMatrix * translationMatrix;
        
        // Cache the result
        _cachedModelMatrix = modelMatrix;
        _transformDirty = false;
        
        return modelMatrix;
    }
    
    /// <summary>
    /// Updates the game object (called each frame)
    /// </summary>
    /// <param name="deltaTime">Time elapsed since last update in seconds</param>
    public virtual void Update(float deltaTime)
    {
        if (!IsActive) return;
        
        // Base implementation does nothing
        // Derived classes can override this for custom behavior
    }
    
    /// <summary>
    /// Renders the game object using the specified renderer with directional lighting
    /// </summary>
    /// <param name="renderer">DirectX renderer</param>
    public virtual void Render(object? renderer = null)
    {
        if (!IsActive || Mesh == null || Material == null) return;
        
        Console.WriteLine($"Rendering GameObject '{Name}' with directional lighting");
        Console.WriteLine($"  - Position: ({Position.X:F2}, {Position.Y:F2}, {Position.Z:F2})");
        Console.WriteLine($"  - Rotation: ({Rotation.X:F2}, {Rotation.Y:F2}, {Rotation.Z:F2})");
        Console.WriteLine($"  - Scale: ({Scale.X:F2}, {Scale.Y:F2}, {Scale.Z:F2})");
        
        // Get model matrix for this object
        Matrix4x4 modelMatrix = GetModelMatrix();
        
        Console.WriteLine($"  - Model matrix calculated");
        Console.WriteLine($"  - Mesh: {Mesh.VertexCount} vertices, {Mesh.TriangleCount} triangles");
        Console.WriteLine($"  - Material: {Material.Name} (lighting-enabled)");
        
        // Use DirectX renderer with lighting if available
        if (renderer is DirectXRenderer dxRenderer)
        {
            // Set model matrix for this object
            // Note: View and projection matrices should be set by the main render loop
            dxRenderer.SetMatrices(modelMatrix, Matrix4x4.Identity, Matrix4x4.Identity);
            
            // Render with lighting
            dxRenderer.RenderMesh(Mesh, Material);
            
            Console.WriteLine("  - Rendered with DirectX lighting pipeline");
        }
        else
        {
            // Fallback rendering without lighting
            Material.Bind(renderer);
            
            if (Mesh.BuffersCreated)
            {
                Console.WriteLine("  - Drawing mesh with existing buffers (no lighting)");
            }
            else
            {
                Console.WriteLine("  - Creating mesh buffers before drawing (no lighting)");
                Mesh.CreateBuffers(renderer);
            }
        }
        
        Console.WriteLine($"GameObject '{Name}' rendered successfully with lighting applied");
    }
    
    /// <summary>
    /// Gets the world-space bounding box of this game object
    /// </summary>
    /// <returns>Transformed bounding box</returns>
    public (Vector3 min, Vector3 max) GetWorldBoundingBox()
    {
        if (Mesh == null)
            return (Position, Position);
        
        var (localMin, localMax) = Mesh.GetBoundingBox();
        Matrix4x4 modelMatrix = GetModelMatrix();
        
        // Transform bounding box corners
        Vector3[] corners = new Vector3[]
        {
            new Vector3(localMin.X, localMin.Y, localMin.Z),
            new Vector3(localMax.X, localMin.Y, localMin.Z),
            new Vector3(localMin.X, localMax.Y, localMin.Z),
            new Vector3(localMax.X, localMax.Y, localMin.Z),
            new Vector3(localMin.X, localMin.Y, localMax.Z),
            new Vector3(localMax.X, localMin.Y, localMax.Z),
            new Vector3(localMin.X, localMax.Y, localMax.Z),
            new Vector3(localMax.X, localMax.Y, localMax.Z)
        };
        
        Vector3 worldMin = Vector3.Transform(corners[0], modelMatrix);
        Vector3 worldMax = worldMin;
        
        for (int i = 1; i < corners.Length; i++)
        {
            Vector3 worldCorner = Vector3.Transform(corners[i], modelMatrix);
            worldMin = Vector3.Min(worldMin, worldCorner);
            worldMax = Vector3.Max(worldMax, worldCorner);
        }
        
        return (worldMin, worldMax);
    }
    
    /// <summary>
    /// Gets the world-space center of this game object
    /// </summary>
    /// <returns>World-space center position</returns>
    public Vector3 GetWorldCenter()
    {
        var (min, max) = GetWorldBoundingBox();
        return (min + max) * 0.5f;
    }
    
    /// <summary>
    /// Checks if this game object is valid for rendering
    /// </summary>
    /// <returns>True if the object has valid mesh and material</returns>
    public bool IsValidForRendering()
    {
        return IsActive && Mesh != null && Material != null && Material.IsValid();
    }
    
    public override string ToString()
    {
        return $"GameObject '{Name}' - Position: {Position}, Rotation: {Rotation}, Scale: {Scale}, Active: {IsActive}";
    }
    
    public void Dispose()
    {
        if (_disposed) return;
        
        Console.WriteLine($"Disposing GameObject '{Name}'...");
        
        // Note: We don't dispose Mesh and Material here as they might be shared
        // between multiple GameObjects. The SceneManager should handle their disposal.
        
        _disposed = true;
        Console.WriteLine($"GameObject '{Name}' disposed successfully");
    }
}