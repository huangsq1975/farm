using System;
using UnityEngine;

public class NamedArrayAttribute : PropertyAttribute
{
	public readonly string[] names;

	public NamedArrayAttribute(string[] names)
	{
		this.names = names;
	}

	public NamedArrayAttribute(Type type, int type_max, int category_max)
	{
		names = new string[category_max * type_max];
		Array array = Enum.GetNames(type);
		string empty = string.Empty;
		for (int i = 0; i < category_max; i++)
		{
			empty = i + 1 + ". ";
			for (int j = 0; j < type_max; j++)
			{
				names[i * type_max + j] = empty + " " + array.GetValue(j);
			}
		}
	}
}
