<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <RuntimeVersion>7.0</RuntimeVersion>
</Query>

#load "input.linq"

const string testInput = @"    [D]    
[N] [C]    
[Z] [M] [P]
 1   2   3 

move 1 from 2 to 1
move 3 from 1 to 3
move 2 from 2 to 1
move 1 from 1 to 2";

async Task Main()
{
	var input = await GetInput(5);
	//SolvePartOne(testInput, "\r\n\r\n").Dump();
	SolvePartOne(input, "\n\n").Dump();

	//SolvePartTwo(testInput, "\r\n\r\n").Dump();
	SolvePartTwo(input, "\n\n").Dump();
}

string SolvePartOne(string input, string delimeter)
{
	var (instructions, stacks) = ProcessInput(input, delimeter);
			
	ProcessInstructions9000(stacks, instructions);
			
	return new string(stacks.Select(s => (char)s.Peek()).ToArray());
}

string SolvePartTwo(string input, string delimeter)
{
	var (instructions, stacks) = ProcessInput(input, delimeter);

	ProcessInstructions9001(stacks, instructions);

	return new string(stacks.Select(s => (char)s.Peek()).ToArray());
}

(IEnumerable<Instruction>, Stack[]) ProcessInput(string input, string delimeter)
{
	var stacks =
		input
			.Split(delimeter, StringSplitOptions.RemoveEmptyEntries)[0]
			.Split("\n", StringSplitOptions.RemoveEmptyEntries)
			.SelectMany(s => s.Select((c, i) => (container: c, stack: (i / 4) + 1)).Where(cs => Char.IsLetter(cs.container)))
			.OrderBy(cs => cs.stack)
			.GroupBy(cs => cs.stack)
			.Select(grp => new Stack(grp.Reverse().Select(cs => cs.container).ToList()))
			.ToArray();

	var instructions =
		input
			.Split(delimeter, StringSplitOptions.RemoveEmptyEntries)[1]
			.Split("\n", StringSplitOptions.RemoveEmptyEntries)
			.Select(s =>
			{
				var match = Regex.Match(s, @"move\s(?<cnt>\d+)\sfrom\s(?<from>\d+)\sto\s(?<to>\d+)");

				return new Instruction(
					Convert.ToInt32(match.Groups["cnt"].Value),
					Convert.ToInt32(match.Groups["from"].Value),
					Convert.ToInt32(match.Groups["to"].Value));
			});
			
	return (instructions, stacks);
}

void ProcessInstructions9000(Stack[] stacks, IEnumerable<Instruction> instructions)
{
	foreach (var instruction in instructions)
	{
		for (int i = 0; i < instruction.Count; i++)
		{
			var container = stacks[instruction.From - 1].Pop();
			stacks[instruction.To - 1].Push(container);
		}
	}
}

void ProcessInstructions9001(Stack[] stacks, IEnumerable<Instruction> instructions)
{
	foreach (var instruction in instructions)
	{
		var grabbedContainers = new Stack();
		for (int i = 0; i < instruction.Count; i++)
		{
			grabbedContainers.Push(stacks[instruction.From - 1].Pop());
		}

		while (grabbedContainers.Count > 0)
		{
			stacks[instruction.To - 1].Push(grabbedContainers.Pop());
		}
	}
}

record Instruction(int Count, int From, int To);
