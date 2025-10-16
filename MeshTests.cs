using System.Numerics;
using Xunit;

namespace MySilkProgram;

public class MeshTests
{
    [Fact]
    public void Vertex_Constructor_SetsPropertiesCorrectly()
    {
        // Arrange
        var position = new Vector3(1, 2, 3);
        var normal = new Vector3(0, 1, 0);
        var texCoords = new Vector2(0.5f, 0.5f);
        
        // Act
        var vertex = new Vertex(position, normal, texCoords);
        
        // Assert
        Assert.Equal(position, vertex.Position);
        Assert.Equal(normal, vertex.Normal);
        Assert.Equal(texCoords, vertex.TexCoords);
    }
    
    [Fact]
    public void Vertex_PositionOnlyConstructor_SetsPositionAndZerosOthers()
    {
        // Arrange
        var position = new Vector3(1, 2, 3);
        
        // Act
        var vertex = new Vertex(position);
        
        // Assert
        Assert.Equal(position, vertex.Position);
        Assert.Equal(Vector3.Zero, vertex.Normal);
        Assert.Equal(Vector2.Zero, vertex.TexCoords);
    }
    
    [Fact]
    public void Vertex_SizeInBytes_Returns32Bytes()
    {
        // Act & Assert
        Assert.Equal(32, Vertex.SizeInBytes);
    }
    
    [Fact]
    public void Vertex_IsValid_ReturnsTrueForValidVertex()
    {
        // Arrange
        var vertex = new Vertex(
            new Vector3(1, 2, 3),
            Vector3.Normalize(new Vector3(0, 1, 0)),
            new Vector2(0.5f, 0.5f)
        );
        
        // Act & Assert
        Assert.True(vertex.IsValid());
    }
    
    [Fact]
    public void Vertex_IsValid_ReturnsFalseForInfinitePosition()
    {
        // Arrange
        var vertex = new Vertex(
            new Vector3(float.PositiveInfinity, 2, 3),
            Vector3.UnitY,
            new Vector2(0.5f, 0.5f)
        );
        
        // Act & Assert
        Assert.False(vertex.IsValid());
    }
    
    [Fact]
    public void Vertex_IsValid_ReturnsFalseForNaNNormal()
    {
        // Arrange
        var vertex = new Vertex(
            new Vector3(1, 2, 3),
            new Vector3(float.NaN, 1, 0),
            new Vector2(0.5f, 0.5f)
        );
        
        // Act & Assert
        Assert.False(vertex.IsValid());
    }
    
    [Fact]
    public void Vertex_IsValid_ReturnsFalseForUnnormalizedNormal()
    {
        // Arrange
        var vertex = new Vertex(
            new Vector3(1, 2, 3),
            new Vector3(0, 5, 0), // Length = 5, not normalized
            new Vector2(0.5f, 0.5f)
        );
        
        // Act & Assert
        Assert.False(vertex.IsValid());
    }
    
    [Fact]
    public void Vertex_IsValid_ReturnsTrueForZeroNormal()
    {
        // Arrange
        var vertex = new Vertex(
            new Vector3(1, 2, 3),
            Vector3.Zero, // Zero normal is allowed
            new Vector2(0.5f, 0.5f)
        );
        
        // Act & Assert
        Assert.True(vertex.IsValid());
    }
    
    [Fact]
    public void Mesh_Constructor_ValidData_CreatesSuccessfully()
    {
        // Arrange
        var vertices = new Vertex[]
        {
            new Vertex(new Vector3(0, 0, 0), Vector3.UnitZ, new Vector2(0, 0)),
            new Vertex(new Vector3(1, 0, 0), Vector3.UnitZ, new Vector2(1, 0)),
            new Vertex(new Vector3(0, 1, 0), Vector3.UnitZ, new Vector2(0, 1))
        };
        var indices = new uint[] { 0, 1, 2 };
        
        // Act
        var mesh = new Mesh(vertices, indices);
        
        // Assert
        Assert.Equal(3, mesh.VertexCount);
        Assert.Equal(3, mesh.IndexCount);
        Assert.Equal(1, mesh.TriangleCount);
        Assert.False(mesh.BuffersCreated);
    }
    
