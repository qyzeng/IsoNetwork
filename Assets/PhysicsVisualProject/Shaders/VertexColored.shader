// Upgrade NOTE: replaced 'SeperateSpecular' with 'SeparateSpecular'

Shader " VertexColored" {
Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _SpecColor ("Spec Color", Color) = (1,1,1,1)
    _Emission ("Emmisive Color", Color) = (0,0,0,0)
    _Shininess ("Shininess", Range (0.01, 1)) = 0.7
    _MainTex ("Base (RGB)", 2D) = "white" {}
}

SubShader {
    Pass {
        Material {
            Shininess [_Shininess]
            Specular [_SpecColor]
            Emission [_Emission]    
        }
        ColorMaterial AmbientAndDiffuse
        Cull Off
        Lighting On
        SeparateSpecular On
        SetTexture [_MainTex] {
            Combine texture * primary, texture * primary
        }
        SetTexture [_MainTex] {
            constantColor [_Color]
            Combine previous * constant DOUBLE, previous * constant
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
//FallBack "Transparent/Cutout/Diffuse"
}

