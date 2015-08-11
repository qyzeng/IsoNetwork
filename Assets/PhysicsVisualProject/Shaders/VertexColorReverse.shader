Shader "VertexColorReverse" {
Properties {
  _Color ("Main Color", Color) = (1,1,1,1)
  _MainTex ("Base (RGB)", 2D) = "white" {}
  //BumpMap ("Bump (RGB) Illumin (A)", 2D) = "bump" {}
}
SubShader {     
  //UsePass "Self-Illumin/VertexLit/BASE"
  //UsePass "Bumped Diffuse/PPL"
  //Ambient pass
  Pass {
  Name "BASE"
  Tags {"LightMode" = "Always" /* Upgrade NOTE: changed from PixelOrNone to Always */}
  Color [_PPLAmbient]
  SetTexture [_BumpMap] {
   constantColor (.5,.5,.5)
   combine constant lerp (texture) previous
  }
  SetTexture [_MainTex] {
   constantColor [_Color]
   Combine texture * previous DOUBLE, texture*constant
  }
  }
  //Vertex lights

 Pass {
  Name "BASE"
  Tags {"LightMode" = "Vertex"}
  Material {
   Diffuse [_Color]
   Emission [_PPLAmbient]
   Shininess [_Shininess]
   Specular [_SpecColor]
  }
  SeparateSpecular On
  Lighting On
  Cull Off
  SetTexture [_BumpMap] {
   constantColor (.5,.5,.5)
   combine constant lerp (texture) previous
  }
  SetTexture [_MainTex] {
   Combine texture * previous DOUBLE, texture*primary
  }
  }
  
      	Pass{
			Blend One One
			Lighting Off
			Cull Off

			BindChannels{
				Bind "Vertex", vertex
				Bind "TexCoord", texcoord
				Bind "Color", color
				Bind "Normal",Normal
			}

			SetTexture [_MainTex]{
				Combine primary * texture
			}
			SetTexture [_MainTex]{
				constantColor [_Color]
				Combine previous * constant
			}
		}
}
Fallback " VertexLit", 1

}
