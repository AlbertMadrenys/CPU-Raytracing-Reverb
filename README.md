# CPU-Raytracing-Reverb
CPU Raytracing Reverb is a Unity prototype for a tool that dynamically adapts
reverb parameters based on the environment. It uses Unity Raycasts to gather
data on the environment’s geometry and materials, allowing spatialized audio
to respond to these changes. or more details on the acoustic principles and
their implementation using ray tracing, check out the [documentation](./docs/Adaptive_Reverberation_in_Unity_Games-Albert_Madrenys.pdf).

This project uses Unity version 2022.3.12

The executable can be found in [itch.io](https://albertmadrenys.itch.io/cpu-raytracing-reverb).

## License

The original code is under the [MIT License](./LICENSE).

The code from the Synthesis ToolKit has its own [STK License](./LICENSE-STK).

STK:
https://github.com/thestk/stk

STK reverb port to Unity:
https://github.com/keijiro/unity-audio-filters

