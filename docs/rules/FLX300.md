Reducer method signature

Reducer methods should have one of the following signatures:

```cs
[Reducer()]
static FeatureState Reduce(FeatureState state, Action action)

[Reducer(typeof(Action)]
static FeatureState Reduce(FeatureState state)
```

[Details](https://github.com/mrpmorris/Fluxor/tree/master/Source/Tutorials/01-BasicConcepts/01A-StateActionsReducersTutorial#user-content-reacting-to-the-action-to-change-state)