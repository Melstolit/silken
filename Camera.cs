using System.Numerics;

namespace MySilkProgram;

/// <summary>
/// Camera class that handles 3D transformations, view matrix, and projection matrix calculations
/// </summary>
public class Camera
{
    private Vector3 _position;
    private Vector3 _rotation; // pitch, yaw, roll in radians
    private Matrix4x4 _viewMatrix;
    private Matrix4x4 _projectionMatrix;
    private float _fieldOfView;
    private float _aspectRatio;
    private float _nearPlane;
    private float _farPlane;
    private bool _viewMatrixDirty = true;
    private bool _projectionMatrixDirty = true;

    /// <summary>
    /// Gets or sets the camera position in world space
    /// </summary>
    public Vector3 Position
    {
        get => _position;
        set
        {
            _position = value;
            _viewMatrixDirty = true;
        }
    }

    /// <summary>
    /// Gets or sets the camera rotation (pitch, yaw, roll) in radians
    /// </summary>
    public Vector3 Rotation
    {
        get => _rotation;
        set
        {
            _rotation = value;
            _viewMatrixDirty = true;
        }
    }

    /// <summary>
    /// Gets or sets the field of view in radians
    /// </summary>
    public float FieldOfView
    {
        get => _fieldOfView;
        set
        {
            _fieldOfView = value;
            _projectionMatrixDirty = true;
        }
    }

    /// <summary>
    /// Gets or sets the aspect ratio (width/height)
    /// </summary>
    public float AspectRatio
    {
        get => _aspectRatio;
        set
        {
            _aspectRatio = value;
            _projectionMatrixDirty = true;
        }
    }

    /// <summary>
    /// Gets or sets the near clipping plane distance
    /// </summary>
    public float NearPlane
    {
        get => _nearPlane;
        set
        {
            _nearPlane = value;
            _projectionMatrixDirty = true;
        }
    }

    /// <summary>
    /// Gets or sets the far clipping plane distance
    /// </summary>
    public float FarPlane
    {
        get => _farPlane;
        set
        {
            _farPlane = value;
            _projectionMatrixDirty = true;
        }
    }

    /// <summary>
    /// Initializes a new Camera with default values
    /// </summary>
    public Camera()
    {
        _position = Vector3.Zero;
        _rotation = Vector3.Zero;
        _fieldOfView = MathF.PI / 4.0f; // 45 degrees
        _aspectRatio = 16.0f / 9.0f;
        _nearPlane = 0.1f;
        _farPlane = 1000.0f;
        _viewMatrixDirty = true;
        _projectionMatrixDirty = true;
    }

    /// <summary>
    /// Initializes a new Camera with specified parameters
    /// </summary>
    public Camera(Vector3 position, Vector3 rotation, float fieldOfView, float aspectRatio, float nearPlane, float farPlane)
    {
        _position = position;
        _rotation = rotation;
        _fieldOfView = fieldOfView;
        _aspectRatio = aspectRatio;
        _nearPlane = nearPlane;
        _farPlane = farPlane;
        _viewMatrixDirty = true;
        _projectionMatrixDirty = true;
    }

    /// <summary>
    /// Moves the camera by the specified direction and speed
    /// </summary>
    public void Move(Vector3 direction, float speed)
    {
        // Transform direction by camera's rotation to move relative to camera orientation
        // Note: Negate yaw to match expected coordinate system behavior
        var rotationMatrix = Matrix4x4.CreateFromYawPitchRoll(-_rotation.Y, _rotation.X, _rotation.Z);
        var transformedDirection = Vector3.Transform(direction, rotationMatrix);
        
        _position += transformedDirection * speed;
        _viewMatrixDirty = true;
    }

    /// <summary>
    /// Rotates the camera by the specified pitch, yaw, and roll amounts
    /// </summary>
    public void Rotate(float pitch, float yaw, float roll)
    {
        _rotation.X += pitch; // Pitch
        _rotation.Y += yaw;   // Yaw
        _rotation.Z += roll;  // Roll
        
        // Clamp pitch to prevent camera flipping
        _rotation.X = Math.Clamp(_rotation.X, -MathF.PI / 2.0f + 0.01f, MathF.PI / 2.0f - 0.01f);
        
        _viewMatrixDirty = true;
    }

    /// <summary>
    /// Updates and returns the view matrix
    /// </summary>
    public Matrix4x4 GetViewMatrix()
    {
        if (_viewMatrixDirty)
        {
            UpdateViewMatrix();
            _viewMatrixDirty = false;
        }
        return _viewMatrix;
    }

    /// <summary>
    /// Updates and returns the projection matrix
    /// </summary>
    public Matrix4x4 GetProjectionMatrix()
    {
        if (_projectionMatrixDirty)
        {
            UpdateProjectionMatrix();
            _projectionMatrixDirty = false;
        }
        return _projectionMatrix;
    }

    /// <summary>
    /// Updates the view matrix based on current position and rotation
    /// </summary>
    public void UpdateViewMatrix()
    {
        // Create rotation matrix from yaw, pitch, roll
        var rotationMatrix = Matrix4x4.CreateFromYawPitchRoll(_rotation.Y, _rotation.X, _rotation.Z);
        
        // Calculate forward, right, and up vectors from rotation
        var forward = Vector3.Transform(-Vector3.UnitZ, rotationMatrix);
        var right = Vector3.Transform(Vector3.UnitX, rotationMatrix);
        var up = Vector3.Transform(Vector3.UnitY, rotationMatrix);
        
        // Create view matrix using look-at
        var target = _position + forward;
        _viewMatrix = Matrix4x4.CreateLookAt(_position, target, up);
    }

    /// <summary>
    /// Updates the projection matrix based on current parameters
    /// </summary>
    public void UpdateProjectionMatrix()
    {
        _projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(_fieldOfView, _aspectRatio, _nearPlane, _farPlane);
    }

    /// <summary>
    /// Gets the forward direction vector of the camera
    /// </summary>
    public Vector3 GetForwardVector()
    {
        var rotationMatrix = Matrix4x4.CreateFromYawPitchRoll(_rotation.Y, _rotation.X, _rotation.Z);
        return Vector3.Transform(-Vector3.UnitZ, rotationMatrix);
    }

    /// <summary>
    /// Gets the right direction vector of the camera
    /// </summary>
    public Vector3 GetRightVector()
    {
        var rotationMatrix = Matrix4x4.CreateFromYawPitchRoll(_rotation.Y, _rotation.X, _rotation.Z);
        return Vector3.Transform(Vector3.UnitX, rotationMatrix);
    }

    /// <summary>
    /// Gets the up direction vector of the camera
    /// </summary>
    public Vector3 GetUpVector()
    {
        var rotationMatrix = Matrix4x4.CreateFromYawPitchRoll(_rotation.Y, _rotation.X, _rotation.Z);
        return Vector3.Transform(Vector3.UnitY, rotationMatrix);
    }
}