using System.Numerics;

namespace silken;

/// <summary>
/// Visual quality enhancement utilities for better rendering
/// </summary>
public static class VisualQualityEnhancer
{
    /// <summary>
    /// Enhances scene lighting for better visual quality
    /// </summary>
    public static void EnhanceLighting(DirectXRenderer renderer)
    {
        Console.WriteLine("🌟 Enhancing Scene Lighting Quality...");
        
        // Enhanced directional light with better color temperature
        renderer.SetDirectionalLight(
            direction: Vector3.Normalize(new Vector3(-0.4f, -0.8f, -0.3f)), // Slightly adjusted angle
            color: new Vector3(1.0f, 0.96f, 0.82f), // Warmer sunlight
            intensity: 1.1f // Slightly brighter
        );
        
        // Enhanced ambient lighting for better scene visibility
        renderer.SetAmbientLight(0.28f); // Balanced ambient
        
        // Enhanced specular for more realistic reflections
        renderer.SetSpecularStrength(0.4f);
        
        Console.WriteLine("  ✓ Directional light: Warmer sunlight with optimal angle");
        Console.WriteLine("  ✓ Ambient lighting: Balanced for natural outdoor look");
        Console.WriteLine("  ✓ Specular highlights: Enhanced for realistic reflections");
    }
    
    /// <summary>
    /// Enhances atmospheric effects for better immersion
    /// </summary>
    public static void EnhanceAtmosphere(DirectXRenderer renderer)
    {
        Console.WriteLine("🌫️ Enhancing Atmospheric Effects...");
        
        // Enhanced fog with better color and range
        renderer.SetFogParameters(
            enabled: true,
            start: 18.0f,    // Start fog slightly closer
            end: 65.0f,      // Extend fog range
            color: new Vector3(0.78f, 0.87f, 0.96f) // Slightly warmer atmospheric color
        );
        
        Console.WriteLine("  ✓ Atmospheric fog: Enhanced range and color");
        Console.WriteLine("  ✓ Fog start: 18 units (closer for better depth perception)");
        Console.WriteLine("  ✓ Fog end: 65 units (extended for better atmosphere)");
        Console.WriteLine("  ✓ Fog color: Warmer blue-gray for natural sky");
    }
    
    /// <summary>
    /// Enhances material properties for better visual appeal
    /// </summary>
    public static void EnhanceMaterials(SceneManager sceneManager)
    {
        Console.WriteLine("🎨 Enhancing Material Visual Quality...");
        
        foreach (var material in sceneManager.Materials)
        {
            switch (material.Name)
            {
                case "Wood":
                    // Premium wood material with rich colors
                    material.SetDiffuseColor(new Vector3(0.68f, 0.45f, 0.24f)); // Rich oak brown
                    material.SetAmbientColor(new Vector3(0.28f, 0.20f, 0.10f)); // Deep brown shadows
                    material.SetSpecularColor(new Vector3(0.35f, 0.28f, 0.22f)); // Warm wood highlights
                    material.Shininess = 45.0f; // Polished wood finish
                    Console.WriteLine("  ✓ Wood: Premium oak appearance with polished finish");
                    break;
                    
                case "Grass":
                    // Lush grass material with natural variation
                    material.SetDiffuseColor(new Vector3(0.28f, 0.68f, 0.28f)); // Vibrant green grass
                    material.SetAmbientColor(new Vector3(0.10f, 0.28f, 0.10f)); // Rich green shadows
                    material.SetSpecularColor(new Vector3(0.18f, 0.25f, 0.18f)); // Natural grass highlights
                    material.Shininess = 15.0f; // Natural matte grass finish
                    Console.WriteLine("  ✓ Grass: Lush green with natural matte finish");
                    break;
                    
                case "Corn":
                    // Healthy corn plant material with realistic colors
                    material.SetDiffuseColor(new Vector3(0.48f, 0.78f, 0.28f)); // Healthy corn green
                    material.SetAmbientColor(new Vector3(0.20f, 0.38f, 0.10f)); // Deep plant shadows
                    material.SetSpecularColor(new Vector3(0.38f, 0.45f, 0.28f)); // Natural plant sheen
                    material.Shininess = 52.0f; // Healthy plant surface
                    Console.WriteLine("  ✓ Corn: Healthy plant appearance with natural sheen");
                    break;
            }
        }
    }
    
