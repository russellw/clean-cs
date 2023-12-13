static class Program {
	static bool inplace;

	static void Main(string[] args) {
		var options = true;
		var recursive = false;
		var paths = new List<string>();
		foreach (var arg in args) {
			var s = arg;
			if (options) {
				if (s == "--") {
					options = false;
					continue;
				}
				if (s.StartsWith('-')) {
					while (s.StartsWith('-'))
						s = s[1..];
					switch (s) {
					case "i":
						inplace = true;
						break;
					case "r":
					case "recursive":
						recursive = true;
						break;
					case "?":
					case "h":
					case "help":
						Help();
						return;
					case "V":
					case "v":
					case "version":
						Version();
						return;
					default:
						Console.WriteLine("{0}: unknown option", arg);
						Environment.Exit(1);
						break;
					}
					continue;
				}
			}
			paths.Add(s);
		}
		if (paths.Count == 0) {
			Help();
			return;
		}

		foreach (var path in paths)
			if (Directory.Exists(path)) {
				if (!recursive) {
					Console.WriteLine(path + " is a directory, use -r to recur on all .cs files therein");
					Environment.Exit(1);
				}
				Descend(path);
			} else
				Do(path);
	}

	static void Help() {
		var name = typeof(Program).Assembly.GetName().Name;
		Console.WriteLine($"Usage: {name} [options] path...");
		Console.WriteLine("");
		Console.WriteLine("-h  Show help");
		Console.WriteLine("-V  Show version");
		Console.WriteLine("-i  In-place edit");
		Console.WriteLine("-r  Recur into directories");
	}

	static void Version() {
		var name = typeof(Program).Assembly.GetName().Name;
		var version = typeof(Program).Assembly.GetName()?.Version?.ToString(2);
		Console.WriteLine($"{name} {version}");
	}

	static void Descend(string path) {
		foreach (var entry in new DirectoryInfo(path).EnumerateFileSystemInfos())
			if (entry is DirectoryInfo)
				Descend(entry.FullName);
			else if (entry.Extension == ".cs")
				Do(entry.FullName);
	}

	static void Do(string file) {
		var v = new List<string>(File.ReadLines(file));
		var old = new List<string>(v);

		// Capitalize comments
		for (int i = 1; i < v.Count; i++) {
			var s = v[i].TrimStart();
			if (s.StartsWith("// ") && !v[i - 1].TrimStart().StartsWith("//")) {
				s = s[3..];
				if (s == "")
					continue;
				if (char.IsUpper(s, 0))
					continue;
				if (s.StartsWith("http"))
					continue;
				v[i] = v[i][..(v[i].Length - s.Length)] + char.ToUpperInvariant(s[0]) + s[1..];
			}
		}

		if (inplace) {
			if (v.SequenceEqual(old))
				return;
			WriteLines(file, v);
			Console.WriteLine(file);
			return;
		}
		foreach (var s in v)
			Console.WriteLine(s);
	}

	static void WriteLines(string file, IEnumerable<string> v) {
		using var writer = new StreamWriter(file);
		writer.NewLine = "\n";
		foreach (var s in v)
			writer.WriteLine(s);
	}
}
