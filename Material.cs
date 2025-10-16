using System.Numerics;

namespace MySilkProgram;

/// <summary>
/// Represents a material with lighting properties for 3D rendering
/// </summary>
public class Material : IDisposable
{
    private bool _disposed = false;
    private IntPtr _texture = IntPtr.Zero;
    private bool _textureLoaded = false;
    
    /// <summary>
    /// Diffuse color component (main surface color)
    /// </summary>
    public Vector3 DiffuseColor { get; set; }
    
    /// <summary>
    /// Ambient color component (color in shadow/indirect lighting)
    /// </summary>
    public Vector3 AmbientColor { get; set; }
    
    /// <summary>
    /// Specular color component (highlight color)
    /// </summary>
    public Vector3 SpecularColor { get; set; }
    
    /// <summary>
    /// Shininess factor for specular highlights (higher = smaller, sharper highlights)
    /// </summary>
    public float Shininess { get; set; }
    
    /// <summary>
    /// Gets whether a texture has been loaded for this material
    /// </summary>
    public bool HasTexture => _textureLoaded;
    
    /// <summary>
    /// Gets the name of this material
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Creates a new material with default properties
    /// </summary>
    /// <param name="name">Name of the material</param>
    public Material(string name = "DefaultMaterial")
    {
        Name = name;
        DiffuseColor = new Vector3(0.8f, 0.8f, 0.8f); // Light gray
        AmbientColor = new Vector3(0.2f, 0.2f, 0.2f); // Dark gray
        SpecularColor = new Vector3(1.0f, 1.0f, 1.0f); // White
        Shininess = 32.0f; // Moderate shininess
    }
    
    /// <summary>
    /// Creates a new material with specified colors
    /// </summary>
    /// <param name="diffuse">Diffuse color</param>
    /// <param name="ambient">Ambient color</param>
    /// <param name="specular">Specular color</param>
    /// <param name="shininess">Shininess factor</param>
    /// <param name="name">Name of the material</param>
    public Material(Vector3 diffuse, Vector3 ambient, Vector3 specular, float shininess, string name = "CustomMaterial")
    {
        Name = name;
        DiffuseColor = diffuse;
        AmbientColor = ambient;
        SpecularColor = specular;
        Shininess = Math.Max(1.0f, shininess); // Ensure shininess is at least 1
    }
    
    /// <summary>
    /// Sets the diffuse color of the material
    /// </summary>
    /// <param name="color">RGB color values (0-1 range)</param>
    public void SetDiffuseColor(Vector3 color)
    {
        DiffuseColor = Vector3.Clamp(color, Vector3.Zero, Vector3.One);
    }
    
    /// <summary>
    /// Sets the diffuse color from RGB values
    /// </summary>
    /// <param name="r">Red component (0-1)</param>
    /// <param name="g">Green component (0-1)</param>
    /// <param name="b">Blue component (0-1)</param>
    public void SetDiffuseColor(float r, float g, float b)
    {
        SetDiffuseColor(new Vector3(r, g, b));
    }
    
    /// <summary>
    /// Sets the ambient color of the material
    /// </summary>
    /// <param name="color">RGB color values (0-1 range)</param>
    public void SetAmbientColor(Vector3 color)
    {
        AmbientColor = Vector3.Clamp(color, Vector3.Zero, Vector3.One);
    }
    
    /// <summary>
    /// Sets the ambient color from RGB values
    /// </summary>
    /// <param name="r">Red component (0-1)</param>
    /// <param name="g">Green component (0-1)</param>
    /// <param name="b">Blue component (0-1)</param>
    public void SetAmbientColor(float r, float g, float b)
    {
        SetAmbientColor(new Vector3(r, g, b));
    }
    
    /// <summary>
    /// Sets the specular color of the material
    /// </summary>
    /// <param name="color">RGB color values (0-1 range)</param>
    public void SetSpecularColor(Vector3 color)
    {
        SpecularColor = Vector3.Clamp(color, Vector3.Zero, Vector3.One);
    }
    
