using System.Numerics;
using Xunit;

namespace MySilkProgram.Tests;

public class FarmBuildingTests : IDisposable
{
    private readonly List<Mesh> _createdMeshes = new();
    private readonly List<GameObject> _createdObjects = new();
    
    [Fact]
    public void CreateFarmHouse_WithDefaultParameters_CreatesValidMesh()
    {
        var mesh = FarmBuilding.CreateFarmHouse();
        _createdMeshes.Add(mesh);
        
        Assert.NotNull(mesh);
        Assert.True(mesh.VertexCount > 0);
        Assert.True(mesh.IndexCount > 0);
        Assert.True(mesh.TriangleCount > 0);
        
        // A house with cube base and triangular roof should have a reasonable number of vertices
        // Cube: 24 vertices (4 per face * 6 faces), Roof: ~14 vertices
        Assert.True(mesh.VertexCount >= 30, $"Expected at least 30 vertices, got {mesh.VertexCount}");
        
        // Should have multiple triangles for walls and roof
        Assert.True(mesh.TriangleCount >= 16, $"Expected at least 16 triangles, got {mesh.TriangleCount}");
    }
    
    [Fact]
    public void CreateFarmHouse_WithCustomDimensions_CreatesCorrectSizedMesh()
    {
        float width = 6.0f, height = 4.0f, depth = 5.0f, roofHeight = 2.5f;
        
        var mesh = FarmBuilding.CreateFarmHouse(width, height, depth, roofHeight);
        _createdMeshes.Add(mesh);
        
        Assert.NotNull(mesh);
        
        // Check that the mesh has reasonable bounds based on input dimensions
        var (min, max) = mesh.GetBoundingBox();
        
        // Width check (X axis)
        float actualWidth = max.X - min.X;
        Assert.True(Math.Abs(actualWidth - width) < 0.1f, 
            $"Expected width ~{width}, got {actualWidth}");
        
        // Total height check (Y axis) - should include roof
        float actualHeight = max.Y - min.Y;
        float expectedTotalHeight = height + roofHeight;
        Assert.True(Math.Abs(actualHeight - expectedTotalHeight) < 0.1f, 
            $"Expected total height ~{expectedTotalHeight}, got {actualHeight}");
        
        // Depth check (Z axis)
        float actualDepth = max.Z - min.Z;
        Assert.True(Math.Abs(actualDepth - depth) < 0.1f, 
            $"Expected depth ~{depth}, got {actualDepth}");
    }
    
    [Fact]
    public void CreateBarn_CreatesLargerMeshThanHouse()
    {
        var house = FarmBuilding.CreateFarmHouse();
        var barn = FarmBuilding.CreateBarn();
        
        _createdMeshes.Add(house);
        _createdMeshes.Add(barn);
        
        var (houseMin, houseMax) = house.GetBoundingBox();
        var (barnMin, barnMax) = barn.GetBoundingBox();
        
        float houseVolume = (houseMax.X - houseMin.X) * (houseMax.Y - houseMin.Y) * (houseMax.Z - houseMin.Z);
        float barnVolume = (barnMax.X - barnMin.X) * (barnMax.Y - barnMin.Y) * (barnMax.Z - barnMin.Z);
        
        // Barn should be larger than house
        Assert.True(barnVolume > houseVolume, 
            $"Barn volume ({barnVolume}) should be larger than house volume ({houseVolume})");
    }
    
    [Fact]
    public void CreateShed_CreatesSmallerMeshThanHouse()
    {
        var house = FarmBuilding.CreateFarmHouse();
        var shed = FarmBuilding.CreateShed();
        
        _createdMeshes.Add(house);
        _createdMeshes.Add(shed);
        
        var (houseMin, houseMax) = house.GetBoundingBox();
        var (shedMin, shedMax) = shed.GetBoundingBox();
        
        float houseVolume = (houseMax.X - houseMin.X) * (houseMax.Y - houseMin.Y) * (houseMax.Z - houseMin.Z);
        float shedVolume = (shedMax.X - shedMin.X) * (shedMax.Y - shedMin.Y) * (shedMax.Z - shedMin.Z);
        
        // Shed should be smaller than house
        Assert.True(shedVolume < houseVolume, 
            $"Shed volume ({shedVolume}) should be smaller than house volume ({houseVolume})");
    }
    
    [Fact]
    public void CreateFarmHouse_GeneratesValidVertexData()
    {
        var mesh = FarmBuilding.CreateFarmHouse();
        _createdMeshes.Add(mesh);
        
        // Check that all vertices are valid
        foreach (var vertex in mesh.Vertices)
        {
            Assert.True(vertex.IsValid(), $"Invalid vertex: {vertex}");
            
            // Check that normals are normalized (length close to 1)
            float normalLength = vertex.Normal.Length();
            Assert.True(Math.Abs(normalLength - 1.0f) < 0.1f, 
                $"Normal not normalized: {vertex.Normal}, length: {normalLength}");
            
            // Check that texture coordinates are in valid range
            Assert.True(vertex.TexCoords.X >= 0 && vertex.TexCoords.X <= 1, 
                $"Invalid texture U coordinate: {vertex.TexCoords.X}");
            Assert.True(vertex.TexCoords.Y >= 0 && vertex.TexCoords.Y <= 1, 
                $"Invalid texture V coordinate: {vertex.TexCoords.Y}");
        }
    }
    
