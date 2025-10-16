using System.Numerics;
using Xunit;

namespace MySilkProgram;

public class MaterialTests
{
    [Fact]
    public void Material_DefaultConstructor_SetsDefaultProperties()
    {
        // Act
        var material = new Material();
        
        // Assert
        Assert.Equal("DefaultMaterial", material.Name);
        Assert.Equal(new Vector3(0.8f, 0.8f, 0.8f), material.DiffuseColor);
        Assert.Equal(new Vector3(0.2f, 0.2f, 0.2f), material.AmbientColor);
        Assert.Equal(new Vector3(1.0f, 1.0f, 1.0f), material.SpecularColor);
        Assert.Equal(32.0f, material.Shininess);
        Assert.False(material.HasTexture);
    }
    
    [Fact]
    public void Material_NamedConstructor_SetsName()
    {
        // Act
        var material = new Material("TestMaterial");
        
        // Assert
        Assert.Equal("TestMaterial", material.Name);
    }
    
    [Fact]
    public void Material_CustomConstructor_SetsAllProperties()
    {
        // Arrange
        var diffuse = new Vector3(0.5f, 0.6f, 0.7f);
        var ambient = new Vector3(0.1f, 0.2f, 0.3f);
        var specular = new Vector3(0.8f, 0.9f, 1.0f);
        float shininess = 64.0f;
        string name = "CustomMaterial";
        
        // Act
        var material = new Material(diffuse, ambient, specular, shininess, name);
        
        // Assert
        Assert.Equal(name, material.Name);
        Assert.Equal(diffuse, material.DiffuseColor);
        Assert.Equal(ambient, material.AmbientColor);
        Assert.Equal(specular, material.SpecularColor);
        Assert.Equal(shininess, material.Shininess);
    }
    
    [Fact]
    public void Material_CustomConstructor_ClampsShininessToMinimum()
    {
        // Arrange
        float invalidShininess = 0.5f; // Less than 1.0
        
        // Act
        var material = new Material(Vector3.One, Vector3.Zero, Vector3.One, invalidShininess);
        
        // Assert
        Assert.Equal(1.0f, material.Shininess);
    }
    
    [Fact]
    public void Material_SetDiffuseColor_Vector3_SetsCorrectly()
    {
        // Arrange
        var material = new Material();
        var color = new Vector3(0.3f, 0.4f, 0.5f);
        
        // Act
        material.SetDiffuseColor(color);
        
        // Assert
        Assert.Equal(color, material.DiffuseColor);
    }
    
    [Fact]
    public void Material_SetDiffuseColor_RGB_SetsCorrectly()
    {
        // Arrange
        var material = new Material();
        
        // Act
        material.SetDiffuseColor(0.3f, 0.4f, 0.5f);
        
        // Assert
        Assert.Equal(new Vector3(0.3f, 0.4f, 0.5f), material.DiffuseColor);
    }
    
    [Fact]
    public void Material_SetDiffuseColor_ClampsValues()
    {
        // Arrange
        var material = new Material();
        
        // Act
        material.SetDiffuseColor(-0.5f, 1.5f, 0.5f);
        
        // Assert
        Assert.Equal(new Vector3(0.0f, 1.0f, 0.5f), material.DiffuseColor);
    }
    
    [Fact]
    public void Material_SetAmbientColor_Vector3_SetsCorrectly()
    {
        // Arrange
        var material = new Material();
        var color = new Vector3(0.1f, 0.2f, 0.3f);
        
        // Act
        material.SetAmbientColor(color);
        
        // Assert
        Assert.Equal(color, material.AmbientColor);
    }
    
    [Fact]
    public void Material_SetAmbientColor_RGB_SetsCorrectly()
    {
        // Arrange
        var material = new Material();
        
        // Act
        material.SetAmbientColor(0.1f, 0.2f, 0.3f);
        
        // Assert
        Assert.Equal(new Vector3(0.1f, 0.2f, 0.3f), material.AmbientColor);
    }
    
