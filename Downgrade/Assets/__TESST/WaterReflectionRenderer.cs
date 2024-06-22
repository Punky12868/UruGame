using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200015A RID: 346
public class WaterReflectionRenderer : MonoBehaviour
{
    // Token: 0x06000DB6 RID: 3510 RVA: 0x0004F0D9 File Offset: 0x0004D2D9
    private void Start()
    {
        if (this.enableOutsideArena)
        {
            this.Initialize();
        }
    }

    // Token: 0x06000DB7 RID: 3511 RVA: 0x0004F0EC File Offset: 0x0004D2EC
    public bool AddGameObject(GameObject gameObject)
    {
        bool result = false;
        Renderer component = gameObject.GetComponent<Renderer>();
        if (component != null)
        {
            Mesh mesh = null;
            MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
            if (component is SpriteRenderer)
            {
                SpriteRenderer spriteRenderer = component as SpriteRenderer;
                mesh = new Mesh();
                mesh.vertices = Array.ConvertAll<Vector2, Vector3>(spriteRenderer.sprite.vertices, (Vector2 i) => i);
                mesh.uv = spriteRenderer.sprite.uv;
                mesh.triangles = Array.ConvertAll<ushort, int>(spriteRenderer.sprite.triangles, (ushort i) => (int)i);
                if (spriteRenderer.gameObject.GetComponent<Animator>() != null)
                {
                    this.animatedSprites.Add(spriteRenderer);
                }
                this.UpdateMaterialPropertyBlock(materialPropertyBlock, spriteRenderer.sprite.texture, spriteRenderer.color);
                materialPropertyBlock.SetTexture(this.mainTextureID, spriteRenderer.sprite.texture);
                materialPropertyBlock.SetColor(this.rendererColorID, spriteRenderer.color);
            }
            else
            {
                MeshFilter component2 = gameObject.GetComponent<MeshFilter>();
                if (component2 != null && component2.mesh != null)
                {
                    mesh = component2.mesh;
                    this.UpdateMaterialPropertyBlock(materialPropertyBlock, component.material.HasProperty(this.mainTextureID) ? component.material.mainTexture : null, component.material.HasProperty(this.colorID) ? component.material.color : Color.white);
                    if (component.material.HasProperty(this.mainTextureID) && component.material.mainTexture != null)
                    {
                        materialPropertyBlock.SetTexture(this.mainTextureID, component.material.mainTexture);
                    }
                    if (component.material.HasProperty(this.colorID) && component.material.color != Color.white)
                    {
                        materialPropertyBlock.SetColor(this.rendererColorID, component.material.color);
                    }
                }
            }
            if (mesh != null && this.renderers.Add(component))
            {
                this.meshCache.Add(component, mesh);
                if (!materialPropertyBlock.isEmpty)
                {
                    this.materialPropertyBlocks.Add(component, materialPropertyBlock);
                }
                result = true;
            }
        }
        return result;
    }

    // Token: 0x06000DB8 RID: 3512 RVA: 0x0004F350 File Offset: 0x0004D550
    public bool RemoveGameObject(GameObject gameObject)
    {
        bool result = false;
        Renderer component = gameObject.GetComponent<Renderer>();
        if (component != null)
        {
            this.CleanUpRenderer(component);
            result = this.renderers.Remove(component);
        }
        return result;
    }

    // Token: 0x06000DB9 RID: 3513 RVA: 0x0004F384 File Offset: 0x0004D584
    private void CleanUpRenderer(Renderer renderer)
    {
        this.meshCache.Remove(renderer);
        this.materialPropertyBlocks.Remove(renderer);
        if (renderer is SpriteRenderer)
        {
            this.animatedSprites.Remove(renderer as SpriteRenderer);
        }
    }

    // Token: 0x06000DBA RID: 3514 RVA: 0x0004F3BC File Offset: 0x0004D5BC
    public void Initialize()
    {
        if (SystemInfo.graphicsShaderLevel < 30)
        {
            MeshRenderer component = base.GetComponent<MeshRenderer>();
            if (component != null)
            {
                component.material.shader.maximumLOD = 100;
            }
            base.enabled = false;
            return;
        }
        this.waterPlaneID = Shader.PropertyToID("_WaterPlane");
        this.mainTextureID = Shader.PropertyToID("_MainTex");
        this.colorID = Shader.PropertyToID("_Color");
        this.rendererColorID = Shader.PropertyToID("_RendererColor");
        foreach (ReflectionComponent reflectionComponent in UnityEngine.Object.FindObjectsOfType<ReflectionComponent>())
        {
            this.AddGameObject(reflectionComponent.gameObject);
        }
        if (this.ReflectionMaterial != null)
        {
            this.reflectionMaterialSprite = new Material(this.ReflectionMaterial);
            this.reflectionMaterialSprite.EnableKeyword("SPRITE_RENDERER");
            this.reflectionMaterialSprite.SetFloat("_Cull", 0f);
            this.ReflectionMaterial.DisableKeyword("SPRITE_RENDERER");
            this.ReflectionMaterial.SetFloat("_Cull", 1f);
        }
        MeshFilter component2 = base.GetComponent<MeshFilter>();
        if (component2 != null)
        {
            this.waterMesh = component2.mesh;
        }
    }

