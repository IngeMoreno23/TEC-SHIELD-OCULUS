
# Prefabs

Solamente 2 prefabs para arrastrar a la escena, solamente el XR Origin () es útil en cualquier escena.

## XR Origin (Simple No-Movement)

Esta pensado para mantener al jugador en 1 solo lugar, poder rotar, y seleccionar objetos de la escena e interactuar con UI.

## SphereScene1

No es verdaderamente reutilizable. Es un prefab que se instancia en el GameManager1 al iniciar la escena, por lo que es necesario para que las esferas spawneen de manera correcta (spawnean en runtime, no en el editor).

# Scripts

La mayoría de los scripts están guardados en la carpeta `Scripts > ScriptsScene1`.

Ahí se encontrarán los siguientes scripts con las siguientes funcionalidades:

## FadeCamera

Es un scripts que lo contiene la Cámara del XR Origin, esta pensado para que al iniciar la escena se haga una transición con fade desde negro hasta transparente.

También contiene funciones para entrar y salir con transición a una pantalla en negro, útil para transiciones simples.

## GameManager1

Contiene la lógica para iniciar la primera simulación.

### GameManager1Editor

Añade 3 botones visibles en el inspector para probar splines en el editor. Útil para debuggear el movimiento de las esferas en la simulación.

## SelectableSphere1

Script con la lógica de selección y hover para la simulación 1. Contiene un método para devolver su ID asignada, y si fue seleccionada o deseleccionada. Útil para marcar la secuencia de selecciones en GameManager1.

## SplineBatchCreator y SplineBatchCreatorCollisions

Ambos son herramientas del editor para crear un nuevo set de splines, se generan de manera aleatoria, la diferencia entre los 2 es que el collisions permite que no se generen knots (de los splines) acumulados en un mismo lugar, y con suerte evitar agrupamientos.

Un problema que tienen, es que los splines resultan en diferentes velocidades, porque a mayor longitud, y mismo tiempo para iniciar y acabar, resulta en velocidades desiguales entre los splines.

![Imagen](<./Images/Pasted image 20250513222858.png>)

Para usarlo, necesitas ubicar la carpeta **SplineContainer** en jerarquía:

![Imagen](<./Images/Pasted image 20250513223216.png>)

Necesita estar vacía y debes de arrastrarla a la ventana del Spline Batch Creator:

![Imagen](<./Images/Pasted image 20250513223327.png>)

## UIGamePanel

Por la adición de un nuevo sistema de UI (UI Toolkit) que te permite generar un UI como si fuera un HTML (C# + UXML + USS) se intentó generar un UI por este método.

Hace uso de **UI_InputMapper** el cual es utilizado para interactuar con UI Documents en worldspace.

Este panel se usa para ver las iteraciones y esferas seleccionadas de la iteración actual del juego.

## UI_InputMapper

Si quieres un UI Document en worldspace, es necesario para mapear el cursor desde un Quad, a un UI Document visualizado con un RenderTexture.

¿Cómo funciona? Puedes ver el siguiente video como referencia. https://www.youtube.com/watch?v=gXx_j-6z8jY&list=PLA3-0SlTm4xcSQCWETwLhpzancgkQI7_q&index=18

A partir del minuto 5:45, **muestra el problema del UI Document en worldspace, y pasos para solucionarlo**.

## **UIDataVisualizer**

**Sin acabar**. No encontré una forma de visualizar los datos, actualmente se guardan en una clase definida dentro de GameManager1, aunque puede que sea poco optima, así como esta, es posible subir a una base de datos, pero algo difícil si no se guarda un Identificador In-Game.

Esperaba generar contenedores para instanciarlos pero no pude. Ahorita mismo solo hace un display raw de los datos, en una lista de Labels, pero no sirve para visualizar datos reales ni de manera clara.
# Otros

Shaders - Solamente genera un material Outline usado en la esfera de la simulación 1 para el outline effect.
