import cv2
import numpy as np
import mediapipe as mp
from mediapipe.tasks import python as p
from mediapipe.tasks.python import vision
import time

selfieSeg = mp.solutions.selfie_segmentation
segment = selfieSeg.SelfieSegmentation(model_selection = 0)

model_path = "MediaPipe/deeplab_v3.tflite"

BG_COLOR = (0, 255, 0) # green

def selfieModel(img):
    result = segment.process(cv2.cvtColor(img, cv2.COLOR_BGR2RGB))

    condition = np.stack((result.segmentation_mask,) * 3, axis = -1) > 0.1

    bg_image = np.zeros(img.shape, dtype=np.uint8)
    bg_image[:] = BG_COLOR
    output_image = np.where(condition, img, bg_image)

    return output_image


def deeplab(img):
# Create the options that will be used for ImageSegmenter
    base_options = p.BaseOptions(model_asset_buffer = open(model_path, "rb").read())
    options = vision.ImageSegmenterOptions(base_options=base_options,
                                        output_category_mask=True)
    
    # Create the image segmenter
    with vision.ImageSegmenter.create_from_options(options) as segmenter:

        # Create the MediaPipe image file that will be segmented
        image = mp.Image.create_from_file("images/" + img)

        # Retrieve the masks for the segmented image
        segmentation_result = segmenter.segment(image)
        category_mask = segmentation_result.category_mask

        # Generate solid color images for showing the output segmentation mask.
        image_data = image.numpy_view()
        bg_image = np.zeros(image_data.shape, dtype=np.uint8)
        bg_image[:] = BG_COLOR

        im_mat = cv2.imread("images/" +img)

        condition = np.stack((category_mask.numpy_view(),) * 3, axis=-1) > 0.1
        output_image = np.where(condition, im_mat, bg_image)

        cv2.imwrite("MediaPipe/deeplab/iter1/" + img, output_image)



imgs = ["blackswan1.jpg", "kea1.jpg", "kea2.jpg", "kea3.jpg", "kereru1.jpg", "kereru2.jpg", "kereru3.jpg",
        "seagull1.jpg", "seagull2.jpg", "seagull3.jpg"]

def selfieIter():
    for img in imgs:
        start = time.time()
        output = selfieModel(cv2.imread("images/" + img))
        cv2.imwrite("MediaPipe/selfie/" + img, output)
        print(time.time() - start)

def deeplabIter():
    times = []
    for img in imgs:
        start = time.time()
        deeplab(img)
        times.append(time.time() - start)
    
    for t in times:
        print(t)


deeplabIter()
