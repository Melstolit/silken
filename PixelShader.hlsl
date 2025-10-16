// Pixel Shader with Phong lighting model
// Implements basic lighting calculations and texture sampling

// Constant buffer for lighting and fog parameters
cbuffer LightingBuffer : register(b1)
{
    float3 lightDirection;
    float lightIntensity;
    float3 lightColor;
    float ambientStrength;
    float3 cameraPosition;
    float specularStrength;
    float fogEnabled;
    float fogStart;
    float fogEnd;
    float fogPadding; // Padding for 16-byte alignment
    float3 fogColor;
    float fogPadding2; // Padding for 16-byte alignment
};

// Constant buffer for material properties
cbuffer MaterialBuffer : register(b2)
{
    float3 materialDiffuse;
    float materialShininess;
    float3 materialAmbient;
    float materialSpecular;
};

// Texture and sampler
Texture2D diffuseTexture : register(t0);
SamplerState textureSampler : register(s0);

// Input from vertex shader
struct PixelInput
{
    float4 position : SV_POSITION;
    float3 worldPosition : POSITION;
    float3 normal : NORMAL;
    float2 texCoords : TEXCOORD0;
};

// Output color
struct PixelOutput
{
    float4 color : SV_TARGET;
};

PixelOutput main(PixelInput input)
{
    PixelOutput output;
    
    // Sample the diffuse texture
    float4 textureColor = diffuseTexture.Sample(textureSampler, input.texCoords);
    
    // Normalize the normal vector
    float3 normal = normalize(input.normal);
    
    // Calculate light direction (assuming directional light)
    float3 lightDir = normalize(-lightDirection);
    
    // Ambient lighting
    float3 ambient = ambientStrength * materialAmbient * lightColor;
    
    // Diffuse lighting (Lambertian)
    float diffuseFactor = max(dot(normal, lightDir), 0.0f);
    float3 diffuse = diffuseFactor * materialDiffuse * lightColor * lightIntensity;
    
    // Specular lighting (Phong)
    float3 viewDir = normalize(cameraPosition - input.worldPosition);
    float3 reflectDir = reflect(-lightDir, normal);
    float specularFactor = pow(max(dot(viewDir, reflectDir), 0.0f), materialShininess);
    float3 specular = specularStrength * specularFactor * materialSpecular * lightColor;
    
    // Combine all lighting components
    float3 litColor = (ambient + diffuse + specular) * textureColor.rgb;
    
    // Apply distance-based fog if enabled
    float3 finalColor = litColor;
    if (fogEnabled > 0.5f) // Check if fog is enabled
    {
        // Calculate distance from camera to fragment
        float distance = length(cameraPosition - input.worldPosition);
        
        // Calculate fog factor (0 = no fog, 1 = full fog)
        float fogFactor = saturate((distance - fogStart) / (fogEnd - fogStart));
        
        // Linear interpolation between lit color and fog color
        finalColor = lerp(litColor, fogColor, fogFactor);
    }
    
    // Output final color with alpha
    output.color = float4(finalColor, textureColor.a);
    
    return output;
}