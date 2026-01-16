#pragma multi_compile_local _ USE_ATLAS

float4 _textureRect;

//uvが描画内のパーツかどうか判定する
//内側なら1を返す。そうじゃなかったら0を返す
int IsInner(float2 uv, float2 allasSize, float4 textureRect)
{
    float width = allasSize.x;
    float minX = textureRect.x / width;  //パーツの左端
    float maxX = (textureRect.x + textureRect.z) / width;   //パーツの右端
    
    int insideOfLeftEdge = step(minX, uv.x);    //uv.xがパーツの左端より内側か
    int insideOfRightEdge = step(uv.x, maxX);   //uv.xがパーツの右端より内側か
    
    float height = allasSize.y;
    float minY = textureRect.y / height;     //パーツの下端座標
    float maxY = (textureRect.y + textureRect.w) / height;     //パーツの上端座標
    
    int insideOfBottomEdge = step(minY, uv.y);     //パーツ下端より内側か
    int insideOfTopEdge = step(uv.y, maxY);        //パーツ上端より内側か
    
    //上下左右の端より内側か判定
    return insideOfLeftEdge * insideOfRightEdge * insideOfBottomEdge * insideOfTopEdge;

}

float remap(float value, float inputMin, float inputMax, float outputMin, float outputMax)
{
    return (value - inputMin) * ((outputMax - outputMin) / (inputMax - inputMin)) + outputMin;

}

float2 AtlasUVtoMeshUV(float2 uv, float2 allasSize, float4 textureRect)
{
    float u = uv.x;
    float width = allasSize.x;
    float minX = textureRect.x / width;  //パーツの左端
    float maxX = (textureRect.x + textureRect.z) / width;    //パーツの右端
    u = remap(u, minX, maxX, 0, 1);
    
    float v = uv.y;
    float height = allasSize.y;
    float minY = textureRect.y / height;    //パーツの下端座標
    float maxY = (textureRect.y + textureRect.w) / height;    //パーツの上端座標
    v = remap(v, minY, maxY, 0, 1);
    
    float2 localUv = float2(u, v);
    return localUv;

}

float2 MeshUVtoAtlasUV(float2 localUV, float2 allasSize, float4 textureRect)
{
    float width = textureRect.z;
    //atlas内のpixel座標を求める
    float x = textureRect.x + width * localUV.x;
    float height = textureRect.w;
    float y = textureRect.y + height * localUV.y;
    
    //0～1に正規化する
    return float2(x / allasSize.x, y / allasSize.y);

}