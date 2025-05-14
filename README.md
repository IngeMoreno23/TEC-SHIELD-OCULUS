
# Instalar Proyecto

Para añadir un proyecto y trabajar en el, recomendaría clonar el repositorio de GitHub, y abrir la carpeta donde se clonó desde Unity Hub

![Imagen](<./Docs/Images/Pasted image 20250513153128.png>)

Como referencia, el proyecto actual inició en la ***editor version*** **6000.0.33f1** 

La instalación del editor debe contar con los siguientes módulos para **buildear en Android**

- Visual Studio (opcional)
- Windows Build Support (opcional)
- Android Build Support
	- OpenJDK
	- Android SDK & NDK Tools

![Imagen](<./Docs/Images/Pasted image 20250513153338.png>)
![Imagen](<./Docs/Images/Pasted image 20250513153400.png>)

Para descargar el proyecto, clona el repositorio siguiente en alguna carpeta vacía, o descárgalo como zip si no vas a usar git inmediatamente.

Después de clonarlo, lo añadirás a los proyectos de unity hub en **Add project from disk**

![Imagen](<./Docs/Images/Pasted image 20250513155103.png>)

Verás el proyecto abajo en la lista de proyectos, al abrirlo se instalarán los archivos locales faltantes.

# Correr el proyecto sin Oculus Conectado

La paquetería XR Interaction Toolkit te provee un dispositivo virtual de un visor que puede ser controlado con la escena de unity, con teclado y ratón.

La ruta del prefab es: `Assets/Samples/XR Interaction Toolkit/3.0.8/XR Devise Simulator`. Dentro se encuentra un asset que puedes arrastrar a la jerarquía, no es necesario configurarlo.

Al iniciar el Play Mode se usará el dispositivo virtual para controlar el XR Origin.

**Es muy importante que al buildear, el XR Devise Simulator no se encuentre en la escena, de lo contrario, el Juego no responderá a las entradas del Oculus**.

# Buildear en Oculus Standalone

**Es muy importante que al buildear, el XR Devise Simulator no se encuentre en la escena, de lo contrario, el Juego no responderá a las entradas del Oculus**

Primero, entra a Build Profiles:

![Imagen](<./Docs/Images/Pasted image 20250514130816.png>)

Asegúrate que estas buildeando para Android.

En Platform Settings > Run Device, asegúrate de tener conectado por cable el Oculus, y te aparecerá un identificador del Oculus al momento de abrir la lista desplegable. Lo seleccionas:

![Imagen](<./Docs/Images/Pasted image 20250514130902.png>)

Al tenerlo conectado, sal y da clic en Build And Run.

![Imagen](<./Docs/Images/Pasted image 20250514131457.png>)

**Por alguna razón, creo que no es posible buildear sin esta opción. Creo que solo con Build And Run lo descarga en el dispositivo seleccionado en Build Profiles**.