import cv2
import numpy as np
from vidgear.gears import WriteGear
from vidgear.gears.stabilizer import Stabilizer
import time
import database

def grabCutImage(frame, rect):
    """
    Segments given image based on the given bounding box
    """

    mask = np.zeros(frame.shape[:2], np.uint8)

    bgdModel = np.zeros((1, 65), np.float64)
    fgdModel = np.zeros((1, 65), np.float64)

    cv2.grabCut(frame, mask, rect, bgdModel, fgdModel, 4, cv2.GC_INIT_WITH_RECT)
    mask2 = np.where((mask == 2) | (mask == 0), 0, 1).astype('uint8')

    img_seg = np.copy(frame)
    img_seg[(mask2 == 0)] = [0, 255, 0]

    return img_seg


def img_to_AR(conn, id):
    """
    Wrapper function for cutting an image file for AR 
    """
    s = database.Sighting()
    filename = s.get_by_id(conn, database.Entity.SIGHTING, id)[0][-1]
    input_name = "server/received/" + filename
    output_name = "server/converted/" + filename

    box = (10, 10, 480, 480) 
    img = cv2.imread(input_name)

    processed = grabCutImage(img, box)
    cv2.imwrite(output_name, processed)

    c = database.Cell()
    c.update_status(conn, id, database.ConversionStatus.READY)



def vid_to_AR(conn, id):
    """
    Wrapper function for conversion
    """
    s = database.Sighting()
    filename = s.get_by_id(conn, database.Entity.SIGHTING, id)[0][-1]
    input_name = "server/received/" + filename
    output_name = "server/converted/" + filename

    c = database.Cell()

    result = grabcutProcessor(input_name, output_name)

    if result == 0:
        c.update_status(conn, id, database.ConversionStatus.READY)

    elif result == 1:
        raise ValueError('Error converting file')
    


def grabcutProcessor(source, output_name):

    start = time.time()

    # note: avc1 is h264
    output_params = {"-fps": 30, '-fourcc': 'H264'}
   
    capture = cv2.VideoCapture(source)
    writer = WriteGear(output = output_name, compression_mode=False, logging=True, **output_params)

    #check if the source is ready
    if not capture.isOpened():
        print("Source cannot be opened. Please check. Exiting...")
        return 1

    #extract initial frame
    success, frame = capture.read()
    #Check if frame extraction was successful. Exit if fail
    if not success:
        print("Could not extract the first frame. Exiting...")
        return 1

    i = 0
    h, w , _= frame.shape
    box=(50, 50, w - 50, h - 50)

    while (True):
        success, frame = capture.read()
        print(i)
        if (success == False):
            print("all frames gone")
            break
        # if (i > 10): #temp limiter
        #     break
    
        processed = grabCutImage(frame, box)

        writer.write(processed)
        
        i+= 1

    capture.release()
    writer.close()
    end = time.time()
    print(f"finished. Elapsed time: {end - start}")
    return 0

start = time.time()
grabcutProcessor("ConversionTrials/crane.mp4","crane_grabcut.mp4")
print(time.time() - start)
