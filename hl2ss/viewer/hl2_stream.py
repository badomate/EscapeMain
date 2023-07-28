#------------------------------------------------------------------------------
# This script receives spatial input data from the HoloLens, which comprises:
# 1) Head pose, 2) Hand tracking, with finger keyponints and prints it. 30 Hz sample rate.
# Press esc to stop.
#------------------------------------------------------------------------------

from pynput import keyboard
import socket
import hl2ss
import json
import argparse
import subprocess


# Settings --------------------------------------------------------------------

HOST_UNITY, PORT_UNITY = "127.0.0.1", 25001
HOST, PORT = "192.168.10.44", hl2ss.StreamPort.SPATIAL_INPUT

#------------------------------------------------------------------------------

enable = True

all_elements = vars(hl2ss.SI_HandJointKind)
data_dict = {}


def on_press(key):
    global enable
    enable = key != keyboard.Key.esc
    return enable


def arg_parser():
    # Use argparse to handle command-line arguments
    parser = argparse.ArgumentParser(description="HL2 Data Streaming Script")
    parser.add_argument(
        "--fake",
        action="store_true",
        help="Send fake data instead of connecting to real HL2."
    )
    args = parser.parse_args()
    return args

def send_data_to_unity(data, client):
    data_json = json.dumps(data)
    client.sendall(data_json.encode("utf-8"))
    response = client.recv(1024).decode("utf-8")
    print(response)


def fake_data(client, HOST, PORT, HOST_UNITY, PORT_UNITY):
    print("Sending fake data")
    print("HOST: ", HOST)
    print("PORT: ", PORT)
    client.connect( (HOST, PORT) )
    json_decoder = json.JSONDecoder()

    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as client_unity:
        client_unity.connect( (HOST_UNITY, PORT_UNITY) )

        while(enable):
            data = client.recv(65536).decode("utf-8")
            client.sendall("Received".encode("utf-8"))

            received_data = json.loads(data)
            print(f'Tracking status at time {received_data["timestamp"]}')

            send_data_to_unity(received_data, client_unity)


def hand_position(hand, data_dict):
    for name, value in all_elements.items():
        if isinstance(value,int) and 0 <= value <= 25:
            pose = hand.get_joint_pose(all_elements[name])
            print(f'Left {name} pose: Position={pose.position} Orientation={pose.orientation} Radius={pose.radius} Accuracy={pose.accuracy}')
            data_dict[f'Left-{name}'] = [pose.position, pose.orientation, pose.radius]

def hl2_data(HOST, PORT, HOST_UNITY, PORT_UNITY):
    print("Sending real data")
    print("HOST: ", HOST)
    print("PORT: ", PORT)
    
    client = hl2ss.rx_si(HOST, PORT, hl2ss.ChunkSize.SPATIAL_INPUT)
    client.open()

    connect_unity(HOST_UNITY, PORT_UNITY, client)

    while(enable):
        received_data = client.get_next_packet()
        si = hl2ss.unpack_si(data.payload)

        if (si.is_valid_head_pose()):
            head_pose = si.get_head_pose()
            print(f'Head pose: Position={head_pose.position} Forward={head_pose.forward} Up={head_pose.up}')
            # right = cross(up, -forward)
            # up => y, forward => -z, right => x
            data_dict['Head'] = [head_pose.position, head_pose.forward, head_pose.up]
        else:
            print('No head pose data')

        # # See
        # https://learn.microsoft.com/en-us/uwp/api/windows.perception.people.jointpose?view=winrt-22621
        # for hand data details
        if (si.is_valid_hand_left()):
            hand_left = si.get_hand_left()
            hand_position(hand_left, data_dict)
            # for name, value in all_elements.items():
            #     if isinstance(value,int) and 0 <= value <= 25:
            #         pose = hand_left.get_joint_pose(all_elements[name])
            #         print(f'Left {name} pose: Position={pose.position} Orientation={pose.orientation} Radius={pose.radius} Accuracy={pose.accuracy}')
            #         data_dict[f'Left-{name}'] = [pose.position, pose.orientation, pose.radius]
        else:
            print('No left hand data')

        if (si.is_valid_hand_right()):
            hand_right = si.get_hand_right()
            hand_position(hand_right, data_dict)
            # for name, value in all_elements.items():
            #     if isinstance(value,int) and 0 <= value <= 25:
            #         pose = hand_right.get_joint_pose(all_elements[name])
            #         print(f'Right {name} pose: Position={pose.position} Orientation={pose.orientation} Radius={pose.radius} Accuracy={pose.accuracy}')
            #         data_dict[f'Right-{name}'] = [pose.position, pose.orientation, pose.radius]
        else:
            print('No right hand data')


        send_data_to_unity(data_dict)

def main(send_fake_data=False):
    
    with  socket.socket(socket.AF_INET, socket.SOCK_STREAM) as client:
        listener = keyboard.Listener(on_press=on_press)
        listener.start()

        if send_fake_data:
            fake_data(client, "127.0.0.1", PORT, HOST_UNITY, PORT_UNITY)
        else:
            hl2_data(HOST, PORT, HOST_UNITY, PORT_UNITY)

        listener.join()


if __name__ == "__main__":
    args = arg_parser()
    main(send_fake_data=args.fake)
   

