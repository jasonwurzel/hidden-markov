using System.IO;

namespace HiddenMarkovTests
{
	/// <summary>
	///     Lange vermißt: Erweiterungsfunktionen für Strings betreffend Pfade
	/// </summary>
	public static class PathExtension
	{
		/// <summary>
		///     Ein Verzeichnis, Dateiname oder längeren Pfad zu einem bestehenden
		///     Pfadstring hinzufügen
		/// </summary>
		public static string AddPath(this string path, string anotherPath) { return Path.Combine(path, anotherPath); }
	}
}