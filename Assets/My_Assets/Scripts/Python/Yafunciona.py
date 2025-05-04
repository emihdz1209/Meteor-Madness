from pylsl import StreamInlet, resolve_streams
import pandas as pd
import numpy as np
from scipy.signal import butter, bessel, filtfilt, welch
import threading
import socket
import struct
import time

# configuración de IP y puerto (misma que Unity)
HOST = "10.5.0.2"
PORT = 50505

# Inicializar LSL
streams = resolve_streams()
if not streams:
    raise RuntimeError("No LSL streams found.")
inlet = StreamInlet(streams[0])

# Columnas y almacenamiento de datos
columns = ['Time', 'Val', 'FZ', 'C3', 'CZ', 'C4', 'PZ', 'PO7', 'OZ', 'PO8',
           'AccX', 'AccY', 'AccZ', 'Gyro1', 'Gyro2', 'Gyro3', 'Battery', 'Counter', 'Validation']
total_data = {k: [] for k in columns}

latest_focus = 0.0


def compute_psd_single_window(data, fs, window_length):
    freqs, psd = welch(data, fs, nperseg=window_length)
    return freqs, psd


def bandpower(psd, freqs, band):
    mask = (freqs >= band[0]) & (freqs <= band[1])
    return np.trapz(psd[mask], freqs[mask])


def apply_notch_filter(data, fs):
    notch_freq = 60.0
    bandwidth = 1.0
    b, a = butter(2,
                 [(notch_freq - bandwidth) / (0.5 * fs),
                  (notch_freq + bandwidth) / (0.5 * fs)],
                 btype='bandstop')
    padlen = max(3 * max(len(a), len(b)), 3)
    if len(data) > padlen:
        return filtfilt(b, a, data, padlen=padlen)
    return data


def apply_lowpass_filter(data, fs, cutoff_freq):
    nyquist = 0.5 * fs
    norm_cut = cutoff_freq / nyquist
    b, a = butter(2, norm_cut, btype='low')
    padlen = max(3 * max(len(a), len(b)), 3)
    if len(data) > padlen:
        return filtfilt(b, a, data, padlen=padlen)
    return data


def apply_highpass_filter(data, fs, cutoff_freq):
    nyquist = 0.5 * fs
    norm_cut = cutoff_freq / nyquist
    b, a = bessel(5, norm_cut, btype='high')
    padlen = max(3 * max(len(a), len(b)), 3)
    if len(data) > padlen:
        return filtfilt(b, a, data, padlen=padlen)
    return data


def recorder(duration_s, value):
    global latest_focus
    data_temp = {k: [] for k in columns}
    window_size, step_size = 500, 25

    while len(total_data['Time']) < 250 * duration_s:
        sample, timestamp = inlet.pull_sample()
        data_temp['Time'].append(timestamp)
        data_temp['Val'].append(value)
        for i, v in enumerate(sample):
            data_temp[columns[i + 2]].append(v)

        if len(data_temp['Time']) >= window_size:
            dp = tp = ap = bp = gp = 0
            for ch in columns[2:10]:
                freqs, psd = compute_psd_single_window(
                    np.array(data_temp[ch]), fs=250, window_length=window_size)
                dp += bandpower(psd, freqs, (0.5, 4))
                tp += bandpower(psd, freqs, (4, 8))
                ap += bandpower(psd, freqs, (8, 12))
                bp += bandpower(psd, freqs, (12, 30))
                gp += bandpower(psd, freqs, (30, 50))

            # Promedio por canal
            dp /= 8; tp /= 8; ap /= 8; bp /= 8; gp /= 8

            concentration_index = bp / (tp + ap) if (tp + ap) > 0 else 0
            concentration_index_normal = concentration_index * 100 / 2.25

            latest_focus = concentration_index_normal

            # Desplazar ventana
            for k in columns:
                total_data[k].extend(data_temp[k][-step_size:])
            for k in data_temp:
                data_temp[k] = data_temp[k][step_size:]


def save_data():
    df = pd.DataFrame.from_dict(total_data)
    df.to_csv('EEG021.csv', index=False)
    print("Data saved to 'EEG021.csv'")


def start_socket():
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
        s.bind((HOST, PORT))
        s.listen(1)
        print(f"Esperando conexión en {HOST}:{PORT}...")
        conn, addr = s.accept()
        print(f"Conectado con {addr}")

        # Iniciar grabación en background apenas conecte
        threading.Thread(target=recorder, args=(120, 1), daemon=True).start()

        try:
            while True:
                jaw = 0
                focus = latest_focus
                data = struct.pack("fi", focus, jaw)
                conn.sendall(data)
                print(f"Enviado: focus={focus:.2f}, jaw={jaw}")
                time.sleep(0.5)
        except KeyboardInterrupt:
            print("Detenido.")


if __name__ == "__main__":
    try:
        start_socket()
    except Exception as e:
        print("An error occurred:", e)
        save_data()
