using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using LightTubeEmbedder;

DiscordClient client = new(new DiscordConfiguration
{
	Token = Environment.GetEnvironmentVariable("TOKEN") ?? throw new ArgumentNullException(null, "what the fuck is wrong with you"),
	Intents = DiscordIntents.AllUnprivileged | DiscordIntents.MessageContents
});

SlashCommandsExtension sce = client.UseSlashCommands();
sce.RegisterCommands(typeof(EmbedCommands), 917263628846108683);

client.Ready += (sender, eventArgs) =>
{
	Console.WriteLine(client.CurrentApplication.GenerateOAuthUri(null, Permissions.None, OAuthScope.Bot,
		OAuthScope.ApplicationsCommands).Replace(" ", "%20"));
	return Task.CompletedTask;
};

await client.ConnectAsync();
await Task.Delay(-1);