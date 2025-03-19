namespace Kayno.AI.Studio
{

	/// <summary>
	/// SDWebUIのペイロード設定に使う汎用のプロパティ一式。
	/// </summary>
	public class PayloadTemplate
	{
		#region ## Properties

		public string TPropertyName { get; set; }
		public object TPropertyValue { get; set; }
		public string? TThumbPath { get; set; }
		public string? TLabel { get; set; }
		public string? TCategory { get; set; }
		public string? TCategory2 { get; set; }
		public string? TPath { get; set; }
		public string? TParentDir { get; set; }
		public List<string>? TTags { get; set; }
		public string? TDescription { get; set; }
		public string? TParameter { get; set; }
		public string? TComment { get; set; }

		/*
		e.g.
		
		{
			PropertyName = "model_name",
			PropertyValue = "~~_mix_v2.safetensor",
			Category = "Anime2D",
			Path = "~~/sd/anime2d/....safetensor",
			ParentDir = "anime2d",
			Tags = { "anime", "sd1.5", "厚塗り風" },
			...
		}

		or

		PropertyName	PropertyValue		Category	Tags								...
		--------------------------------------------------
		001						gray						color			grey, dark, グレー	...
		002						yellow					color			黄色, ...
		003						black and white	color			白黒
		004						xx,yy,zz				others		???
		...

		 */

		#endregion


		public PayloadTemplate() { }

	}


}