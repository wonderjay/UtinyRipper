﻿using System.Collections.Generic;
using System.IO;
using UtinyRipper.AssetExporters;

namespace UtinyRipper.Exporters.Scripts
{
	public abstract class ScriptExportType
	{
		public abstract void Init(IScriptExportManager manager);
		
		public void Export(TextWriter writer)
		{
			ExportUsings(writer);
			if (Namespace == string.Empty)
			{
				Export(writer, 0);
			}
			else
			{
				writer.WriteLine("namespace {0}", Namespace);
				writer.WriteLine('{');
				Export(writer, 1);
				writer.WriteLine('}');
			}
		}

		public virtual void Export(TextWriter writer, int intent)
		{
			if (IsSerializable)
			{
				writer.WriteIntent(intent);
				writer.WriteLine("[{0}]", ScriptExportAttribute.SerializableName);
			}

			writer.WriteIntent(intent);
			writer.Write("{0} {1} {2}", Keyword, IsStruct ? "struct" : "class", TypeName);

			if (Base != null && !ScriptType.IsBasic(Base.Namespace, Base.Name))
			{
				writer.Write(" : {0}", Base.GetTypeNestedName(DeclaringType));
			}
			writer.WriteLine();

			writer.WriteIntent(intent++);
			writer.WriteLine('{');

			foreach (ScriptExportType nestedType in NestedTypes)
			{
				nestedType.Export(writer, intent);
				writer.WriteLine();
			}

			foreach (ScriptExportEnum nestedEnum in NestedEnums)
			{
				nestedEnum.Export(writer, intent);
				writer.WriteLine();
			}

			foreach (ScriptExportDelegate @delegate in Delegates)
			{
				@delegate.Export(writer, intent);
			}
			if (Delegates.Count > 0)
			{
				writer.WriteLine();
			}

			foreach (ScriptExportField field in Fields)
			{
				field.Export(writer, intent);
			}

			writer.WriteIntent(--intent);
			writer.WriteLine('}');
		}

		public virtual void GetTypeNamespaces(ICollection<string> namespaces)
		{
			if (ScriptType.IsCPrimitive(Namespace, ClearName))
			{
				return;
			}
			namespaces.Add(ExportNamespace());
		}

		public virtual void GetUsedNamespaces(ICollection<string> namespaces)
		{
			GetTypeNamespaces(namespaces);
			if (Base != null)
			{
				Base.GetTypeNamespaces(namespaces);
			}
			foreach(ScriptExportType nestedType in NestedTypes)
			{
				nestedType.GetUsedNamespaces(namespaces);
			}
			foreach (ScriptExportDelegate @delegate in Delegates)
			{
				@delegate.GetUsedNamespaces(namespaces);
			}
			foreach (ScriptExportField field in Fields)
			{
				field.GetUsedNamespaces(namespaces);
			}
		}

		public virtual bool HasMember(string name)
		{
			switch (name)
			{
				case "audio":
				case "light":
				case "collider":
					return ScriptType.IsComponent(Namespace, Name);
			}
			return false;
		}

		public string GetTypeNestedName(ScriptExportType relativeType)
		{
			if(relativeType == null)
			{
				return Name;
			}
			if (ScriptType.IsEngineObject(Namespace, Name))
			{
				return $"{Namespace}.{Name}";
			}
			if (DeclaringType == null)
			{
				return TypeName;
			}
			if (relativeType == DeclaringType)
			{
				return TypeName;
			}

			string declaringName = DeclaringType.GetTypeNestedName(relativeType);
			return $"{declaringName}.{TypeName}";
		}

		public override string ToString()
		{
			if(FullName == null)
			{
				return base.ToString();
			}
			else
			{
				return FullName;
			}
		}

		protected void AddAsNestedType()
		{
			DeclaringType.m_nestedTypes.Add(this);
		}

		protected void AddAsNestedEnum()
		{
			DeclaringType.m_nestedEnums.Add((ScriptExportEnum)this);
		}

		protected void AddAsNestedDelegate()
		{
			DeclaringType.m_nestedDelegates.Add((ScriptExportDelegate)this);
		}

		private void ExportUsings(TextWriter writer)
		{
			HashSet<string> namespaces = new HashSet<string>();
			GetUsedNamespaces(namespaces);
			namespaces.Remove(string.Empty);
			namespaces.Remove(Namespace);
			foreach (string @namespace in namespaces)
			{
				writer.WriteLine("using {0};", @namespace);
			}
			if(namespaces.Count > 0)
			{
				writer.WriteLine();
			}
		}

		private string ExportNamespace()
		{
			if(Namespace == ScriptType.UnityEngineName)
			{
				switch (ClearName)
				{
					case "NavMeshAgent":
					case "OffMeshLink":
						return $"{ScriptType.UnityEngineName}.AI";
				}
			}
			return Namespace;
		}

		public abstract string FullName { get; }
		public abstract string Name { get; }
		public abstract string TypeName { get; }
		public abstract string ClearName { get; }
		public abstract string Namespace { get; }
		public abstract string Module { get; }
		public virtual bool IsEnum => false;

		public abstract ScriptExportType DeclaringType { get; }
		public abstract ScriptExportType Base { get; }

		public IReadOnlyList<ScriptExportType> NestedTypes => m_nestedTypes;
		public IReadOnlyList<ScriptExportEnum> NestedEnums => m_nestedEnums;
		public IReadOnlyList<ScriptExportDelegate> Delegates => m_nestedDelegates;
		public abstract IReadOnlyList<ScriptExportField> Fields { get; }

		protected abstract string Keyword { get; }
		protected abstract bool IsStruct { get; }
		protected abstract bool IsSerializable { get; }

		protected const string PublicKeyWord = "public";
		protected const string InternalKeyWord = "internal";
		protected const string ProtectedKeyWord = "protected";
		protected const string PrivateKeyWord = "private";
		
		protected readonly List<ScriptExportType> m_nestedTypes = new List<ScriptExportType>();
		protected readonly List<ScriptExportEnum> m_nestedEnums = new List<ScriptExportEnum>();
		protected readonly List<ScriptExportDelegate> m_nestedDelegates = new List<ScriptExportDelegate>();
	}
}
