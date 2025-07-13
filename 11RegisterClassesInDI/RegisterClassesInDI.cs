﻿using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;

namespace _11RegisterClassesInDI;

/// <summary>
/// This is the replacement for the former package.json data. This is required for all mods.
///
/// This is where we define all the metadata associated with this mod.
/// You don't have to do anything with it, other than fill it out.
/// All properties must be overriden, properties you don't use may be left null.
/// It is read by the mod loader when this mod is loaded.
/// </summary>
public record ModMetadata : AbstractModMetadata
{
    public override string ModGuid { get; set; } = "com.sp-tarkov.examples.registerclassesindi";
    public override string Name { get; set; } = "RegisterClassesInDIExample";
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

// We want to load after PostDBModLoader is complete, so we set our type priority to that, plus 1.
[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class RegisterClassesInDi(
    SingletonClassExample singletonClassExample,
    ScopedClassExample scopedClassExample)
    : IOnLoad
{
    // We inject 2 classes (singleton and scoped) we've made below

    public Task OnLoad()
    {
        singletonClassExample.IncrementCounterAndLog();
        singletonClassExample.IncrementCounterAndLog();
        singletonClassExample.IncrementCounterAndLog();

        scopedClassExample.IncrementCounterAndLog();
        scopedClassExample.IncrementCounterAndLog();
        scopedClassExample.IncrementCounterAndLog();
        
        return Task.CompletedTask;
    }
}

// This class is registered as a singleton. This means ONE and only ONE instance
// of this class will ever exist.
[Injectable(InjectionType.Singleton)]
public class SingletonClassExample(ISptLogger<SingletonClassExample> logger)
{
    private int _counter = 0;

    public void IncrementCounterAndLog()
    {
        _counter++;
        logger.Success($"{_counter}");
    }
}

// This class is being registered as default or scoped. This means that
// every time a class requests an instance of this type a new one will be created
[Injectable(InjectionType.Scoped)] // [Injectable] is the same as doing this
public class ScopedClassExample(
    ISptLogger<ScopedClassExample> logger)
{
    private int _counter = 0;

    public void IncrementCounterAndLog()
    {
        _counter++;
        logger.Success($"{_counter}");
    }
}
