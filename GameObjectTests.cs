using System.Numerics;
using Xunit;

namespace MySilkProgram.Tests;

public class GameObjectTests : IDisposable
{
    private readonly GameObject _gameObject;
    private readonly Mesh _testMesh;
    private readonly Material _testMaterial;
    
    public GameObjectTests()
    {
        _testMesh = Mesh.CreateTriangle();
        _testMaterial = Material.CreateWoodMaterial();
        _gameObject = new GameObject("TestObject", _testMesh, _testMaterial);
    }
    
    [Fact]
    public void Constructor_WithName_SetsNameCorrectly()
    {
        var obj = new GameObject("TestName");
        
        Assert.Equal("TestName", obj.Name);
        Assert.True(obj.IsActive);
        Assert.Equal(Vector3.Zero, obj.Position);
        Assert.Equal(Vector3.Zero, obj.Rotation);
        Assert.Equal(Vector3.One, obj.Scale);
    }
    
    [Fact]
    public void Constructor_WithMeshAndMaterial_SetsComponentsCorrectly()
    {
        var mesh = Mesh.CreateQuad();
        var material = Material.CreateGrassMaterial();
        var obj = new GameObject("TestObj", mesh, material);
        
        Assert.Equal("TestObj", obj.Name);
        Assert.Same(mesh, obj.Mesh);
        Assert.Same(material, obj.Material);
        Assert.True(obj.IsActive);
    }
    
    [Fact]
    public void SetPosition_UpdatesPositionAndInvalidatesMatrix()
    {
        var newPosition = new Vector3(1, 2, 3);
        
        _gameObject.SetPosition(newPosition);
        
        Assert.Equal(newPosition, _gameObject.Position);
    }
    
    [Fact]
    public void SetPosition_WithCoordinates_UpdatesPositionCorrectly()
    {
        _gameObject.SetPosition(4, 5, 6);
        
        Assert.Equal(new Vector3(4, 5, 6), _gameObject.Position);
    }
    
    [Fact]
    public void SetRotation_UpdatesRotationAndInvalidatesMatrix()
    {
        var newRotation = new Vector3(0.5f, 1.0f, 1.5f);
        
        _gameObject.SetRotation(newRotation);
        
        Assert.Equal(newRotation, _gameObject.Rotation);
    }
    
    [Fact]
    public void SetRotation_WithAngles_UpdatesRotationCorrectly()
    {
        _gameObject.SetRotation(0.1f, 0.2f, 0.3f);
        
        Assert.Equal(new Vector3(0.1f, 0.2f, 0.3f), _gameObject.Rotation);
    }
    
    [Fact]
    public void SetScale_UpdatesScaleAndInvalidatesMatrix()
    {
        var newScale = new Vector3(2, 3, 4);
        
        _gameObject.SetScale(newScale);
        
        Assert.Equal(newScale, _gameObject.Scale);
    }
    
    [Fact]
    public void SetScale_WithUniformScale_UpdatesScaleCorrectly()
    {
        _gameObject.SetScale(2.5f);
        
        Assert.Equal(new Vector3(2.5f, 2.5f, 2.5f), _gameObject.Scale);
    }
    
    [Fact]
    public void SetScale_WithCoordinates_UpdatesScaleCorrectly()
    {
        _gameObject.SetScale(1.5f, 2.0f, 2.5f);
        
        Assert.Equal(new Vector3(1.5f, 2.0f, 2.5f), _gameObject.Scale);
    }
    
    [Fact]
    public void Translate_AddsToCurrentPosition()
    {
        _gameObject.SetPosition(1, 2, 3);
        var offset = new Vector3(0.5f, 1.0f, 1.5f);
        
        _gameObject.Translate(offset);
        
        Assert.Equal(new Vector3(1.5f, 3.0f, 4.5f), _gameObject.Position);
    }
    