    // Token: 0x06000DBB RID: 3515 RVA: 0x0004F4E8 File Offset: 0x0004D6E8
    private void Update()
    {
        if (this.renderers.Count > 0 && this.ReflectionMaterial != null)
        {
            if (this.waterMesh != null && this.BeforeMaterial != null)
            {
                Graphics.DrawMesh(this.waterMesh, base.transform.localToWorldMatrix, this.BeforeMaterial, base.gameObject.layer, null, 0, null, false, false, false);
            }
            this.ReflectionMaterial.SetFloat(this.waterPlaneID, base.transform.position.y);
            this.reflectionMaterialSprite.SetFloat(this.waterPlaneID, base.transform.position.y);
            foreach (Renderer renderer2 in this.renderers)
            {
                this.DrawRenderer(renderer2);
            }
            this.renderers.RemoveWhere((Renderer renderer) => renderer == null);
            if (this.waterMesh != null && this.AfterMaterial != null)
            {
                Graphics.DrawMesh(this.waterMesh, base.transform.localToWorldMatrix, this.AfterMaterial, base.gameObject.layer, null, 0, null, false, false, false);
            }
        }
    }

    // Token: 0x06000DBC RID: 3516 RVA: 0x0004F65C File Offset: 0x0004D85C
    private void UpdateMaterialPropertyBlock(MaterialPropertyBlock materialPropertyBlock, Texture texture, Color color)
    {
        materialPropertyBlock.SetTexture(this.mainTextureID, (texture != null) ? texture : Texture2D.whiteTexture);
        materialPropertyBlock.SetColor(this.rendererColorID, color);
    }

    // Token: 0x06000DBD RID: 3517 RVA: 0x0004F688 File Offset: 0x0004D888
    private void DrawRenderer(Renderer renderer)
    {
        if (renderer != null)
        {
            Mesh mesh = null;
            Material reflectionMaterial = this.ReflectionMaterial;
            if (reflectionMaterial != null && this.meshCache.TryGetValue(renderer, out mesh))
            {
                MaterialPropertyBlock materialPropertyBlock = null;
                this.materialPropertyBlocks.TryGetValue(renderer, out materialPropertyBlock);
                if (renderer is SpriteRenderer)
                {
                    SpriteRenderer spriteRenderer = renderer as SpriteRenderer;
                    if (this.animatedSprites.Contains(spriteRenderer))
                    {
                        mesh.Clear();
                        if (spriteRenderer.sprite != null)
                        {
                            mesh.vertices = Array.ConvertAll<Vector2, Vector3>(spriteRenderer.sprite.vertices, (Vector2 i) => i);
                            mesh.uv = spriteRenderer.sprite.uv;
                            mesh.triangles = Array.ConvertAll<ushort, int>(spriteRenderer.sprite.triangles, (ushort i) => (int)i);
                        }
                    }
                    this.UpdateMaterialPropertyBlock(materialPropertyBlock, spriteRenderer.sprite.texture, spriteRenderer.color);
                    reflectionMaterial = this.reflectionMaterialSprite;
                }
                for (int j = 0; j < mesh.subMeshCount; j++)
                {
                    Graphics.DrawMesh(mesh, renderer.transform.localToWorldMatrix, reflectionMaterial, renderer.gameObject.layer, null, j, materialPropertyBlock, false, false, false);
                }
                return;
            }
        }
        else
        {
            this.CleanUpRenderer(renderer);
        }
    }

    // Token: 0x04001402 RID: 5122
    public Material BeforeMaterial;

    // Token: 0x04001403 RID: 5123
    public Material ReflectionMaterial;

    // Token: 0x04001404 RID: 5124
    public Material AfterMaterial;

    // Token: 0x04001405 RID: 5125
    [Space]
    public bool enableOutsideArena;

    // Token: 0x04001406 RID: 5126
    private Material reflectionMaterialSprite;

    // Token: 0x04001407 RID: 5127
    private int waterPlaneID;

    // Token: 0x04001408 RID: 5128
    private int mainTextureID;

    // Token: 0x04001409 RID: 5129
    private int colorID;

    // Token: 0x0400140A RID: 5130
    private int rendererColorID;

    // Token: 0x0400140B RID: 5131
    private HashSet<Renderer> renderers = new HashSet<Renderer>();

    // Token: 0x0400140C RID: 5132
    private Dictionary<Renderer, Mesh> meshCache = new Dictionary<Renderer, Mesh>();

    // Token: 0x0400140D RID: 5133
    private Dictionary<Renderer, MaterialPropertyBlock> materialPropertyBlocks = new Dictionary<Renderer, MaterialPropertyBlock>();

    // Token: 0x0400140E RID: 5134
    private HashSet<SpriteRenderer> animatedSprites = new HashSet<SpriteRenderer>();

    // Token: 0x0400140F RID: 5135
    private Mesh waterMesh;
}
