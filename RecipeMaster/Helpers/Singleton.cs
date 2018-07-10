using System;
using System.Collections.Concurrent;

namespace RecipeMaster.Helpers
{
	internal static class Singleton<T> where T : new()
	{
		#region Public Properties

		public static T Instance
		{
			get
			{
				return _instances.GetOrAdd(typeof(T), (t) => new T());
			}
		}

		#endregion Public Properties

		#region Private Fields

		private static ConcurrentDictionary<Type, T> _instances = new ConcurrentDictionary<Type, T>();

		#endregion Private Fields
	}
}