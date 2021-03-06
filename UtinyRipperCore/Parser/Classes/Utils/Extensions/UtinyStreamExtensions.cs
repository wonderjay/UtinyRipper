﻿namespace UtinyRipper.Classes
{
	public static class UtinyStreamExtensions
	{
		public static Vector4f[] ReadVector3Array(this AssetReader reader)
		{
			int count = reader.ReadInt32();
			Vector4f[] array = new Vector4f[count];
			for(int i = 0; i < count; i++)
			{
				Vector4f vector = new Vector4f();
				vector.Read3(reader);
				array[i] = vector;
			}
			return array;
		}
	}
}
