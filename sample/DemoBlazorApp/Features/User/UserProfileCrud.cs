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

    //Wrong return
    [EffectMethod(typeof(UpdateEmail))]
    public void UpdateEmail1(IDispatcher _) { }

    //Missing dispatcher
    [EffectMethod]
    public Task UpdateEmail2(UpdateEmail action) 
      => Task.CompletedTask;

    //Missing action
    [EffectMethod]
    public Task UpdateEmail3(IDispatcher _) 
      => Task.CompletedTask;

    //wrong argument order.
    [EffectMethod]
    public static Task UpdateEmail4(IDispatcher dispatcher, UpdateEmail action)
    {return Task.CompletedTask;}

  }
}