    [Fact]
    public void Mesh_Constructor_EmptyVertices_ThrowsException()
    {
        // Arrange
        var vertices = Array.Empty<Vertex>();
        var indices = new uint[] { 0, 1, 2 };
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Mesh(vertices, indices));
    }
    
    [Fact]
    public void Mesh_Constructor_EmptyIndices_ThrowsException()
    {
        // Arrange
        var vertices = new Vertex[]
        {
            new Vertex(new Vector3(0, 0, 0)),
            new Vertex(new Vector3(1, 0, 0)),
            new Vertex(new Vector3(0, 1, 0))
        };
        var indices = Array.Empty<uint>();
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Mesh(vertices, indices));
    }
    
    [Fact]
    public void Mesh_Constructor_IndicesNotMultipleOfThree_ThrowsException()
    {
        // Arrange
        var vertices = new Vertex[]
        {
            new Vertex(new Vector3(0, 0, 0)),
            new Vertex(new Vector3(1, 0, 0)),
            new Vertex(new Vector3(0, 1, 0))
        };
        var indices = new uint[] { 0, 1 }; // Only 2 indices, not multiple of 3
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Mesh(vertices, indices));
    }
    
    [Fact]
    public void Mesh_Constructor_IndexOutOfRange_ThrowsException()
    {
        // Arrange
        var vertices = new Vertex[]
        {
            new Vertex(new Vector3(0, 0, 0)),
            new Vertex(new Vector3(1, 0, 0)),
            new Vertex(new Vector3(0, 1, 0))
        };
        var indices = new uint[] { 0, 1, 5 }; // Index 5 is out of range for 3 vertices
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Mesh(vertices, indices));
    }
    
    [Fact]
    public void Mesh_CreateBuffers_CallsSuccessfully()
    {
        // Arrange
        var mesh = Mesh.CreateTriangle();
        
        // Act
        mesh.CreateBuffers();
        
        // Assert
        Assert.True(mesh.BuffersCreated);
    }
    
    [Fact]
    public void Mesh_CreateBuffers_CalledTwice_ShowsWarning()
    {
        // Arrange
        var mesh = Mesh.CreateTriangle();
        mesh.CreateBuffers();
        
        // Act & Assert (should not throw, just show warning)
        mesh.CreateBuffers();
        Assert.True(mesh.BuffersCreated);
    }
    
    [Fact]
    public void Mesh_GetVertexBufferView_WithoutBuffers_ThrowsException()
    {
        // Arrange
        var mesh = Mesh.CreateTriangle();
        
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => mesh.GetVertexBufferView());
    }
    
    [Fact]
    public void Mesh_GetIndexBufferView_WithoutBuffers_ThrowsException()
    {
        // Arrange
        var mesh = Mesh.CreateTriangle();
        
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => mesh.GetIndexBufferView());
    }
    
    [Fact]
    public void Mesh_GetVertexBufferView_WithBuffers_ReturnsView()
    {
        // Arrange
        var mesh = Mesh.CreateTriangle();
        mesh.CreateBuffers();
        
        // Act
        var view = mesh.GetVertexBufferView();
        
        // Assert
        Assert.NotNull(view);
    }
    
    [Fact]
    public void Mesh_GetIndexBufferView_WithBuffers_ReturnsView()
    {
        // Arrange
        var mesh = Mesh.CreateTriangle();
        mesh.CreateBuffers();
        
        // Act
        var view = mesh.GetIndexBufferView();
        
        // Assert
        Assert.NotNull(view);
    }
    
    [Fact]
    public void Mesh_GetBoundingBox_CalculatesCorrectly()
    {
        // Arrange
        var vertices = new Vertex[]
        {
            new Vertex(new Vector3(-1, -2, -3)),
            new Vertex(new Vector3(2, 1, 0)),
            new Vertex(new Vector3(0, 3, 1))
        };
        var indices = new uint[] { 0, 1, 2 };
        var mesh = new Mesh(vertices, indices);
        
        // Act
        var (min, max) = mesh.GetBoundingBox();
        
        // Assert
        Assert.Equal(new Vector3(-1, -2, -3), min);
        Assert.Equal(new Vector3(2, 3, 1), max);
    }
    
    [Fact]
    public void Mesh_GetCenter_CalculatesCorrectly()
    {
        // Arrange
        var vertices = new Vertex[]
        {
            new Vertex(new Vector3(-2, -2, -2)),
            new Vertex(new Vector3(2, 2, 2))
        };
        var indices = new uint[] { 0, 1, 0 }; // Dummy triangle
        var mesh = new Mesh(vertices, indices);
        
        // Act
        var center = mesh.GetCenter();
        
        // Assert
        Assert.Equal(Vector3.Zero, center);
    }
    
    [Fact]
    public void Mesh_CreateTriangle_CreatesValidTriangle()
    {
        // Act
        var mesh = Mesh.CreateTriangle();
        
        // Assert
        Assert.Equal(3, mesh.VertexCount);
        Assert.Equal(3, mesh.IndexCount);
        Assert.Equal(1, mesh.TriangleCount);
        
        // Verify vertices are valid
        foreach (var vertex in mesh.Vertices)
        {
            Assert.True(vertex.IsValid());
        }
    }
    
    [Fact]
    public void Mesh_CreateQuad_CreatesValidQuad()
    {
        // Act
        var mesh = Mesh.CreateQuad();
        
        // Assert
        Assert.Equal(4, mesh.VertexCount);
        Assert.Equal(6, mesh.IndexCount);
        Assert.Equal(2, mesh.TriangleCount);
        
        // Verify vertices are valid
        foreach (var vertex in mesh.Vertices)
        {
            Assert.True(vertex.IsValid());
        }
    }
    
    [Fact]
    public void Mesh_Dispose_CleansUpResources()
    {
        // Arrange
        var mesh = Mesh.CreateTriangle();
        mesh.CreateBuffers();
        Assert.True(mesh.BuffersCreated);
        
        // Act
        mesh.Dispose();
        
        // Assert
        Assert.False(mesh.BuffersCreated);
    }
}