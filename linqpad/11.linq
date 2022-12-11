<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Drawing</Namespace>
  <RuntimeVersion>7.0</RuntimeVersion>
</Query>

#load "input.linq"

string testInput = @"Monkey 0:
  Starting items: 79, 98
  Operation: new = old * 19
  Test: divisible by 23
    If true: throw to monkey 2
    If false: throw to monkey 3

Monkey 1:
  Starting items: 54, 65, 75, 74
  Operation: new = old + 6
  Test: divisible by 19
    If true: throw to monkey 2
    If false: throw to monkey 0

Monkey 2:
  Starting items: 79, 60, 97
  Operation: new = old * old
  Test: divisible by 13
    If true: throw to monkey 1
    If false: throw to monkey 3

Monkey 3:
  Starting items: 74
  Operation: new = old + 3
  Test: divisible by 17
    If true: throw to monkey 0
    If false: throw to monkey 1"
	.Replace("\r\n", "\n");

async Task Main()
{
	var input = await GetInput(11);

	SolvePartOne(input).Dump();
	SolvePartTwo(input).Dump();
}

long SolvePartOne(string input)
{
	var monkeys = InitialiseMonkeys(input);
	
	SimulateMonkeys(monkeys, 20, i => i / 3);
	
	var monkeyBusiness =
		monkeys
			.Select(m => m.InspectedItems)
			.Dump()
			.OrderByDescending(m => m)
			.Take(2)
			.Aggregate(1L, (a, b)=> a * b);
			
	return monkeyBusiness;
}

long SolvePartTwo(string input)
{
	var monkeys = InitialiseMonkeys(input);
	var mod = monkeys.Aggregate(1, (mod, monkey) => mod * monkey.Test);
	
	SimulateMonkeys(monkeys, 10000, i => i % mod);

	var monkeyBusiness =
		monkeys
			.Select(m => m.InspectedItems)
			.Dump()
			.OrderByDescending(m => m)
			.Take(2)
			.Aggregate(1L, (a, b) => a * b);

	return monkeyBusiness;
}

void SimulateMonkeys(Monkey[] monkeys, int numberOfRounds, Func<long, long> modWorry) =>
	Enumerable
		.Range(0, numberOfRounds)
		.Select(round => (round, monkeys.Select(m => m.PlayRound(monkeys, round, modWorry)).ToArray()))
		.ToArray();

Monkey[] InitialiseMonkeys(string input) =>
	input
		.Split("\n\n", StringSplitOptions.RemoveEmptyEntries)
		.Select(monkeyDef =>
		{
			var match = Regex.Match(monkeyDef, @".*(?<id>\d+):\n.*:\s(?<items>.*)\n.*old\s(?<worry>.+)\n.*\s(?<test>\d+)\n.*true.*(?<true>\d+)\n.*false.*(?<false>\d+)");
			
			var monkeyId = Convert.ToInt32(match.Groups["id"].Value);
			var startingItems = match.Groups["items"].Value.Split(",").Select(long.Parse);
			var worryOperation = ParseWorry(match.Groups["worry"].Value);
			var test = Convert.ToInt32(match.Groups["test"].Value);
			var onTrue = Convert.ToInt32(match.Groups["true"].Value);
			var onFalse = Convert.ToInt32(match.Groups["false"].Value);

			var monkey = new Monkey(
				monkeyId,
				worryOperation,
				test,
				onTrue,
				onFalse);
				
			monkey.Items.AddRange(startingItems);

			return monkey;
		})
		.ToArray();
		
Func<long, long> ParseWorry(string worry) =>
	worry.Split(" ") switch
	{
		["+", var increment] => new Func<long, long>(i => i + ParseIncrement(increment, i)),
		["*", var increment] => new Func<long, long>(i => i * ParseIncrement(increment, i)),
		_ => throw new NotSupportedException($"worry definition not supported: {worry}")
	};
	
long ParseIncrement(string increment, long old) => 
	int.TryParse(increment, out var parsed)
	? parsed
	: old;

record Monkey(int Id, Func<long, long> Worry, int Test, int OnTrue, int OnFalse)
{
	public int InspectedItems { get; private set; }
	
	public List<long> Items { get; } = new();
	
	public Monkey PlayRound(Monkey[] monkeys, int round, Func<long, long> manageWorry)
	{
		foreach(var item in Items)
		{
			var newWorry = manageWorry(Worry(item));
			
			var targetMonkey = newWorry % Test == 0
				? OnTrue
				: OnFalse;
			
			monkeys[targetMonkey].Items.Add(newWorry);
			
			InspectedItems++;
		}
		Items.Clear();
		
		return this;
	}
}


