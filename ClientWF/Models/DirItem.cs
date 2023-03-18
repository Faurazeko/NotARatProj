namespace ClientWF.Models
{
	class DirItem
	{
		public string Filename { get; set; }
		public string FullPath { get; set; }
		public DirItemType Type { get; set; }
		public string Size { get; set; }
		public string LocalDateModified { get; set; }
		public string LocalDateAccessed { get; set; }
		public string LocalDateCreated { get; set; }

	}

	public enum DirItemType
	{
		File = 0,
		Directory = 1
	}
}
