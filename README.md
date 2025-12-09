# 🎲 Isometric Roguelite PCG Engine

> Motor de Generación Procedural de Niveles y Diseño para un Shoot 'em Up Isométrico.

![Unity](https://img.shields.io/badge/Unity-2022.3%2B-black?style=flat&logo=unity)
![Language](https://img.shields.io/badge/Language-C%23-blue?style=flat&logo=csharp)
![License](https://img.shields.io/badge/License-MIT-green)

## 📋 Sobre el Proyecto

Este proyecto consiste en un **motor de generación procedural (PCG)** desarrollado en Unity, diseñado para crear niveles infinitos, coherentes y jugables para un videojuego híbrido del género **Roguelite Shoot 'em Up** con perspectiva isométrica.

El sistema no solo genera la topología del mapa, sino que también gestiona la colocación de enemigos, patrones de ataque (Bullet Hell), distribución de ítems y el balanceo dinámico de la dificultad.

## ⚙️ Mecanismos del Motor

El sistema está construido siguiendo un **Marco de Trabajo Ingenieril**, dividido en 6 mecanismos fundamentales:

*   **M01 - Generador de Topología:** Utiliza **Particionamiento Espacial Binario (BSP)** para la estructura macro y **Autómatas Celulares** para dar formas orgánicas a las salas. Conexión mediante grafos (Delaunay/MST).
*   **M02 - Colocador de Enemigos:** Distribuye entidades hostiles basándose en el espacio disponible y el "presupuesto" de dificultad de la sala.
*   **M03 - Generador de Patrones:** Asigna coreografías de proyectiles (*bullet patterns*) específicas para mantener la esencia del género Shmup.
*   **M04 - Balanceador de Dificultad:** Ajusta los parámetros de generación en tiempo real según la progresión del jugador (Dificultad Adaptativa).
*   **M05 - Colocador de Ítems:** Distribuye recursos y power-ups asegurando su accesibilidad.
*   **M06 - Validador de Jugabilidad:** Algoritmo que verifica la conectividad y jugabilidad del nivel antes de mostrarlo (Flood Fill + A*).

## 🛠️ Tecnologías

*   **Motor:** Unity 2022 LTS.
*   **Lenguaje:** C#.
*   **Herramientas:** 2D Tilemap Extras, ScriptableObjects para configuración.

## 🚀 Instalación y Uso

1.  **Clonar el repositorio:**
    ```bash
    git clone https://github.com/TU_USUARIO/TU_REPO.git
    ```
2.  **Abrir en Unity:**
    *   Abre Unity Hub.
    *   Selecciona "Add" y busca la carpeta del proyecto.
    *   Abre con Unity 2022.3 o superior.
3.  **Ejecutar:**
    *   Ve a la carpeta `Assets/Scenes`.
    *   Abre la escena `DemoGenerator`.
    *   Dale a **Play**.
    *   Usa el botón "Regenerate" en la UI para ver diferentes semillas.

## 📐 Arquitectura

El proyecto sigue una arquitectura modular basada en componentes y ScriptableObjects para facilitar el diseño y las pruebas.


## 📄 Licencia

Este proyecto está bajo la Licencia MIT - ver el archivo [LICENSE](LICENSE) para más detalles.

---
Desarrollado como parte del Proyecto de Investigación y Desarrollo - Facultad de Tecnologías Interactivas, UCI.
 
