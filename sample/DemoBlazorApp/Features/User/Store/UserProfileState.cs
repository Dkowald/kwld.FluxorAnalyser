using DemoBlazorApp.Features.User.Store.Actions;

using Fluxor;

namespace DemoBlazorApp.Features.User.Store
{
  [FeatureState]
  public record UserProfileState
  (string Name, string Email)
  {
    public UserProfileState()
        : this("Guest", "Guest@Nowhere.com") { }

    [ReducerMethod]
    public static UserProfileState UpdateEmail(UserProfileState state, UpdateEmail action)
      => state with { Email = action.EMail };

    //Prefer static over instance.
    [ReducerMethod]
    public UserProfileState UpdateEmail1(UserProfileState state, UpdateEmail action)
      => state with { Email = action.EMail };

    //Missing target action.
    [ReducerMethod]
    public static UserProfileState UpdateEmail2(UserProfileState state)
      => state with { };

    //Not reducing feature state
    [ReducerMethod(typeof(UpdateEmail))]
    public static UserAccountState UpdateEmail3(UserAccountState state)
      => state with { };

    //wrong argument order
    [ReducerMethod]
    public static UserProfileState UpdateEmail4(UpdateEmail action, UserProfileState state)
      => state with { };

    //missing state argument.
    [ReducerMethod]
    public static UserProfileState UpdateEmail5(UpdateEmail action)
      => new();

    //Not taking feature-state as argument.
    [ReducerMethod]
    public static UserProfileState UpdateEmail6(UserAccountState state, UpdateEmail action)
      => new();
  }
}

