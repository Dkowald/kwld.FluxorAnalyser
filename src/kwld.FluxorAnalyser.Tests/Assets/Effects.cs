using System.Threading.Tasks;

using Fluxor;

namespace kwld.FluxorAnalyser.Tests.Assets;

//ignore expected faults.
#pragma warning disable CA1822

internal class Effects
{
  [EffectMethod]
  public Task OnUpdateName(UpdateName _, IDispatcher _1)
    => Task.CompletedTask;

  [EffectMethod(typeof(UpdateName))]
  public Task OnUpdateName2(IDispatcher _)
    => Task.CompletedTask;
  
  /// <summary>Can be static</summary>
  [EffectMethod(typeof(UpdateName))]
  public static Task OnUpdateName3(IDispatcher _)
    => Task.CompletedTask;

  [EffectMethod(typeof(UpdateName))]
  public Task<int> FLX201_WrongReturnType(IDispatcher _)
    => Task.FromResult(1);

  [EffectMethod]
  public Task FLX202_NoAction(IDispatcher _)
    => Task.CompletedTask;
  
  [EffectMethod]
  public Task FLX203_IncorrectArgumentOrder(IDispatcher _, UpdateName _1)
    => Task.CompletedTask;

  [EffectMethod(typeof(UpdateName))]
  public Task FLX203_MissingDispatcher(UpdateName _)
    => Task.FromResult(1);

  [EffectMethod]
  public Task FLX200_TooManyArguments(UpdateName _, string _1, IDispatcher _2)
    => Task.CompletedTask;
}

