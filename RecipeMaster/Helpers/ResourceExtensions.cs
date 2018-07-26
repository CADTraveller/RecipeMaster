using Windows.ApplicationModel.Resources;

namespace RecipeMaster.Helpers
{
	internal static class ResourceExtensions
	{
		#region Public Methods

		public static string GetLocalized(this string resourceKey)
		{
			return _resLoader.GetString(resourceKey);
		}

		#endregion Public Methods

		#region Private Fields

		private static ResourceLoader _resLoader = new ResourceLoader();

		#endregion Private Fields
	}
}