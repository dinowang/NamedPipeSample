# NamedPipe Sample

[Recording](./doc/recording/2024-09-05_21-08-50.mp4)

1. Left side `NamedPipeServer` (C#)
   - Process with 10 worker threads for named pipe server

2. Right side `NamedPipeClient` (C#)
   - Emulate multiple clients (thread) to send and receive message from named pipe server

## Remarkable

1. When server process unexpectedly terminated. All connected clients will not be terminated immediately, but wait server restart and connect again. Then message process continue.

2. In Windows, named pipe can be used for inter-process communication (IPC) between processes faster than TCP/IP. And will not consume local ports.