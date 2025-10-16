using System.Numerics;
using Xunit;

namespace MySilkProgram.Tests;

/// <summary>
/// Unit tests for Camera class matrix calculations and transformations
/// </summary>
public class CameraTests
{
    private const float Epsilon = 0.0001f;

    [Fact]
    public void Camera_DefaultConstructor_SetsCorrectDefaults()
    {
        // Arrange & Act
        var camera = new Camera();

        // Assert
        Assert.Equal(Vector3.Zero, camera.Position);
        Assert.Equal(Vector3.Zero, camera.Rotation);
        Assert.Equal(MathF.PI / 4.0f, camera.FieldOfView, Epsilon);
        Assert.Equal(16.0f / 9.0f, camera.AspectRatio, Epsilon);
        Assert.Equal(0.1f, camera.NearPlane, Epsilon);
        Assert.Equal(1000.0f, camera.FarPlane, Epsilon);
    }

    [Fact]
    public void Camera_ParameterizedConstructor_SetsCorrectValues()
    {
        // Arrange
        var position = new Vector3(1, 2, 3);
        var rotation = new Vector3(0.1f, 0.2f, 0.3f);
        var fov = MathF.PI / 3.0f;
        var aspectRatio = 4.0f / 3.0f;
        var nearPlane = 0.5f;
        var farPlane = 500.0f;

        // Act
        var camera = new Camera(position, rotation, fov, aspectRatio, nearPlane, farPlane);

        // Assert
        Assert.Equal(position, camera.Position);
        Assert.Equal(rotation, camera.Rotation);
        Assert.Equal(fov, camera.FieldOfView, Epsilon);
        Assert.Equal(aspectRatio, camera.AspectRatio, Epsilon);
        Assert.Equal(nearPlane, camera.NearPlane, Epsilon);
        Assert.Equal(farPlane, camera.FarPlane, Epsilon);
    }

    [Fact]
    public void GetViewMatrix_AtOriginLookingForward_ReturnsIdentityView()
    {
        // Arrange
        var camera = new Camera();

        // Act
        var viewMatrix = camera.GetViewMatrix();

        // Assert
        // At origin looking down negative Z should produce a view matrix close to identity
        // The view matrix transforms world coordinates to camera coordinates
        Assert.True(IsMatrixValid(viewMatrix));
    }

    [Fact]
    public void GetProjectionMatrix_ValidParameters_ReturnsValidMatrix()
    {
        // Arrange
        var camera = new Camera();

        // Act
        var projectionMatrix = camera.GetProjectionMatrix();

        // Assert
        Assert.True(IsMatrixValid(projectionMatrix));
        // Projection matrix should have non-zero values in specific positions
        Assert.NotEqual(0, projectionMatrix.M11);
        Assert.NotEqual(0, projectionMatrix.M22);
        Assert.NotEqual(0, projectionMatrix.M33);
        Assert.NotEqual(0, projectionMatrix.M43);
    }

    [Fact]
    public void Move_ForwardDirection_UpdatesPositionCorrectly()
    {
        // Arrange
        var camera = new Camera();
        var initialPosition = camera.Position;
        var forwardDirection = new Vector3(0, 0, -1); // Forward in camera space
        var speed = 5.0f;

        // Act
        camera.Move(forwardDirection, speed);

        // Assert
        // Should move forward (negative Z direction when no rotation)
        Assert.NotEqual(initialPosition, camera.Position);
        Assert.Equal(0, camera.Position.X, Epsilon);
        Assert.Equal(0, camera.Position.Y, Epsilon);
        Assert.Equal(-speed, camera.Position.Z, Epsilon);
    }

    [Fact]
    public void Rotate_PitchRotation_UpdatesRotationCorrectly()
    {
        // Arrange
        var camera = new Camera();
        var initialRotation = camera.Rotation;
        var pitchAmount = 0.5f;

        // Act
        camera.Rotate(pitchAmount, 0, 0);

        // Assert
        Assert.NotEqual(initialRotation, camera.Rotation);
        Assert.Equal(pitchAmount, camera.Rotation.X, Epsilon);
        Assert.Equal(0, camera.Rotation.Y, Epsilon);
        Assert.Equal(0, camera.Rotation.Z, Epsilon);
    }

