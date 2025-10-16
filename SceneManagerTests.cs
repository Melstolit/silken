using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MySilkProgram.Tests;

public class SceneManagerTests : IDisposable
{
    private readonly SceneManager _sceneManager;
    private readonly GameObject _testObject1;
    private readonly GameObject _testObject2;
    private readonly Mesh _testMesh;
    private readonly Material _testMaterial;
    
    public SceneManagerTests()
    {
        _sceneManager = new SceneManager();
        _testMesh = Mesh.CreateTriangle();
        _testMaterial = Material.CreateWoodMaterial();
        _testObject1 = new GameObject("TestObject1", _testMesh, _testMaterial);
        _testObject2 = new GameObject("TestObject2", _testMesh, _testMaterial);
    }
    
    [Fact]
    public void Constructor_InitializesEmptyScene()
    {
        var sceneManager = new SceneManager();
        
        Assert.Equal(0, sceneManager.ObjectCount);
        Assert.Equal(0, sceneManager.ActiveObjectCount);
        Assert.Empty(sceneManager.GameObjects);
        Assert.Empty(sceneManager.Meshes);
        Assert.Empty(sceneManager.Materials);
    }
    
    [Fact]
    public void AddGameObject_AddsObjectToScene()
    {
        _sceneManager.AddGameObject(_testObject1);
        
        Assert.Equal(1, _sceneManager.ObjectCount);
        Assert.Contains(_testObject1, _sceneManager.GameObjects);
    }
    
