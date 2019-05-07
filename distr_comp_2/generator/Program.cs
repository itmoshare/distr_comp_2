using System;
using System.IO;
using core.FileReader;

namespace generator
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length != 2)
				throw new ArgumentException();

			var name = args[0];
			var size = int.Parse(args[1]);
			using (var file = new StreamWriter(name))
			{
				var rnd = new Random();
				var time = 0;
				for (var i = 0; i < size; i++)
				{
					var values = Enum.GetValues(typeof(CommandType));
					var command = (CommandType)values.GetValue(rnd.Next(values.Length));
					switch (command)
					{
						case CommandType.Insert:
							var key = rnd.Next(0, size).ToString();
							var value = rnd.Next();
							file.WriteLine($"{time}, INSERT {key}, {value}");
							break;
						case CommandType.Select:
							if (rnd.Next(0, 50) == 0)
							{
								file.WriteLine($"{time}, SELECT");
							}
							else
							{
								var selectKey = rnd.Next(0, size).ToString();
								file.WriteLine($"{time}, SELECT {selectKey}");
							}
							break;
					}

					time += rnd.Next(1, 50);
				}
			}
		}
	}
}