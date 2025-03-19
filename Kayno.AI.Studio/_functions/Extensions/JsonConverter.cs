using System.Net;
using System.Net.Http;
using System.Text.Json;


public static class JsonConverter
{


	public static async Task<T> RestoreLastSession<T>( string path )
	{
		try
		{
			var stream = await File.ReadAllTextAsync( path );
			var data = JsonSerializer.Deserialize<T>( stream );

			return data;
		}
		catch ( Exception ex )
		{
			Console.WriteLine( ex.Message );

			return default( T );
		}

	}

	public static async Task SaveAsLastSession<T>( T data, string path )
	{
		try
		{
			var json = JsonSerializer.Serialize<T>( data );
			await File.WriteAllTextAsync( path, json );
		}
		catch ( Exception ex )
		{
			Console.WriteLine( $"{ex.Message}" );
		}
	}

	/// <summary>
	/// Get json data from Http Response
	/// </summary>
	/// <param name="response"></param>
	/// <param name="Key">Property name you want to get.</param>
	/// <remarks>
	/// Please see the comments inside.
	/// </remarks>
	/// <returns></returns>
	public static async Task<List<string>> GetJsonListStringFromResponse( this HttpResponseMessage response, string Key )
	{
		/// json data:
		/// 
		/// [								← jsonRoot
		///		{							← one of models
		///			"key1":"value",
		///			"key2":"value",			← x, value is x.GetProperty(key) ★
		///			...
		///		},
		///		{
		///			"key1":"value",
		///			"key2":"value",			← x, value is x.GetProperty(key) ★
		///			...
		///		},
		///		...
		/// 
		/// ]
		/// 
		/// →　you can get List(string) of ★.

		if ( response.StatusCode == HttpStatusCode.OK )
		{
			var json = await response.Content.ReadAsStringAsync();
			using ( var jsonDocument = JsonDocument.Parse( json ) )
			{
				var jsonRoot = jsonDocument.RootElement;
				var models = jsonRoot.EnumerateArray();
				var model_names = models.Select( x => x.GetProperty( Key ).ToString() ).ToList();

				return model_names;
			}
		}
		else
		{
			return null;
		}

	}


	/// <summary>
	/// Get json data from Http Response
	/// </summary>
	/// <param name="response"></param>
	/// <param name="Key">Property name you want to get.</param>
	/// <remarks>Please see the comment inside.</remarks>
	/// <returns></returns>
	public static async Task<List<string>> GetJsonArrayDataFromResponse( this HttpResponseMessage response, string Key )
	{
		/// json data:
		/// 
		/// [								← RootElement
		///		"Key1": "value",			
		///		"Key2": "value",
		///		"Key3": "value",
		///		"Key4":						← jsonElement
		///		"[							← array
		///			"value1",				← x1 ★
		///			"value2",				← x2 ★...
		///			...
		///		]",
		///		...
		/// 
		/// ]
		/// 
		/// →　you can get List(string) of x(★).
		if ( response.StatusCode == HttpStatusCode.OK )
		{
			var json = await response.Content.ReadAsStringAsync();
			using ( var jsonDocument = JsonDocument.Parse( json ) )
			{
				var jsonRoot = jsonDocument.RootElement;
				var jsonElement = jsonRoot.GetProperty( Key );
				var array = jsonElement.EnumerateArray();
				var res = array.Select( x => x.ToString() ).ToList();

				return res;
			}
		}
		else
		{
			return new List<string>();
		}

	}

}
