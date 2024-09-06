from segment_anything import SamPredictor, sam_model_registry
import cv2
import numpy as np
import matplotlib as plt
import torch
import time

start = time.time()

DEVICE = torch.device('cuda:0' if torch.cuda.is_available() else 'cpu')
MODEL_TYPE = "vit_h"

def show_output(result_dict,axes=None):
     if axes:
        ax = axes
     else:
        ax = plt.gca()
        ax.set_autoscale_on(False)
     sorted_result = sorted(result_dict, key=(lambda x: x['area']),      reverse=True)
     # Plot for each segment area
     for val in sorted_result:
        mask = val['segmentation']
        img = np.ones((mask.shape[0], mask.shape[1], 3))
        color_mask = np.random.random((1, 3)).tolist()[0]
        for i in range(3):
            img[:,:,i] = color_mask[i]
            ax.imshow(np.dstack((img, mask*0.5)))

sam = sam_model_registry["default"](checkpoint="SAM/sam_vit_h_4b8939.pth")

mask_predictor = SamPredictor(sam)

img_path = "images/seagull3.jpg"

img = cv2.imread(img_path)

img_rgb = cv2.cvtColor(img, cv2.COLOR_BGR2RGB)

print("img conv done")

mask_predictor.set_image(img_rgb)
h = img.shape[0]
w = img.shape[1]
box = np.array([10,10, w-10,h-10])
masks, scores, logits = mask_predictor.predict(box=box, multimask_output=True)

r, g, b = masks
mask2 = np.where((r == True) | (g == True) | (b == True), 0, 1).astype('uint8')

img_seg = np.copy(img)
img_seg[(mask2 == 0)] = [0, 255, 0]

print("mask created")

cv2.imwrite("SAM/seagull3.jpg", img_seg)
print(time.time() - start)
