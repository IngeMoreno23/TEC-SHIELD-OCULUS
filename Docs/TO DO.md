
# Simulation1

Actualmente la escena simplemente tiene un panel mostrar el estado de la simulación y puedes interactuar con las esferas según la secuencia del juego.

![Imagen](<./Images/Pasted image 20250514125534.png>)

La escena necesita un Menu para pausar, salir del juego. Y al acabar la secuencia, podría esperar un poco y regresar al menu principal.

Sobre los datos, necesita guardar los datos y serializarlos, esto puede ser en un json. Actualmente no se guardan ni se representan de alguna forma.

La idea inicial era por cada juego, serializar una lista en un json de SelectionData, un struct dentro de GameManager1:

![Imagen](<./Images/Pasted image 20250514125826.png>)

Con esta lista puedes generar una secuencia de acciones/inputs del usuario y después de analizarla, puedes obtener resultados. Las esferas correctas esta dada por `SelectionData.BallId < GameManager1.m_CorrectSphereCount`.

En el caso de querer hacer el numero de esferas totales o correctas variables, se deben de guardar estos datos para poder comparar si la selección fue correcta o no, en su contraparte, puedes tener una lista de IDs correctas por juego, o simplemente agregar al SelectionData un `bool IsCorrect`.

# Para nuevas simulaciones / escenas

En el MainMenu, reemplazar el script de MainMenuController para que MainMenuController.Play() te lleve a una nueva pantalla para seleccionar escenas, y que cada escena se vea en un botón llamando a una función que contenga `SceneManager.LoadScene()`

# Documento del primer protocolo

https://docs.google.com/document/d/10HOAzAywZyG0yRHxRzOqdhTd_pTXo766YOnigzCaKoY/edit?tab=t.ffqe9h5cz87u