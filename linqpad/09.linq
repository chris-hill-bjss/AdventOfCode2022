<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Drawing</Namespace>
  <RuntimeVersion>7.0</RuntimeVersion>
</Query>

#load "input.linq"

const string testInput = @"R 5
U 8
L 8
D 3
R 17
D 10
L 25
U 20";

async Task Main()
{
	var input = await GetInput(9);

	SolvePartOne(input).Dump();
	SolvePartTwo(input).Dump();
}

int SolvePartOne(string input)
{
	var head = new Knot();
	var tail = new Knot();

	return 
		input
			.Split("\n", StringSplitOptions.RemoveEmptyEntries)
			.Select(s =>
			{
				var parts = s.Trim().Split(' ');
				return new Instruction(parts[0], Convert.ToInt32(parts[1]));
			})
			.Select(instruction => ProcessInstruction(instruction, head, tail))
			.ToArray()
			.Last()
			.Tail.Visited
			.Distinct()
			.Count();
}

int SolvePartTwo(string input)
{
	var knots = 
		Enumerable
			.Range(0, 10)
			.Select(_ => new Knot())
			.ToArray();
	
	input
		.Split("\n", StringSplitOptions.RemoveEmptyEntries)
		.Select(s =>
		{
			var parts = s.Trim().Split(' ');
			return new Instruction(parts[0], Convert.ToInt32(parts[1]));
		})
		.SelectMany(instruction => ProcessInstructionTwo(instruction, knots))
		.Last()
		.Visited
		.Distinct()
		.Count()
		.Dump();
		
 	return 0;
}

(Knot Head, Knot Tail) ProcessInstruction(Instruction instruction, Knot head, Knot tail) =>
	Enumerable
		.Range(0, instruction.Distance)
		.Select(_ => (head.Move(instruction.Direction, 1), tail.Follow(head)))
		.ToArray()
		.Last();

Knot[] ProcessInstructionTwo(Instruction instruction, Knot[] knots)
{
	for (int i = 0; i < instruction.Distance; i++)
	{
		knots[0].Move(instruction.Direction, 1);
		
		for (int k = 1; k < knots.Length; k++)
		{
			knots[k].Follow(knots[k-1]);
		}
	}
	
	return knots;
}

record Instruction(string Direction, int Distance);

record Knot()
{
	public List<Point> Visited = new() { new Point(0, 0) };

	public Point Position => Visited.Last();
	
	public Knot Move(string direction, int distance)
	{
		Visited.Add(direction switch
		{
			"U" => new Point(Position.X, Position.Y + 1),
			"D" => new Point(Position.X, Position.Y - 1),
			"L" => new Point(Position.X - 1, Position.Y),
			"R" => new Point(Position.X + 1, Position.Y),
			"UR" => new Point(Position.X + 1, Position.Y + 1),
			"UL" => new Point(Position.X - 1, Position.Y + 1),
			"DR" => new Point(Position.X + 1, Position.Y - 1),
			"DL" => new Point(Position.X - 1, Position.Y - 1),
			_ => throw new ArgumentException("unexpected direction"),
		});
		
		return this;
	}
	
	public Knot Follow(Knot parent) 
	{
		var manhattanDistance = Math.Abs(parent.Position.X - Position.X) + Math.Abs(parent.Position.Y - Position.Y);
		if (manhattanDistance > 1)
		{
			if (parent.Position.Y == Position.Y)
			{
				return Move(parent.Position.X > Position.X ? "R" : "L", 1);
			}
			
			if (parent.Position.X == Position.X)
			{
				return Move(parent.Position.Y > Position.Y ? "U" : "D", 1);
			}

			if (manhattanDistance > 2)
			{
				if (parent.Position.X > Position.X && parent.Position.Y > Position.Y)
				{
					return Move("UR", 1);
				}


				if (parent.Position.X < Position.X && parent.Position.Y > Position.Y)
				{
					return Move("UL", 1);
				}


				if (parent.Position.X > Position.X && parent.Position.Y < Position.Y)
				{
					return Move("DR", 1);
				}


				if (parent.Position.X < Position.X && parent.Position.Y < Position.Y)
				{
					return Move("DL", 1);
				}
			}
		}
		
		return this;
	}
}