    /// <summary>
    /// Sets the specular color from RGB values
    /// </summary>
    /// <param name="r">Red component (0-1)</param>
    /// <param name="g">Green component (0-1)</param>
    /// <param name="b">Blue component (0-1)</param>
    public void SetSpecularColor(float r, float g, float b)
    {
        SetSpecularColor(new Vector3(r, g, b));
    }
    
    /// <summary>
    /// Sets the shininess factor
    /// </summary>
    /// <param name="shininess">Shininess value (minimum 1.0)</param>
    public void SetShininess(float shininess)
    {
        Shininess = Math.Max(1.0f, shininess);
    }
    
    /// <summary>
    /// Loads a texture for this material (placeholder implementation)
    /// </summary>
    /// <param name="texturePath">Path to the texture file</param>
    public void LoadTexture(string texturePath)
    {
        if (string.IsNullOrEmpty(texturePath))
            throw new ArgumentException("Texture path cannot be null or empty", nameof(texturePath));
            
        try
        {
            Console.WriteLine($"Loading texture for material '{Name}': {texturePath}");
            
            // Validate file exists (in a real implementation)
            if (!File.Exists(texturePath))
            {
                Console.WriteLine($"Warning: Texture file not found: {texturePath}");
                Console.WriteLine("Using default white texture");
                CreateDefaultTexture();
                return;
            }
            
            // Simulate texture loading process
            // In full implementation, this would:
            // 1. Load image data using a library like ImageSharp or STB
            // 2. Create DirectX 12 texture resource
            // 3. Upload texture data to GPU
            // 4. Create shader resource view (SRV)
            
            Console.WriteLine($"Simulating texture load: {Path.GetFileName(texturePath)}");
            Console.WriteLine("  - Reading image data");
            Console.WriteLine("  - Creating DirectX 12 texture resource");
            Console.WriteLine("  - Uploading texture data to GPU");
            Console.WriteLine("  - Creating shader resource view");
            
            // Simulate successful texture creation
            _texture = new IntPtr(Random.Shared.Next(1000, 9999)); // Fake handle
            _textureLoaded = true;
            
            Console.WriteLine($"Texture loaded successfully for material '{Name}'");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load texture '{texturePath}': {ex.Message}");
            Console.WriteLine("Falling back to default white texture");
            CreateDefaultTexture();
        }
    }
    
    /// <summary>
    /// Creates a default white texture when texture loading fails
    /// </summary>
    private void CreateDefaultTexture()
    {
        Console.WriteLine("Creating default 1x1 white texture");
        
        // Simulate creating a 1x1 white texture
        // In full implementation, this would create a minimal DirectX texture
        
        _texture = new IntPtr(1); // Default texture handle
        _textureLoaded = true;
        
        Console.WriteLine("Default texture created");
    }
    
