<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Net.Http</Namespace>
</Query>

#load "input.linq"

const string testInput = @"A Y
B X
C Z";

async Task Main()
{
	var input = await GetInput(2);
	SolvePartOne(input).Dump();
	SolvePartTwo(input).Dump();
}

int SolvePartOne(string input) =>
	input
		.Split("\n", StringSplitOptions.RemoveEmptyEntries)
		.Select(s => new Game(s.Split(" ")[0].Trim(), s.Split(" ")[1].Trim()))
		.Select(game => PlayPartOneRules(game))
		.Sum(result => result.Score);


int SolvePartTwo(string input) =>
	input
		.Split("\n", StringSplitOptions.RemoveEmptyEntries)
		.Select(s => new Game(s.Split(" ")[0].Trim(), s.Split(" ")[1].Trim()))
		.Select(game => PlayPartTwoRules(game))
		.Sum(result => result.Score);

Result PlayPartOneRules(Game game)
{
	return game switch
	{
		{ PlayerOne: "A", PlayerTwo: "X" } => new Result(ResultType.Draw, game.PlayerTwo),
		{ PlayerOne: "A", PlayerTwo: "Y" } => new Result(ResultType.Win, game.PlayerTwo),
		{ PlayerOne: "A", PlayerTwo: "Z" } => new Result(ResultType.Loss, game.PlayerTwo),

		{ PlayerOne: "B", PlayerTwo: "X" } => new Result(ResultType.Loss, game.PlayerTwo),
		{ PlayerOne: "B", PlayerTwo: "Y" } => new Result(ResultType.Draw, game.PlayerTwo),
		{ PlayerOne: "B", PlayerTwo: "Z" } => new Result(ResultType.Win, game.PlayerTwo),

		{ PlayerOne: "C", PlayerTwo: "X" } => new Result(ResultType.Win, game.PlayerTwo),
		{ PlayerOne: "C", PlayerTwo: "Y" } => new Result(ResultType.Loss, game.PlayerTwo),
		{ PlayerOne: "C", PlayerTwo: "Z" } => new Result(ResultType.Draw, game.PlayerTwo),
		
		_ => throw new InvalidOperationException("Unrecognised game inputs.")
	};
}

Result PlayPartTwoRules(Game game)
{
	return game switch
	{
		{ PlayerOne: "A", PlayerTwo: "X" } => new Result(ResultType.Loss, "C"),
		{ PlayerOne: "B", PlayerTwo: "X" } => new Result(ResultType.Loss, "A"),
		{ PlayerOne: "C", PlayerTwo: "X" } => new Result(ResultType.Loss, "B"),

		{ PlayerOne: "A", PlayerTwo: "Y" } => new Result(ResultType.Draw, "A"),
		{ PlayerOne: "B", PlayerTwo: "Y" } => new Result(ResultType.Draw, "B"),
		{ PlayerOne: "C", PlayerTwo: "Y" } => new Result(ResultType.Draw, "C"),

		{ PlayerOne: "A", PlayerTwo: "Z" } => new Result(ResultType.Win, "B"),
		{ PlayerOne: "B", PlayerTwo: "Z" } => new Result(ResultType.Win, "C"),
		{ PlayerOne: "C", PlayerTwo: "Z" } => new Result(ResultType.Win, "A"),

		_ => throw new InvalidOperationException("Unrecognised game inputs.")
	};
}

record Game(string PlayerOne, string PlayerTwo);

enum ResultType {
	Loss,
	Draw,
	Win, 
}

record Result(ResultType Type, string PlayerTwoMove)
{
	public int Score => CalcScore();
	
	int CalcScore()
	{
		return Type switch
		{
			ResultType.Win => 6 + ScoreMove(PlayerTwoMove),
			ResultType.Draw => 3 + ScoreMove(PlayerTwoMove),
			ResultType.Loss => 0 + ScoreMove(PlayerTwoMove),
			_ => 0
		};
	}
	
	int ScoreMove(string move) 
	{
		return move switch 
		{
			"A" or "X" => 1,
			"B" or "Y" => 2,
			"C" or "Z" => 3,
			_ => 0
		};
	}
}
