using DemoBlazorApp.Features.User.Store.Actions;
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
    public Task UpdateEmail(UpdateEmail _, IDispatcher _2)
    {
      return Task.CompletedTask;
    }

    //Missing dispatcher
    //Wrong return
    [EffectMethod]
    public void UpdateEmail(UpdateEmail action){}

    //wrong argument order.
    [EffectMethod]
    public static Task UpdateEmail2(IDispatcher dispatcher, UpdateEmail action)
    {return Task.CompletedTask;}

  }
}
