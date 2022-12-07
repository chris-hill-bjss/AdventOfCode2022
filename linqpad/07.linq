<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <RuntimeVersion>7.0</RuntimeVersion>
</Query>

#load "input.linq"

const string testInput = @"$ cd /
$ ls
dir a
14848514 b.txt
8504156 c.dat
dir d
$ cd a
$ ls
dir e
29116 f
2557 g
62596 h.lst
$ cd e
$ ls
584 i
$ cd ..
$ cd ..
$ cd d
$ ls
4060174 j
8033020 d.log
5626152 d.ext
7214296 k";

async Task Main()
{
	var input = await GetInput(7);

	SolvePartOne(input).Dump();
	SolvePartTwo(input).Dump();
}

int SolvePartOne(string input) =>
	 MapFileSystem(input)
		.Where(d => d.TotalSize <= 100000)
		.Sum(d => d.TotalSize);


int SolvePartTwo(string input)
{
	var fileSystem = MapFileSystem(input);

	var freeSpace = 70000000 - fileSystem[0].TotalSize;
	var requiredSpace = 30000000 - freeSpace;
	
	return 
		fileSystem
			.Where(d => d.TotalSize >= requiredSpace)
			.OrderBy(d => d.TotalSize)
			.Select(d => (d.Name, d.TotalSize))
			.First()
			.TotalSize;
}

List<Directory> MapFileSystem(string input)
{
	var currentDirectory = new Directory("root");
	var allDirectories = new List<Directory>();
	allDirectories.Add(currentDirectory);

	foreach (var line in input.Split("\n", StringSplitOptions.RemoveEmptyEntries))
	{
		currentDirectory = line.Trim().Split(' ') switch
		{
			["$", "cd", var target] => ChangeDirectory(target, currentDirectory, allDirectories),
			["$", "ls"] => currentDirectory,
			["dir", _] => currentDirectory,
			_ => AddDirectoryContent(line, currentDirectory)
		};
	}

	return allDirectories;
}

Directory AddDirectoryContent(string line, Directory currentDirectory)
{
	currentDirectory.AddContent(line);
	return currentDirectory;
}

Directory ChangeDirectory(string target, Directory currentDirectory, List<Directory> allDirectories)
{
	if (target == "..")
		return currentDirectory.Parent;
	
	var dir = new Directory(target, currentDirectory);
	currentDirectory.AddChild(dir);
	allDirectories.Add(dir);
	
	return dir;
}

record Directory(string Name, Directory Parent = null)
{
	public List<Directory> Children = new();
	public Dictionary<string, int> Contents = new();
	
	public int TotalSize => Contents.Sum(kvp => kvp.Value) + Children.Sum(c => c.TotalSize);
	
	internal void AddChild(Directory child) => Children.Add(child);
	
	internal void AddContent(string line)
	{
		var parts = line.Split(' ');

		var size = parts[0];
		var name = parts[1];
		
		Contents.Add(name, Convert.ToInt32(size));
	}
}