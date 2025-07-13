﻿using SPTarkov.DI.Annotations;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Utils;
using System.Reflection;

namespace _6._1OverrideMethodHarmony;

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
    public override string ModGuid { get; set; } = "com.sp-tarkov.examples.overridemethodharmony";
    public override string Name { get; set; } = "OverrideMethodHarmonyExample";
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

[Injectable(TypePriority = OnLoadOrder.PreSptModLoader)]
public class StartAsyncHarmonyPatchExample(
    ISptLogger<StartAsyncHarmonyPatchExample> logger) : IOnLoad
{
    public Task OnLoad()
    {
        // You will need to enable your patch in an OnLoad, preferably during PreSptModLoader
        new StartAsyncPatch().Enable();

        logger.Success($"StartAsync harmony patch has successfully loaded!");

        return Task.CompletedTask;
    }
}

public class StartAsyncPatch : AbstractPatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(App).GetMethod(nameof(App.StartAsync));
    }

    [PatchPrefix]
    public static bool Prefix()
    {
        // We add a log message to the StartAsync method
        ServiceLocator.ServiceProvider.GetService<ISptLogger<App>>().Success("This is a StartAsync harmony patch mod override!");

        // You can perform any code here before the method actually runs

        // This runs the original method, can be set to false, skipping the original method.
        return true;
    }

    [PatchPostfix]
    public static async Task Postfix(Task __result)
    {
        // Optionally here you could modify the result after it has run, or run code afterwards.
        ServiceLocator.ServiceProvider.GetService<ISptLogger<App>>().Success("StartAsync harmony patch OnLoad has ran!");

        // Have to await a result here because of async, this will not be necessary on a non-async method
        await __result;
    }
}
