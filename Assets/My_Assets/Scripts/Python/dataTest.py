import socket
import struct
import time
import random

# Configura IP y puerto (debe coincidir con Unity)
HOST = "10.43.46.33"
PORT = 50505

with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
    s.bind((HOST, PORT))
    s.listen(1)
    print(f"Esperando conexión en {HOST}:{PORT}...")

    conn, addr = s.accept()
    with conn:
        print(f"✅ Conectado con {addr}")

        try:
            while True:
                focus = random.uniform(0, 100)     # float
                jaw = random.randint(0, 10)         # int (0 o 1)

                data = struct.pack("fi", focus, jaw)  # float (4 bytes) + int (4 bytes)
                conn.sendall(data)

                print(f"Enviado: focus={focus:.2f}, jaw={jaw}")
                time.sleep(0.5)
        except KeyboardInterrupt:
            print("Detenido.")