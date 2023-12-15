Effect Method Signature

Effect methods should have one of the following signatures:

```cs
[Effect()]
Task Method(Action, IDispatcher)

[Effect(typeof(Action))]
Task Method(IDispatcher)
```
[Details](https://github.com/mrpmorris/Fluxor/blob/master/Source/Tutorials/02-Blazor/02B-EffectsTutorial/README.md#requesting-data-from-the-server-via-an-effect)