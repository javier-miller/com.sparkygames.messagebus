# Sparky Games Message Bus

Bus de mensajes síncrono para comunicar componentes y sistemas de Unity sin referencias directas entre emisor y receptor.

- Paquete UPM: `com.sparkygames.messagebus`.
- Versión: `2.0.0`.
- Unity mínimo declarado: `6000.0`.
- Ensamblado: `SparkyGames.MessageBus`.
- Namespace: `SparkyGames.MessageBus`.
- Dependencias UPM: ninguna.

## Instalación

En Package Manager, usar **Add package from git URL** con `https://github.com/javier-miller/com.sparkygames.messagebus.git#v2.0.0` una vez publicado el tag. Para desarrollar desde una copia local, usar **Add package from disk** y elegir el `package.json` de esta carpeta. El paquete incluye el sample **Basic Messaging** en la pestaña Samples.

## Uso desde un MonoBehaviour

Un mensaje solo necesita implementar `IMessage`:

```csharp
using SparkyGames.MessageBus;

public sealed class PlayerDamaged : IMessage
{
    public PlayerDamaged(int amount) { Amount = amount; }
    public int Amount { get; }
}
```

Cada receptor conserva y libera su propia suscripción:

```csharp
using System;
using SparkyGames.MessageBus;
using UnityEngine;

public sealed class DamageDisplay : MonoBehaviour
{
    private IDisposable _subscription;

    private void OnEnable()
    {
        _subscription = UnityMessageBroker.GetOrCreateBus("Gameplay")
            .Subscribe<PlayerDamaged>(OnPlayerDamaged);
    }

    private void OnDisable()
    {
        _subscription?.Dispose();
        _subscription = null;
    }

    private void OnPlayerDamaged(PlayerDamaged message)
    {
        Debug.Log($"Damage: {message.Amount}");
    }
}
```

El emisor publica con `UnityMessageBroker.GetOrCreateBus("Gameplay").Publish(new PlayerDamaged(5))`.

## Propiedad y ciclo de vida

`GetOrCreateBus(name)` devuelve el mismo bus global para ese nombre, distinguiendo mayúsculas y minúsculas. Cada receptor es dueño únicamente del token `IDisposable` que recibió al suscribirse. `UnityMessageBroker.RemoveBus(name)` elimina y dispone explícitamente un bus global cuando el sistema propietario termina; todas las referencias antiguas dejan de ser válidas. `GetAll()` devuelve una instantánea de los buses registrados.

Para un bus propio de una escena o sistema, usar `new Bus(name)`, distribuir esa instancia y disponerla desde su propietario. El registro global se reinicia al empezar una nueva sesión de Play, incluso con la recarga de dominio desactivada. Un componente receptor nunca dispone un bus compartido.

## Contrato de entrega

- La publicación es síncrona, en el hilo que llama, y sigue el orden de suscripción. Desde componentes de Unity, usar el hilo principal. No hay cola ni seguridad multihilo.
- `Subscribe<TMessage>` recibe también mensajes derivados o que implementen la interfaz `TMessage`. `Subscribe(Action<IMessage>)` recibe todos los mensajes.
- Cada publicación conserva una instantánea del orden de receptores. Una suscripción nueva participa desde la siguiente publicación; una baja deja de recibir inmediatamente. Una publicación anidada se ejecuta en ese momento con su propia instantánea.
- Si un receptor falla, los demás siguen ejecutándose. Al terminar, `Publish` lanza una `AggregateException` con los errores recibidos. El emisor decide cómo registrar o tratarla.
- Los mensajes y callbacks nulos se rechazan. Publicar o suscribirse en un bus dispuesto lanza `ObjectDisposedException`. Disponer varias veces un token o un bus es seguro.

Si un emisor necesita registrar los errores de los receptores, puede capturar `AggregateException` alrededor de `Publish` e inspeccionar `InnerExceptions`. El bus no convierte errores en mensajes ni los oculta.

## Sample y pruebas

El sample **Basic Messaging** contiene un emisor y un receptor `MonoBehaviour`. Importarlo desde Package Manager, añadir cada componente a un GameObject activo y entrar en Play; el receptor registra el mensaje en Console. Su README indica los pasos exactos.

Las pruebas del paquete están en `Tests/Editor` y `Tests/Runtime`. Para ejecutarlas desde Package Manager, el proyecto de prueba debe tener Unity Test Framework y añadir `"com.sparkygames.messagebus"` a la lista `testables` de `Packages/manifest.json`. Ejecutar los suites Edit Mode y Play Mode desde Test Runner. La identidad `com.sparkygames.messagebus` se importó desde Git y se verificó en Unity `6000.6.2f1` con 8 pruebas Edit Mode y 3 Play Mode, incluido el sample. Antes del cambio de nombre, el runtime de `2.0.0` también se verificó en Unity `6000.6.0f1`; el mínimo `6000.0` se declara para la línea Unity 6, aunque esa revisión exacta no estaba disponible para probarla. Las versiones anteriores a Unity 6 ya no forman parte del soporte previsto.

Los cambios incompatibles con `1.0.0` y los pasos de migración están en `CHANGELOG.md`.
