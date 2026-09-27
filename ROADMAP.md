# Roadmap de Sparky Games Message Bus

## Objetivo

Convertir el paquete en un bus de mensajes local, síncrono y fiable para comunicar componentes y sistemas de Unity. La revisión propuesta será `2.0.0`: el paquete está publicado en Git, pero no tiene consumidores en producción, así que podemos simplificar contratos públicos sin mantener adaptadores para la API actual.

## Estado (2026-09-27)

- **Fases 1–5 implementadas y validadas para `2.0.0`.** Esa versión declaraba Unity `6000.0` como mínimo y ofrecía el sample Basic Messaging en Package Manager.
- **Validación de `2.0.0`:** la identidad final `com.sparkygames.messagebus` se importó desde Git en Unity `6000.6.2f1` y pasó 8 pruebas Edit Mode y 3 Play Mode, incluida una prueba del sample importado. La implementación anterior al cambio de nombre se validó también en Unity `6000.6.0f1` con los mismos resultados. El mínimo exacto `6000.0` no estaba instalado. En la fase 4 se verificó además el reinicio de Play con recarga de dominio activada y desactivada.
- **Publicación Git:** `v2.0.0` y `v3.0.0` están publicados en el repositorio `javier-miller/com.sparkygames.messagebus`. `3.0.0` separa la propiedad del bus de `IBus` y admite mensajes de valor.
- **Validación de `3.0.0`:** importación Git en proyectos limpios con Unity `6000.6.0f1` y `6000.6.2f1`; en cada uno pasaron 10 pruebas Edit Mode y 3 Play Mode, incluida la prueba del sample importado. El mínimo declarado es `6000.6` y se probó en `6000.6.0f1`.

## Contrato que debe quedar definido antes de cambiar la API

- **Entrega:** `Publish` ejecuta los receptores de forma síncrona, en el hilo que llama y en orden de suscripción. El uso desde componentes de Unity se documenta para el hilo principal; el bus no promete seguridad multihilo.
- **Tipos:** una suscripción a `TMessage` recibe instancias de `TMessage` y de tipos derivados o que implementen esa interfaz. Una suscripción general recibe todos los mensajes. Mantener esta semántica actual y documentarla.
- **Cambios durante la entrega:** cada publicación toma una instantánea de las suscripciones. Una suscripción nueva empieza a recibir en la siguiente publicación; una suscripción dada de baja deja de recibir inmediatamente, aunque aparezca en la instantánea. Una publicación anidada se ejecuta inmediatamente y usa su propia instantánea.
- **Errores:** un receptor que lanza una excepción no impide ejecutar los demás. Al terminar, `Publish` comunica todos los errores mediante `AggregateException`. Solo se reserva memoria adicional para recoger errores cuando ocurre alguno.
- **Propiedad:** quien se suscribe es dueño de su token de suscripción; quien crea un bus independiente es dueño del bus. Un componente suscriptor nunca dispone un bus compartido. El broker ofrece buses globales por nombre y se limpia al iniciar una nueva sesión de Play.
- **Entradas inválidas:** `Publish(null)` y `Subscribe(null)` lanzan `ArgumentNullException`. `Publish` y `Subscribe` sobre un bus dispuesto lanzan `ObjectDisposedException`. Disponer dos veces un token o un bus es seguro.

Si alguna de estas decisiones cambia al implementar, actualizar primero este contrato y las pruebas correspondientes.

## Fase 1 — Corregir el núcleo del bus

1. Sustituir el recorrido directo de la lista de `Bus` por una entrega segura ante altas, bajas, disposición del bus y publicaciones anidadas.
2. Hacer que el token de suscripción desregistre exactamente una vez y deje de retener el callback después de disponerlo. Impedir que una suscripción dispuesta invoque directamente su callback.
3. Comprobar el estado dispuesto del bus en todas sus operaciones públicas. Al disponer el bus, invalidar sus tokens y liberar las referencias a los receptores.
4. Eliminar la duplicación entre `Publish(IMessage)` y `Publish<TMessage>`; conservar una única entrada pública si cubre ambos usos sin perder tipado en `Subscribe<TMessage>`.
5. Aplicar la política de excepciones y validación de argumentos definida arriba.

**Terminado cuando:** una baja o alta dentro de un callback no rompe la publicación; los receptores posteriores siguen ejecutándose tras un error; un bus dispuesto no vuelve a aceptar operaciones ni conserva callbacks.