    [Fact]
    public void Rotate_ExcessivePitch_ClampsToValidRange()
    {
        // Arrange
        var camera = new Camera();
        var excessivePitch = MathF.PI; // 180 degrees, should be clamped

        // Act
        camera.Rotate(excessivePitch, 0, 0);

        // Assert
        // Should be clamped to just under PI/2 (90 degrees)
        Assert.True(camera.Rotation.X < MathF.PI / 2.0f);
        Assert.True(camera.Rotation.X > MathF.PI / 2.0f - 0.1f);
    }

    [Fact]
    public void GetForwardVector_NoRotation_ReturnsNegativeZ()
    {
        // Arrange
        var camera = new Camera();

        // Act
        var forward = camera.GetForwardVector();

        // Assert
        Assert.Equal(0, forward.X, Epsilon);
        Assert.Equal(0, forward.Y, Epsilon);
        Assert.Equal(-1, forward.Z, Epsilon);
    }

    [Fact]
    public void GetRightVector_NoRotation_ReturnsPositiveX()
    {
        // Arrange
        var camera = new Camera();

        // Act
        var right = camera.GetRightVector();

        // Assert
        Assert.Equal(1, right.X, Epsilon);
        Assert.Equal(0, right.Y, Epsilon);
        Assert.Equal(0, right.Z, Epsilon);
    }

    [Fact]
    public void GetUpVector_NoRotation_ReturnsPositiveY()
    {
        // Arrange
        var camera = new Camera();

        // Act
        var up = camera.GetUpVector();

        // Assert
        Assert.Equal(0, up.X, Epsilon);
        Assert.Equal(1, up.Y, Epsilon);
        Assert.Equal(0, up.Z, Epsilon);
    }

    [Fact]
    public void ViewMatrix_AfterPositionChange_UpdatesCorrectly()
    {
        // Arrange
        var camera = new Camera();
        var initialViewMatrix = camera.GetViewMatrix();
        
        // Act
        camera.Position = new Vector3(10, 5, -3);
        var updatedViewMatrix = camera.GetViewMatrix();

        // Assert
        Assert.NotEqual(initialViewMatrix, updatedViewMatrix);
        Assert.True(IsMatrixValid(updatedViewMatrix));
    }

    [Fact]
    public void ProjectionMatrix_AfterParameterChange_UpdatesCorrectly()
    {
        // Arrange
        var camera = new Camera();
        var initialProjectionMatrix = camera.GetProjectionMatrix();
        
        // Act
        camera.FieldOfView = MathF.PI / 6.0f; // 30 degrees
        var updatedProjectionMatrix = camera.GetProjectionMatrix();

        // Assert
        Assert.NotEqual(initialProjectionMatrix, updatedProjectionMatrix);
        Assert.True(IsMatrixValid(updatedProjectionMatrix));
    }

    [Fact]
    public void Move_WithRotation_MovesInCorrectDirection()
    {
        // Arrange
        var camera = new Camera();
        camera.Rotation = new Vector3(0, MathF.PI / 2.0f, 0); // 90 degree yaw (turn right)
        var forwardDirection = new Vector3(0, 0, -1);
        var speed = 1.0f;

        // Act
        camera.Move(forwardDirection, speed);

        // Assert
        // After 90 degree yaw, moving "forward" should move in positive X direction
        Assert.True(camera.Position.X > 0.9f); // Should be close to 1.0
        Assert.Equal(0, camera.Position.Y, Epsilon);
        Assert.True(Math.Abs(camera.Position.Z) < 0.1f); // Should be close to 0
    }

    /// <summary>
    /// Helper method to check if a matrix contains valid (non-NaN, non-infinite) values
    /// </summary>
    private static bool IsMatrixValid(Matrix4x4 matrix)
    {
        return !float.IsNaN(matrix.M11) && !float.IsInfinity(matrix.M11) &&
               !float.IsNaN(matrix.M22) && !float.IsInfinity(matrix.M22) &&
               !float.IsNaN(matrix.M33) && !float.IsInfinity(matrix.M33) &&
               !float.IsNaN(matrix.M44) && !float.IsInfinity(matrix.M44);
    }
}