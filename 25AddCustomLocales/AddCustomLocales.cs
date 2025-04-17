using SPTarkov.Common.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.External;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Utils;

namespace _25AddCustomLocales;

[Injectable]
public class AddCustomLocales : IPostSptLoadMod
{
    // Our logger we create in the constructor below
    private readonly ISptLogger<AddCustomLocales> _logger;
    private readonly LocaleService _localeService;
    public static Dictionary<string, string> Locales;

    // Constructor - Inject a 'ISptLogger' with your mods Class inside the diamond brackets
    public AddCustomLocales(
        ISptLogger<AddCustomLocales> logger,
        LocaleService localeService
    )
    {
        // Save the logger we're injecting into a private variable that is scoped to this class (only this class has access to it)
        _logger = logger;
        // save the locale service into a private variable that is scoped to this class (only this class has access to it)
        _localeService = localeService;
    }

    public void PostSptLoad()
    {
        // Add a custom locale to the en game locales
        _localeService.AddCustomClientLocale("en", "TestingLocales", "This is a test locale");

        _logger.Success("Added a custom locale to the database");
        Locales = _localeService.GetLocaleDb("en");
        // Log this so we can see it in the console
        _logger.Info(Locales["TestingLocales"]);
    }
}