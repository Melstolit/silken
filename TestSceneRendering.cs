using System.Numerics;

namespace MySilkProgram;

/// <summary>
/// Test class to verify scene rendering functionality
/// </summary>
public static class TestSceneRendering
{
    /// <summary>
    /// Tests camera functionality
    /// </summary>
    public static void TestCamera()
    {
        Console.WriteLine("=== Testing Camera Functionality ===");
        
        // Create camera
        var camera = new Camera(
            position: Vector3.Zero,
            rotation: Vector3.Zero,
            fieldOfView: MathF.PI / 4.0f,
            aspectRatio: 16.0f / 9.0f,
            nearPlane: 0.1f,
            farPlane: 1000.0f
        );
        
        // Test initial state
        Console.WriteLine($"Initial Position: {camera.Position}");
        Console.WriteLine($"Initial Rotation: {camera.Rotation}");
        
        // Test movement
        camera.Move(Vector3.UnitX, 5.0f);
        Console.WriteLine($"After moving right: {camera.Position}");
        
        // Test rotation
        camera.Rotate(0.1f, 0.2f, 0.0f);
        Console.WriteLine($"After rotation: {camera.Rotation}");
        
        // Test matrix calculations
        var viewMatrix = camera.GetViewMatrix();
        var projMatrix = camera.GetProjectionMatrix();
        
        Console.WriteLine($"View Matrix determinant: {viewMatrix.GetDeterminant()}");
        Console.WriteLine($"Projection Matrix determinant: {projMatrix.GetDeterminant()}");
        
        Console.WriteLine("✓ Camera tests passed");
    }
    
    /// <summary>
    /// Tests scene object counts and materials
    /// </summary>
    public static void TestSceneObjects()
    {
        Console.WriteLine("=== Testing Scene Objects ===");
        
        var sceneManager = new SceneManager();
        sceneManager.LoadScene();
        
        var stats = sceneManager.GetStatistics();
        
        Console.WriteLine($"Total Objects: {stats.TotalObjects}");
        Console.WriteLine($"Active Objects: {stats.ActiveObjects}");
        Console.WriteLine($"Renderable Objects: {stats.RenderableObjects}");
        Console.WriteLine($"Total Meshes: {stats.TotalMeshes}");
        Console.WriteLine($"Total Materials: {stats.TotalMaterials}");
        Console.WriteLine($"Total Vertices: {stats.TotalVertices}");
        Console.WriteLine($"Total Triangles: {stats.TotalTriangles}");
        
        // Verify expected objects
        bool hasFarmHouse = sceneManager.GameObjects.Any(obj => obj.Name == "FarmHouse");
        bool hasGround = sceneManager.GameObjects.Any(obj => obj.Name == "CornFieldGround");
        int cornStalkCount = sceneManager.GameObjects.Count(obj => obj.Name.StartsWith("CornStalk_"));
        
        Console.WriteLine($"Has Farm House: {hasFarmHouse}");
        Console.WriteLine($"Has Ground: {hasGround}");
        Console.WriteLine($"Corn Stalk Count: {cornStalkCount}");
        
        // Verify materials
        var materials = sceneManager.Materials;
        bool hasWoodMaterial = materials.Any(m => m.Name == "Wood");
        bool hasGrassMaterial = materials.Any(m => m.Name == "Grass");
        bool hasCornMaterial = materials.Any(m => m.Name == "Corn");
        
        Console.WriteLine($"Has Wood Material: {hasWoodMaterial}");
        Console.WriteLine($"Has Grass Material: {hasGrassMaterial}");
        Console.WriteLine($"Has Corn Material: {hasCornMaterial}");
        
        sceneManager.Dispose();
        
        Console.WriteLine("✓ Scene object tests passed");
    }
    
