<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <RuntimeVersion>7.0</RuntimeVersion>
</Query>

#load "input.linq"

const string testInput = @"30373
25512
65332
33549
35390";

async Task Main()
{
	var input = await GetInput(8);

	SolvePartOne(input).Dump();
	SolvePartTwo(input).Dump();
}

int SolvePartOne(string input) 
{
	var treeMap = InputToTreeMap(input);
	
	for (int y = 0; y < treeMap.Length; y++)
	{
		for (int x = 0; x < treeMap[y].Length; x++)
		{
			var tree = treeMap[y][x];
			if (x == 0 || y == 0 || x == treeMap[0].Length - 1 || y == treeMap.Length - 1)
			{
				tree.Visible = true;
				continue;
			}

			var northernTrees = treeMap[0..y].Select(row => row[x]);
			var southernTrees = treeMap[(y+1)..(treeMap.Length)].Select(row => row[x]);
			var westernTrees = treeMap[y][0..x];
			var easternTrees = treeMap[y][(x+1)..(treeMap[0].Length)];
			
			tree.Visible =
				northernTrees.All(t => t.Height < tree.Height) ||
				southernTrees.All(t => t.Height < tree.Height) || 
				westernTrees.All(t => t.Height < tree.Height) ||
				easternTrees.All(t => t.Height < tree.Height);
		}
	}
		
	return treeMap.Sum(r => r.Count(t => t.Visible));
}


int SolvePartTwo(string input)
{
	var treeMap = InputToTreeMap(input);

	for (int y = 0; y < treeMap.Length; y++)
	{
		for (int x = 0; x < treeMap[y].Length; x++)
		{
			var tree = treeMap[y][x];

			if (x == 0 || y == 0 || x == treeMap[0].Length - 1 || y == treeMap.Length - 1)
			{
				continue;
			}

			var northernTrees = countVisibleTrees(treeMap[0..y].Select(row => row[x]).Reverse().ToArray(), tree);
			var southernTrees = countVisibleTrees(treeMap[(y + 1)..(treeMap.Length)].Select(row => row[x]).ToArray(), tree);
			var westernTrees = countVisibleTrees(treeMap[y][0..x].Reverse().ToArray(), tree);
			var easternTrees = countVisibleTrees(treeMap[y][(x + 1)..(treeMap[0].Length)].ToArray(), tree);

			tree.Score = northernTrees * westernTrees * southernTrees * easternTrees;
		}
	}


	return treeMap.SelectMany(t => t.Select(t => t.Score)).Max(t => t);
}

int countVisibleTrees(Tree[] trees, Tree current)
{
	int visible = 0;
	int tallest = 0;
	
	for (int i = 0; i < trees.Length; i++)
	{
		visible++;
		
		if (trees[i].Height >= current.Height)
			break;
	}
	
	return visible;
}

Tree[][] InputToTreeMap(string input) =>
	input
		.Split("\n", StringSplitOptions.RemoveEmptyEntries)
		.Select(s => s.Trim().Select(c => new Tree((int)Char.GetNumericValue(c))).ToArray())
		.ToArray();
		
record Tree(int Height) {
	public bool Visible { get; set; }
	public int Score { get; set; }
}