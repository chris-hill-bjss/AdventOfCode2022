<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <RuntimeVersion>7.0</RuntimeVersion>
</Query>

#load "input.linq"

const string testInput = @"bvwbjplbgvbhsrlpgdmjqwftvncz";

async Task Main()
{
	var input = await GetInput(6);
	
	SolvePartOne(input).Dump();
	SolvePartTwo(input).Dump();
}

int SolvePartOne(string input) => FindFirstStartOfPacketMarker(input, 4);
int SolvePartTwo(string input) => FindFirstStartOfPacketMarker(input, 14);

public int FindFirstStartOfPacketMarker(string input, int length) 
{
	for (int i = 0; i < input.Length - length-1; i++)
	{
		var window = input[i..(i + length)];
		if (window.Distinct().Count() == length)
		{
			return i + length;
		}
	}
	
	return 0;
}