    /// <summary>
    /// Enhances scene composition for better visual balance
    /// </summary>
    public static void EnhanceSceneComposition(SceneManager sceneManager, Camera camera)
    {
        Console.WriteLine("🏞️ Enhancing Scene Composition...");
        
        // Optimize camera position for best scene view
        var optimalPosition = new Vector3(0, 2.5f, 8); // Slightly elevated and back
        var optimalRotation = new Vector3(-0.1f, 0, 0); // Slight downward angle
        
        if (Vector3.Distance(camera.Position, optimalPosition) > 1.0f)
        {
            Console.WriteLine($"  ✓ Camera positioned for optimal scene view: {optimalPosition}");
            camera.Position = optimalPosition;
            camera.Rotation = optimalRotation;
        }
        
        // Enhance object positioning for better visual balance
        var farmHouse = sceneManager.GameObjects.FirstOrDefault(obj => obj.Name == "FarmHouse");
        if (farmHouse != null)
        {
            // Ensure farm house is prominently positioned
            farmHouse.Position = new Vector3(0, 0, 0);
            farmHouse.Scale = new Vector3(1.1f, 1.1f, 1.1f); // Slightly larger for prominence
            Console.WriteLine("  ✓ Farm house: Enhanced size and central positioning");
        }
        
        // Enhance corn field layout for better visual appeal
        int enhancedCornStalks = 0;
        foreach (var obj in sceneManager.GameObjects.Where(o => o.Name.StartsWith("CornStalk")))
        {
            // Add slight height variation for more natural look
            var baseScale = obj.Scale.X;
            var heightVariation = (float)(Random.Shared.NextDouble() * 0.3 + 0.85); // 0.85 to 1.15 range
            obj.Scale = new Vector3(baseScale, baseScale * heightVariation, baseScale);
            enhancedCornStalks++;
        }
        
        Console.WriteLine($"  ✓ Corn field: Added natural height variation to {enhancedCornStalks} stalks");
    }
    
    /// <summary>
    /// Applies comprehensive visual quality enhancements
    /// </summary>
    public static void EnhanceAll(DirectXRenderer renderer, SceneManager sceneManager, Camera camera)
    {
        Console.WriteLine("🎯 Applying Comprehensive Visual Quality Enhancements...");
        Console.WriteLine();
        
        EnhanceLighting(renderer);
        Console.WriteLine();
        
        EnhanceAtmosphere(renderer);
        Console.WriteLine();
        
        EnhanceMaterials(sceneManager);
        Console.WriteLine();
        
        EnhanceSceneComposition(sceneManager, camera);
        Console.WriteLine();
        
        Console.WriteLine("✨ Visual Quality Enhancement Complete!");
        Console.WriteLine("  🌟 Enhanced lighting with warmer sunlight");
        Console.WriteLine("  🌫️ Improved atmospheric fog effects");
        Console.WriteLine("  🎨 Premium material appearances");
        Console.WriteLine("  🏞️ Optimized scene composition");
        Console.WriteLine("  📸 Camera positioned for best view");
    }
    
    /// <summary>
    /// Validates visual quality settings
    /// </summary>
    public static void ValidateVisualQuality(DirectXRenderer renderer, SceneManager sceneManager)
    {
        Console.WriteLine("🔍 Validating Visual Quality Settings...");
        
        var stats = sceneManager.GetStatistics();
        
        // Check scene complexity
        if (stats.TotalTriangles > 500)
        {
            Console.WriteLine("  ⚠️  High triangle count detected - consider LOD optimization");
        }
        else
        {
            Console.WriteLine("  ✅ Triangle count optimal for performance");
        }
        
        // Check material count
        if (stats.TotalMaterials > 5)
        {
            Console.WriteLine("  ⚠️  Many materials detected - consider material atlasing");
        }
        else
        {
            Console.WriteLine("  ✅ Material count optimal");
        }
        
        // Check object count
        if (stats.RenderableObjects > 100)
        {
            Console.WriteLine("  ⚠️  High object count - consider instancing");
        }
        else
        {
            Console.WriteLine("  ✅ Object count manageable");
        }
        
        Console.WriteLine("🎯 Visual Quality Validation Complete");
    }
}