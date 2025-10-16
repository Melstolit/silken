using System.Numerics;
using Xunit;

namespace MySilkProgram.Tests;

public class CornFieldTests : IDisposable
{
    private readonly List<Mesh> _createdMeshes = new();
    private readonly List<GameObject> _createdObjects = new();
    
    [Fact]
    public void CreateGroundPlane_WithDefaultParameters_CreatesValidMesh()
    {
        var mesh = CornField.CreateGroundPlane();
        _createdMeshes.Add(mesh);
        
        Assert.NotNull(mesh);
        Assert.True(mesh.VertexCount > 0);
        Assert.True(mesh.IndexCount > 0);
        Assert.True(mesh.TriangleCount > 0);
        
        // Default is 20x20 with 4 subdivisions, should create (4+1)^2 = 25 vertices
        Assert.Equal(25, mesh.VertexCount);
        
        // Should create 4x4 = 16 quads = 32 triangles
        Assert.Equal(32, mesh.TriangleCount);
    }
    
    [Fact]
    public void CreateGroundPlane_WithCustomDimensions_CreatesCorrectSizedMesh()
    {
        float width = 10.0f, depth = 15.0f;
        int subdivisions = 2;
        
        var mesh = CornField.CreateGroundPlane(width, depth, subdivisions);
        _createdMeshes.Add(mesh);
        
        Assert.NotNull(mesh);
        
        // Check bounding box
        var (min, max) = mesh.GetBoundingBox();
        
        float actualWidth = max.X - min.X;
        float actualDepth = max.Z - min.Z;
        
        Assert.True(Math.Abs(actualWidth - width) < 0.01f, 
            $"Expected width {width}, got {actualWidth}");
        Assert.True(Math.Abs(actualDepth - depth) < 0.01f, 
            $"Expected depth {depth}, got {actualDepth}");
        
        // Should be flat (Y = 0)
        Assert.True(Math.Abs(min.Y) < 0.01f && Math.Abs(max.Y) < 0.01f, 
            $"Ground should be flat, got Y range {min.Y} to {max.Y}");
    }
    
