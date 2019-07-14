﻿// Copyright (c) Xenko contributors (https://xenko.com) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System.ComponentModel;
using Xenko.Graphics;

namespace Xenko.Rendering
{
    /// <summary>
    /// Pipline processor for <see cref="RenderMesh"/> that cast shadows, to properly disable culling and depth clip.
    /// </summary>
    public class VoxelPipelineProcessor : PipelineProcessor
    {
        public RenderStage VoxelRenderStage { get; set; }

        [DefaultValue(false)]
        public bool DepthClipping { get; set; } = false;

        public override void Process(RenderNodeReference renderNodeReference, ref RenderNode renderNode, RenderObject renderObject, PipelineStateDescription pipelineState)
        {
            // Disable culling and depth clip
            if (renderNode.RenderStage == VoxelRenderStage)
            {
                pipelineState.RasterizerState = new RasterizerStateDescription(CullMode.None) { DepthClipEnable = DepthClipping };
            }
        }
    }
}