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

host_unity, port_unity = "127.0.0.1", 25001
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

# HoloLens address
host = "192.168.10.44"

# Port
port = hl2ss.StreamPort.SPATIAL_INPUT

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

def main(send_fake_data=False):
    print("0")
    listener = keyboard.Listener(on_press=on_press)
    listener.start()
    print("00")
    if not send_fake_data:
        client = hl2ss.rx_si(host, port, hl2ss.ChunkSize.SPATIAL_INPUT)
        client.open()
    else:
        client = sock.connect(("127.0.0.1", port))


    try:
        # Connect to the server and send the data
        if not send_fake_data:
            sock.connect((host_unity, port_unity))
        print("2")
        
        # tracking...
        while (enable):
            if not send_fake_data:
                data = client.get_next_packet()
                si = hl2ss.unpack_si(data.payload)
            else:
                 data = client.recv(1024).decode("utf-8")
            
            print("22")
            if not data:
                break
            # Parse the received JSON data
            received_data = json.loads(data)
            print(received_data)
            break
            print("3")
            print("/////////////////////")
            print(f'Tracking status at time {data.timestamp}')

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
                for name, value in all_elements.items():
                    if isinstance(value,int) and 0 <= value <= 25:
                        pose = hand_left.get_joint_pose(all_elements[name])
                        print(f'Left {name} pose: Position={pose.position} Orientation={pose.orientation} Radius={pose.radius} Accuracy={pose.accuracy}')
                        data_dict[f'Left-{name}'] = [pose.position, pose.orientation, pose.radius]
            else:
                print('No left hand data')

            if (si.is_valid_hand_right()):
                hand_right = si.get_hand_right()
                for name, value in all_elements.items():
                    if isinstance(value,int) and 0 <= value <= 25:
                        pose = hand_right.get_joint_pose(all_elements[name])
                        print(f'Right {name} pose: Position={pose.position} Orientation={pose.orientation} Radius={pose.radius} Accuracy={pose.accuracy}')
                        data_dict[f'Right-{name}'] = [pose.position, pose.orientation, pose.radius]
            else:
                print('No right hand data')
            
            # convert the pose.position ndarray into string
            # data_socket = str(pose.position)
            data_json = json.dump(data_dict)
            sock.sendall(data_json.encode("utf-8"))
            response = sock.recv(1024).decode("utf-8")
            print(response)
    except Exception as e:
        print(f"The error is: {e}")
    finally:

        sock.close()
    if not send_fake_data:
        client.close()
    listener.join()


if __name__ == "__main__":
    args = arg_parser()
    main(send_fake_data=args.fake)
   

