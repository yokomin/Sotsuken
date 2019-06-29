sampler2D _GrabTexture;
half _Blur;
int _Size;
struct v2fAve
{
    float4 pos: SV_POSITION;
    float4 screenPos: TEXCOORD1;
};
v2fAve vertAve(appdata_full v)
{
    v2fAve o;
    o.pos = UnityObjectToClipPos(v.vertex);
    o.screenPos = ComputeScreenPos(o.pos);
    return o;
}
half4 fragAve(v2fAve i): SV_Target
{
    float2 uv = i.screenPos.xy / i.screenPos.w;
    half4 frost = 0.0;
    #ifdef _AVE
        for (int m = - (_Size - 1) / 2; m <= (_Size - 1) / 2; m ++)
        {
            for (int n = - (_Size - 1) / 2; n <= (_Size - 1) / 2; n ++)
            {
                frost += tex2D(_GrabTexture, uv + float2(_Blur * m, _Blur * n));
            }
        }
        frost /= _Size * _Size;
    #else
        frost = tex2D(_GrabTexture, uv);
    #endif
    return half4(frost.xyz, 1);
}
