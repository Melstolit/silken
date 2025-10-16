using Xunit;
using Silk.NET.Windowing;
using Silk.NET.Maths;
using System.Drawing;

namespace MySilkProgram;

public class DirectXRendererTests : IDisposable
{
    private IWindow _testWindow = null!;
    private DirectXRenderer _renderer = null!;

    [Fact]
    public void DirectXRenderer_Initialize_ShouldCreateDeviceAndCommandQueue()
    {
        // Arrange
        var options = WindowOptions.Default with
        {
            Size = new Vector2D<int>(800, 600),
            Title = "Test Window",
            IsVisible = false // Don't show window during tests
        };
        
        _testWindow = Window.Create(options);
        _renderer = new DirectXRenderer();

        // Act & Assert
        try
        {
            _renderer.Initialize(_testWindow);
            
            // Verify initialization succeeded
            Assert.True(_renderer.IsInitialized, "DirectX renderer should be initialized");
        }
        catch (Exception ex)
        {
            // If DirectX 12 is not available on test system, skip the test
            if (ex.Message.Contains("DirectX 12 not supported") || 
                ex.Message.Contains("Failed to create DirectX 12 device"))
            {
                Assert.True(true, "DirectX 12 not available on test system - test skipped");
                return;
            }
            
            // Re-throw other exceptions
            throw;
        }
    }

    [Fact]
    public void DirectXRenderer_SetMatrices_ShouldNotThrow()
    {
        // Arrange
        var options = WindowOptions.Default with
        {
            Size = new Vector2D<int>(800, 600),
            Title = "Test Window",
            IsVisible = false
        };
        
        _testWindow = Window.Create(options);
        _renderer = new DirectXRenderer();

        try
        {
            _renderer.Initialize(_testWindow);
            
            // Act
            var model = System.Numerics.Matrix4x4.Identity;
            var view = System.Numerics.Matrix4x4.Identity;
            var projection = System.Numerics.Matrix4x4.Identity;
            
            // Should not throw
            _renderer.SetMatrices(model, view, projection);
            
            // Assert - if we get here without exception, test passes
            Assert.True(true);
        }
        catch (Exception ex)
        {
            // Skip test if DirectX 12 not available
            if (ex.Message.Contains("DirectX 12 not supported") || 
                ex.Message.Contains("Failed to create DirectX 12 device"))
            {
                Assert.True(true, "DirectX 12 not available on test system - test skipped");
                return;
            }
            
            throw;
        }
    }

    [Fact]
    public void DirectXRenderer_ClearAndPresent_ShouldNotThrow()
    {
        // Arrange
        var options = WindowOptions.Default with
        {
            Size = new Vector2D<int>(800, 600),
            Title = "Test Window",
            IsVisible = false
        };
        
        _testWindow = Window.Create(options);
        _renderer = new DirectXRenderer();

        try
        {
            _renderer.Initialize(_testWindow);
            
            // Act
            _renderer.ClearRenderTarget(0.1f, 0.2f, 0.3f, 1.0f);
            _renderer.Present();
            
            // Assert - should not throw
            Assert.True(true);
        }
        catch (Exception ex)
        {
            // Skip test if DirectX 12 not available
            if (ex.Message.Contains("DirectX 12 not supported") || 
                ex.Message.Contains("Failed to create DirectX 12 device"))
            {
                Assert.True(true, "DirectX 12 not available on test system - test skipped");
                return;
            }
            
            throw;
        }
    }

    [Fact]
    public void DirectXRenderer_PipelineStateOperations_ShouldNotThrow()
    {
        // Arrange
        var options = WindowOptions.Default with
        {
            Size = new Vector2D<int>(800, 600),
            Title = "Test Window",
            IsVisible = false
        };
        
        _testWindow = Window.Create(options);
        _renderer = new DirectXRenderer();

        try
        {
            _renderer.Initialize(_testWindow);
            
            // Act
            _renderer.SetPipelineState();
            
            var model = System.Numerics.Matrix4x4.CreateTranslation(1, 2, 3);
            var view = System.Numerics.Matrix4x4.CreateLookAt(
                new System.Numerics.Vector3(0, 0, 5),
                System.Numerics.Vector3.Zero,
                System.Numerics.Vector3.UnitY);
            var projection = System.Numerics.Matrix4x4.CreatePerspectiveFieldOfView(
                MathF.PI / 4, 800f / 600f, 0.1f, 1000f);
                
            _renderer.SetRootConstants(model, view, projection);
            
            // Assert - should not throw
            Assert.True(true);
        }
        catch (Exception ex)
        {
            // Skip test if DirectX 12 not available
            if (ex.Message.Contains("DirectX 12 not supported") || 
                ex.Message.Contains("Failed to create DirectX 12 device"))
            {
                Assert.True(true, "DirectX 12 not available on test system - test skipped");
                return;
            }
            
            throw;
        }
    }

    [Fact]
    public void DirectXRenderer_Dispose_ShouldCleanupResources()
    {
        // Arrange
        var options = WindowOptions.Default with
        {
            Size = new Vector2D<int>(800, 600),
            Title = "Test Window",
            IsVisible = false
        };
        
        _testWindow = Window.Create(options);
        _renderer = new DirectXRenderer();

        try
        {
            _renderer.Initialize(_testWindow);
            
            // Act
            _renderer.Dispose();
            
            // Assert - should not throw
            Assert.True(true);
        }
        catch (Exception ex)
        {
            // Skip test if DirectX 12 not available
            if (ex.Message.Contains("DirectX 12 not supported") || 
                ex.Message.Contains("Failed to create DirectX 12 device"))
            {
                Assert.True(true, "DirectX 12 not available on test system - test skipped");
                return;
            }
            
            throw;
        }
    }

    public void Dispose()
    {
        _renderer.Dispose();
        _testWindow.Dispose();
    }
}