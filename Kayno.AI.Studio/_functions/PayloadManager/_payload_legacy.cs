using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kayno.AI.Studio.Functions.PayloadManager
{
    class Class1
    {


		// ↓　Legacy　↓



		void _initPayloadSample()
		{
			var payload = new Payload();
			payload.PropertyName = "model_name";
			payload.PropertyValue = "miaomiao_harem_v14.safetensor";
			payload.UI = UISelector.List;
			payload.UI_IsPinned = false;
			payload.UI_ItemsSource = ModelCollection;

			CurrentPayloadCollection.Add(payload);

			var payload1 = new Payload();
			payload1.PropertyName = "prompt";
			payload1.PropertyValue = "1boy, androgynous, masterpiece";
			payload1.UI = UISelector.TextBox;
			payload1.UI_IsPinned = true;
			payload1.UI_ItemsSource = PromptTagCollection;

			CurrentPayloadCollection.Add(payload1);

			// ...

		}

		public async Task<T> Json_JsonToClass<T>(string path)
		{
			try
			{
				var stream = await File.ReadAllTextAsync(path, Encoding.UTF8);
				var _data = JsonSerializer.Deserialize<T>(stream);
				//Debug.WriteLine(_data.ToString());

				return _data;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);

				return default(T);
			}

		}

		public async Task Json_ClassToJson<T>(T data, string path)
		{
			try
			{
				var json = JsonSerializer.Serialize<T>(data);
				await File.WriteAllTextAsync(path, json);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"{ex.Message}");
			}
		}

		// list of properties of Person class marked with DataMemeber attribute
		private PropertyInfo[] _personProperties =
			typeof(Payload)
				.GetProperties()
				.Where(x => x.GetCustomAttribute(typeof(DataMemberAttribute)) != null).ToArray();

		public string SerializeTSV<T>(string separator, IEnumerable<T> objectlist)
		{
			StringBuilder tsvdata = new StringBuilder();

			string header = string.Join(separator, _personProperties.Select(x => x.Name).ToArray());
			tsvdata.AppendLine(header);
			foreach (var o in objectlist)
			{
				List<string> lineValues = new List<string>();
				// enumerate through the properties
				foreach (var pi in _personProperties)
				{
					lineValues.Add(pi.GetValue(o).ToString());
				}
				tsvdata.AppendLine(string.Join(separator, lineValues.ToArray()));
			}

			return tsvdata.ToString();
		}

		void _loadPayloadCSV(string path)
		{

			using var reader = new StreamReader(path);

			var config = new CsvConfiguration(CultureInfo.CurrentCulture)
			{
				Mode = CsvMode.NoEscape,
				Delimiter = "\t"
			};
			using var csv = new CsvReader(reader, config);
			CurrentPayloadCollection = new ObservableCollection<Payload>(csv.GetRecords<Payload>());


			//using var fs = new FileStream(path, FileMode.Open);
			//var d = CSVLoader.Load<Payload>(fs).ToArray();
			//foreach(var item in d)
			//{
			//	CurrentPayloadCollection.Add(item);
			//}

			/*
			//using var txtParser = new TextFieldParser(path);
			//txtParser.SetDelimiters(",");
			//// 
			//
			//while (!txtParser.EndOfData)
			//{
			//	string[] line = txtParser.ReadFields();
			//	int i = 1;
			//	foreach (string column in line)
			//		Console.WriteLine(string.Format("{0}: {1}", i++, column));
			//}
			 */

			Debug.WriteLine("Payload Loaded.");
			Debug.WriteLine(path);
			foreach (var item in CurrentPayloadCollection)
			{
				Debug.WriteLine(item.PropertyValue);
			}


			//BuildUI();
		}

		private void BuildUI()
		{
			for (int i = 0; i < CurrentPayloadCollection.Count; i++)
			{
				var item = CurrentPayloadCollection[i];

				if (item.UI == UISelector.TextBox)
				{
					var grid = new Grid();
					var cl1 = new ColumnDefinition { Width = new GridLength(4, GridUnitType.Star) };
					var cl2 = new ColumnDefinition { Width = new GridLength(4, GridUnitType.Star) };
					var columns = new
					{
						cl1,
						cl2,
					};

					var textbox = new TextBox();
					textbox.Style = FindResource("textBox1") as Style;

					grid.Children.Add(textbox);

				}
			}

		}

		void _savePayloadCSV(string path)
		{
			using var writer = new StreamWriter(path);
			var config = new CsvConfiguration(CultureInfo.CurrentCulture)
			{
				//Mode = CsvMode.NoEscape,
				Delimiter = "\t"
			};
			using var csv = new CsvWriter(writer, config);

			csv.WriteRecords(CurrentPayloadCollection);

			Debug.WriteLine("Payload Saved.");
			Debug.WriteLine(path);
		}


	}
}
