using DemoBlazorApp.Features.User.Store.Actions;
using Fluxor;

namespace DemoBlazorApp.Features.User.Store;

public record UserAccountState(bool IsGuest, bool EmailIsVerified)
{

  [ReducerMethod(typeof(EmailConfirmationReceived))]
  public static UserAccountState EmailConfirmationReceived(UserAccountState state)
    => state with {EmailIsVerified = true};
}