    [Fact]
    public void CreateGroundPlane_WithInvalidParameters_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => CornField.CreateGroundPlane(0, 10, 2));
        Assert.Throws<ArgumentException>(() => CornField.CreateGroundPlane(-5, 10, 2));
        Assert.Throws<ArgumentException>(() => CornField.CreateGroundPlane(10, 0, 2));
        Assert.Throws<ArgumentException>(() => CornField.CreateGroundPlane(10, -5, 2));
        Assert.Throws<ArgumentException>(() => CornField.CreateGroundPlane(10, 10, 0));
    }
    
    [Fact]
    public void CreateCornStalk_WithDefaultParameters_CreatesValidMesh()
    {
        var mesh = CornField.CreateCornStalk();
        _createdMeshes.Add(mesh);
        
        Assert.NotNull(mesh);
        Assert.True(mesh.VertexCount > 0);
        Assert.True(mesh.IndexCount > 0);
        Assert.True(mesh.TriangleCount > 0);
        
        // Cross-shaped corn stalk should have 8 vertices (4 per quad * 2 quads)
        Assert.Equal(8, mesh.VertexCount);
        
        // Should have 4 triangles (2 per quad * 2 quads)
        Assert.Equal(4, mesh.TriangleCount);
    }
    
    [Fact]
    public void CreateCornStalk_WithCustomDimensions_CreatesCorrectSizedMesh()
    {
        float height = 3.0f, width = 0.5f;
        
        var mesh = CornField.CreateCornStalk(height, width);
        _createdMeshes.Add(mesh);
        
        Assert.NotNull(mesh);
        
        // Check bounding box
        var (min, max) = mesh.GetBoundingBox();
        
        float actualHeight = max.Y - min.Y;
        float actualWidth = Math.Max(max.X - min.X, max.Z - min.Z);
        
        Assert.True(Math.Abs(actualHeight - height) < 0.01f, 
            $"Expected height {height}, got {actualHeight}");
        Assert.True(Math.Abs(actualWidth - width) < 0.01f, 
            $"Expected width {width}, got {actualWidth}");
    }
    
    [Fact]
    public void CreateCornStalk_WithInvalidParameters_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => CornField.CreateCornStalk(0, 0.5f));
        Assert.Throws<ArgumentException>(() => CornField.CreateCornStalk(-1, 0.5f));
        Assert.Throws<ArgumentException>(() => CornField.CreateCornStalk(2.5f, 0));
        Assert.Throws<ArgumentException>(() => CornField.CreateCornStalk(2.5f, -0.3f));
    }
    
    [Fact]
    public void CreateCornStalks_WithDefaultParameters_CreatesMultipleObjects()
    {
        var cornStalks = CornField.CreateCornStalks();
        _createdObjects.AddRange(cornStalks);
        
        Assert.NotNull(cornStalks);
        Assert.True(cornStalks.Count > 0);
        
        // With default 16x16 field and 2.0 spacing, should create 8x8 = 64 stalks
        Assert.Equal(64, cornStalks.Count);
        
        // Check that all objects are valid
        foreach (var stalk in cornStalks)
        {
            Assert.NotNull(stalk);
            Assert.NotNull(stalk.Mesh);
            Assert.NotNull(stalk.Material);
            Assert.True(stalk.IsValidForRendering());
            Assert.Contains("CornStalk", stalk.Name);
        }
    }
    
    [Fact]
    public void CreateCornStalks_WithCustomParameters_CreatesCorrectNumber()
    {
        float fieldWidth = 8.0f, fieldDepth = 6.0f, spacing = 2.0f;
        
        var cornStalks = CornField.CreateCornStalks(fieldWidth, fieldDepth, spacing);
        _createdObjects.AddRange(cornStalks);
        
        int expectedX = (int)(fieldWidth / spacing);  // 4
        int expectedZ = (int)(fieldDepth / spacing);  // 3
        int expectedTotal = expectedX * expectedZ;    // 12
        
        Assert.Equal(expectedTotal, cornStalks.Count);
    }
    
    [Fact]
    public void CreateCornStalks_WithInvalidParameters_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => CornField.CreateCornStalks(0, 10, 2));
        Assert.Throws<ArgumentException>(() => CornField.CreateCornStalks(-5, 10, 2));
        Assert.Throws<ArgumentException>(() => CornField.CreateCornStalks(10, 0, 2));
        Assert.Throws<ArgumentException>(() => CornField.CreateCornStalks(10, -5, 2));
        Assert.Throws<ArgumentException>(() => CornField.CreateCornStalks(10, 10, 0));
        Assert.Throws<ArgumentException>(() => CornField.CreateCornStalks(10, 10, -1));
    }
    
    [Fact]
    public void CreateCornStalks_PositionsAreWithinField()
    {
        float fieldWidth = 10.0f, fieldDepth = 8.0f;
        
        var cornStalks = CornField.CreateCornStalks(fieldWidth, fieldDepth, 2.0f);
        _createdObjects.AddRange(cornStalks);
        
        float halfWidth = fieldWidth * 0.5f;
        float halfDepth = fieldDepth * 0.5f;
        
        foreach (var stalk in cornStalks)
        {
            var pos = stalk.Position;
            
            // Allow some tolerance for randomization
            Assert.True(pos.X >= -halfWidth - 1.0f && pos.X <= halfWidth + 1.0f, 
                $"Corn stalk X position {pos.X} outside field bounds");
            Assert.True(pos.Z >= -halfDepth - 1.0f && pos.Z <= halfDepth + 1.0f, 
                $"Corn stalk Z position {pos.Z} outside field bounds");
            Assert.True(Math.Abs(pos.Y) < 0.1f, 
                $"Corn stalk should be at ground level, got Y = {pos.Y}");
        }
    }
    
    [Fact]
    public void CreateCornStalks_HaveVariation()
    {
        var cornStalks = CornField.CreateCornStalks(6.0f, 6.0f, 3.0f);
        _createdObjects.AddRange(cornStalks);
        
        Assert.True(cornStalks.Count >= 2, "Need at least 2 stalks to test variation");
        
        // Check that stalks have different rotations and scales
        bool hasRotationVariation = false;
        bool hasScaleVariation = false;
        
        for (int i = 1; i < cornStalks.Count; i++)
        {
            var stalk1 = cornStalks[0];
            var stalk2 = cornStalks[i];
            
            if (Math.Abs(stalk1.Rotation.Y - stalk2.Rotation.Y) > 0.01f)
                hasRotationVariation = true;
                
            if (Math.Abs(stalk1.Scale.X - stalk2.Scale.X) > 0.01f)
                hasScaleVariation = true;
        }
        
        Assert.True(hasRotationVariation, "Corn stalks should have rotation variation");
        Assert.True(hasScaleVariation, "Corn stalks should have scale variation");
    }
    
    [Fact]
    public void CreateCornFieldScene_CreatesCompleteScene()
    {
        var (groundPlane, cornStalks) = CornField.CreateCornFieldScene(12.0f, 10.0f, 2.5f);
        
        _createdObjects.Add(groundPlane);
        _createdObjects.AddRange(cornStalks);
        
        // Check ground plane
        Assert.NotNull(groundPlane);
        Assert.Equal("CornFieldGround", groundPlane.Name);
        Assert.NotNull(groundPlane.Mesh);
        Assert.NotNull(groundPlane.Material);
        Assert.Equal("Grass", groundPlane.Material.Name);
        Assert.True(groundPlane.IsValidForRendering());
        
        // Check corn stalks
        Assert.NotNull(cornStalks);
        Assert.True(cornStalks.Count > 0);
        
        foreach (var stalk in cornStalks)
        {
            Assert.NotNull(stalk);
            Assert.NotNull(stalk.Material);
            Assert.Equal("Corn", stalk.Material.Name);
            Assert.True(stalk.IsValidForRendering());
        }
    }
    
    [Fact]
    public void CreateDirtPath_WithDefaultParameters_CreatesValidMesh()
    {
        var mesh = CornField.CreateDirtPath();
        _createdMeshes.Add(mesh);
        
        Assert.NotNull(mesh);
        Assert.True(mesh.VertexCount > 0);
        Assert.True(mesh.IndexCount > 0);
        Assert.True(mesh.TriangleCount > 0);
        
        // Simple rectangular path should have 4 vertices and 2 triangles
        Assert.Equal(4, mesh.VertexCount);
        Assert.Equal(2, mesh.TriangleCount);
    }
    
    [Fact]
    public void CreateDirtPath_WithCustomDimensions_CreatesCorrectSizedMesh()
    {
        float width = 2.0f, length = 15.0f;
        
        var mesh = CornField.CreateDirtPath(width, length);
        _createdMeshes.Add(mesh);
        
        var (min, max) = mesh.GetBoundingBox();
        
        float actualWidth = max.X - min.X;
        float actualLength = max.Z - min.Z;
        
        Assert.True(Math.Abs(actualWidth - width) < 0.01f, 
            $"Expected width {width}, got {actualWidth}");
        Assert.True(Math.Abs(actualLength - length) < 0.01f, 
            $"Expected length {length}, got {actualLength}");
    }
    
    [Fact]
    public void CreateDirtPath_WithInvalidParameters_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => CornField.CreateDirtPath(0, 10));
        Assert.Throws<ArgumentException>(() => CornField.CreateDirtPath(-1, 10));
        Assert.Throws<ArgumentException>(() => CornField.CreateDirtPath(2, 0));
        Assert.Throws<ArgumentException>(() => CornField.CreateDirtPath(2, -5));
    }
    
    public void Dispose()
    {
        foreach (var mesh in _createdMeshes)
        {
            mesh?.Dispose();
        }
        
        foreach (var obj in _createdObjects)
        {
            obj?.Dispose();
        }
        
        _createdMeshes.Clear();
        _createdObjects.Clear();
    }
}