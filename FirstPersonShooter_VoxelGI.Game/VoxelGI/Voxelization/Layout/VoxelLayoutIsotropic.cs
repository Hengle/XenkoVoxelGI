﻿using System;
using System.Collections.Generic;
using System.Text;
using Xenko.Core;
using Xenko.Core.Annotations;
using Xenko.Shaders;
using Xenko.Rendering.Materials;

namespace Xenko.Rendering.Voxels
{
    [DataContract(DefaultMemberMode = DataMemberMode.Default)]
    [Display("Isotropic (single)")]
    public class VoxelLayoutIsotropic : IVoxelLayout
    {
        ShaderClassSource writer = new ShaderClassSource("VoxelIsotropicWriter_Float4");
        ShaderClassSource sampler = new ShaderClassSource("VoxelIsotropicSampler");

        [NotNull]
        public IStorageMethod StorageMethod { get; set; } = new StorageMethodIndirect();

        IVoxelStorageTexture IsotropicTex;

        public void PrepareLocalStorage(VoxelStorageContext context, IVoxelStorage storage)
        {
            StorageMethod.PrepareLocalStorage(context, storage, 4, 1);
            storage.UpdateTexture(context, ref IsotropicTex, Graphics.PixelFormat.R16G16B16A16_Float, 1);
        }

        ObjectParameterKey<Xenko.Graphics.Texture> DirectOutput;
        public void UpdateLayout(string compositionName)
        {
            DirectOutput = VoxelIsotropicWriter_Float4Keys.DirectOutput.ComposeWith(compositionName);
        }

        public ShaderSource GetSampler() {
            var mixin = new ShaderMixinSource();
            mixin.Mixins.Add(sampler);
            mixin.AddComposition("storage", IsotropicTex.GetSampler());
            return mixin;
        }

        public void UpdateSamplerLayout(string compositionName)
        {
            IsotropicTex.UpdateSamplerLayout("storage."+compositionName);
        }
        public void ApplyViewParameters(ParameterCollection parameters)
        {
            IsotropicTex.ApplyViewParameters(parameters);
        }

        public void ApplyWriteParameters(ParameterCollection parameters)
        {
            IsotropicTex.ApplyParametersWrite(DirectOutput, parameters);
        }
        public void PostProcess(RenderDrawContext drawContext)
        {
            IsotropicTex.PostProcess(drawContext);
        }


        public ShaderSource GetShaderFloat4(List<IVoxelModifierEmissionOpacity> modifiers)
        {
            var mixin = new ShaderMixinSource();
            mixin.Mixins.Add(writer);
            StorageMethod.Apply(mixin);
            foreach (var attr in modifiers)
            {
                ShaderSource applier = attr.GetApplier("Isotropic");
                if (applier!=null)
                    mixin.AddCompositionToArray("Modifiers", applier);
            }
            return mixin;
        }
        public ShaderSource GetShaderFloat3() { return null; }
        public ShaderSource GetShaderFloat2() { return null; }
        public ShaderSource GetShaderFloat1() { return null; }
    }
}
