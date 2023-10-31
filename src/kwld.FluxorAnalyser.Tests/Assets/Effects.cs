using System;
using System.Threading.Tasks;
using Fluxor;

namespace kwld.FluxorAnalyser.Tests.Assets;

internal class Effects
{
  [EffectMethod]
  public Task OnUpdateName(UpdateName action, IDispatcher dispatcher)
    => Task.CompletedTask;

  [EffectMethod(typeof(UpdateName))]
  public Task OnUpdateName2(IDispatcher dispatcher)
    => Task.CompletedTask;
  
  /// <summary>Can be static</summary>
  [EffectMethod(typeof(UpdateName))]
  public static Task OnUpdateName3(IDispatcher dispatcher)
    => Task.CompletedTask;

  [EffectMethod(typeof(UpdateName))]
  public Task<int> FLX004_WrongReturnType(IDispatcher _) 
    => Task.FromResult(1);

  [EffectMethod]
  public Task FLX005_NoAction(IDisposable disposable) 
    => Task.CompletedTask;
  
  [EffectMethod]
  public Task FLX007_IncorrectArgumentOrder(IDispatcher dispatcher, UpdateName action)
    => Task.CompletedTask;

  [EffectMethod(typeof(UpdateName))]
  public Task FLX006_MissingDispatcher()
    => Task.FromResult(1);
}

