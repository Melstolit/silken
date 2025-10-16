namespace MySilkProgram;

using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using System.Drawing;
using System.Numerics;

public class Program
{
    private static IWindow _window = null!;
    private static bool _directXInitialized = false;
    private static Camera _camera = null!;
    private static InputManager _inputManager = null!;
    private static DirectXRenderer _renderer = null!;
    private static SceneManager _sceneManager = null!;
    
    // Frame timing variables for VSync synchronization
    private static DateTime _lastFrameTime = DateTime.Now;
    private static double _targetFrameTime = 1.0 / 60.0; // Target 60 FPS
    private static double _accumulatedTime = 0.0;
    private static int _frameCount = 0;
    private static DateTime _fpsCounterStart = DateTime.Now;
    public static void Main(string[] args)
    {
        // Run scene rendering tests if requested
        if (args.Length > 0 && args[0] == "--test")
        {
            TestSceneRendering.RunAllTests();
            return;
        }
        
        WindowOptions options = WindowOptions.Default with
        {
            Size = new Vector2D<int>(800, 600),
            Title = "Silk.NET DirectX 12 3D Engine",
            VSync = true, // Enable VSync for smooth rendering
            UpdatesPerSecond = 60, // Target 60 updates per second
            FramesPerSecond = 60   // Target 60 frames per second
        };
        _window = Window.Create(options);

        _window.Load += OnLoad;
        _window.Update += OnUpdate;
        _window.Render += OnRender;
        _window.Closing += OnClosing;

        _window.Run();
    }

    public static void OnLoad() 
    {
        Console.WriteLine("Setting up DirectX 12 foundation...");
        
        try
        {
            // Initialize camera
            _camera = new Camera(
                position: new Vector3(0, 0, 5), // Start 5 units back from origin
                rotation: Vector3.Zero,
                fieldOfView: MathF.PI / 4.0f,
                aspectRatio: 800.0f / 600.0f, // Match window size
                nearPlane: 0.1f,
                farPlane: 1000.0f
            );
            
            // Initialize input handling
            IInputContext input = _window.CreateInput();
            
            // Set up InputManager for camera controls
            _inputManager = new InputManager();
            _inputManager.Initialize(input, _camera);
            
            // Keep existing escape key handler
            for (int i = 0; i < input.Keyboards.Count; i++)
                input.Keyboards[i].KeyDown += KeyDown;
            
            // Initialize DirectX 12 renderer
            _renderer = new DirectXRenderer();
            _renderer.Initialize(_window);
            
            // Initialize scene manager and load the farm scene
            _sceneManager = new SceneManager();
            _sceneManager.LoadScene();
            
            _directXInitialized = true;
            Console.WriteLine("DirectX 12 foundation setup completed successfully!");
            Console.WriteLine("Window created with DirectX 12 context support");
            Console.WriteLine("VSync enabled for smooth rendering");
            Console.WriteLine("Camera and input system initialized");
            Console.WriteLine("Farm scene loaded with all game objects");
            Console.WriteLine("Controls: Arrow keys = move camera, WASD = rotate camera, Escape = exit");
            
            // Print scene statistics
            _sceneManager.PrintStatistics();
            
            // Run comprehensive performance optimization
            Console.WriteLine();
            PerformanceOptimizer.OptimizeAll(_camera, _renderer, _sceneManager);
            Console.WriteLine();
            
            // Apply visual quality enhancements
            VisualQualityEnhancer.EnhanceAll(_renderer, _sceneManager, _camera);
            Console.WriteLine();
            
            // Validate final quality settings
            VisualQualityEnhancer.ValidateVisualQuality(_renderer, _sceneManager);
            Console.WriteLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"DirectX 12 foundation setup failed: {ex.Message}");
            Console.WriteLine("Continuing with basic window functionality...");
        }
    }

    private static void OnUpdate(double deltaTime) 
    { 
        // Calculate accurate delta time for VSync synchronization
        var currentTime = DateTime.Now;
        var actualDeltaTime = (currentTime - _lastFrameTime).TotalSeconds;
        _lastFrameTime = currentTime;
        
        // Clamp delta time to prevent large jumps (e.g., when debugging)
        actualDeltaTime = Math.Min(actualDeltaTime, 0.1); // Max 100ms delta
        
        // Update input manager to process camera controls with accurate delta time
        _inputManager?.Update(actualDeltaTime);
        
        // Update scene manager and all game objects
        _sceneManager?.Update((float)actualDeltaTime);
        
        // Update frame timing statistics
        _accumulatedTime += actualDeltaTime;
        _frameCount++;
        
        // Log FPS and performance every 5 seconds
        if ((currentTime - _fpsCounterStart).TotalSeconds >= 5.0)
        {
            var avgFps = _frameCount / _accumulatedTime;
            Console.WriteLine($"🚀 Performance: {avgFps:F1} FPS (Target: {1.0 / _targetFrameTime:F1} FPS)");
            
            // Print detailed performance report every 5 seconds
            if (_sceneManager != null)
            {
                PerformanceOptimizer.PrintPerformanceReport(_sceneManager);
            }
            
            // Reset counters
            _frameCount = 0;
            _accumulatedTime = 0.0;
            _fpsCounterStart = currentTime;
        }
    }

    public static void OnRender(double deltaTime)
    { 
        if (!_directXInitialized || _renderer == null)
            return;
            
        // Start performance timing
        PerformanceOptimizer.StartFrame();
            
        // DirectX 12 rendering with VSync synchronization
        try
        {
            // Set up rendering pipeline
            _renderer.SetPipelineState();
            
            // Get camera matrices for shader constants
            var view = _camera.GetViewMatrix();
            var projection = _camera.GetProjectionMatrix();
            
            // Clear render target with consistent timing
            _renderer.ClearRenderTarget(0.0f, 0.2f, 0.4f, 1.0f); // Dark blue background
            
            // Render all scene objects with directional lighting
            if (_sceneManager != null)
            {
                // Set global view and projection matrices for all objects
                _renderer.SetMatrices(Matrix4x4.Identity, view, projection);
                
                // Render scene with directional lighting and camera position
                _sceneManager.Render(_renderer, _camera.Position);
                
                Console.WriteLine($"Rendered scene with directional lighting applied");
            }
            
            // Present with VSync for smooth frame delivery
            _renderer.Present();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Render error: {ex.Message}");
        }
        finally
        {
            // End performance timing
            PerformanceOptimizer.EndFrame();
        }
    }

    private static void KeyDown(IKeyboard keyboard, Key key, int keyCode)
    {
        if (key == Key.Escape)
        {
            _window.Close();
        }
    }
    
    private static void OnClosing()
    {
        Console.WriteLine("Cleaning up DirectX 12 foundation...");
        
        // Clean up all engine components in proper order
        _sceneManager?.Dispose();
        _inputManager?.Dispose();
        _renderer?.Dispose();
        
        Console.WriteLine("All engine components cleaned up successfully.");
    }
    

}
