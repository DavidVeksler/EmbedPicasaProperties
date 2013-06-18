using System;
using System.IO;

namespace EmbedPicasaProperties
{
	class MainClass
	{
		private const string IniFileName = @".picasa.ini";

		private static void Main (string[] args)
		{
			string startFolder = @"/Users/davidv/Pictures/";

			Console.WriteLine (startFolder);
						
			UpdateFoldersInPath (startFolder);

		}
		
		private static void UpdateFoldersInPath (string folder)
		{
			string[] directories = Directory.GetDirectories (folder);
			
			if (directories.Length > 0)
				Console.WriteLine ("{0} folders in {1}", directories.Length, folder);
			
				foreach (string directory in directories) {
				try {
					string iniPath = Path.Combine (directory, IniFileName);
					
					if (File.Exists (iniPath)) {
						Console.WriteLine ("Parse " + iniPath);


						foreach (string line in File.ReadAllLines(iniPath)) {
							// date=29586.603900
							if (!line.StartsWith ("date="))
								continue;
							
							string dateString = line.Substring (5);
							
							DateTime date = ConverPicasaDateToDateTime (dateString);
							
							DateTime originalTime = Directory.GetCreationTime (directory);
							
							if (date.Year < 2010 && originalTime.Subtract(date).TotalDays > 365) {
								Console.WriteLine ("{0} to {1}", originalTime, date);
								Directory.SetCreationTime (directory, date);

								Directory.SetLastWriteTime (directory, date);
							}

							var files = System.IO.Directory.GetFiles (directory,"*.*", SearchOption.TopDirectoryOnly);
						
							foreach (string file in files) {

								Console.WriteLine (file);

								DateTime originalFileTime = Directory.GetCreationTime (file);

								if (originalFileTime.Subtract(date).Days > 365) {

									Console.WriteLine (String.Format("convert {0} to {1}",originalFileTime,date));

									File.SetCreationTime (file, date);
									File.SetLastWriteTime (file,date);
								}
							}


							break;
						}
					}
				} catch (Exception ex) {
					Console.WriteLine (ex);
				}
				
				try {
					UpdateFoldersInPath (directory);
				} catch (Exception ex) {
					Console.WriteLine (ex);
				}
			}
		}
		
		// convert =29586.603900 to date time format
		private static DateTime ConverPicasaDateToDateTime (string dateString)
		{
			var startDate = new DateTime (1900, 1, 1);
			
			DateTime date = startDate.AddDays (Convert.ToDouble (dateString) - 2);
			
			Console.WriteLine ("new date: " + date);
			
			return date;
		}
	}
}
