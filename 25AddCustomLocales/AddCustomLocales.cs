using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services;

namespace _25AddCustomLocales;

public record ModMetadata : AbstractModMetadata
{
    public override string Name { get; set; } = "AddCustomLocalesExample";
    public override string Author { get; set; } = "SPTarkov";
    public override List<string>? Contributors { get; set; }
    public override string Version { get; set; } = "1.0.0";
    public override string SptVersion { get; set; } = "4.0.0";
    public override List<string>? LoadBefore { get; set; }
    public override List<string>? LoadAfter { get; set; }
    public override List<string>? Incompatibilities { get; set; }
    public override Dictionary<string, string>? ModDependencies { get; set; }
    public override string? Url { get; set; }
    public override bool? IsBundleMod { get; set; }
    public override string? Licence { get; set; } = "MIT";
}

[Injectable(TypePriority = OnLoadOrder.PostSptModLoader + 1)]
public class AddCustomLocales(
    ISptLogger<AddCustomLocales> logger,
    DatabaseService databaseService,
    LocaleService localeService)
    : IOnLoad
{
    // Constructor - Inject a 'ISptLogger' with your mods Class inside the diamond brackets
    // Save the logger we're injecting into a private variable that is scoped to this class (only this class has access to it)
    // save the locale service into a private variable that is scoped to this class (only this class has access to it)

    public Task OnLoad()
    {
        // Add a custom locale to the en game locales
       if (databaseService.GetLocales().Global.TryGetValue("en", out var lazyloadedValue))
        {
            // We have to add a transformer here, because locales are lazy loaded due to them taking up huge space in memory
            // The transformer will make sure that each time the locales are requested, the ones changed or added below are included
            lazyloadedValue.AddTransformer(lazyloadedLocaleData =>
            {
                lazyloadedLocaleData["Attention! This is a Beta version of Escape from Tarkov for testing purposes."] = "Testing change of beta version warning";
                lazyloadedLocaleData.Add("TestingLocales", "Testing Locales");

                return lazyloadedLocaleData;
            });

            logger.Success("Added a custom locale to the database");
        }

        var _locales = localeService.GetLocaleDb("en");
        // Log this so we can see it in the console
        logger.Info(_locales["TestingLocales"]);
        logger.Info(_locales["Attention! This is a Beta version of Escape from Tarkov for testing purposes."]);
        return Task.CompletedTask;
    }
}