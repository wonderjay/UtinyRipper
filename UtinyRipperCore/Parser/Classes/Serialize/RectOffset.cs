﻿using System.Collections.Generic;
using UtinyRipper.AssetExporters;
using UtinyRipper.Exporter.YAML;
using UtinyRipper.SerializedFiles;

namespace UtinyRipper.Classes
{
	public struct RectOffset : IScriptStructure
	{
		public RectOffset(RectOffset copy)
		{
			Left = copy.Left;
			Right = copy.Right;
			Top = copy.Top;
			Bottom = copy.Bottom;
		}

		public IScriptStructure CreateCopy()
		{
			return new RectOffset(this);
		}

		public void Read(AssetReader reader)
		{
			Left = reader.ReadInt32();
			Right = reader.ReadInt32();
			Top = reader.ReadInt32();
			Bottom = reader.ReadInt32();
		}

		public YAMLNode ExportYAML(IExportContainer container)
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add("m_Left", Left);
			node.Add("m_Right", Right);
			node.Add("m_Top", Top);
			node.Add("m_Bottom", Bottom);
			return node;
		}

		public IEnumerable<Object> FetchDependencies(ISerializedFile file, bool isLog = false)
		{
			yield break;
		}

		public IScriptStructure Base => null;
		public string Namespace => ScriptType.UnityEngineName;
		public string Name => ScriptType.RectOffsetName;

		public int Left { get; private set; }
		public int Right { get; private set; }
		public int Top { get; private set; }
		public int Bottom { get; private set; }
	}
}
