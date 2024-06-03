import cv2
import numpy as np
from vidgear.gears import WriteGear
from vidgear.gears.stabilizer import Stabilizer
import time

def grabCutter(frame, rect):
    """
    Segments given image based on the given bounding box
    """

    mask = np.zeros(frame.shape[:2], np.uint8)

    bgdModel = np.zeros((1, 65), np.float64)
    fgdModel = np.zeros((1, 65), np.float64)

    cv2.grabCut(frame, mask, rect, bgdModel, fgdModel, 10, cv2.GC_INIT_WITH_RECT)
    mask2 = np.where((mask == 2) | (mask == 0), 0, 1).astype('uint8')

    img_seg = np.copy(frame)
    img_seg[(mask2 == 0)] = [0, 255, 0]

    return img_seg


def grabcut(source, output_name):
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

    box = (10, 10, 480, 480) 

    i = 0

    while (True):
        success, frame = capture.read()
        if (success == False):
            print("all frames gone")
            break
        if (i > 120): #temp limiter
            break
        processed = grabCutter(frame, box)

        writer.write(processed)
        
        i+= 1

    capture.release()
    writer.close()
    end = time.time()
    print(f"finished. Elapsed time: {end - start}")
    return 0



def to_AR(filename, conn, id):
    """
    Wrapper function for conversion
    """
    input_name = "server/received/" + filename
    output_name = "server/converted/" + filename

    c = conn.cursor()
    query = None

    result = grabcut(input_name, output_name)

    if result == 0:
        query = f"UPDATE AR SET filename = '{output_name}', status='converted' WHERE ar_id = {id};"

    elif result == 1:
        query = f"UPDATE AR SET filename = 'None', status='File Error' WHERE ar_id = {id};"

    c.execute(query)
    conn.commit()
    




 



