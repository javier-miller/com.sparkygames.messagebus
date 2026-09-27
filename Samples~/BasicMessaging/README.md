# Basic Messaging

Import this sample from the package's **Samples** tab in Package Manager.

1. Add `MessageBusSampleReceiver` to one active GameObject in a scene.
2. Add `MessageBusSampleSender` to another active GameObject and optionally edit its **Text** field.
3. Enter Play mode. The Console shows the message received by the first component.
4. Disable the receiver and enter Play mode again to see that it no longer handles messages.

The receiver owns only its subscription token. The named bus is shared for the Play session and is reset automatically when the next session begins.
