using System.Text.RegularExpressions;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace LightTubeEmbedder;

public class EmbedCommands : ApplicationCommandModule
{
	Regex ytRegex =
		new(
			"(?:https?:\\/\\/)?(?:(?:(?:www\\.?)?youtube\\.com(?:\\/(?:(?:watch\\?.*?(v=[^&\\s]+).*)|(?:v(\\/.*))|(channel\\/.+)|(?:user\\/(.+))|(?:results\\?(search_query=.+))))?)|(?:youtu\\.be(\\/.*)?))");

	[ContextMenu(ApplicationCommandType.MessageContextMenu, "Embed YouTube video")]
	public async Task EmbedContextMenu(ContextMenuContext ctx)
	{
		MatchCollection matches = ytRegex.Matches(ctx.TargetMessage.Content);
		List<string> ids = new();
		foreach (Match match in matches)
		{
			string id = ExtractIdFromMatch(match);
			if (id == null) continue;
			id = id.Trim('/');
			if (id.StartsWith("v=")) id = id[2..];
			ids.Add(id);
		}

		IEnumerable<string> urls = ids.Select(x => $"https://tube.kuylar.dev/proxy/media/{x}/18");

		if (ids.Count > 0)
			await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().AsEphemeral(true)
				.WithContent(string.Join("\n", urls)));
		else
			await ctx.CreateResponseAsync(
				new DiscordInteractionResponseBuilder().AsEphemeral()
					.WithContent("no youtube links found in that mesage :("));
	}

	[SlashCommand("youtube", "Send a YouTube video in a playable MP4 format")]
	public async Task EmbedCommand(InteractionContext ctx, [Option("yt-url", "YouTube URL of the video")] string input)
	{
		Match match = ytRegex.Match(input);
		string? id = ExtractIdFromMatch(match);
		if (id == null)
		{
			await ctx.CreateResponseAsync(
				new DiscordInteractionResponseBuilder().AsEphemeral()
					.WithContent("Invalid YouTube URL :("));
		}
		else
		{
			id = id.Trim('/');
			if (id.StartsWith("v=")) id = id[2..];
			string url = $"https://tube.kuylar.dev/proxy/media/{id}/18";

			await ctx.CreateResponseAsync(
				new DiscordInteractionResponseBuilder()
					//.AddEmbed(new DiscordEmbedBuilder()
					//	.WithImageUrl(url)
					//	.WithTitle("vv there should be a video there uwu owo"))
					.WithContent(url));
		}
	}

	private string? ExtractIdFromMatch(Match match)
	{
		int[] groups = { 1, 2, 6 };

		return (from groupId in groups where match.Groups[groupId].Length != 0 select match.Groups[groupId].Value)
			.FirstOrDefault();
	}
}