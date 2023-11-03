using Fluxor;

namespace kwld.FluxorAnalyser.Tests.Assets;

public class Reducers
{
  [ReducerMethod(typeof(UpdateName))]
  public static SomeState Reduce1(SomeState state) 
    => state with { };

  [ReducerMethod]
  public static SomeState Reduce2(SomeState state, UpdateName _)
    => state with { };
    
  [ReducerMethod(typeof(UpdateName))]
  public SomeState FLX304_NonStaticReduce(SomeState state)
    => state with { };

  [ReducerMethod]
  public static SomeMoreState FLX303_StateMismatch(SomeState _, UpdateName _1)
    => new ("X");

  [ReducerMethod]
  public static SomeState FLX301_NoAction(SomeState state)
    => state with { };

  [ReducerMethod(typeof(UpdateName))]
  public static SomeState FLX303_NoState(UpdateName _)
    => new("x");

  [ReducerMethod]
  public static SomeService FLX302_NotFeatureState(SomeService _, UpdateName _1)
    => new ();

  [ReducerMethod]
  public static SomeState FLX300_WrongNumberOFArguments(SomeService _, string _1, UpdateName _2)
    => new("S");
}