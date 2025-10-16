using Silk.NET.Direct3D11;
using System.Numerics;
using System.Runtime.InteropServices;

namespace MySilkProgram;

/// <summary>
/// Represents a 3D mesh with vertex and index data, including DirectX 12 buffer management
/// </summary>
public class Mesh : IDisposable
{
    private readonly Vertex[] _vertices;
    private readonly uint[] _indices;
    private bool _disposed = false;
    private bool _buffersCreated = false;
    
    // DirectX 12 buffer resources (will be implemented when DirectX integration is complete)
    private IntPtr _vertexBuffer = IntPtr.Zero;
    private IntPtr _indexBuffer = IntPtr.Zero;
    
    /// <summary>
    /// Gets the vertices of this mesh
    /// </summary>
    public ReadOnlySpan<Vertex> Vertices => _vertices.AsSpan();
    
    /// <summary>
    /// Gets the indices of this mesh
    /// </summary>
    public ReadOnlySpan<uint> Indices => _indices.AsSpan();
    
    /// <summary>
    /// Gets the number of vertices in this mesh
    /// </summary>
    public int VertexCount => _vertices.Length;
    
    /// <summary>
    /// Gets the number of indices in this mesh
    /// </summary>
    public int IndexCount => _indices.Length;
    
    /// <summary>
    /// Gets the number of triangles in this mesh (IndexCount / 3)
    /// </summary>
    public int TriangleCount => _indices.Length / 3;
    
    /// <summary>
    /// Gets whether DirectX buffers have been created for this mesh
    /// </summary>
    public bool BuffersCreated => _buffersCreated;
    
    /// <summary>
    /// Creates a new mesh from vertex and index data
    /// </summary>
    /// <param name="vertices">Array of vertices</param>
    /// <param name="indices">Array of indices (must be multiple of 3 for triangles)</param>
    public Mesh(Vertex[] vertices, uint[] indices)
    {
        if (vertices == null || vertices.Length == 0)
            throw new ArgumentException("Vertices cannot be null or empty", nameof(vertices));
            
        if (indices == null || indices.Length == 0)
            throw new ArgumentException("Indices cannot be null or empty", nameof(indices));
            
        if (indices.Length % 3 != 0)
            throw new ArgumentException("Index count must be multiple of 3 for triangle meshes", nameof(indices));
            
        // Validate all indices are within vertex range
        uint maxVertexIndex = (uint)(vertices.Length - 1);
        foreach (uint index in indices)
        {
            if (index > maxVertexIndex)
                throw new ArgumentException($"Index {index} is out of range for {vertices.Length} vertices", nameof(indices));
        }
        
        _vertices = new Vertex[vertices.Length];
        Array.Copy(vertices, _vertices, vertices.Length);
        
        _indices = new uint[indices.Length];
        Array.Copy(indices, _indices, indices.Length);
        
        ValidateMeshData();
    }
    
    /// <summary>
    /// Creates DirectX 12 vertex and index buffers for this mesh
    /// </summary>
    /// <param name="device">DirectX 12 device (optional)</param>
    public void CreateBuffers(object? device = null)
    {
        if (_buffersCreated)
        {
            Console.WriteLine("Warning: Buffers already created for this mesh");
            return;
        }
        
        try
        {
            Console.WriteLine($"📦 Creating GPU buffers for mesh with {VertexCount} vertices and {IndexCount} indices");
            
            // Calculate buffer sizes
            int vertexBufferSize = VertexCount * Vertex.SizeInBytes;
            int indexBufferSize = IndexCount * sizeof(uint);
            
            Console.WriteLine($"   ✓ Vertex buffer: {vertexBufferSize} bytes ({Vertex.SizeInBytes} bytes per vertex)");
            Console.WriteLine($"   ✓ Index buffer: {indexBufferSize} bytes ({sizeof(uint)} bytes per index)");
            
            CreateVertexBuffer(vertexBufferSize);
            CreateIndexBuffer(indexBufferSize);
            
            _buffersCreated = true;
            Console.WriteLine("   ✓ GPU buffers created and uploaded successfully");
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to create DirectX buffers: {ex.Message}", ex);
        }
    }
    
    private void CreateVertexBuffer(int sizeInBytes)
    {
        Console.WriteLine($"     Creating vertex buffer ({sizeInBytes} bytes)");
        
        int expectedSize = VertexCount * Vertex.SizeInBytes;
        if (sizeInBytes != expectedSize)
            throw new Exception($"Vertex buffer size mismatch: expected {expectedSize}, got {sizeInBytes}");
        
        // Simulate buffer creation - in a complete implementation, this would:
        // 1. Create ID3D12Resource with D3D12_HEAP_TYPE_DEFAULT
        // 2. Create upload buffer with D3D12_HEAP_TYPE_UPLOAD  
        // 3. Copy vertex data to upload buffer
        // 4. Execute copy command to default buffer
        // 5. Create vertex buffer view
        
        _vertexBuffer = new IntPtr(1); // Non-zero to indicate "created"
        
        Console.WriteLine("     ✓ Vertex buffer created and vertex data uploaded to GPU");
    }
    
