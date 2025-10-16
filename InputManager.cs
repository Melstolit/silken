using Silk.NET.Input;
using System.Numerics;

namespace MySilkProgram;

/// <summary>
/// InputManager class that processes keyboard input and translates to camera movements
/// </summary>
public class InputManager
{
    private IKeyboard _keyboard;
    private Camera _camera;
    private readonly Dictionary<Key, bool> _keyStates;

    /// <summary>
    /// Gets or sets the movement speed for camera position changes
    /// </summary>
    public float MovementSpeed { get; set; } = 5.0f;

    /// <summary>
    /// Gets or sets the rotation speed for camera rotation changes
    /// </summary>
    public float RotationSpeed { get; set; } = 2.0f;

    /// <summary>
    /// Initializes a new InputManager
    /// </summary>
    public InputManager()
    {
        _keyStates = new Dictionary<Key, bool>();
    }

    /// <summary>
    /// Initializes the InputManager with input context and camera
    /// </summary>
    /// <param name="inputContext">The Silk.NET input context</param>
    /// <param name="camera">The camera to control</param>
    public void Initialize(IInputContext inputContext, Camera camera)
    {
        _camera = camera ?? throw new ArgumentNullException(nameof(camera));
        
        if (inputContext.Keyboards.Count > 0)
        {
            _keyboard = inputContext.Keyboards[0];
            _keyboard.KeyDown += OnKeyDown;
            _keyboard.KeyUp += OnKeyUp;
        }
        else
        {
            throw new InvalidOperationException("No keyboard found in input context");
        }
    }

    /// <summary>
    /// Updates input processing and applies camera movements
    /// </summary>
    /// <param name="deltaTime">Time elapsed since last update in seconds</param>
    public void Update(double deltaTime)
    {
        if (_camera == null || _keyboard == null)
            return;

        ProcessCameraMovement(deltaTime);
    }

    /// <summary>
    /// Processes camera movement based on current key states
    /// </summary>
    /// <param name="deltaTime">Time elapsed since last update in seconds</param>
    public void ProcessCameraMovement(double deltaTime)
    {
        if (_camera == null)
            return;

        var dt = (float)deltaTime;

        // Arrow key movement (camera position)
        var moveDirection = Vector3.Zero;

        if (IsKeyPressed(Key.Up))
        {
            moveDirection += new Vector3(0, 0, -1); // Forward
        }
        if (IsKeyPressed(Key.Down))
        {
            moveDirection += new Vector3(0, 0, 1); // Backward
        }
        if (IsKeyPressed(Key.Left))
        {
            moveDirection += new Vector3(-1, 0, 0); // Left
        }
        if (IsKeyPressed(Key.Right))
        {
            moveDirection += new Vector3(1, 0, 0); // Right
        }

        // Apply movement if any direction keys are pressed
        if (moveDirection != Vector3.Zero)
        {
            // Normalize to prevent faster diagonal movement
            moveDirection = Vector3.Normalize(moveDirection);
            _camera.Move(moveDirection, MovementSpeed * dt);
        }

        // WASD rotation (camera rotation)
        var rotationChange = Vector3.Zero;

        if (IsKeyPressed(Key.W))
        {
            rotationChange.X -= RotationSpeed * dt; // Pitch up
        }
        if (IsKeyPressed(Key.S))
        {
            rotationChange.X += RotationSpeed * dt; // Pitch down
        }
        if (IsKeyPressed(Key.A))
        {
            rotationChange.Y -= RotationSpeed * dt; // Yaw left
        }
        if (IsKeyPressed(Key.D))
        {
            rotationChange.Y += RotationSpeed * dt; // Yaw right
        }

        // Apply rotation if any rotation keys are pressed
        if (rotationChange != Vector3.Zero)
        {
            _camera.Rotate(rotationChange.X, rotationChange.Y, rotationChange.Z);
        }
    }

    /// <summary>
    /// Checks if a specific key is currently pressed
    /// </summary>
    /// <param name="key">The key to check</param>
    /// <returns>True if the key is pressed, false otherwise</returns>
    public bool IsKeyPressed(Key key)
    {
        return _keyStates.TryGetValue(key, out bool isPressed) && isPressed;
    }

    /// <summary>
    /// Sets the camera to control
    /// </summary>
    /// <param name="camera">The camera to control</param>
    public void SetCamera(Camera camera)
    {
        _camera = camera ?? throw new ArgumentNullException(nameof(camera));
    }

    /// <summary>
    /// Gets the currently controlled camera
    /// </summary>
    /// <returns>The camera being controlled, or null if none is set</returns>
    public Camera GetCamera()
    {
        return _camera;
    }

    /// <summary>
    /// Handles key down events
    /// </summary>
    private void OnKeyDown(IKeyboard keyboard, Key key, int keyCode)
    {
        _keyStates[key] = true;
    }

    /// <summary>
    /// Handles key up events
    /// </summary>
    private void OnKeyUp(IKeyboard keyboard, Key key, int keyCode)
    {
        _keyStates[key] = false;
    }

    /// <summary>
    /// Cleans up input event handlers
    /// </summary>
    public void Dispose()
    {
        if (_keyboard != null)
        {
            _keyboard.KeyDown -= OnKeyDown;
            _keyboard.KeyUp -= OnKeyUp;
            _keyboard = null;
        }
        
        _keyStates.Clear();
        _camera = null;
    }
}