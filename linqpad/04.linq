<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Net.Http</Namespace>
</Query>

#load "input.linq"

const string testInput = @"2-4,6-8
2-3,4-5
5-7,7-9
2-8,3-7
6-6,4-6
2-6,4-8";

async Task Main()
{
	var input = await GetInput(4);
	SolvePartOne(input).Dump();
	SolvePartTwo(input).Dump();
}

int SolvePartOne(string input) =>
	input
		.Split("\n", StringSplitOptions.RemoveEmptyEntries)
		.Select(s => {
			var elfOne = new ElfAssignment(s.Split(",")[0]);
			var elfTwo = new ElfAssignment(s.Split(",")[1]);
			
			return elfOne.Sections.All(s => elfTwo.Sections.Contains(s)) || elfTwo.Sections.All(s => elfOne.Sections.Contains(s));
		})
		.Count(a => a);

int SolvePartTwo(string input) =>
	input
		.Split("\n", StringSplitOptions.RemoveEmptyEntries)
		.Select(s =>
		{
			var elfOne = new ElfAssignment(s.Split(",")[0]);
			var elfTwo = new ElfAssignment(s.Split(",")[1]);

			return elfOne.Sections.Intersect(elfTwo.Sections).Any();
		})
		.Count(a => a);

class ElfAssignment
{
	private readonly int _from;
	private readonly int _to;
	
	public ElfAssignment(string assignment)
	{
		_from = Convert.ToInt32(assignment.Split("-")[0]);
		_to = Convert.ToInt32(assignment.Split("-")[1]);
	}

	public IEnumerable<int> Sections => Enumerable.Range(_from, _to - _from + 1);
}
