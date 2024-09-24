import cv2
import numpy as np

def accuracy(folders, imgs):

    for f in folders:
        print(f)
        for i in imgs:
            cimg = cv2.cvtColor(cv2.imread(f + i), cv2.COLOR_BGR2RGB)
            mimg = cv2.cvtColor(cv2.imread(man_path + i), cv2.COLOR_BGR2RGB)

            total = cimg.shape[0] * cimg.shape[1]
            count = 0

            for h in range(cimg.shape[0]):
                for w in range(cimg.shape[1]):
                    if (cimg[h][w] == mimg[h][w]).all():
                        count += 1
                    
            print(count/total)


def man_mask_processor(imgs, man_path):
    # converting all m_green to i_green on manual masking
    count = 0
    for i in imgs:
        path = man_path + i
        img = cv2.imread(path)

        h, w, _ = img.shape

        for row in range(h):
            for col in range(w):
                if (img[row][col] != bgr).all():
                    img[row][col] = [0, 255, 0]
                
        cv2.imwrite("manual/processed/" + i, img)

    print(count)

imgs = ["blackswan1.jpg", "kea1.jpg", "kea2.jpg", "kea3.jpg", "kereru1.jpg", "kereru2.jpg", "kereru3.jpg",
        "seagull1.jpg", "seagull2.jpg", "seagull3.jpg"]

man_path = "manual/processed/"

folders = [ "MediaPipe/deeplab/iter1/"]

# folders = ["SAM/", "GrabCut/iter1/", "GrabCut/iter2/", "GrabCut/iter3/",
#            "GrabCut/iter4/", "GrabCut/iter5/", "GrabCut/iter6/",
#            "GrabCut/iter7/", "GrabCut/iter8/", "GrabCut/iter9/", "GrabCut/iter10/",
#            "MediaPipe/selfie/iter1/", "MediaPipe/deeplab/iter2/"]

m_green = np.array([0, 179,75]) # green used within manual masking
i_green = np.array([0, 255, 0]) # green used within automated masking

bgr = np.array([75, 179, 0]) # m_green (RGB) in BGR format

accuracy(folders=folders, imgs=imgs)
