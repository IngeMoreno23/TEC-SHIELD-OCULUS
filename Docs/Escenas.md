
El proyecto actual cuenta con 2 escenas, MainMenu y Simulation1.

# MainMenu

Solamente cuenta con un asset del estadio, y un canvas para iniciar a la primera simulación o salir del juego, no se implementan opciones por ahora.

El botón de Jugar puede mandarte primero a una pantalla de Simulaciones, y así poder iniciar cualquier escena deseada.

# Simulation1

La primera escena no cuenta con assets especiales, solamente cuenta con un GameManager con el script GameManager1 que se encarga de la lógica de selección.

La simulación inicia al presionar un botón Start en frente del jugador, las esferas se pueden poner en estado seleccionable y no seleccionable, para no poder hoverear cuando se estan moviendo, y al detenerse, comienza la lógica de selección, esto se repite 3 veces.

Al acabar la simulación, la escena no hace nada más. No guarda ni muestra resultados.