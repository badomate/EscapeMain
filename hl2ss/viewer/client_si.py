#------------------------------------------------------------------------------
# This script receives spatial input data from the HoloLens, which comprises:
# 1) Head pose, 2) Eye ray, 3) Hand tracking, and prints it. 30 Hz sample rate.
# Press esc to stop.
#------------------------------------------------------------------------------

from pynput import keyboard
import socket
import hl2ss

# Settings --------------------------------------------------------------------

host_unity, port_unity = "127.0.0.1", 25001
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

# HoloLens address
host = "192.168.10.44"
# host = "10.10.113.95"x

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

# data = "1,2,3"

try:
#     # Connect to the server and send the data
    sock.connect((host_unity, port_unity))
    
# tracking...
    while (enable):
     

        data = client.get_next_packet()
        si = hl2ss.unpack_si(data.payload)

        print(f'Tracking status at time {data.timestamp}')

        # if (si.is_valid_head_pose()):
        #     head_pose = si.get_head_pose()
        #     print(f'Head pose: Position={head_pose.position} Forward={head_pose.forward} Up={head_pose.up}')
        #     # right = cross(up, -forward)
        #     # up => y, forward => -z, right => x
        # else:
        #     print('No head pose data')

        # if (si.is_valid_eye_ray()):
        #     eye_ray = si.get_eye_ray()
        #     print(f'Eye ray: Origin={eye_ray.origin} Direction={eye_ray.direction}')
        # else:
        #     print('No eye tracking data')

        # # See
        # https://learn.microsoft.com/en-us/uwp/api/windows.perception.people.jointpose?view=winrt-22621
        # for hand data details

        if (si.is_valid_hand_left()):
            hand_left = si.get_hand_left()
            # print(f"left hand state: {hand_left}")
            print("---")
            pose = hand_left.get_joint_pose(hl2ss.SI_HandJointKind.Wrist)
            print(f'Left wrist pose: Position={pose.position} Orientation={pose.orientation} Radius={pose.radius} Accuracy={pose.accuracy}')
            #convert the pose.position ndarray into string
            data_socket = str(pose.position)
            print(data_socket)
            sock.sendall(data_socket.encode("utf-8"))
            response = sock.recv(1024).decode("utf-8")
            print(response)

            # print(f'proximal {hand_left.get_joint_pose(hl2ss.SI_HandJointKind.ThumbProximal).position}')
            # print(f'distal {hand_left.get_joint_pose(hl2ss.SI_HandJointKind.ThumbDistal).position}')
            # print(f'tip {hand_left.get_joint_pose(hl2ss.SI_HandJointKind.ThumbTip).position}')
        else:
            print('No left hand data')

        # if (si.is_valid_hand_right()):
        #     hand_right = si.get_hand_right()
        #     pose = hand_right.get_joint_pose(hl2ss.SI_HandJointKind.Wrist)
        #     print(f'Right wrist pose: Position={pose.position} Orientation={pose.orientation} Radius={pose.radius} Accuracy={pose.accuracy}')
        # else:
        #     print('No right hand data')
finally:
    sock.close()
client.close()
listener.join()
