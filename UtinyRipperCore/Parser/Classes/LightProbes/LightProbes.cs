﻿using System.Collections.Generic;
using UtinyRipper.AssetExporters;
using UtinyRipper.Classes.LightProbess;
using UtinyRipper.Classes.RenderSettingss;
using UtinyRipper.Exporter.YAML;

namespace UtinyRipper.Classes
{
	public sealed class LightProbes : NamedObject
	{
		public LightProbes(AssetInfo assetInfo):
			base(assetInfo)
		{
		}

		/// <summary>
		/// 5.0.0b1
		/// </summary>
		public static bool IsReadBakedCoefficients11(Version version)
		{
			return version.IsEqual(5, 0, 0, VersionType.Beta, 1);
		}
		/// <summary>
		/// 5.4.0 and greater
		/// </summary>
		public static bool IsReadBakedLightOcclusion(Version version)
		{
			return version.IsGreaterEqual(5, 4);
		}

		/// <summary>
		/// Less than 5.0.0
		/// </summary>
		private static bool IsReadBakedPositions(Version version)
		{
			return version.IsLess(5);
		}
		/// <summary>
		/// Less than 5.0.0
		/// </summary>
		private static bool IsReadBakedCoefficientsFirst(Version version)
		{
			return version.IsLess(5);
		}

		public override void Read(AssetReader reader)
		{
			base.Read(reader);

			if(IsReadBakedPositions(reader.Version))
			{
				m_bakedPositions = reader.ReadArray<Vector3f>();
			}
			if(IsReadBakedCoefficientsFirst(reader.Version))
			{
				m_bakedCoefficients = reader.ReadArray<SphericalHarmonicsL2>();
			}

			Data.Read(reader);
			if (!IsReadBakedCoefficientsFirst(reader.Version))
			{
				if(IsReadBakedCoefficients11(reader.Version))
				{
					m_bakedCoefficients11 = reader.ReadArray<SHCoefficientsBaked>();
				}
				else
				{
					m_bakedCoefficients = reader.ReadArray<SphericalHarmonicsL2>();
				}
			}
			if(IsReadBakedLightOcclusion(reader.Version))
			{
				m_bakedLightOcclusion = reader.ReadArray<LightProbeOcclusion>();
			}
		}

		protected override YAMLMappingNode ExportYAMLRoot(IExportContainer container)
		{
#warning TODO: values acording to read version (current 2017.3.0f3)
			YAMLMappingNode node = base.ExportYAMLRoot(container);
			node.Add("m_Data", Data.ExportYAML(container));
			node.Add("m_BakedCoefficients", BakedCoefficients.ExportYAML(container));
			node.Add("m_BakedLightOcclusion", BakedLightOcclusion.ExportYAML(container));
			return node;
		}

		public IReadOnlyList<Vector3f> BakedPositions => m_bakedPositions;
		public IReadOnlyList<SphericalHarmonicsL2> BakedCoefficients => m_bakedCoefficients;
		public IReadOnlyList<SHCoefficientsBaked> BakedCoefficients11 => m_bakedCoefficients11;
		public IReadOnlyList<LightProbeOcclusion> BakedLightOcclusion => m_bakedLightOcclusion;

		public LightProbeData Data;

		private Vector3f[] m_bakedPositions;
		private SphericalHarmonicsL2[] m_bakedCoefficients;
		private SHCoefficientsBaked[] m_bakedCoefficients11;
		private LightProbeOcclusion[] m_bakedLightOcclusion;
	}
}
