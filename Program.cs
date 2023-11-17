var options = true;
var inplace = false;
var files = new List<string>();
foreach (var arg in args) {
	var s = arg;
	if (options) {
		if (s == "--") {
			options = false;
			continue;
		}
		if (s.StartsWith("-")) {
			if (s.StartsWith("--"))
				s = s[1..];
			switch (s) {
			case "-i":
				inplace = true;
				break;
			case "-?":
			case "-h":
			case "-help":
				Help();
				return 0;
			case "-V":
			case "-v":
			case "-version":
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

foreach (var file in files) {
	var v = new List<string>(File.ReadLines(file));
	var old = new List<string>(v);

	// capitalize comments
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
		if (v == old)
			continue;
		WriteLines(file, v);
		Console.WriteLine(file);
		continue;
	}
	foreach (var s in v)
		Console.WriteLine(s);
}
return 0;

static void Help() {
	Console.WriteLine("Usage: clean-cs [options] file...");
	Console.WriteLine("");
	Console.WriteLine("-h  Show help");
	Console.WriteLine("-V  Show version");
	Console.WriteLine("-i  In-place edit");
}

static void WriteLines(string file, IEnumerable<string> v) {
	using var writer = new StreamWriter(file);
	writer.NewLine = "\n";
	foreach (var s in v)
		writer.WriteLine(s);
}