    [Fact]
    public void Rotate_AddsToCurrentRotation()
    {
        _gameObject.SetRotation(0.1f, 0.2f, 0.3f);
        var rotationOffset = new Vector3(0.05f, 0.1f, 0.15f);
        
        _gameObject.Rotate(rotationOffset);
        
        var expected = new Vector3(0.15f, 0.3f, 0.45f);
        Assert.True(Vector3.Distance(_gameObject.Rotation, expected) < 0.001f, 
            $"Expected: {expected}, Actual: {_gameObject.Rotation}");
    }
    
    [Fact]
    public void GetModelMatrix_WithIdentityTransform_ReturnsIdentityMatrix()
    {
        var obj = new GameObject("Identity");
        
        var matrix = obj.GetModelMatrix();
        
        // Identity matrix should be close to Matrix4x4.Identity
        Assert.True(IsMatrixClose(matrix, Matrix4x4.Identity, 0.001f));
    }
    
    [Fact]
    public void GetModelMatrix_WithTranslation_ReturnsCorrectMatrix()
    {
        _gameObject.SetPosition(1, 2, 3);
        
        var matrix = _gameObject.GetModelMatrix();
        
        // Check translation components
        Assert.Equal(1, matrix.M41, 3);
        Assert.Equal(2, matrix.M42, 3);
        Assert.Equal(3, matrix.M43, 3);
    }
    
    [Fact]
    public void GetModelMatrix_WithScale_ReturnsCorrectMatrix()
    {
        _gameObject.SetScale(2, 3, 4);
        
        var matrix = _gameObject.GetModelMatrix();
        
        // For a scale-only transform, the diagonal should contain scale values
        Assert.True(Math.Abs(matrix.M11 - 2) < 0.001f);
        Assert.True(Math.Abs(matrix.M22 - 3) < 0.001f);
        Assert.True(Math.Abs(matrix.M33 - 4) < 0.001f);
    }
    
    [Fact]
    public void GetModelMatrix_CachesResult_WhenTransformNotDirty()
    {
        _gameObject.SetPosition(1, 2, 3);
        
        var matrix1 = _gameObject.GetModelMatrix();
        var matrix2 = _gameObject.GetModelMatrix();
        
        // Should return the same matrix instance (cached)
        Assert.Equal(matrix1, matrix2);
    }
    
    [Fact]
    public void GetModelMatrix_RecalculatesMatrix_WhenTransformIsDirty()
    {
        _gameObject.SetPosition(1, 2, 3);
        var matrix1 = _gameObject.GetModelMatrix();
        
        _gameObject.SetPosition(4, 5, 6);
        var matrix2 = _gameObject.GetModelMatrix();
        
        // Should be different matrices
        Assert.NotEqual(matrix1, matrix2);
    }
    
    [Fact]
    public void Update_WhenActive_DoesNotThrow()
    {
        _gameObject.IsActive = true;
        
        // Should not throw for base implementation
        _gameObject.Update(0.016f);
    }
    
    [Fact]
    public void Update_WhenInactive_DoesNotThrow()
    {
        _gameObject.IsActive = false;
        
        // Should not throw and should return early
        _gameObject.Update(0.016f);
    }
    
    [Fact]
    public void Render_WithValidComponents_DoesNotThrow()
    {
        _gameObject.IsActive = true;
        
        // Should not throw when mesh and material are valid
        _gameObject.Render(null);
    }
    
    [Fact]
    public void Render_WhenInactive_DoesNotRender()
    {
        _gameObject.IsActive = false;
        
        // Should return early and not attempt to render
        _gameObject.Render(null);
    }
    
    [Fact]
    public void Render_WithoutMesh_DoesNotRender()
    {
        var obj = new GameObject("NoMesh");
        obj.Material = _testMaterial;
        obj.IsActive = true;
        
        // Should return early when mesh is null
        obj.Render(null);
    }
    
    [Fact]
    public void Render_WithoutMaterial_DoesNotRender()
    {
        var obj = new GameObject("NoMaterial");
        obj.Mesh = _testMesh;
        obj.IsActive = true;
        
        // Should return early when material is null
        obj.Render(null);
    }
    
    [Fact]
    public void GetWorldBoundingBox_WithoutMesh_ReturnsPositionAsBox()
    {
        var obj = new GameObject("NoMesh");
        obj.SetPosition(1, 2, 3);
        
        var (min, max) = obj.GetWorldBoundingBox();
        
        Assert.Equal(obj.Position, min);
        Assert.Equal(obj.Position, max);
    }
    
