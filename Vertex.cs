using System.Numerics;
using System.Runtime.InteropServices;

namespace MySilkProgram;

/// <summary>
/// Represents a vertex with position, normal, and texture coordinates for 3D rendering
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct Vertex
{
    /// <summary>
    /// 3D position of the vertex in object space
    /// </summary>
    public Vector3 Position;
    
    /// <summary>
    /// Normal vector for lighting calculations
    /// </summary>
    public Vector3 Normal;
    
    /// <summary>
    /// Texture coordinates for material mapping
    /// </summary>
    public Vector2 TexCoords;
    
    /// <summary>
    /// Creates a new vertex with the specified position, normal, and texture coordinates
    /// </summary>
    /// <param name="position">3D position in object space</param>
    /// <param name="normal">Normal vector for lighting</param>
    /// <param name="texCoords">Texture coordinates</param>
    public Vertex(Vector3 position, Vector3 normal, Vector2 texCoords)
    {
        Position = position;
        Normal = normal;
        TexCoords = texCoords;
    }
    
    /// <summary>
    /// Creates a new vertex with position only (normal and texCoords set to zero)
    /// </summary>
    /// <param name="position">3D position in object space</param>
    public Vertex(Vector3 position) : this(position, Vector3.Zero, Vector2.Zero)
    {
    }
    
    /// <summary>
    /// Gets the size of a vertex in bytes (32 bytes total)
    /// </summary>
    public static int SizeInBytes => Marshal.SizeOf<Vertex>();
    
    /// <summary>
    /// Validates that the vertex data is reasonable for rendering
    /// </summary>
    /// <returns>True if vertex data is valid</returns>
    public bool IsValid()
    {
        // Check for NaN or infinite values
        if (!IsFiniteVector3(Position) || !IsFiniteVector3(Normal) || !IsFiniteVector2(TexCoords))
            return false;
            
        // Normal should be normalized (length close to 1) if not zero
        if (Normal != Vector3.Zero)
        {
            float normalLength = Normal.Length();
            if (normalLength < 0.9f || normalLength > 1.1f)
                return false;
        }
        
        // Texture coordinates should be reasonable (typically 0-1 range, but can be outside)
        if (Math.Abs(TexCoords.X) > 100f || Math.Abs(TexCoords.Y) > 100f)
            return false;
            
        return true;
    }
    
    private static bool IsFiniteVector3(Vector3 v)
    {
        return float.IsFinite(v.X) && float.IsFinite(v.Y) && float.IsFinite(v.Z);
    }
    
    private static bool IsFiniteVector2(Vector2 v)
    {
        return float.IsFinite(v.X) && float.IsFinite(v.Y);
    }
    
    public override string ToString()
    {
        return $"Vertex(Pos: {Position}, Normal: {Normal}, TexCoords: {TexCoords})";
    }
    
    public override bool Equals(object? obj)
    {
        if (obj is not Vertex other)
            return false;
            
        return Position.Equals(other.Position) && 
               Normal.Equals(other.Normal) && 
               TexCoords.Equals(other.TexCoords);
    }
    
    public override int GetHashCode()
    {
        return HashCode.Combine(Position, Normal, TexCoords);
    }
}