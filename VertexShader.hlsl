// Vertex Shader for 3D rendering with MVP transformations
// Transforms vertices and passes data to pixel shader

// Constant buffer for matrix transformations
cbuffer MatrixBuffer : register(b0)
{
    matrix modelMatrix;
    matrix viewMatrix;
    matrix projectionMatrix;
    matrix mvpMatrix; // Pre-computed model-view-projection matrix
};

// Input structure from vertex buffer
struct VertexInput
{
    float3 position : POSITION;
    float3 normal : NORMAL;
    float2 texCoords : TEXCOORD0;
};

// Output structure to pixel shader
struct VertexOutput
{
    float4 position : SV_POSITION;
    float3 worldPosition : POSITION;
    float3 normal : NORMAL;
    float2 texCoords : TEXCOORD0;
};

VertexOutput main(VertexInput input)
{
    VertexOutput output;
    
    // Transform vertex position to world space
    float4 worldPos = mul(float4(input.position, 1.0f), modelMatrix);
    output.worldPosition = worldPos.xyz;
    
    // Transform to clip space using MVP matrix
    output.position = mul(worldPos, viewMatrix);
    output.position = mul(output.position, projectionMatrix);
    
    // Transform normal to world space (assuming uniform scaling)
    output.normal = normalize(mul(input.normal, (float3x3)modelMatrix));
    
    // Pass through texture coordinates
    output.texCoords = input.texCoords;
    
    return output;
}