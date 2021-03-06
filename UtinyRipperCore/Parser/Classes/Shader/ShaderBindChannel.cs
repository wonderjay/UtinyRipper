﻿namespace UtinyRipper.Classes.Shaders
{
	public struct ShaderBindChannel : IAssetReadable
	{
		public ShaderBindChannel(ShaderChannel source, VertexComponent target)
		{
			Source = source;
			Target = target;
		}

		public void Read(AssetReader reader)
		{
			Source = (ShaderChannel)reader.ReadByte();
			Target = (VertexComponent)reader.ReadByte();
		}

		public ShaderChannel Source { get; private set; }
		public VertexComponent Target { get; private set; }
	}
}
