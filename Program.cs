var options = true;
var inplace = false;
var recursive = false;
var files = new List<string>();
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
				return 0;
			case "V":
			case "v":
			case "version":
				Console.WriteLine("clean-cs 1.0");
				return 0;
			default:
				Console.WriteLine("{0}: unknown option", arg);
				return 1;
			}
			continue;
		}
	}
	files.Add(s);
}
if (files.Count == 0) {
	Help();
	return 0;
}

foreach (var path in files)
	if (Directory.Exists(path)) {
		if (!recursive) {
			Console.WriteLine(path + " is a directory, use -r to recur on all .cs files therein");
			return 1;
		}
		Descend(path);
	} else
		Do(path);
return 0;

static void Help() {
	Console.WriteLine("Usage: clean-cs [options] path...");
	Console.WriteLine("");
	Console.WriteLine("-h  Show help");
	Console.WriteLine("-V  Show version");
	Console.WriteLine("-i  In-place edit");
	Console.WriteLine("-r  Recur into directories");
}

void Descend(string path) {
	foreach (var entry in new DirectoryInfo(path).EnumerateFileSystemInfos())
		if (entry is DirectoryInfo)
			Descend(entry.FullName);
		else if (entry.Extension == ".cs")
			Do(entry.FullName);
}

void Do(string file) {
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
