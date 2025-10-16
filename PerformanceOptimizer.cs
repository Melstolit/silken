using System.Numerics;
using System.Diagnostics;

namespace MySilkProgram;

/// <summary>
/// Performance optimization utilities for the 3D engine
/// </summary>
public static class PerformanceOptimizer
{
    private static readonly Stopwatch _frameTimer = new();
    private static readonly Queue<double> _frameTimeHistory = new();
    private static double _averageFrameTime = 0.0;
    private static int _framesSinceLastOptimization = 0;
    private static readonly int MaxFrameHistory = 60; // Track last 60 frames
    
    /// <summary>
    /// Performance metrics for monitoring
    /// </summary>
    public struct PerformanceMetrics
    {
        public double AverageFrameTime;
        public double AverageFPS;
        public double MinFrameTime;
        public double MaxFrameTime;
        public int TotalObjects;
        public int RenderedObjects;
        public int TotalVertices;
        public int TotalTriangles;
        public double CPUTime;
        public double RenderTime;
    }
    
    /// <summary>
    /// Starts frame timing
    /// </summary>
    public static void StartFrame()
    {
        _frameTimer.Restart();
    }
    
    /// <summary>
    /// Ends frame timing and updates metrics
    /// </summary>
    public static void EndFrame()
    {
        _frameTimer.Stop();
        var frameTime = _frameTimer.Elapsed.TotalMilliseconds;
        
        // Add to history
        _frameTimeHistory.Enqueue(frameTime);
        if (_frameTimeHistory.Count > MaxFrameHistory)
        {
            _frameTimeHistory.Dequeue();
        }
        
        // Update average
        _averageFrameTime = _frameTimeHistory.Average();
        _framesSinceLastOptimization++;
    }
    
    /// <summary>
    /// Gets current performance metrics
    /// </summary>
    public static PerformanceMetrics GetMetrics(SceneManager? sceneManager = null)
    {
        var stats = sceneManager?.GetStatistics();
        
        return new PerformanceMetrics
        {
            AverageFrameTime = _averageFrameTime,
            AverageFPS = _averageFrameTime > 0 ? 1000.0 / _averageFrameTime : 0,
            MinFrameTime = _frameTimeHistory.Count > 0 ? _frameTimeHistory.Min() : 0,
            MaxFrameTime = _frameTimeHistory.Count > 0 ? _frameTimeHistory.Max() : 0,
            TotalObjects = stats?.TotalObjects ?? 0,
            RenderedObjects = stats?.RenderableObjects ?? 0,
            TotalVertices = stats?.TotalVertices ?? 0,
            TotalTriangles = stats?.TotalTriangles ?? 0,
            CPUTime = _averageFrameTime,
            RenderTime = _averageFrameTime * 0.7 // Estimate 70% render time
        };
    }
    
    /// <summary>
    /// Optimizes camera settings for better performance
    /// </summary>
    public static void OptimizeCamera(Camera camera)
    {
        // Optimize field of view for performance vs quality balance
        var currentFOV = camera.FieldOfView;
        var optimalFOV = MathF.PI / 4.0f; // 45 degrees is optimal
        
        if (Math.Abs(currentFOV - optimalFOV) > 0.1f)
        {
            Console.WriteLine($"Optimizing camera FOV from {currentFOV:F2} to {optimalFOV:F2}");
            camera.FieldOfView = optimalFOV;
        }
        
        // Optimize near/far planes for better depth precision
        if (camera.NearPlane < 0.1f)
        {
            Console.WriteLine($"Optimizing near plane from {camera.NearPlane:F2} to 0.1");
            camera.NearPlane = 0.1f;
        }
        
        if (camera.FarPlane > 1000.0f)
        {
            Console.WriteLine($"Optimizing far plane from {camera.FarPlane:F2} to 1000.0");
            camera.FarPlane = 1000.0f;
        }
    }
    
    /// <summary>
    /// Optimizes scene objects for better performance
    /// </summary>
    public static void OptimizeScene(SceneManager sceneManager)
    {
        Console.WriteLine("=== Scene Performance Optimization ===");
        
        var stats = sceneManager.GetStatistics();
        Console.WriteLine($"Before optimization: {stats.TotalObjects} objects, {stats.TotalVertices} vertices");
        
        // Frustum culling simulation (objects outside camera view)
        int culledObjects = 0;
        foreach (var obj in sceneManager.GameObjects)
        {
            // Simple distance-based culling for performance
            var distance = obj.Position.Length();
            if (distance > 50.0f) // Objects beyond 50 units
            {
                obj.IsActive = false;
                culledObjects++;
            }
            else
            {
                obj.IsActive = true;
            }
        }
        
        Console.WriteLine($"Culled {culledObjects} distant objects for performance");
        
        // Level of detail (LOD) simulation
        int lodOptimized = 0;
        foreach (var obj in sceneManager.GameObjects.Where(o => o.IsActive))
        {
            var distance = obj.Position.Length();
            if (distance > 25.0f && obj.Name.StartsWith("CornStalk"))
            {
                // Reduce scale for distant corn stalks (LOD simulation)
                obj.Scale *= 0.8f;
                lodOptimized++;
            }
        }
        
        Console.WriteLine($"Applied LOD optimization to {lodOptimized} objects");
        
        var newStats = sceneManager.GetStatistics();
        Console.WriteLine($"After optimization: {newStats.RenderableObjects} active objects");
        Console.WriteLine("=====================================");
    }
    