    /// <summary>
    /// Binds this material to the DirectX command list for rendering (placeholder)
    /// </summary>
    /// <param name="commandList">DirectX command list (placeholder)</param>
    public void Bind(object? commandList = null)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(Material));
            
        Console.WriteLine($"Binding material '{Name}' to command list");
        Console.WriteLine($"  - Diffuse: ({DiffuseColor.X:F2}, {DiffuseColor.Y:F2}, {DiffuseColor.Z:F2})");
        Console.WriteLine($"  - Ambient: ({AmbientColor.X:F2}, {AmbientColor.Y:F2}, {AmbientColor.Z:F2})");
        Console.WriteLine($"  - Specular: ({SpecularColor.X:F2}, {SpecularColor.Y:F2}, {SpecularColor.Z:F2})");
        Console.WriteLine($"  - Shininess: {Shininess:F1}");
        Console.WriteLine($"  - Has Texture: {HasTexture}");
        
        // In full implementation, this would:
        // 1. Set material constant buffer data
        // 2. Bind texture SRV to shader if available
        // 3. Update root signature parameters
        
        if (HasTexture)
        {
            Console.WriteLine("  - Binding texture to shader slot t0");
        }
        else
        {
            Console.WriteLine("  - Using default white texture");
        }
        
        Console.WriteLine($"Material '{Name}' bound successfully");
    }
    
    /// <summary>
    /// Creates a predefined wood material
    /// </summary>
    public static Material CreateWoodMaterial()
    {
        var material = new Material("Wood")
        {
            DiffuseColor = new Vector3(0.6f, 0.4f, 0.2f), // Brown
            AmbientColor = new Vector3(0.2f, 0.15f, 0.1f), // Dark brown
            SpecularColor = new Vector3(0.3f, 0.3f, 0.3f), // Low specular
            Shininess = 8.0f // Low shininess for matte wood
        };
        
        Console.WriteLine("Created wood material with brown coloring");
        return material;
    }
    
    /// <summary>
    /// Creates a predefined grass material
    /// </summary>
    public static Material CreateGrassMaterial()
    {
        var material = new Material("Grass")
        {
            DiffuseColor = new Vector3(0.2f, 0.6f, 0.2f), // Green
            AmbientColor = new Vector3(0.1f, 0.2f, 0.1f), // Dark green
            SpecularColor = new Vector3(0.1f, 0.1f, 0.1f), // Very low specular
            Shininess = 4.0f // Very low shininess for matte grass
        };
        
        Console.WriteLine("Created grass material with green coloring");
        return material;
    }
    
    /// <summary>
    /// Creates a predefined corn material
    /// </summary>
    public static Material CreateCornMaterial()
    {
        var material = new Material("Corn")
        {
            DiffuseColor = new Vector3(0.4f, 0.7f, 0.2f), // Bright green
            AmbientColor = new Vector3(0.15f, 0.25f, 0.1f), // Dark green
            SpecularColor = new Vector3(0.2f, 0.2f, 0.2f), // Low specular
            Shininess = 6.0f // Low shininess for plant material
        };
        
        Console.WriteLine("Created corn material with bright green coloring");
        return material;
    }
    
    /// <summary>
    /// Creates a predefined metal material
    /// </summary>
    public static Material CreateMetalMaterial()
    {
        var material = new Material("Metal")
        {
            DiffuseColor = new Vector3(0.7f, 0.7f, 0.7f), // Light gray
            AmbientColor = new Vector3(0.1f, 0.1f, 0.1f), // Dark gray
            SpecularColor = new Vector3(1.0f, 1.0f, 1.0f), // Bright white
            Shininess = 128.0f // High shininess for shiny metal
        };
        
        Console.WriteLine("Created metal material with high reflectivity");
        return material;
    }
    
    /// <summary>
    /// Validates that the material properties are reasonable
    /// </summary>
    public bool IsValid()
    {
        // Check for valid color ranges (0-1)
        if (!IsValidColor(DiffuseColor) || !IsValidColor(AmbientColor) || !IsValidColor(SpecularColor))
            return false;
            
        // Check for valid shininess (positive value)
        if (Shininess <= 0 || !float.IsFinite(Shininess))
            return false;
            
        // Check for valid name
        if (string.IsNullOrEmpty(Name))
            return false;
            
        return true;
    }
    
    private static bool IsValidColor(Vector3 color)
    {
        return float.IsFinite(color.X) && float.IsFinite(color.Y) && float.IsFinite(color.Z) &&
               color.X >= 0 && color.X <= 1 &&
               color.Y >= 0 && color.Y <= 1 &&
               color.Z >= 0 && color.Z <= 1;
    }
    
    public override string ToString()
    {
        return $"Material '{Name}' - Diffuse: {DiffuseColor}, Ambient: {AmbientColor}, Specular: {SpecularColor}, Shininess: {Shininess}, HasTexture: {HasTexture}";
    }
    
    public void Dispose()
    {
        if (_disposed) return;
        
        Console.WriteLine($"Disposing material '{Name}'...");
        
        if (_textureLoaded && _texture != IntPtr.Zero)
        {
            Console.WriteLine("  - Releasing texture resource");
            // In full implementation, this would release the DirectX texture resource
            _texture = IntPtr.Zero;
            _textureLoaded = false;
        }
        
        _disposed = true;
        Console.WriteLine($"Material '{Name}' disposed successfully");
    }
}