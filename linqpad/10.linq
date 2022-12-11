<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Drawing</Namespace>
  <RuntimeVersion>7.0</RuntimeVersion>
</Query>

#load "input.linq"

const string testInput = @"addx 15
addx -11
addx 6
addx -3
addx 5
addx -1
addx -8
addx 13
addx 4
noop
addx -1
addx 5
addx -1
addx 5
addx -1
addx 5
addx -1
addx 5
addx -1
addx -35
addx 1
addx 24
addx -19
addx 1
addx 16
addx -11
noop
noop
addx 21
addx -15
noop
noop
addx -3
addx 9
addx 1
addx -3
addx 8
addx 1
addx 5
noop
noop
noop
noop
noop
addx -36
noop
addx 1
addx 7
noop
noop
noop
addx 2
addx 6
noop
noop
noop
noop
noop
addx 1
noop
noop
addx 7
addx 1
noop
addx -13
addx 13
addx 7
noop
addx 1
addx -33
noop
noop
noop
addx 2
noop
noop
noop
addx 8
noop
addx -1
addx 2
addx 1
noop
addx 17
addx -9
addx 1
addx 1
addx -3
addx 11
noop
noop
addx 1
noop
addx 1
noop
noop
addx -13
addx -19
addx 1
addx 3
addx 26
addx -30
addx 12
addx -1
addx 3
addx 1
noop
noop
noop
addx -9
addx 18
addx 1
addx 2
noop
noop
addx 9
noop
noop
noop
addx -1
addx 2
addx -37
addx 1
addx 3
noop
addx 15
addx -21
addx 22
addx -6
addx 1
noop
addx 2
addx 1
noop
addx -10
noop
noop
addx 20
addx 1
addx 2
addx 2
addx -6
addx -11
noop
noop
noop";

async Task Main()
{
	var input = await GetInput(10);

	SolvePartOne(input).Dump();
	SolvePartTwo(input).Dump();
}

int SolvePartOne(string input)
{
	var cpu = new CPU();
	
	var log = cpu.RunProgram(input).ToArray();

	return
		log[19].SignalStrength +
		log[59].SignalStrength +
		log[99].SignalStrength +
		log[139].SignalStrength +
		log[179].SignalStrength +
		log[219].SignalStrength;
}

string SolvePartTwo(string input)
{
	var cpu = new CPU();

	var log = cpu.RunProgram(input).ToArray();
	
	log
		.Chunk(40)
		.Select(row => row.Select((log, i) =>
		{
			var spritePixels = new[] { log.X - 1, log.X, log.X + 1 };
			var pixel = spritePixels.Contains(i) ? '#' : '.';

			return pixel;
		}))
		.Select(o => new string(o.ToArray()))
		.Dump();
		
	return "";
}

class CPU
{
	private int _x = 1;
	
	private readonly List<LogEntry> _log = new();
	
	public List<LogEntry> RunProgram(string program)
	{
		var instructionQueue = new Queue<Instruction>(
			program
				.Split("\n", StringSplitOptions.RemoveEmptyEntries)
				.Select(s => new Instruction(s)));
		
		var cycle = 1;
		ExecutingInstruction currentInstruction = null;
		while(instructionQueue.Any() || currentInstruction != null)
		{
			if (currentInstruction == null)
			{
				var instruction = instructionQueue.Dequeue();
				currentInstruction = 
					new ExecutingInstruction(
						instruction,
						cycle + instruction.Lifetime);
			}	
			
			_log.Add(new LogEntry(cycle, _x));
						
			if (cycle == currentInstruction.CompletionCycle)
			{
				_x += currentInstruction.Instruction.Increment;
				
				currentInstruction = null;
			}
			cycle++;
		}
		
		return _log;
	}
}

record Instruction(string Command)
{	
	public int Lifetime => Command.Split(' ')[0] switch 
	{
		"noop" => 0,
		"addx" => 1, 
		_ => 0
	};
	
	public int Increment => Command.Split(' ')[0] switch
	{
		"noop" => 0,
		"addx" => Convert.ToInt32(Command.Split(' ')[1]),
		_ => 0
	};
};

record ExecutingInstruction(Instruction Instruction, int CompletionCycle);

record LogEntry(int Cycle, int X)
{
	public int SignalStrength => Cycle * X;
};