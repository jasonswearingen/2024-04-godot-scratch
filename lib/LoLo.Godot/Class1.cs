using Godot;

namespace LoLo.Godot;
public class Class1
{

}


internal static class VariantExtensions
{
	[Obsolete("Use As<T> instead.", true)]
	public static T? Unbox<T>(this Variant variant)
	{
		return variant.VariantType switch
		{
			Variant.Type.Nil => default,
			Variant.Type.Bool => (T)(object)variant.AsBool(),
			Variant.Type.Int => (T)(object)variant.AsInt32(),
			Variant.Type.Float => (T)(object)variant.AsSingle(),
			Variant.Type.String => (T)(object)variant.AsString(),
			Variant.Type.Vector2 => (T)(object)variant.AsVector2(),
			Variant.Type.Vector2I => (T)(object)variant.AsVector2I(),
			Variant.Type.Rect2 => (T)(object)variant.AsRect2(),
			Variant.Type.Rect2I => (T)(object)variant.AsRect2I(),
			Variant.Type.Vector3 => (T)(object)variant.AsVector3(),
			Variant.Type.Vector3I => (T)(object)variant.AsVector3I(),
			Variant.Type.Transform2D => (T)(object)variant.AsTransform2D(),
			Variant.Type.Vector4 => (T)(object)variant.AsVector4(),
			Variant.Type.Vector4I => (T)(object)variant.AsVector4I(),
			Variant.Type.Plane => (T)(object)variant.AsPlane(),
			Variant.Type.Quaternion => (T)(object)variant.AsQuaternion(),
			Variant.Type.Aabb => (T)(object)variant.AsAabb(),
			Variant.Type.Basis => (T)(object)variant.AsBasis(),
			Variant.Type.Transform3D => (T)(object)variant.AsTransform3D(),
			Variant.Type.Projection => (T)(object)variant.AsProjection(),
			Variant.Type.Color => (T)(object)variant.AsColor(),
			Variant.Type.StringName => (T)(object)variant.AsStringName(),
			Variant.Type.NodePath => (T)(object)variant.AsNodePath(),
			Variant.Type.Rid => (T)(object)variant.AsRid(),
			Variant.Type.Object => (T)(object)variant.AsGodotObject(),
			Variant.Type.Callable => (T)(object)variant.AsCallable(),
			Variant.Type.Signal => (T)(object)variant.AsSignal(),
			Variant.Type.Dictionary => (T)(object)variant.AsGodotDictionary(),
			Variant.Type.Array => (T)(object)variant.AsGodotArray(),
			Variant.Type.PackedByteArray => (T)(object)variant.AsByteArray(),
			Variant.Type.PackedInt32Array => (T)(object)variant.AsInt32Array(),
			Variant.Type.PackedInt64Array => (T)(object)variant.AsInt64Array(),
			Variant.Type.PackedFloat32Array => (T)(object)variant.AsFloat32Array(),
			Variant.Type.PackedFloat64Array => (T)(object)variant.AsFloat64Array(),
			Variant.Type.PackedStringArray => (T)(object)variant.AsStringArray(),
			Variant.Type.PackedVector2Array => (T)(object)variant.AsVector2Array(),
			Variant.Type.PackedVector3Array => (T)(object)variant.AsVector3Array(),
			Variant.Type.PackedColorArray => (T)(object)variant.AsColorArray(),
			Variant.Type.Max => (T)(object)variant.VariantType,
			_ => throw new ArgumentOutOfRangeException(nameof(T), typeof(T), "Unknown type.")


		};

		
	}
}