using DemoBlazorApp.Features.User.Store;
using Fluxor;

namespace DemoBlazorApp.Features.User
{
  public class UserProfileCrud
  {
    private readonly IHttpClientFactory _clientFactory;

    public UserProfileCrud(IHttpClientFactory clientFactory)
    {
      _clientFactory = clientFactory;
    }

    [EffectMethod]
    public void UpdateEmail(UpdateEmail action)
    {}

    [EffectMethod]
    public static Task UpdateEmail2(IDispatcher dispatcher, UpdateEmail action)
    {return Task.CompletedTask;}

  }
}
