

// This compares nearby points in the texture
void SampleValueSpread_float(in Texture2D t, float2 uv, sampler s, float offset, uint samples, out float Out) {

  float c = t.SampleLevel(s, uv, 0);

  float difs = 0;

  for(uint i = 0; i < samples; i++) {
    
    float2 pos = uv + float2(offset, 0) * (i + 1);
    float v = t.SampleLevel(s, pos, 0);
    if (v != c) difs++;

    pos = uv - float2(offset, 0) * (i + 1);
    v = t.SampleLevel(s, pos, 0);
    if (v != c) difs++;
  }
  
  for(uint j = 0; j < samples; j++) {
    
    float2 pos = uv + float2(0, offset) * (j + 1);
    float v = t.SampleLevel(s, pos, 0);
    if (v != c) difs++;

    pos = uv - float2(0, offset) * (j + 1); 
    v = t.SampleLevel(s, pos, 0);
    if (v != c) difs++;
  }


  for(uint k = 0; k < samples; k++) {
    
    float2 pos = uv + float2(offset, offset) * (k + 1);
    float v = t.SampleLevel(s, pos, 0);
    if (v != c) difs++;

    pos = uv - float2(offset, offset) * (k + 1); 
    v = t.SampleLevel(s, pos, 0);
    if (v != c) difs++;
  }

  for(uint l = 0; l < samples; l++) {
    
    float2 pos = uv + float2(-offset, offset) * (l + 1); 
    float v = t.SampleLevel(s, pos, 0);
    if (v != c) difs++;

    pos = uv - float2(-offset, offset) * (l + 1); 
    v = t.SampleLevel(s, pos, 0);
    if (v != c) difs++;
  }

  Out = difs / (samples * 8);
}
