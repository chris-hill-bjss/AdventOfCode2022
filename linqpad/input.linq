<Query Kind="Program">
  <NuGetReference>Microsoft.Extensions.Configuration</NuGetReference>
  <NuGetReference>Microsoft.Extensions.Configuration.Json</NuGetReference>
  <Namespace>Microsoft.Extensions.Configuration</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

private async Task<string> GetInput(int day)
{
	var config = new ConfigurationBuilder()
		.AddJsonFile("/src/AdventOfCode2022/linqpad/config.json", false)
		.Build();
		
	using var httpClient = new HttpClient { BaseAddress = new Uri("https://adventofcode.com/") };
	httpClient.DefaultRequestHeaders.Add("Cookie", $"session={config["Session"]};");
	var response = await httpClient.GetAsync(new Uri($"/2022/day/{day}/input", UriKind.Relative));
	response.EnsureSuccessStatusCode();

	var content = await response.Content.ReadAsStringAsync();

	return content;
}