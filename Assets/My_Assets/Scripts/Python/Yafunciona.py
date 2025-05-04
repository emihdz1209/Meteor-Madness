# Made by Wavesense
# This program has the capabilities to save data directly from the Unicorn Hybrid Black to a csv file
# It also can calculate band power in real time using a sliding window
# The data obtained is preprocessed using digital filters

from pylsl import StreamInlet, resolve_streams
import pandas as pd
import numpy as np
from scipy.signal import butter, bessel, filtfilt, welch
import keyboard
import socket #para socket
import struct #para socket
import time #para socket
import random #para simular la de la mandibula

#configuración de IP y puerto (misma que unity)
HOST = "10.5.0.2"
PORT = 50505


def compute_psd_single_window(data, fs, window_length):
    # Computes PSD using the Welch method for a single window of data.
    freqs, psd = welch(data, fs, nperseg=window_length)
    return freqs, psd


def bandpower(psd, freqs, band):
    # Calculates band power for a specific frequency band.
    band_idx = np.logical_and(freqs >= band[0], freqs <= band[1])
    return np.trapz(psd[band_idx], freqs[band_idx])


def apply_notch_filter(data, fs):
    # Function to apply Notch filter
    notch_freq = 60.0  # Notch frequency
    bandwidth = 1.0  # Adjust this as needed
    b, a = butter(2, [(notch_freq - bandwidth) / (0.5 * fs), (notch_freq + bandwidth) / (0.5 * fs)], btype='bandstop', analog=False)
    padlen = max(3 * max(len(a), len(b)), 3)

    # Check if the length of data is sufficient for padding
    if len(data) > padlen:
        return filtfilt(b, a, data, padlen=padlen)
    else:
        print("Input data length is insufficient for padding. Skipping filtering.")
        return data


def apply_lowpass_filter(data, fs, cutoff_freq):
    # Function to apply Low Pass filter
    nyquist = 0.5 * fs  # Nyquist frequency
    normalized_cutoff = cutoff_freq / nyquist  # Normalize the cutoff frequency
    b, a = butter(2, normalized_cutoff, btype='low', analog=False)  # 2nd-order Butterworth filter
    padlen = max(3 * max(len(a), len(b)), 3)  # Calculate padding length

    # Check if the length of data is sufficient for padding
    if len(data) > padlen:
        return filtfilt(b, a, data, padlen=padlen)
    else:
        print("Input data length is insufficient for padding. Skipping filtering.")
        return data


def apply_highpass_filter(data, fs, cutoff_freq):
    # Function to apply High Pass filter
    nyquist = 0.5 * fs  # Nyquist frequency
    normalized_cutoff = cutoff_freq / nyquist  # Normalize the cutoff frequency
    b, a = bessel(5, normalized_cutoff, btype='high', analog=False)  # 2nd-order Butterworth high-pass filter
    padlen = max(3 * max(len(a), len(b)), 3)  # Calculate padding length

    # Check if the length of data is sufficient for padding
    if len(data) > padlen:
        return filtfilt(b, a, data, padlen=padlen)
    else:
        print("Input data length is insufficient for padding. Skipping filtering.")
        return data


""" def recorder(time, value):
    # Function to start saving data and processing it
    data_temp = dict((k, []) for k in columns)

    window_size = int(500)
    step_size = int(25)

    while True:
        # Get the streamed data
        data, timestamp = inlet.pull_sample()
        all_data = [timestamp] + data

        # Updating data dictionary with newly transmitted samples
        data_temp['Time'].append(timestamp)
        data_temp['Val'].append(value)
        for i, val in enumerate(all_data[1:]):
            data_temp[columns[i + 2]].append(val)

        # Data collection stops after 5 seconds (adjustable)
        if len(total_data['Time']) >= 250 * time:
            break

        if len(data_temp['Time']) >= window_size:

            delta_power = 0
            theta_power = 0
            alpha_power = 0
            beta_power = 0
            gamma_power = 0

            data_dict = dict((k, []) for k in columns)
            for key in data_temp:
                data_dict[key].extend(data_temp[key])

            for i, channel in enumerate(columns[2:10]):
                # data_dict[channel] = apply_notch_filter(np.array(data_dict[channel]), fs=250)
                # data_dict[channel] = apply_lowpass_filter(np.array(data_dict[channel]), fs=250, cutoff_freq=50)
                # data_dict[channel] = apply_highpass_filter(np.array(data_dict[channel]), fs=250, cutoff_freq=0.5)

                freqs, psd = compute_psd_single_window(data_dict[channel], fs=250, window_length=window_size)

                # Calculate power for each EEG band
                delta_power += bandpower(psd, freqs, (0.5, 4))
                theta_power += bandpower(psd, freqs, (4, 8))
                alpha_power += bandpower(psd, freqs, (8, 12))
                beta_power += bandpower(psd, freqs, (12, 30))
                gamma_power += bandpower(psd, freqs, (30, 50))

            delta_power = delta_power / 8
            theta_power = theta_power / 8
            alpha_power = alpha_power / 8
            beta_power = beta_power / 8
            gamma_power = gamma_power / 8

            concentration_index = beta_power / (theta_power + alpha_power) if (theta_power + alpha_power) > 0 else 0
            concentration_index_normal = concentration_index*(100)/2.25

            # Print or store the resultss
            print(f"Concentration Index: {concentration_index_normal}")

            if len(total_data['Time']) == 0:
                for key in data_dict:
                    total_data[key].extend(data_dict[key])
            else:
                for key in data_dict:
                    total_data[key].extend(data_dict[key][-step_size:])  # Append only last step_size elements

            for key in data_temp:
                data_temp[key] = data_temp[key][step_size:]
 """                

with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
    s.bind((HOST, PORT))
    s.listen(1)
    print(f"Esperando conexión en {HOST}:{PORT}...")

    conn, addr = s.accept()
    with conn:
        print(" Conectado con {addr}")

        try:
            while True:
                focus = random.randint(1, 100)   #concentration_index_normal float
                jaw = random.randint(0, 10)         # int (0 o 1)

                data = struct.pack("fi", focus, jaw)  # float (4 bytes) + int (4 bytes)
                conn.sendall(data)

                print(f"Enviado: focus={focus:.2f}, jaw={jaw}")
                time.sleep(0.5)
        except KeyboardInterrupt:
            print("Detenido.")

try:
    # Initialize the streaming layer
    streams = resolve_stream()
    if not streams:
        raise RuntimeError("No streams found.")
    else:
        print("Found streams:", streams)
    inlet = StreamInlet(streams[0])

    # Initialize the columns of your data and your dictionary to capture the data
    columns = ['Time', 'Val', 'FZ', 'C3', 'CZ', 'C4', 'PZ', 'PO7', 'OZ', 'PO8', 'AccX', 'AccY', 'AccZ',
               'Gyro1', 'Gyro2', 'Gyro3', 'Battery', 'Counter', 'Validation']
    total_data = {k: [] for k in columns}

    while True:
        if keyboard.is_pressed('s'):
            print("Starting 5-second data recording...")
            recorder(120, 1)  # Record data for the amount selected in seconds
        elif keyboard.is_pressed('esc'):
            print("Stopping and saving data...")
            data_df = pd.DataFrame.from_dict(total_data)
            data_df.to_csv('EEG021.csv', index=False)
            print("Data saved to 'EEGdataTest4.csv'")
            break

except Exception as e:
    print("An error occurred:", e)
