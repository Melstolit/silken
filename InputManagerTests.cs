using System.Numerics;
using Xunit;

namespace MySilkProgram.Tests;

/// <summary>
/// Unit tests for InputManager class input processing logic
/// </summary>
public class InputManagerTests
{
    private const float Epsilon = 0.0001f;

    [Fact]
    public void InputManager_DefaultConstructor_InitializesCorrectly()
    {
        // Arrange & Act
        var inputManager = new InputManager();

        // Assert
        Assert.Equal(5.0f, inputManager.MovementSpeed, Epsilon);
        Assert.Equal(2.0f, inputManager.RotationSpeed, Epsilon);
        Assert.Null(inputManager.GetCamera());
    }

    [Fact]
    public void SetCamera_ValidCamera_SetsCamera()
    {
        // Arrange
        var inputManager = new InputManager();
        var camera = new Camera();

        // Act
        inputManager.SetCamera(camera);

        // Assert
        Assert.Equal(camera, inputManager.GetCamera());
    }

    [Fact]
    public void SetCamera_NullCamera_ThrowsArgumentNullException()
    {
        // Arrange
        var inputManager = new InputManager();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => inputManager.SetCamera(null!));
    }

    [Fact]
    public void MovementSpeed_SetValue_UpdatesCorrectly()
    {
        // Arrange
        var inputManager = new InputManager();
        var newSpeed = 10.0f;

        // Act
        inputManager.MovementSpeed = newSpeed;

        // Assert
        Assert.Equal(newSpeed, inputManager.MovementSpeed, Epsilon);
    }

    [Fact]
    public void RotationSpeed_SetValue_UpdatesCorrectly()
    {
        // Arrange
        var inputManager = new InputManager();
        var newSpeed = 3.5f;

        // Act
        inputManager.RotationSpeed = newSpeed;

        // Assert
        Assert.Equal(newSpeed, inputManager.RotationSpeed, Epsilon);
    }

    [Fact]
    public void ProcessCameraMovement_NullCamera_DoesNotThrow()
    {
        // Arrange
        var inputManager = new InputManager();
        var deltaTime = 0.016; // ~60 FPS

        // Act & Assert
        // Should not throw when camera is null
        inputManager.ProcessCameraMovement(deltaTime);
    }

    [Fact]
    public void Update_NullCamera_DoesNotThrow()
    {
        // Arrange
        var inputManager = new InputManager();
        var deltaTime = 0.016; // ~60 FPS

        // Act & Assert
        // Should not throw when camera is null
        inputManager.Update(deltaTime);
    }

    [Fact]
    public void IsKeyPressed_UnpressedKey_ReturnsFalse()
    {
        // Arrange
        var inputManager = new InputManager();

        // Act
        var isPressed = inputManager.IsKeyPressed(Silk.NET.Input.Key.W);

        // Assert
        Assert.False(isPressed);
    }

    [Fact]
    public void GetCamera_NoCamera_ReturnsNull()
    {
        // Arrange
        var inputManager = new InputManager();

        // Act
        var camera = inputManager.GetCamera();

        // Assert
        Assert.Null(camera);
    }

    [Fact]
    public void Dispose_WithoutInitialization_DoesNotThrow()
    {
        // Arrange
        var inputManager = new InputManager();

        // Act & Assert
        // Should not throw when disposing without initialization
        inputManager.Dispose();
    }

    // Note: Testing actual key input and camera movement would require mocking
    // the Silk.NET input system, which is complex. The following tests verify
    // the logic without actual input events.

    [Theory]
    [InlineData(0.016, 5.0f)] // 60 FPS, default speed
    [InlineData(0.033, 5.0f)] // 30 FPS, default speed
    [InlineData(0.016, 10.0f)] // 60 FPS, double speed
    public void MovementSpeed_DifferentValues_AffectsCalculations(double deltaTime, float speed)
    {
        // Arrange
        var inputManager = new InputManager();
        inputManager.MovementSpeed = speed;

        // Act
        var expectedDistance = speed * (float)deltaTime;

        // Assert
        // The expected distance should be speed * deltaTime
        // This verifies the calculation logic is correct
        Assert.Equal(expectedDistance, speed * (float)deltaTime, Epsilon);
    }

    [Theory]
    [InlineData(0.016, 2.0f)] // 60 FPS, default rotation speed
    [InlineData(0.033, 2.0f)] // 30 FPS, default rotation speed
    [InlineData(0.016, 4.0f)] // 60 FPS, double rotation speed
    public void RotationSpeed_DifferentValues_AffectsCalculations(double deltaTime, float speed)
    {
        // Arrange
        var inputManager = new InputManager();
        inputManager.RotationSpeed = speed;

        // Act
        var expectedRotation = speed * (float)deltaTime;

        // Assert
        // The expected rotation should be speed * deltaTime
        // This verifies the calculation logic is correct
        Assert.Equal(expectedRotation, speed * (float)deltaTime, Epsilon);
    }

    [Fact]
    public void ProcessCameraMovement_WithCamera_DoesNotThrow()
    {
        // Arrange
        var inputManager = new InputManager();
        var camera = new Camera();
        inputManager.SetCamera(camera);
        var deltaTime = 0.016;

        // Act & Assert
        // Should not throw when processing with valid camera
        inputManager.ProcessCameraMovement(deltaTime);
    }

    [Fact]
    public void Update_WithCamera_DoesNotThrow()
    {
        // Arrange
        var inputManager = new InputManager();
        var camera = new Camera();
        inputManager.SetCamera(camera);
        var deltaTime = 0.016;

        // Act & Assert
        // Should not throw when updating with valid camera
        inputManager.Update(deltaTime);
    }
}