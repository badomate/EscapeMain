#------------------------------------------------------------------------------
# This script receives spatial input data from the HoloLens, which comprises:
# 1) Head pose, 2) Hand tracking, with finger keyponints and prints it. 30 Hz sample rate.
# Press esc to stop.
#------------------------------------------------------------------------------

from pynput import keyboard
import socket
import hl2ss
import json

# Settings --------------------------------------------------------------------

host_unity, port_unity = "127.0.0.1", 25001
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

# HoloLens address
host = "192.168.10.44"

# Port
port = hl2ss.StreamPort.SPATIAL_INPUT

#------------------------------------------------------------------------------

enable = True

def on_press(key):
    global enable
    enable = key != keyboard.Key.esc
    return enable

listener = keyboard.Listener(on_press=on_press)
listener.start()

client = hl2ss.rx_si(host, port, hl2ss.ChunkSize.SPATIAL_INPUT)
client.open()
all_elements = vars(hl2ss.SI_HandJointKind)


data_dict = {}

try:
    # Connect to the server and send the data
    sock.connect((host_unity, port_unity))
    
    
    # tracking...
    while (enable):
     

        data = client.get_next_packet()
        si = hl2ss.unpack_si(data.payload)
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
client.close()
listener.join()