    [Fact]
    public void Material_SetSpecularColor_Vector3_SetsCorrectly()
    {
        // Arrange
        var material = new Material();
        var color = new Vector3(0.7f, 0.8f, 0.9f);
        
        // Act
        material.SetSpecularColor(color);
        
        // Assert
        Assert.Equal(color, material.SpecularColor);
    }
    
    [Fact]
    public void Material_SetSpecularColor_RGB_SetsCorrectly()
    {
        // Arrange
        var material = new Material();
        
        // Act
        material.SetSpecularColor(0.7f, 0.8f, 0.9f);
        
        // Assert
        Assert.Equal(new Vector3(0.7f, 0.8f, 0.9f), material.SpecularColor);
    }
    
    [Fact]
    public void Material_SetShininess_SetsCorrectly()
    {
        // Arrange
        var material = new Material();
        
        // Act
        material.SetShininess(128.0f);
        
        // Assert
        Assert.Equal(128.0f, material.Shininess);
    }
    
    [Fact]
    public void Material_SetShininess_ClampsToMinimum()
    {
        // Arrange
        var material = new Material();
        
        // Act
        material.SetShininess(0.5f);
        
        // Assert
        Assert.Equal(1.0f, material.Shininess);
    }
    
    [Fact]
    public void Material_LoadTexture_ValidPath_LoadsSuccessfully()
    {
        // Arrange
        var material = new Material();
        
        // Create a temporary file for testing
        string tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, "dummy texture data");
        