    /// <summary>
    /// Optimizes lighting for better performance
    /// </summary>
    public static void OptimizeLighting(DirectXRenderer renderer)
    {
        Console.WriteLine("=== Lighting Performance Optimization ===");
        
        // Optimize directional light settings
        renderer.SetDirectionalLight(
            direction: Vector3.Normalize(new Vector3(-0.5f, -1.0f, -0.3f)),
            color: new Vector3(1.0f, 0.95f, 0.8f),
            intensity: 1.0f
        );
        
        // Optimize ambient lighting for performance
        renderer.SetAmbientLight(0.25f); // Slightly reduce ambient for better contrast
        
        // Optimize specular strength for performance
        renderer.SetSpecularStrength(0.3f); // Reduce specular calculations
        
        // Optimize fog for better performance and visual quality
        renderer.SetFogParameters(
            enabled: true,
            start: 20.0f,    // Start fog closer for better performance
            end: 60.0f,      // Extend fog range for visual quality
            color: new Vector3(0.75f, 0.85f, 0.95f) // Slightly brighter fog
        );
        
        Console.WriteLine("✓ Directional light optimized for performance");
        Console.WriteLine("✓ Ambient lighting reduced to 0.25 for better contrast");
        Console.WriteLine("✓ Specular strength reduced to 0.3 for performance");
        Console.WriteLine("✓ Fog parameters optimized (20-60 units range)");
        Console.WriteLine("==========================================");
    }
    
    /// <summary>
    /// Optimizes material properties for better visual quality
    /// </summary>
    public static void OptimizeMaterials(SceneManager sceneManager)
    {
        Console.WriteLine("=== Material Visual Quality Optimization ===");
        
        foreach (var material in sceneManager.Materials)
        {
            switch (material.Name)
            {
                case "Wood":
                    // Enhance wood material for better visual quality
                    material.SetDiffuseColor(new Vector3(0.65f, 0.42f, 0.22f)); // Richer brown
                    material.SetAmbientColor(new Vector3(0.25f, 0.18f, 0.08f)); // Deeper shadows
                    material.SetSpecularColor(new Vector3(0.3f, 0.25f, 0.2f));  // Warmer highlights
                    material.Shininess = 40.0f; // Slightly more polished
                    Console.WriteLine("✓ Wood material enhanced for richer appearance");
                    break;
                    
                case "Grass":
                    // Enhance grass material for more natural look
                    material.SetDiffuseColor(new Vector3(0.25f, 0.65f, 0.25f)); // More vibrant green
                    material.SetAmbientColor(new Vector3(0.08f, 0.25f, 0.08f)); // Deeper green shadows
                    material.SetSpecularColor(new Vector3(0.15f, 0.2f, 0.15f)); // Subtle green highlights
                    material.Shininess = 12.0f; // Natural matte finish
                    Console.WriteLine("✓ Grass material enhanced for natural appearance");
                    break;
                    
                case "Corn":
                    // Enhance corn material for more realistic look
                    material.SetDiffuseColor(new Vector3(0.45f, 0.75f, 0.25f)); // Brighter, more natural green
                    material.SetAmbientColor(new Vector3(0.18f, 0.35f, 0.08f)); // Rich green shadows
                    material.SetSpecularColor(new Vector3(0.35f, 0.4f, 0.25f)); // Natural plant highlights
                    material.Shininess = 48.0f; // Slightly glossy plant surface
                    Console.WriteLine("✓ Corn material enhanced for realistic plant appearance");
                    break;
            }
        }
        
        Console.WriteLine("=============================================");
    }
    
    /// <summary>
    /// Prints detailed performance report
    /// </summary>
    public static void PrintPerformanceReport(SceneManager sceneManager)
    {
        var metrics = GetMetrics(sceneManager);
        
        Console.WriteLine("=== Performance Report ===");
        Console.WriteLine($"Average FPS: {metrics.AverageFPS:F1}");
        Console.WriteLine($"Average Frame Time: {metrics.AverageFrameTime:F2}ms");
        Console.WriteLine($"Min Frame Time: {metrics.MinFrameTime:F2}ms");
        Console.WriteLine($"Max Frame Time: {metrics.MaxFrameTime:F2}ms");
        Console.WriteLine($"Frame Time Variance: {(metrics.MaxFrameTime - metrics.MinFrameTime):F2}ms");
        Console.WriteLine();
        Console.WriteLine($"Scene Complexity:");
        Console.WriteLine($"  - Total Objects: {metrics.TotalObjects}");
        Console.WriteLine($"  - Rendered Objects: {metrics.RenderedObjects}");
        Console.WriteLine($"  - Total Vertices: {metrics.TotalVertices}");
        Console.WriteLine($"  - Total Triangles: {metrics.TotalTriangles}");
        Console.WriteLine();
        Console.WriteLine($"Performance Classification:");
        if (metrics.AverageFPS >= 55)
            Console.WriteLine("  ✅ EXCELLENT - Smooth 60 FPS performance");
        else if (metrics.AverageFPS >= 45)
            Console.WriteLine("  ✅ GOOD - Stable performance with minor drops");
        else if (metrics.AverageFPS >= 30)
            Console.WriteLine("  ⚠️  ACCEPTABLE - Playable but could be improved");
        else
            Console.WriteLine("  ❌ POOR - Performance optimization needed");
        
        Console.WriteLine("===========================");
    }
    
    /// <summary>
    /// Runs comprehensive performance optimization
    /// </summary>
    public static void OptimizeAll(Camera camera, DirectXRenderer renderer, SceneManager sceneManager)
    {
        Console.WriteLine("🚀 Starting Comprehensive Performance Optimization...");
        Console.WriteLine();
        
        // Optimize each component
        OptimizeCamera(camera);
        OptimizeLighting(renderer);
        OptimizeMaterials(sceneManager);
        OptimizeScene(sceneManager);
        
        Console.WriteLine();
        Console.WriteLine("🎯 Performance Optimization Complete!");
        Console.WriteLine();
        
        // Print final performance report
        PrintPerformanceReport(sceneManager);
    }
}