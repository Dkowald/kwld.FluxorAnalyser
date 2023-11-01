using Fluxor;

namespace kwld.FluxorAnalyser.Tests.Assets;

public class Reducers
{
  [ReducerMethod(typeof(UpdateName))]
  public static SomeState Reduce1(SomeState state) 
    => state with { };

  [ReducerMethod]
  public static SomeState Reduce2(SomeState state, UpdateName action)
    => state with { };
    
  [ReducerMethod(typeof(UpdateName))]
  public SomeState FLX011_NonStaticReduce(SomeState state) 
    => state with { };

  [ReducerMethod]
  public static SomeMoreState FLX010_StateMismatch(SomeState state, UpdateName action)
    => new ("X");

  [ReducerMethod]
  public static SomeState FLX008_NoAction(SomeState state) => state with { };

  [ReducerMethod(typeof(UpdateName))]
  public static SomeState FLX009_NoState() => new("x");

  [ReducerMethod]
  public static SomeService FLX009_NotFeatureState(SomeService state, UpdateName action) 
    => new ();
}