## Fase 2 — Aclarar el ciclo de vida en Unity

1. Retirar `MessageManagementComponent`: añadía una capa sin capacidad de filtrado por tipo y confundía la propiedad del bus. El patrón recomendado de `MonoBehaviour` se suscribe en `OnEnable` y libera su token en `OnDisable`; está en `README.md` y en las pruebas Play Mode.
2. Mantener el bus compartido vivo mientras existan otros usuarios. Ningún receptor lo dispone al desactivarse o destruirse.
3. Renombrar `UnityMessageBroker.CreateBus` a una operación que exprese que puede devolver uno existente, por ejemplo `GetOrCreateBus`. Definir cómo se elimina explícitamente un bus global y qué devuelve `GetAll`.
4. Reiniciar el registro estático con `RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)` y comprobar el comportamiento al entrar y salir de Play con la recarga de dominio activada y desactivada.
5. Documentar cuándo usar un bus global por nombre y cuándo crear un `Bus` con propietario explícito para una escena o sistema.

**Terminado cuando:** destruir o desactivar un componente no interrumpe a otros suscriptores del mismo bus; los buses globales no arrastran suscripciones entre sesiones de Play.

## Fase 3 — Simplificar mensajes y superficie pública

1. Quitar el parámetro genérico sin uso de `MessageBase<TType>`. Permitir mensajes tipados sencillos sin obligar a crear un GUID y una fecha para cada evento interno.
2. Si se conservan metadatos opcionales, usar nombres y tiempo UTC inequívocos y mantenerlos separados del contrato mínimo de mensaje.
3. Revisar si `Subscription`, `SubscriptionBase`, `ISubscription` e `ISubscriptionResult` deben ser públicos. Exponer solo el contrato que necesita un consumidor: `IBus`, el mensaje y un token `IDisposable`.
4. Fijar el ID UPM `com.sparkygames.messagebus` y el ensamblado `SparkyGames.MessageBus` antes de publicar, sustituyendo los nombres anteriores. Si se separa el núcleo de la integración Unity en dos ensamblados, hacerlo solo si aporta una ventaja concreta a consumidores o pruebas; documentar la migración.

**Terminado cuando:** se puede publicar y recibir un mensaje propio con un ejemplo breve, sin herencia ni metadatos obligatorios, y cada tipo público restante tiene un propósito claro.

## Fase 4 — Pruebas y validación en Unity

1. Añadir pruebas Edit Mode para orden de entrega, suscripción tipada y general, herencia/interfaz, altas y bajas durante `Publish`, publicación anidada, excepciones y disposición repetida.
2. Añadir pruebas Play Mode o una escena de validación para dos componentes que comparten un bus, activación/desactivación, destrucción y reinicio de Play. Cubrir ambas configuraciones de recarga de dominio.
3. Importar el paquete como dependencia Git en un proyecto Unity limpio y compilar el ensamblado. Comprobar que los `.meta` y GUID de los recursos existentes permanecen estables.

**Terminado cuando:** las pruebas pasan en Unity y la importación desde Git no requiere archivos del proyecto donde se desarrolló el paquete.

## Fase 5 — Documentación y publicación

1. Crear `README.md` con instalación por Git/paquete local, ejemplo de `MonoBehaviour`, propiedad y vida útil de buses y tokens, semántica de entrega y tratamiento de errores.
2. Añadir un sample UPM mínimo con dos componentes que intercambien un mensaje, si la prueba de importación muestra que facilita el primer uso.
3. Crear `CHANGELOG.md` con los cambios incompatibles y la guía para pasar de `1.0.0` a `2.0.0`.
4. Revisar `package.json`: descripción, palabras clave y categoría deben reflejar mensajería general; confirmar la versión mínima de Unity que se va a probar y eliminar restricciones de prerelease innecesarias.
5. Actualizar la versión a `2.0.0`, verificar el árbol Git limpio después del commit y crear el tag solo tras pasar la validación de la fase 4.

## Fuera del alcance de `2.0.0`

Colas, entrega diferida, `async/await`, persistencia, red, priorización de receptores, reflexión para descubrir handlers y seguridad multihilo. Se valorarán cuando aparezca un caso de uso medible que el bus síncrono no cubra.
