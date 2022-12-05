<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Net.Http</Namespace>
</Query>

#load "input.linq"

const string testInput = @"vJrwpWtwJgWrhcsFMMfFFhFp
jqHRNqRjqzjGDLGLrsFMfFZSrLrFZsSL
PmmdzqPrVvPwwTWBwg
wMqvLMZHhHMvwLHjbvcjnnSBnvTQFn
ttgJtRGJQctTZtZT
CrZsJsPPZsGzwwsLwLmpwMDw";

async Task Main()
{
	var input = await GetInput(3);
	//SolvePartOne(input).Dump();
	SolvePartTwo(input).Dump();
}

int SolvePartOne(string input) =>
	input
		.Split("\n")
		.SelectMany(s => new Backpack(s[0..(s.Length / 2)], s[(s.Length / 2)..]).CommonItems.Select(c => c.Rank))
		.Sum();

int SolvePartTwo(string input)
{
	var backpacks = input.Split("\n");
	
	var badges = new List<RankedItem>();
	
	var i = 0;
	while(((i*3)+3) <= backpacks.Length)
	{
		var group = backpacks[(i * 3)..((i * 3) + 3)];
		var uniqueItems = 
			group
				.Select(s => new string(s.Distinct().Where(Char.IsLetter).OrderBy(c => c).ToArray()))
				.ToArray();
				
		badges.AddRange(
		uniqueItems[0]
			.Intersect(uniqueItems[1])
			.Intersect(uniqueItems[2])
			.Select(b => new RankedItem(b)));
		
		i++;
	}
	
	return badges.Sum(b => b.Rank);
}

record Backpack(string CompartmentOne, string CompartmentTwo)
{
	public IEnumerable<RankedItem> CommonItems => 
		CompartmentOne
			.Intersect(CompartmentTwo)
			.Select(item => new RankedItem(item));
}

record RankedItem(char Item) {
	private const string MASK = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
	public int Rank => MASK.IndexOf(Item)+1;
}