        try
        {
            // Act
            material.LoadTexture(tempFile);
            
            // Assert
            Assert.True(material.HasTexture);
        }
        finally
        {
            // Cleanup
            File.Delete(tempFile);
            material.Dispose();
        }
    }
    
    [Fact]
    public void Material_LoadTexture_InvalidPath_CreatesDefaultTexture()
    {
        // Arrange
        var material = new Material();
        
        // Act
        material.LoadTexture("nonexistent_file.png");
        
        // Assert
        Assert.True(material.HasTexture); // Should have default texture
        
        // Cleanup
        material.Dispose();
    }
    
    [Fact]
    public void Material_LoadTexture_EmptyPath_ThrowsException()
    {
        // Arrange
        var material = new Material();
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => material.LoadTexture(""));
        Assert.Throws<ArgumentException>(() => material.LoadTexture(null!));
    }
    
    [Fact]
    public void Material_Bind_CallsSuccessfully()
    {
        // Arrange
        var material = new Material("TestMaterial");
        
        // Act & Assert (should not throw)
        material.Bind();
        
        // Cleanup
        material.Dispose();
    }
    
    [Fact]
    public void Material_Bind_AfterDispose_ThrowsException()
    {
        // Arrange
        var material = new Material();
        material.Dispose();
        
        // Act & Assert
        Assert.Throws<ObjectDisposedException>(() => material.Bind());
    }
    
    [Fact]
    public void Material_CreateWoodMaterial_HasCorrectProperties()
    {
        // Act
        var material = Material.CreateWoodMaterial();
        
        // Assert
        Assert.Equal("Wood", material.Name);
        Assert.Equal(new Vector3(0.6f, 0.4f, 0.2f), material.DiffuseColor);
        Assert.Equal(new Vector3(0.2f, 0.15f, 0.1f), material.AmbientColor);
        Assert.Equal(new Vector3(0.3f, 0.3f, 0.3f), material.SpecularColor);
        Assert.Equal(8.0f, material.Shininess);
        Assert.True(material.IsValid());
        
        // Cleanup
        material.Dispose();
    }
    
    [Fact]
    public void Material_CreateGrassMaterial_HasCorrectProperties()
    {
        // Act
        var material = Material.CreateGrassMaterial();
        
        // Assert
        Assert.Equal("Grass", material.Name);
        Assert.Equal(new Vector3(0.2f, 0.6f, 0.2f), material.DiffuseColor);
        Assert.Equal(new Vector3(0.1f, 0.2f, 0.1f), material.AmbientColor);
        Assert.Equal(new Vector3(0.1f, 0.1f, 0.1f), material.SpecularColor);
        Assert.Equal(4.0f, material.Shininess);
        Assert.True(material.IsValid());
        
        // Cleanup
        material.Dispose();
    }
    
    [Fact]
    public void Material_CreateCornMaterial_HasCorrectProperties()
    {
        // Act
        var material = Material.CreateCornMaterial();
        
        // Assert
        Assert.Equal("Corn", material.Name);
        Assert.Equal(new Vector3(0.4f, 0.7f, 0.2f), material.DiffuseColor);
        Assert.Equal(new Vector3(0.15f, 0.25f, 0.1f), material.AmbientColor);
        Assert.Equal(new Vector3(0.2f, 0.2f, 0.2f), material.SpecularColor);
        Assert.Equal(6.0f, material.Shininess);
        Assert.True(material.IsValid());
        
        // Cleanup
        material.Dispose();
    }
    
    [Fact]
    public void Material_CreateMetalMaterial_HasCorrectProperties()
    {
        // Act
        var material = Material.CreateMetalMaterial();
        
        // Assert
        Assert.Equal("Metal", material.Name);
        Assert.Equal(new Vector3(0.7f, 0.7f, 0.7f), material.DiffuseColor);
        Assert.Equal(new Vector3(0.1f, 0.1f, 0.1f), material.AmbientColor);
        Assert.Equal(new Vector3(1.0f, 1.0f, 1.0f), material.SpecularColor);
        Assert.Equal(128.0f, material.Shininess);
        Assert.True(material.IsValid());
        
        // Cleanup
        material.Dispose();
    }
    
    [Fact]
    public void Material_IsValid_ValidMaterial_ReturnsTrue()
    {
        // Arrange
        var material = new Material();
        
        // Act & Assert
        Assert.True(material.IsValid());
    }
    
    [Fact]
    public void Material_IsValid_InvalidDiffuseColor_ReturnsFalse()
    {
        // Arrange
        var material = new Material();
        material.DiffuseColor = new Vector3(-0.1f, 0.5f, 0.5f); // Negative value
        
        // Act & Assert
        Assert.False(material.IsValid());
    }
    
    [Fact]
    public void Material_IsValid_InvalidShininess_ReturnsFalse()
    {
        // Arrange
        var material = new Material();
        material.Shininess = -1.0f; // Negative shininess
        
        // Act & Assert
        Assert.False(material.IsValid());
    }
    
    [Fact]
    public void Material_IsValid_EmptyName_ReturnsFalse()
    {
        // Arrange
        var material = new Material();
        material.Name = "";
        
        // Act & Assert
        Assert.False(material.IsValid());
    }
    
    [Fact]
    public void Material_ToString_ReturnsCorrectFormat()
    {
        // Arrange
        var material = new Material("TestMaterial");
        
        // Act
        string result = material.ToString();
        
        // Assert
        Assert.Contains("TestMaterial", result);
        Assert.Contains("Diffuse", result);
        Assert.Contains("Ambient", result);
        Assert.Contains("Specular", result);
        Assert.Contains("Shininess", result);
        Assert.Contains("HasTexture", result);
    }
    
    [Fact]
    public void Material_Dispose_CleansUpResources()
    {
        // Arrange
        var material = new Material();
        
        // Create a temporary file and load it as texture
        string tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, "dummy texture data");
        
        try
        {
            material.LoadTexture(tempFile);
            Assert.True(material.HasTexture);
            
            // Act
            material.Dispose();
            
            // Assert - should not throw when disposed
            material.Dispose(); // Multiple dispose calls should be safe
        }
        finally
        {
            File.Delete(tempFile);
        }
    }
}