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
    public UserProfileState UpdateEmail2(UserProfileState state, UpdateEmail action)
      => state with { Email = action.EMail };

    //Missing target action.
    [ReducerMethod]
    public static UserProfileState UpdateEmail3(UserProfileState state)
      => state with { };
    
    //Not reducing state
    [ReducerMethod(typeof(UpdateEmail))]
    public static UserAccountState UpdateEmail3(UserAccountState state)
      => state with { };
  }
}

