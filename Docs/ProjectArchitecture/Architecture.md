# Architecture for the Gesture Escape Game

### Table of Contents

1. [Folder Architecture](#1-folder-architecture)
2. [Code Architecture](#2-code-architecture)
3. [Unity Scene Architecture](#3-unity-scene-architecture)

## 1. [Folder Architecture](#table-of-contents)

<details open>
<summary>Assets</summary>
<ul>
  <details open>
  <summary>MainFiles</summary>
  To better separate our team's files from external packages that get automatically added on Assets.
  <br>Contents:
  <ul>
    <li>
      <details>
      <summary>Agent</summary>
      For the agent's behavior and visuals.
      <br>Contents:
      <ul>
        <li>Animations</li>
        <li>
          <details>
          <summary>Art</summary>
          <ul>
            <li>Materials</li>
            <li>Models</li>
            <li>Textures</li>
          </ul>
          </details>
        </li>
        <li>
          <details>
          <summary>Audio</summary>
          <ul>
            <li>Music</li>
            <li>Sfx</li>
          </ul>
          </details>
        </li>
        <li>Prefabs</li>
        <li>ScriptableObjects</li>
        <li>Scripts</li>
        <li>Shaders</li>
      </ul>
      </details>
    </li>
    <li>
      <details>
      <summary>Gameplay</summary>
      For the gameplay elements and environment.
      <br>Contents:
      <ul>
        <li>Animations</li>
        <li>
          <details>
          <summary>Art</summary>
          <ul>
            <li>
              <details>
              <summary>Materials</summary>
              <ul>
                <li>GameWorld</li>
                <li>UI</li>
              </ul>
            </li>
            <li>Models</li>
            <li>Textures</li>
            <li>UI</li>
          </ul>
          </details>
        </li>
        <li>
          <details>
          <summary>Audio</summary>
            <ul>
              <li>Music</li>
              <li>Sfx</li>
            </ul>
          </details>
        </li>
        <li>Meshes</li>
        <li>Prefabs</li>
        <li>ScriptableObjects</li>
        <li>Scripts</li>
        <li>Shaders</li>
      </ul>
      </details>
    </li>
    <li>
      <details>
      <summary>RoomMirroring</summary>
      For managing the player and room mirroring system.
      <br>Contents:
      <ul>
        <li>
          <details>
          <summary>Art</summary>
          <ul>
            <li>Materials</li>
            <li>Models</li>
            <li>Skybox</li>
            <li>Textures</li>
            <li>UI</li>
          </ul>
          </details>
        </li>
        <li>Meshes</li>
        <li>Prefabs</li>
        <li>Profiles</li>
        <li>Scans</li>
        <li>ScriptableObjects</li>
        <li>Scripts</li>
        <li>Shaders</li>
      </ul>
      </details>
    </li>
    <li>
      <details>
      <summary>Scenes</summary>
      For the project's scenes.
      <br>Contents:
      <ul>
        <li>AgentBehavior</li>
        <li>FullIntegration</li>
        <li>GameEnvironment</li>
      </ul>
      </details>
    </li>
    <li>
      <details>
      <summary>SensorHub</summary>
      For sending and receiving input from external tools (cameras, HoloLens 2).
      <br>Contents:
      <ul>
        <li>Prefabs</li>
        <li>Scripts</li>
      </ul>
      </details>
    </li>
    <li>
      <details>
      <summary>SpareAssets</summary>
      Assets from other games which might be useful to reuse.
      <br>Contents:
      <ul>
        <li>
          <details>
          <summary>Art</summary>
          <ul>
            <li>Fonts</li>
            <li>
              <details>
              <summary>Materials</summary>
              <ul>
                <li>FPSEnemies</li>
                <li>FPSLevel</li>
                <li>FPSVFX</li>
                <li>FPSWeapons</li>
                <li>ModBillboard</li>
                <li>ModCustom</li>
                <li>ModHat</li>
                <li>ModTotems</li>
                <li>ModTrail</li>
              </ul>
            </li>
            <li>
            <details>
              <summary>Models</summary>
              <ul>
                <li>FPS</li>
                <li>Mg3DFood</li>
                <li>ModAssets</li>
              </ul>
            </li>
            <li>Textures</li>
          </ul>
          </details>
        </li>
        <li>
          <details>
          <summary>Audio</summary>
          <ul>
            <li>Music</li>
            <li>Sfx</li>
          </ul>
          </details>
        </li>
        <li>
          <details>
            <summary>Prefabs</summary>
            <ul>
              <li>ModAssets</li>
              <li>VFX</li>
            </ul>
            </details>
        </li>
        <li>Scripts</li>
      </ul>
      </details>
    </li>
  </ul>
  </details>
</ul>
</details>

## 2. [Code Architecture](#table-of-contents)

![Image reading "TBA"](CodeArchitecture.png)

## 3. [Unity Scene Architecture](#table-of-contents)

![Image reading "TBA"](UnitySceneArchitecture.png)
