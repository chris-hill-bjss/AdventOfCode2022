<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Net.Http</Namespace>
</Query>

#load "input.linq"

const string testInput = @"1000
2000
3000

4000

5000
6000

7000
8000
9000

10000";

async Task Main()
{
	var input = await GetInput(1);
	SolvePartTwo(input).Dump();
}

int SolvePartOne(string input) =>
	input
		.Split("\n\n")
		.Select(s => new Elf(
			s
				.Split("\n", StringSplitOptions.RemoveEmptyEntries)
				.Select(s => Convert.ToInt32(s))))
		.MaxBy(e => e.TotalCalories)
		.TotalCalories;

int SolvePartTwo(string input) =>
	input
		.Split("\n\n")
		.Select(s => new Elf(
			s
				.Split("\n", StringSplitOptions.RemoveEmptyEntries)
				.Select(s => Convert.ToInt32(s))))
		.OrderByDescending(e => e.TotalCalories)
		.Take(3)
		.Sum(e => e.TotalCalories);

record Elf(IEnumerable<int> FoodItems) {
	public int TotalCalories => FoodItems.Sum();
}
