import random
import socket
import json
import hl2ss
import threading

# Todo - create predefined fake data for testing moving hand left to right up and down
# hl2 compare datas
def generate_fake_head_pose():
    position = [random.uniform(-1, 1), random.uniform(-1, 1), random.uniform(-1, 1)]
    forward = [random.uniform(-1, 1), random.uniform(-1, 1), random.uniform(-1, 1)]
    up = [random.uniform(-1, 1), random.uniform(-1, 1), random.uniform(-1, 1)]
    return {"Head" :{"Position": position, "Forward": forward, "Up": up}}

def generate_fake_joint_data(joint_name):
    position = [random.uniform(-1, 1), random.uniform(-1, 1), random.uniform(-1, 1)]
    orientation = [random.uniform(-1, 1), random.uniform(-1, 1), random.uniform(-1, 1), random.uniform(-1, 1)]
    radius = [random.uniform(0, 0.1)]
    accuracy = [random.randint(0, 10)]
    return {joint_name: {"Position": position, "Orientation": orientation, "Radius": radius, "Accuracy": accuracy}}

# Generate fake data for all joints
def generate_fake_data():
    fake_data = {}

    #Generate timestamp
    fake_data.update({"timestamp":1})
    # Generate Head pose data
    fake_data.update(generate_fake_head_pose())

    # List of joint names
    joint_names = [
        "LeftPalm", "LeftWrist", "LeftThumbMetacarpal", "LeftThumbProximal", "LeftThumbDistal", "LeftThumbTip",
        "LeftIndexMetacarpal", "LeftIndexProximal", "LeftIndexIntermediate", "LeftIndexDistal", "LeftIndexTip",
        "LeftMiddleMetacarpal", "LeftMiddleProximal", "LeftMiddleIntermediate", "LeftMiddleDistal", "LeftMiddleTip",
        "LeftRingMetacarpal", "LeftRingProximal", "LeftRingIntermediate", "LeftRingDistal", "LeftRingTip",
        "LeftLittleMetacarpal", "LeftLittleProximal", "LeftLittleIntermediate", "LeftLittleDistal", "LeftLittleTip",
        "RightPalm", "RightWrist", "RightThumbMetacarpal", "RightThumbProximal", "RightThumbDistal", "RightThumbTip",
        "RightIndexMetacarpal", "RightIndexProximal", "RightIndexIntermediate", "RightIndexDistal", "RightIndexTip",
        "RightMiddleMetacarpal", "RightMiddleProximal", "RightMiddleIntermediate", "RightMiddleDistal", "RightMiddleTip"
    ]

    # Generate data for each joint
    for joint_name in joint_names:
        fake_data.update(generate_fake_joint_data(joint_name))

    return fake_data

def send_fake_data(connection):
    try:
        while True:
            fake_data = generate_fake_data()
            data_json = json.dumps(fake_data)
            connection.sendall(data_json.encode("utf-8"))
            connection.recv(1024).decode("utf-8")
    except Exception as e:
        print(f"Error sending fake data: {e}")
    finally:
        connection.close()

def start_server(host, port):
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as server_socket:
        server_socket.bind((host, port))
        server_socket.listen(1)
        print(f"Server started. Listening on {host}:{port}...")

        try:
            while True:
                connection, addr = server_socket.accept()
                print(f"Connected to {addr}")
                # Start a new thread to handle the connection
                send_fake_data(connection)
        except KeyboardInterrupt:
            print("Server stopped.")
        finally:
            server_socket.close()



# Example usage
if __name__ == "__main__":
    # Example usage with starting the server to listen for incoming connections
    HOST, PORT = "127.0.0.1", hl2ss.StreamPort.SPATIAL_INPUT
    start_server(HOST, PORT)