import cv2
import numpy as np
import mediapipe as mp
from mediapipe.tasks import python as p
from mediapipe.tasks.python import vision
from vidgear.gears import WriteGear
import time
from segment_anything import SamPredictor, sam_model_registry
import torch
import matplotlib as plt

selfieSeg = mp.solutions.selfie_segmentation
segment = selfieSeg.SelfieSegmentation(model_selection = 0)

# DeepLabV3
model_path = "MediaPipe/deeplab_v3.tflite"
BG_COLOR = (0, 255, 0) # green

def deeplab(img):
# Create the options that will be used for ImageSegmenter
    base_options = p.BaseOptions(model_asset_buffer = open(model_path, "rb").read())
    options = vision.ImageSegmenterOptions(base_options=base_options,
                                        output_category_mask=True)
    
    # Create the image segmenter
    with vision.ImageSegmenter.create_from_options(options) as segmenter:

        # Create the MediaPipe image file that will be segmented
        image = mp.Image(image_format=mp.ImageFormat.SRGB, data=img)

        # Retrieve the masks for the segmented image
        segmentation_result = segmenter.segment(image)
        category_mask = segmentation_result.category_mask

        # Generate solid color images for showing the output segmentation mask.
        image_data = image.numpy_view()
        bg_image = np.zeros(image_data.shape, dtype=np.uint8)
        bg_image[:] = BG_COLOR

        condition = np.stack((category_mask.numpy_view(),) * 3, axis=-1) > 0.3
        output_image = np.where(condition, img, bg_image)

        return output_image

def dl_vid(filename):
    start = time.time()
    output_params = {"-fps": 30, '-fourcc': 'H264'}
   
    capture = cv2.VideoCapture(filename)
    writer = WriteGear(output = "dl_trial.mp4", compression_mode=False, logging=True, **output_params)

    while (True):
        success, frame = capture.read()
        if (success == False):
            print("all frames gone")
            break
    
        processed = deeplab(frame)

        writer.write(processed)

    capture.release()
    writer.close()

    print(f"Time: {time.time() - start}")
        


# MP Selfie

def selfie(img):
    result = segment.process(cv2.cvtColor(img, cv2.COLOR_BGR2RGB))

    condition = np.stack((result.segmentation_mask,) * 3, axis = -1) > 0.1

    bg_image = np.zeros(img.shape, dtype=np.uint8)
    bg_image[:] = BG_COLOR
    output_image = np.where(condition, img, bg_image)

    return output_image

def selfie_vid(filename):
    start = time.time()
    output_params = {"-fps": 30, '-fourcc': 'H264'}
   
    capture = cv2.VideoCapture(filename)
    writer = WriteGear(output = "alba.mp4", compression_mode=False, logging=True, **output_params)

    i = 0
    while (True):
        success, frame = capture.read()
        if (success == False):
            print("all frames gone")
            break
    
        processed = selfie(frame)

        writer.write(processed)
        i+=1

    capture.release()
    writer.close()

    print(f"Time: {time.time() - start}")
    print(i)

# SAM

def sam_vid(img, mask_predictor):
    img_rgb = cv2.cvtColor(img, cv2.COLOR_BGR2RGB)
    mask_predictor.set_image(img_rgb)
    h = img.shape[0]
    w = img.shape[1]
    box = np.array([10,10, w-10,h-10])
    masks, scores, logits = mask_predictor.predict(box=box, multimask_output=True)

    r, g, b = masks
    mask2 = np.where((r == True) | (g == True) | (b == True), 0, 1).astype('uint8')

    img_seg = np.copy(img)
    img_seg[(mask2 == 0)] = [0, 255, 0]

    return img_seg


def sam_trial(filename):
    start = time.time()
    DEVICE = torch.device('cuda:0' if torch.cuda.is_available() else 'cpu')
    MODEL_TYPE = "vit_h"

    sam = sam_model_registry["default"](checkpoint="SAM/sam_vit_h_4b8939.pth")
    mask_predictor = SamPredictor(sam)

    output_params = {"-fps": 30, '-fourcc': 'H264'}

    capture = cv2.VideoCapture(filename)
    writer = WriteGear(output = "sam_trial.mp4", compression_mode=False, logging=True, **output_params)
    i = 0
    while (True):
        success, frame = capture.read()
        if (success == False):
            print("all frames gone")
            break
    
        processed = sam_vid(frame, mask_predictor)

        writer.write(processed)
        i += 1
        print(i)

    capture.release()
    writer.close()

    print(f"Time: {time.time() - start}")

# dl_vid("kokako.mp4")
selfie_vid("albatross.mp4")
# sam_trial("kokako.mp4")