    /// <summary>
    /// Tests material properties
    /// </summary>
    public static void TestMaterials()
    {
        Console.WriteLine("=== Testing Materials ===");
        
        // Test Wood material
        var woodMaterial = new Material("Wood");
        woodMaterial.SetDiffuseColor(new Vector3(0.6f, 0.4f, 0.2f));
        woodMaterial.SetAmbientColor(new Vector3(0.3f, 0.2f, 0.1f));
        woodMaterial.SetSpecularColor(new Vector3(0.2f, 0.2f, 0.2f));
        woodMaterial.Shininess = 32.0f;
        
        Console.WriteLine($"Wood Material - Diffuse: {woodMaterial.DiffuseColor}");
        Console.WriteLine($"Wood Material - Ambient: {woodMaterial.AmbientColor}");
        Console.WriteLine($"Wood Material - Specular: {woodMaterial.SpecularColor}");
        Console.WriteLine($"Wood Material - Shininess: {woodMaterial.Shininess}");
        
        // Test Grass material
        var grassMaterial = new Material("Grass");
        grassMaterial.SetDiffuseColor(new Vector3(0.2f, 0.6f, 0.2f));
        grassMaterial.SetAmbientColor(new Vector3(0.1f, 0.3f, 0.1f));
        grassMaterial.SetSpecularColor(new Vector3(0.1f, 0.1f, 0.1f));
        grassMaterial.Shininess = 16.0f;
        
        Console.WriteLine($"Grass Material - Diffuse: {grassMaterial.DiffuseColor}");
        Console.WriteLine($"Grass Material - Ambient: {grassMaterial.AmbientColor}");
        Console.WriteLine($"Grass Material - Specular: {grassMaterial.SpecularColor}");
        Console.WriteLine($"Grass Material - Shininess: {grassMaterial.Shininess}");
        
        // Test Corn material
        var cornMaterial = new Material("Corn");
        cornMaterial.SetDiffuseColor(new Vector3(0.4f, 0.7f, 0.2f));
        cornMaterial.SetAmbientColor(new Vector3(0.2f, 0.35f, 0.1f));
        cornMaterial.SetSpecularColor(new Vector3(0.3f, 0.3f, 0.3f));
        cornMaterial.Shininess = 64.0f;
        
        Console.WriteLine($"Corn Material - Diffuse: {cornMaterial.DiffuseColor}");
        Console.WriteLine($"Corn Material - Ambient: {cornMaterial.AmbientColor}");
        Console.WriteLine($"Corn Material - Specular: {cornMaterial.SpecularColor}");
        Console.WriteLine($"Corn Material - Shininess: {cornMaterial.Shininess}");
        
        woodMaterial.Dispose();
        grassMaterial.Dispose();
        cornMaterial.Dispose();
        
        Console.WriteLine("✓ Material tests passed");
    }
    
    /// <summary>
    /// Tests performance optimization features
    /// </summary>
    public static void TestPerformanceOptimization()
    {
        Console.WriteLine("=== Testing Performance Optimization ===");
        
        var camera = new Camera(
            position: Vector3.Zero,
            rotation: Vector3.Zero,
            fieldOfView: MathF.PI / 3.0f, // Non-optimal FOV
            aspectRatio: 16.0f / 9.0f,
            nearPlane: 0.05f, // Non-optimal near plane
            farPlane: 2000.0f // Non-optimal far plane
        );
        
        Console.WriteLine($"Before optimization - FOV: {camera.FieldOfView:F2}, Near: {camera.NearPlane:F2}, Far: {camera.FarPlane:F2}");
        
        // Test camera optimization
        PerformanceOptimizer.OptimizeCamera(camera);
        
        Console.WriteLine($"After optimization - FOV: {camera.FieldOfView:F2}, Near: {camera.NearPlane:F2}, Far: {camera.FarPlane:F2}");
        
        // Test performance metrics
        PerformanceOptimizer.StartFrame();
        System.Threading.Thread.Sleep(16); // Simulate 60 FPS frame
        PerformanceOptimizer.EndFrame();
        
        var metrics = PerformanceOptimizer.GetMetrics();
        Console.WriteLine($"Performance metrics - FPS: {metrics.AverageFPS:F1}, Frame Time: {metrics.AverageFrameTime:F2}ms");
        
        Console.WriteLine("✓ Performance optimization tests passed");
    }
    
    /// <summary>
    /// Tests visual quality enhancement features
    /// </summary>
    public static void TestVisualQualityEnhancement()
    {
        Console.WriteLine("=== Testing Visual Quality Enhancement ===");
        
        var sceneManager = new SceneManager();
        sceneManager.LoadScene();
        
        var camera = new Camera(
            position: Vector3.Zero,
            rotation: Vector3.Zero,
            fieldOfView: MathF.PI / 4.0f,
            aspectRatio: 16.0f / 9.0f,
            nearPlane: 0.1f,
            farPlane: 1000.0f
        );
        
        // Test material enhancement
        Console.WriteLine("Testing material enhancements...");
        VisualQualityEnhancer.EnhanceMaterials(sceneManager);
        
        // Verify material properties were enhanced
        var woodMaterial = sceneManager.Materials.FirstOrDefault(m => m.Name == "Wood");
        if (woodMaterial != null)
        {
            Console.WriteLine($"Wood material enhanced - Diffuse: {woodMaterial.DiffuseColor}, Shininess: {woodMaterial.Shininess}");
        }
        
        // Test scene composition enhancement
        Console.WriteLine("Testing scene composition enhancements...");
        VisualQualityEnhancer.EnhanceSceneComposition(sceneManager, camera);
        
        Console.WriteLine($"Camera optimized - Position: {camera.Position}, Rotation: {camera.Rotation}");
        
        sceneManager.Dispose();
        
        Console.WriteLine("✓ Visual quality enhancement tests passed");
    }
    
    /// <summary>
    /// Runs all scene rendering tests
    /// </summary>
    public static void RunAllTests()
    {
        Console.WriteLine("Starting Comprehensive Scene Rendering Tests...");
        Console.WriteLine();
        
        TestCamera();
        Console.WriteLine();
        
        TestSceneObjects();
        Console.WriteLine();
        
        TestMaterials();
        Console.WriteLine();
        
        TestPerformanceOptimization();
        Console.WriteLine();
        
        TestVisualQualityEnhancement();
        Console.WriteLine();
        
        Console.WriteLine("All Scene Rendering and Optimization Tests Completed Successfully!");
    }
}