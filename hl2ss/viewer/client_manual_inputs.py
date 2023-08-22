#------------------------------------------------------------------------------
# This script receives spatial input data from the HoloLens, which comprises:
# 1) Head pose, 2) Eye ray, 3) Hand tracking, and prints it. 30 Hz sample rate.
# Press esc to stop.
#------------------------------------------------------------------------------

from pynput import keyboard
import socket
import hl2ss
import time

# Settings --------------------------------------------------------------------

host_unity, port_unity = "127.0.0.1", 25001
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

# HoloLens address
host = "192.168.10.44"
# host = "10.10.113.95"x

# Port
port = hl2ss.StreamPort.SPATIAL_INPUT


sock.settimeout(5.0)  # Set a timeout of 5 seconds for the connection attempt
retry_attempts = 3
retry_delay = 5  # Retry after 5 seconds

#------------------------------------------------------------------------------

enable = True
data = "Position=[ 1, 1,  1], Position=[ 1, 1,  1]"



def on_press(key):
    global data, enable

    # Check if the key is '5' and update movement data accordingly
    if key == keyboard.KeyCode.from_char('5'):
        data = "Position=[ 5, 0,  0], Position=[ 1, 1,  1],"

    
    if key == keyboard.KeyCode.from_char('2'):
        data = "Position=[ 2, 2,  2], Position=[ 1, 1,  1]"
    
    # Check if the key is 'esc' and set enable to False to quit the program
    if key == keyboard.Key.esc:
        enable = False
        return False  # Stop the listener

    return True

listener = keyboard.Listener(on_press=on_press)
listener.start()

#client = hl2ss.rx_si(host, port, hl2ss.ChunkSize.SPATIAL_INPUT)
#client.open()

while retry_attempts > 0:
    try:
        # Connect to the server and send the data
        sock.connect((host_unity, port_unity))
        break  # Connection successful, exit the loop

    except socket.timeout:
        print("Connection to Unity timed out. Retrying...")
    except ConnectionRefusedError:
        print("Connection to Unity refused. Retrying...")
    
    retry_attempts -= 1
    time.sleep(retry_delay)

if retry_attempts == 0:
    print("Failed to connect after multiple attempts. Make sure Unity is running and accepting connections.")
else:
    try:
    
        while (enable):
            sock.sendall(data.encode("utf-8"))
            print("data sent")
            response = sock.recv(1024).decode("utf-8")
            print("received")

    except socket.timeout:
        print("Connection to Unity timed out. Make sure Unity is running and accepting connections.")
    except ConnectionRefusedError:
        print("Connection to Unity refused. Make sure Unity is running and accepting connections.")

    finally:
        sock.close()
    #client.close()

listener.stop()