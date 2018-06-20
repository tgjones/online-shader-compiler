﻿using ShaderPlayground.Core.Util;

namespace ShaderPlayground.Core.Compilers.SpirvTools
{
    internal sealed class SpirvCfgCompiler : IShaderCompiler
    {
        public string Name { get; } = CompilerNames.SpirvCfg;
        public string DisplayName { get; } = "spirv-cfg";
        public string Description { get; } = "Shows the control flow graph in GraphViz 'dot' form.";

        public string[] InputLanguages { get; } = { LanguageNames.SpirV };

        public ShaderCompilerParameter[] Parameters { get; } =
        {
            CommonParameters.CreateVersionParameter("spirv-tools")
        };

        public ShaderCompilerResult Compile(ShaderCode shaderCode, ShaderCompilerArguments arguments)
        {
            using (var tempFile = TempFile.FromShaderCode(shaderCode))
            {
                ProcessHelper.Run(
                    CommonParameters.GetBinaryPath("spirv-tools", arguments, "spirv-cfg.exe"),
                    $"\"{tempFile.FilePath}\"",
                    out var stdOutput,
                    out var _);

                return new ShaderCompilerResult(
                    true,
                    null,
                    null,
                    new ShaderCompilerOutput("Output", "graphviz", stdOutput));
            }
        }
    }
}