import cv2
import numpy as np
import time

imgs = ["blackswan1.jpg", "kea1.jpg", "kea2.jpg", "kea3.jpg", "kereru1.jpg", "kereru2.jpg", "kereru3.jpg",
        "seagull1.jpg", "seagull2.jpg", "seagull3.jpg"]

for f in imgs:
    img_path = "images/" + f

    start = time.time()

    img = cv2.imread(img_path)

    h = img.shape[0]
    w = img.shape[1]

    box = np.array([10, 10, w - 10, h - 10])

    mask = np.zeros(img.shape[:2], np.uint8)

    bgdModel = np.zeros((1, 65), np.float64)
    fgdModel = np.zeros((1, 65), np.float64)

    cv2.grabCut(img, mask, box, bgdModel, fgdModel, 10, cv2.GC_INIT_WITH_RECT)
    mask2 = np.where((mask == 2) | (mask == 0), 0, 1).astype('uint8')

    img_seg = np.copy(img)
    img_seg[(mask2 == 0)] = [0, 255, 0]

    cv2.imwrite("GrabCut/iter10/" + f, img_seg)
    print(time.time() - start)
