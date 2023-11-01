using Fluxor;

namespace DemoBlazorApp.Features.User.Store
{
  //[FeatureState]
  public record UserProfileState
    (
        string Name,
        string Email
    )
    {
        public UserProfileState()
            :this("Guest", "Guest@Nowhere.com") { }


      //Missing target action.
      [ReducerMethod] 
      public static UserProfileState UpdateEmail(UserProfileState state)
        => state{}
    }
}

