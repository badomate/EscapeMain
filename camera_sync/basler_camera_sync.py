from pypylon import pylon
import numpy as np
import cv2
from imageio import get_writer
from threading import Thread
import queue

MAX_NUMBER_CAMERAS = 2
cv2.namedWindow('Acquisition', cv2.WINDOW_NORMAL)
cv2.resizeWindow('Acquisition', 1280, 512)

def image_saver_thread(image_queue):
    while True:
        try:
            image, filename = image_queue.get(timeout=1)
            cv2.imwrite(filename, image)
        except queue.Empty:
            pass

tlFactory = pylon.TlFactory.GetInstance()
devices = tlFactory.EnumerateDevices()
if len(devices) == 0:
    raise pylon.RUNTIME_EXCEPTION("No camera present.")


cameras = pylon.InstantCameraArray((min(len(devices), MAX_NUMBER_CAMERAS)))

for i, camera in enumerate(cameras):
    camera.Attach(tlFactory.CreateDevice(devices[i]))
 
cameras.Open()

for i, camera in enumerate(cameras):
    camera.TriggerSelector.SetValue("FrameStart")
    camera.TriggerMode.SetValue("On")
    camera.TriggerSource.SetValue("Line1")
    camera.PixelFormat.SetValue("RGB8")
    camera.StreamGrabber.MaxTransferSize = 4194304

# Starts grabbing for all cameras
cameras.StartGrabbing(pylon.GrabStrategy_LatestImageOnly,
                      pylon.GrabLoop_ProvidedByUser)

# Create a queue for image saving
image_queue = queue.Queue()


# Create and start the image saver thread
saver_thread = Thread(target=image_saver_thread, args=(image_queue,))
saver_thread.daemon = True  # The thread will exit when the main program exits
saver_thread.start()

while cameras.IsGrabbing():
    grabResult1 = cameras[0].RetrieveResult(5000,
                         pylon.TimeoutHandling_ThrowException)

    grabResult2 = cameras[1].RetrieveResult(5000,
                         pylon.TimeoutHandling_ThrowException)

    # grabResult3 = cameras[2].RetrieveResult(5000,
    #                      pylon.TimeoutHandling_ThrowException)
    if grabResult1.GrabSucceeded():
        im1 = cv2.cvtColor(grabResult1.GetArray(), cv2.COLOR_BGR2RGB)
        im2 = cv2.cvtColor(grabResult2.GetArray(), cv2.COLOR_BGR2RGB)
        # im3 = cv2.cvtColor(grabResult3.GetArray(), cv2.COLOR_BGR2RG

        cameraContextValue = grabResult1.GetCameraContext()
        cameraContextValue2 = grabResult2.GetCameraContext()
        # cameraContextValue3 = grabResult3.GetCameraContext()
        # writer.append_data(im1)

        image_filename1 = f"cam_0/image_{cameraContextValue}_{grabResult1.ImageNumber}.png"
        image_filename2 = f"cam_1/image_{cameraContextValue2}_{grabResult2.ImageNumber}.png"
        # image_filename3 = f"cam_2/image_{cameraContextValue3}_{grabResult3.ImageNumber}.png"

        # cv2.imwrite(image_filename1, im1)
        # cv2.imwrite(image_filename2, im2)


        # image_queue.put((im1, image_filename1))
        # image_queue.put((im2, image_filename2))

        # cv2.imwrite(image_filename3, im3)
 
        # If ESC is pressed exit and destroy window
        cv2.imshow('Acquisition',np.hstack([im1,im2]))

        if cv2.waitKey(1) & 0xFF == 27:
            break

cv2.destroyAllWindows()