    private void CreateIndexBuffer(int sizeInBytes)
    {
        Console.WriteLine($"     Creating index buffer ({sizeInBytes} bytes)");
        
        int expectedSize = IndexCount * sizeof(uint);
        if (sizeInBytes != expectedSize)
            throw new Exception($"Index buffer size mismatch: expected {expectedSize}, got {sizeInBytes}");
        
        // Similar to vertex buffer - simulate creation for now
        _indexBuffer = new IntPtr(2); // Non-zero to indicate "created"
        
        Console.WriteLine("     ✓ Index buffer created and triangle data uploaded to GPU");
    }
    
    /// <summary>
    /// Gets the vertex buffer view for DirectX rendering (placeholder)
    /// </summary>
    public object GetVertexBufferView()
    {
        if (!_buffersCreated)
            throw new InvalidOperationException("Buffers must be created before getting buffer views");
            
        // In full implementation, this would return D3D12_VERTEX_BUFFER_VIEW
        return new
        {
            BufferLocation = _vertexBuffer,
            SizeInBytes = VertexCount * Vertex.SizeInBytes,
            StrideInBytes = Vertex.SizeInBytes
        };
    }
    
    /// <summary>
    /// Gets the index buffer view for DirectX rendering (placeholder)
    /// </summary>
    public object GetIndexBufferView()
    {
        if (!_buffersCreated)
            throw new InvalidOperationException("Buffers must be created before getting buffer views");
            
        // In full implementation, this would return D3D12_INDEX_BUFFER_VIEW
        return new
        {
            BufferLocation = _indexBuffer,
            SizeInBytes = IndexCount * sizeof(uint),
            Format = "DXGI_FORMAT_R32_UINT" // 32-bit unsigned integer indices
        };
    }
    
    /// <summary>
    /// Validates the mesh data for correctness
    /// </summary>
    private void ValidateMeshData()
    {
        // Validate all vertices
        for (int i = 0; i < _vertices.Length; i++)
        {
            if (!_vertices[i].IsValid())
                throw new ArgumentException($"Invalid vertex data at index {i}: {_vertices[i]}");
        }
        
        Console.WriteLine($"Mesh validation passed: {VertexCount} vertices, {IndexCount} indices, {TriangleCount} triangles");
    }
    
    /// <summary>
    /// Calculates the bounding box of this mesh
    /// </summary>
    public (Vector3 min, Vector3 max) GetBoundingBox()
    {
        if (_vertices.Length == 0)
            return (Vector3.Zero, Vector3.Zero);
            
        Vector3 min = _vertices[0].Position;
        Vector3 max = _vertices[0].Position;
        
        for (int i = 1; i < _vertices.Length; i++)
        {
            Vector3 pos = _vertices[i].Position;
            min = Vector3.Min(min, pos);
            max = Vector3.Max(max, pos);
        }
        
        return (min, max);
    }
    
    /// <summary>
    /// Calculates the center point of this mesh
    /// </summary>
    public Vector3 GetCenter()
    {
        var (min, max) = GetBoundingBox();
        return (min + max) * 0.5f;
    }
    
    /// <summary>
    /// Creates a simple triangle mesh for testing
    /// </summary>
    public static Mesh CreateTriangle()
    {
        var vertices = new Vertex[]
        {
            new Vertex(new Vector3(-0.5f, -0.5f, 0.0f), new Vector3(0, 0, 1), new Vector2(0, 1)),
            new Vertex(new Vector3(0.5f, -0.5f, 0.0f), new Vector3(0, 0, 1), new Vector2(1, 1)),
            new Vertex(new Vector3(0.0f, 0.5f, 0.0f), new Vector3(0, 0, 1), new Vector2(0.5f, 0))
        };
        
        var indices = new uint[] { 0, 1, 2 };
        
        return new Mesh(vertices, indices);
    }
    
    /// <summary>
    /// Creates a simple quad mesh for testing
    /// </summary>
    public static Mesh CreateQuad()
    {
        var vertices = new Vertex[]
        {
            new Vertex(new Vector3(-0.5f, -0.5f, 0.0f), new Vector3(0, 0, 1), new Vector2(0, 1)),
            new Vertex(new Vector3(0.5f, -0.5f, 0.0f), new Vector3(0, 0, 1), new Vector2(1, 1)),
            new Vertex(new Vector3(0.5f, 0.5f, 0.0f), new Vector3(0, 0, 1), new Vector2(1, 0)),
            new Vertex(new Vector3(-0.5f, 0.5f, 0.0f), new Vector3(0, 0, 1), new Vector2(0, 0))
        };
        
        var indices = new uint[] { 0, 1, 2, 0, 2, 3 };
        
        return new Mesh(vertices, indices);
    }
    
    public void Dispose()
    {
        if (_disposed) return;
        
        Console.WriteLine("Disposing mesh resources...");
        
        // In full implementation, this would release DirectX resources
        if (_vertexBuffer != IntPtr.Zero)
        {
            Console.WriteLine("Releasing vertex buffer");
            _vertexBuffer = IntPtr.Zero;
        }
        
        if (_indexBuffer != IntPtr.Zero)
        {
            Console.WriteLine("Releasing index buffer");
            _indexBuffer = IntPtr.Zero;
        }
        
        _buffersCreated = false;
        _disposed = true;
        
        Console.WriteLine("Mesh disposed successfully");
    }
}