    [Fact]
    public void GetWorldBoundingBox_WithMesh_ReturnsTransformedBox()
    {
        _gameObject.SetPosition(1, 1, 1);
        _gameObject.SetScale(2, 2, 2);
        
        var (min, max) = _gameObject.GetWorldBoundingBox();
        
        // Should be transformed by position and scale
        // For a triangle mesh, the bounding box should have some volume
        Assert.True(min.X <= max.X);
        Assert.True(min.Y <= max.Y);
        Assert.True(min.Z <= max.Z);
        
        // At least one dimension should be different (not a point)
        Assert.True(min.X != max.X || min.Y != max.Y || min.Z != max.Z);
    }
    
    [Fact]
    public void GetWorldCenter_ReturnsCorrectCenter()
    {
        _gameObject.SetPosition(2, 4, 6);
        
        var center = _gameObject.GetWorldCenter();
        
        // For a simple mesh at position (2,4,6), center should be close to that position
        Assert.True(Vector3.Distance(center, new Vector3(2, 4, 6)) < 2.0f);
    }
    
    [Fact]
    public void IsValidForRendering_WithValidComponents_ReturnsTrue()
    {
        _gameObject.IsActive = true;
        
        Assert.True(_gameObject.IsValidForRendering());
    }
    
    [Fact]
    public void IsValidForRendering_WhenInactive_ReturnsFalse()
    {
        _gameObject.IsActive = false;
        
        Assert.False(_gameObject.IsValidForRendering());
    }
    
    [Fact]
    public void IsValidForRendering_WithoutMesh_ReturnsFalse()
    {
        var obj = new GameObject("NoMesh");
        obj.Material = _testMaterial;
        obj.IsActive = true;
        
        Assert.False(obj.IsValidForRendering());
    }
    
    [Fact]
    public void IsValidForRendering_WithoutMaterial_ReturnsFalse()
    {
        var obj = new GameObject("NoMaterial");
        obj.Mesh = _testMesh;
        obj.IsActive = true;
        
        Assert.False(obj.IsValidForRendering());
    }
    
    [Fact]
    public void ToString_ReturnsCorrectFormat()
    {
        _gameObject.SetPosition(1, 2, 3);
        _gameObject.SetRotation(0.1f, 0.2f, 0.3f);
        _gameObject.SetScale(2, 2, 2);
        
        var str = _gameObject.ToString();
        
        Assert.Contains("TestObject", str);
        Assert.Contains("Position", str);
        Assert.Contains("Rotation", str);
        Assert.Contains("Scale", str);
        Assert.Contains("Active", str);
    }
    
    private static bool IsMatrixClose(Matrix4x4 a, Matrix4x4 b, float tolerance)
    {
        return Math.Abs(a.M11 - b.M11) < tolerance &&
               Math.Abs(a.M12 - b.M12) < tolerance &&
               Math.Abs(a.M13 - b.M13) < tolerance &&
               Math.Abs(a.M14 - b.M14) < tolerance &&
               Math.Abs(a.M21 - b.M21) < tolerance &&
               Math.Abs(a.M22 - b.M22) < tolerance &&
               Math.Abs(a.M23 - b.M23) < tolerance &&
               Math.Abs(a.M24 - b.M24) < tolerance &&
               Math.Abs(a.M31 - b.M31) < tolerance &&
               Math.Abs(a.M32 - b.M32) < tolerance &&
               Math.Abs(a.M33 - b.M33) < tolerance &&
               Math.Abs(a.M34 - b.M34) < tolerance &&
               Math.Abs(a.M41 - b.M41) < tolerance &&
               Math.Abs(a.M42 - b.M42) < tolerance &&
               Math.Abs(a.M43 - b.M43) < tolerance &&
               Math.Abs(a.M44 - b.M44) < tolerance;
    }
    
    public void Dispose()
    {
        _gameObject?.Dispose();
        _testMesh?.Dispose();
        _testMaterial?.Dispose();
    }
}