    [Fact]
    public void CreateFarmHouse_GeneratesValidIndexData()
    {
        var mesh = FarmBuilding.CreateFarmHouse();
        _createdMeshes.Add(mesh);
        
        // Check that all indices are valid
        uint maxIndex = (uint)(mesh.VertexCount - 1);
        foreach (uint index in mesh.Indices)
        {
            Assert.True(index <= maxIndex, 
                $"Index {index} out of range for {mesh.VertexCount} vertices");
        }
        
        // Check that indices form complete triangles
        Assert.True(mesh.IndexCount % 3 == 0, 
            $"Index count {mesh.IndexCount} is not divisible by 3");
    }
    
    [Fact]
    public void CreateFarmBuildingObject_WithHouseType_CreatesValidGameObject()
    {
        var position = new Vector3(5, 0, 10);
        var gameObject = FarmBuilding.CreateFarmBuildingObject("TestHouse", FarmBuildingType.House, position);
        _createdObjects.Add(gameObject);
        
        Assert.NotNull(gameObject);
        Assert.Equal("TestHouse", gameObject.Name);
        Assert.Equal(position, gameObject.Position);
        Assert.NotNull(gameObject.Mesh);
        Assert.NotNull(gameObject.Material);
        Assert.True(gameObject.IsValidForRendering());
        
        // Should have wood material
        Assert.Equal("Wood", gameObject.Material.Name);
    }
    
    [Fact]
    public void CreateFarmBuildingObject_WithBarnType_CreatesValidGameObject()
    {
        var position = new Vector3(-5, 0, -10);
        var gameObject = FarmBuilding.CreateFarmBuildingObject("TestBarn", FarmBuildingType.Barn, position);
        _createdObjects.Add(gameObject);
        
        Assert.NotNull(gameObject);
        Assert.Equal("TestBarn", gameObject.Name);
        Assert.Equal(position, gameObject.Position);
        Assert.NotNull(gameObject.Mesh);
        Assert.NotNull(gameObject.Material);
        Assert.True(gameObject.IsValidForRendering());
    }
    
    [Fact]
    public void CreateFarmBuildingObject_WithShedType_CreatesValidGameObject()
    {
        var position = new Vector3(0, 0, 5);
        var gameObject = FarmBuilding.CreateFarmBuildingObject("TestShed", FarmBuildingType.Shed, position);
        _createdObjects.Add(gameObject);
        
        Assert.NotNull(gameObject);
        Assert.Equal("TestShed", gameObject.Name);
        Assert.Equal(position, gameObject.Position);
        Assert.NotNull(gameObject.Mesh);
        Assert.NotNull(gameObject.Material);
        Assert.True(gameObject.IsValidForRendering());
    }
    
    [Fact]
    public void CreateFarmHouse_WithZeroDimensions_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => FarmBuilding.CreateFarmHouse(0, 1, 1, 1));
        Assert.Throws<ArgumentException>(() => FarmBuilding.CreateFarmHouse(1, 0, 1, 1));
        Assert.Throws<ArgumentException>(() => FarmBuilding.CreateFarmHouse(1, 1, 0, 1));
    }
    
    [Fact]
    public void CreateFarmHouse_WithNegativeDimensions_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => FarmBuilding.CreateFarmHouse(-1, 1, 1, 1));
        Assert.Throws<ArgumentException>(() => FarmBuilding.CreateFarmHouse(1, -1, 1, 1));
        Assert.Throws<ArgumentException>(() => FarmBuilding.CreateFarmHouse(1, 1, -1, 1));
        Assert.Throws<ArgumentException>(() => FarmBuilding.CreateFarmHouse(1, 1, 1, -1));
    }
    
    [Fact]
    public void FarmBuildingType_EnumHasExpectedValues()
    {
        // Verify that the enum has the expected values
        Assert.True(Enum.IsDefined(typeof(FarmBuildingType), FarmBuildingType.House));
        Assert.True(Enum.IsDefined(typeof(FarmBuildingType), FarmBuildingType.Barn));
        Assert.True(Enum.IsDefined(typeof(FarmBuildingType), FarmBuildingType.Shed));
        
        // Verify enum values
        Assert.Equal(0, (int)FarmBuildingType.House);
        Assert.Equal(1, (int)FarmBuildingType.Barn);
        Assert.Equal(2, (int)FarmBuildingType.Shed);
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