﻿using System;
using System.IO;
using ShaderPlayground.Core.Util;

namespace ShaderPlayground.Core.Compilers.Metal
{
    internal sealed class MetalCompiler : IShaderCompiler
    {
        public string Name { get; } = CompilerNames.Metal;
        public string DisplayName { get; } = "Metal";
        public string Url { get; } = "https://developer.apple.com/documentation/metal/shader_authoring";
        public string Description { get; } = "Metal provides a platform-optimized, low-overhead API for developing the latest 3D pro applications and amazing games using a rich shading language with tighter integration between graphics and compute programs.";

        public string[] InputLanguages { get; } = { LanguageNames.Metal };

        public ShaderCompilerParameter[] Parameters { get; } =
        {
            CommonParameters.CreateVersionParameter("metal"),
            new ShaderCompilerParameter("MetalVersion", "Metal Language Version", ShaderCompilerParameterType.ComboBox, MetalVersions, "macos-metal2.4"),
            CommonParameters.CreateOutputParameter(new[] { LanguageNames.MetalIR }),
        };

        private static readonly string[] MetalVersions =
        {
            "macos-metal1.0",
            "macos-metal1.1",
            "macos-metal1.2",
            "macos-metal2.0",
            "macos-metal2.1",
            "macos-metal2.2",
            "macos-metal2.3",
            "macos-metal2.4",
            "ios-metal1.0",
            "ios-metal1.1",
            "ios-metal1.2",
            "ios-metal2.0",
            "ios-metal2.1",
            "ios-metal2.2",
            "ios-metal2.3",
            "ios-metal2.4",
        };

        public ShaderCompilerResult Compile(ShaderCode shaderCode, ShaderCompilerArguments arguments)
        {
            using var tempFile = TempFile.FromShaderCode(shaderCode);

            var outputPath = $"{tempFile.FilePath}.ll";

            var includePath = Path.Combine(CommonParameters.GetBinaryPath("metal", arguments), "include", "metal");

            ProcessHelper.Run(
                CommonParameters.GetBinaryPath("metal", arguments, "metal.exe"),
                $"-std={arguments.GetString("MetalVersion")} -S -emit-llvm -I \"{includePath}\" -o \"{outputPath}\" \"{tempFile.FilePath}\"",
                out _,
                out var stdError);

            var textOutput = FileHelper.ReadAllTextIfExists(outputPath);

            FileHelper.DeleteIfExists(outputPath);

            var hasCompilationError = textOutput == null;

            return new ShaderCompilerResult(
                !hasCompilationError,
                !hasCompilationError ? new ShaderCode(LanguageNames.MetalIR, textOutput) : null,
                hasCompilationError ? (int?)1 : null,
                new ShaderCompilerOutput("Assembly", LanguageNames.MetalIR, textOutput),
                new ShaderCompilerOutput("Output", null, stdError));
        }
    }
}
