﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UtinyRipper.AssetExporters.Mono;

namespace UtinyRipper
{
	public static class FilenameUtils
	{
		public static bool IsDefaultResource(string fileName)
		{
			return fileName == DefaultResourceName1 || fileName == DefaultResourceName2;
		}

		public static bool IsBuiltinExtra(string fileName)
		{
			return fileName == BuiltinExtraName1 || fileName == BuiltinExtraName2;
		}

		public static string FixFileIdentifier(string name)
		{
			name = name.ToLowerInvariant();
			name = FixDependencyName(name);
			name = FixResourcePath(name);
			if(IsDefaultResource(name))
			{
				name = DefaultResourceName1;
			}
			else if(IsBuiltinExtra(name))
			{
				name = BuiltinExtraName1;
			}
			return name;
		}

		public static string FixDependencyName(string dependency)
		{
			if (dependency.StartsWith(LibraryFolder, StringComparison.Ordinal))
			{
				return dependency.Substring(LibraryFolder.Length + 1);
			}
			else if (dependency.StartsWith(ResourcesFolder, StringComparison.Ordinal))
			{
				return dependency.Substring(ResourcesFolder.Length + 1);
			}
			return dependency;
		}

		public static string FixResourcePath(string resourcePath)
		{
			const string archivePrefix = "archive:/";
			if (resourcePath.StartsWith(archivePrefix, StringComparison.Ordinal))
			{
				resourcePath = Path.GetFileName(resourcePath);
			}
			return resourcePath;
		}

		public static string FixAssemblyName(string assembly)
		{
			if (IsAssemblyIdentifier(assembly))
			{
				assembly = $"Assembly - {assembly}";
			}
			if(assembly.EndsWith(MonoManager.AssemblyExtension))
			{
				assembly = assembly.Substring(0, assembly.Length - MonoManager.AssemblyExtension.Length);
			}
			return assembly;
		}

		public static bool IsProjectAssembly(string assembly)
		{
			const string PrefixName = "Assembly";
			return assembly.StartsWith($"{PrefixName} - ", StringComparison.Ordinal) || assembly.StartsWith($"{PrefixName}-", StringComparison.Ordinal);
		}

		private static bool IsAssemblyIdentifier(string assembly)
		{
			switch(assembly)
			{
				case "Boo":
				case "Boo - first pass":
				case "CSharp":
				case "CSharp - first pass":
				case "UnityScript":
				case "UnityScript - first pass":
					return true;

				default:
					return false;
			}
		}

		public const string LibraryFolder = "library";
		public const string ResourcesFolder = "resources";
		public const string DefaultResourceName1 = "unity default resources";
		public const string DefaultResourceName2 = "unity_default_resources";
		public const string BuiltinExtraName1 = "unity builtin extra";
		public const string BuiltinExtraName2 = "unity_builtin_extra";
	}
}