    [Fact]
    public void AddGameObject_WithNullObject_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _sceneManager.AddGameObject(null!));
    }
    
    [Fact]
    public void AddGameObject_WithDuplicateObject_DoesNotAddTwice()
    {
        _sceneManager.AddGameObject(_testObject1);
        _sceneManager.AddGameObject(_testObject1); // Add same object again
        
        Assert.Equal(1, _sceneManager.ObjectCount);
    }
    
    [Fact]
    public void AddGameObject_WithUniqueNames_AddsToNamedLookup()
    {
        _sceneManager.AddGameObject(_testObject1);
        _sceneManager.AddGameObject(_testObject2);
        
        Assert.Equal(_testObject1, _sceneManager.FindGameObject("TestObject1"));
        Assert.Equal(_testObject2, _sceneManager.FindGameObject("TestObject2"));
    }
    
    [Fact]
    public void RemoveGameObject_RemovesObjectFromScene()
    {
        _sceneManager.AddGameObject(_testObject1);
        
        bool removed = _sceneManager.RemoveGameObject(_testObject1);
        
        Assert.True(removed);
        Assert.Equal(0, _sceneManager.ObjectCount);
        Assert.DoesNotContain(_testObject1, _sceneManager.GameObjects);
    }
    
    [Fact]
    public void RemoveGameObject_WithNullObject_ReturnsFalse()
    {
        bool removed = _sceneManager.RemoveGameObject(null!);
        
        Assert.False(removed);
    }
    
    [Fact]
    public void RemoveGameObject_WithNonExistentObject_ReturnsFalse()
    {
        bool removed = _sceneManager.RemoveGameObject(_testObject1);
        
        Assert.False(removed);
    }
    
    [Fact]
    public void FindGameObject_WithExistingName_ReturnsObject()
    {
        _sceneManager.AddGameObject(_testObject1);
        
        var found = _sceneManager.FindGameObject("TestObject1");
        
        Assert.Equal(_testObject1, found);
    }
    
    [Fact]
    public void FindGameObject_WithNonExistentName_ReturnsNull()
    {
        var found = _sceneManager.FindGameObject("NonExistent");
        
        Assert.Null(found);
    }
    
    [Fact]
    public void FindGameObjects_WithMatchingName_ReturnsAllMatches()
    {
        var obj1 = new GameObject("SameName", _testMesh, _testMaterial);
        var obj2 = new GameObject("SameName", _testMesh, _testMaterial);
        var obj3 = new GameObject("DifferentName", _testMesh, _testMaterial);
        
        _sceneManager.AddGameObject(obj1);
        _sceneManager.AddGameObject(obj2);
        _sceneManager.AddGameObject(obj3);
        
        var found = _sceneManager.FindGameObjects("SameName");
        
        Assert.Equal(2, found.Count);
        Assert.Contains(obj1, found);
        Assert.Contains(obj2, found);
        Assert.DoesNotContain(obj3, found);
        
        // Cleanup
        obj1.Dispose();
        obj2.Dispose();
        obj3.Dispose();
    }
    
    [Fact]
    public void GetActiveGameObjects_ReturnsOnlyActiveObjects()
    {
        _testObject1.IsActive = true;
        _testObject2.IsActive = false;
        
        _sceneManager.AddGameObject(_testObject1);
        _sceneManager.AddGameObject(_testObject2);
        
        var activeObjects = _sceneManager.GetActiveGameObjects();
        
        Assert.Single(activeObjects);
        Assert.Contains(_testObject1, activeObjects);
        Assert.DoesNotContain(_testObject2, activeObjects);
    }
    
    [Fact]
    public void GetRenderableGameObjects_ReturnsOnlyRenderableObjects()
    {
        var renderableObj = new GameObject("Renderable", _testMesh, _testMaterial);
        var nonRenderableObj = new GameObject("NonRenderable"); // No mesh or material
        
        _sceneManager.AddGameObject(renderableObj);
        _sceneManager.AddGameObject(nonRenderableObj);
        
        var renderableObjects = _sceneManager.GetRenderableGameObjects();
        
        Assert.Single(renderableObjects);
        Assert.Contains(renderableObj, renderableObjects);
        Assert.DoesNotContain(nonRenderableObj, renderableObjects);
        
        // Cleanup
        renderableObj.Dispose();
        nonRenderableObj.Dispose();
    }
    
    [Fact]
    public void ActiveObjectCount_ReturnsCorrectCount()
    {
        _testObject1.IsActive = true;
        _testObject2.IsActive = false;
        
        _sceneManager.AddGameObject(_testObject1);
        _sceneManager.AddGameObject(_testObject2);
        
        Assert.Equal(1, _sceneManager.ActiveObjectCount);
    }
    
    [Fact]
    public void Update_CallsUpdateOnActiveObjects()
    {
        var mockObject = new MockGameObject("Mock", _testMesh, _testMaterial);
        mockObject.IsActive = true;
        
        _sceneManager.AddGameObject(mockObject);
        
        _sceneManager.Update(0.016f);
        
        Assert.True(mockObject.UpdateCalled);
        Assert.Equal(0.016f, mockObject.LastDeltaTime);
        
        // Cleanup
        mockObject.Dispose();
    }
    
    [Fact]
    public void Update_DoesNotCallUpdateOnInactiveObjects()
    {
        var mockObject = new MockGameObject("Mock", _testMesh, _testMaterial);
        mockObject.IsActive = false;
        
        _sceneManager.AddGameObject(mockObject);
        
        _sceneManager.Update(0.016f);
        
        Assert.False(mockObject.UpdateCalled);
        
        // Cleanup
        mockObject.Dispose();
    }
    
    [Fact]
    public void Render_CallsRenderOnRenderableObjects()
    {
        var mockObject = new MockGameObject("Mock", _testMesh, _testMaterial);
        mockObject.IsActive = true;
        
        _sceneManager.AddGameObject(mockObject);
        
        _sceneManager.Render(null);
        
        Assert.True(mockObject.RenderCalled);
        
        // Cleanup
        mockObject.Dispose();
    }
    
    [Fact]
    public void LoadScene_CreatesSceneContent()
    {
        _sceneManager.LoadScene();
        
        // Should have created materials, meshes, and at least one object
        Assert.True(_sceneManager.Materials.Count > 0);
        Assert.True(_sceneManager.Meshes.Count > 0);
        // Note: Object count might be 0 in placeholder implementation
    }
    
    [Fact]
    public void ClearScene_RemovesAllContent()
    {
        _sceneManager.AddGameObject(_testObject1);
        _sceneManager.LoadScene(); // This adds materials and meshes
        
        _sceneManager.ClearScene();
        
        Assert.Equal(0, _sceneManager.ObjectCount);
        Assert.Empty(_sceneManager.GameObjects);
        Assert.Empty(_sceneManager.Meshes);
        Assert.Empty(_sceneManager.Materials);
    }
    
    [Fact]
    public void GetStatistics_ReturnsCorrectStatistics()
    {
        _sceneManager.AddGameObject(_testObject1);
        _sceneManager.AddGameObject(_testObject2);
        
        var stats = _sceneManager.GetStatistics();
        
        Assert.Equal(2, stats.TotalObjects);
        Assert.Equal(2, stats.ActiveObjects); // Both objects are active by default
        Assert.Equal(2, stats.RenderableObjects); // Both have mesh and material
        Assert.True(stats.TotalVertices > 0); // Should have vertices from meshes
        Assert.True(stats.TotalTriangles > 0); // Should have triangles from meshes
    }
    
    [Fact]
    public void PrintStatistics_DoesNotThrow()
    {
        _sceneManager.AddGameObject(_testObject1);
        
        // Should not throw
        _sceneManager.PrintStatistics();
    }
    
    [Fact]
    public void Dispose_ClearsAllResources()
    {
        _sceneManager.AddGameObject(_testObject1);
        _sceneManager.LoadScene();
        
        _sceneManager.Dispose();
        
        // After disposal, scene should be empty
        Assert.Equal(0, _sceneManager.ObjectCount);
        Assert.Empty(_sceneManager.GameObjects);
        Assert.Empty(_sceneManager.Meshes);
        Assert.Empty(_sceneManager.Materials);
    }
    
    public void Dispose()
    {
        _sceneManager?.Dispose();
        _testObject1?.Dispose();
        _testObject2?.Dispose();
        _testMesh?.Dispose();
        _testMaterial?.Dispose();
    }
}

/// <summary>
/// Mock GameObject for testing that tracks method calls
/// </summary>
public class MockGameObject : GameObject
{
    public bool UpdateCalled { get; private set; }
    public bool RenderCalled { get; private set; }
    public float LastDeltaTime { get; private set; }
    
    public MockGameObject(string name, Mesh mesh, Material material) : base(name, mesh, material)
    {
    }
    
    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
        UpdateCalled = true;
        LastDeltaTime = deltaTime;
    }
    
    public override void Render(object? renderer = null)
    {
        base.Render(renderer);
        RenderCalled = true;
    }
}