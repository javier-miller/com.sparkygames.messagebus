# Changelog

All notable changes to Sparky Games Service Bus are documented here.

## [2.0.0] - 2026-09-27

### Changed

- Reworked delivery around synchronous, registration-ordered snapshots. Subscriptions may be added or removed during delivery, and nested publications run immediately.
- Subscriber exceptions no longer stop later subscribers. `Publish` raises an `AggregateException` after delivery.
- `IMessage` is now a marker interface. Message ID and timestamp are no longer required; applications may define their own metadata.
- `Subscribe` now returns `IDisposable`. Disposing the token removes only that subscription. `Bus.Dispose()` invalidates the bus and all its tokens.
- `UnityMessageBroker.CreateBus` is replaced by `GetOrCreateBus`. `RemoveBus` disposes and removes a named bus; `GetAll` returns a snapshot. The global registry resets on entry to Play mode, including when domain reload is disabled.
- Minimum declared Unity version is now `6000.0`. The prerelease-specific `unityRelease` field was removed.

### Removed

- The redundant generic `IBus.Publish<TMessage>` overload; `Publish(IMessage)` accepts all messages.
- `MessageBase<TType>`, `MessageManagementComponent`, and the public subscription classes and interfaces. `MonoBehaviour` receivers should subscribe in `OnEnable` and dispose their token in `OnDisable`.

### Added

- Edit Mode and Play Mode tests for dispatch, lifecycle, errors, and Unity broker behavior.
- A Basic Messaging Package Manager sample and usage documentation.

### Migration from 1.0.0

1. Replace `UnityMessageBroker.CreateBus(name)` with `UnityMessageBroker.GetOrCreateBus(name)`.
2. Replace `MessageBase<T>` inheritance with a class implementing `IMessage`. Keep an ID or timestamp in that class only when the application needs one.
3. Store the `IDisposable` returned by `Subscribe` and dispose it when the receiver stops listening. Do not dispose a shared bus from a receiving component.
4. Replace `MessageManagementComponent` with a direct `MonoBehaviour` subscription; see `README.md` or the Basic Messaging sample.
5. If a publisher relied on the first subscriber exception stopping delivery, handle the new `AggregateException` policy explicitly.

There are no production consumers of version 1.0.0 to migrate, but these steps document the public API break for Git users.

## [1.0.0] - 2024

- Initial message bus implementation.
