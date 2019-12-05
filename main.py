import cv2

all_sk_points = []


def calc_video_frame_size(path):
    # Path to video file
    vidObj = cv2.VideoCapture(path)
    success, image = vidObj.read()

    if success:
        width, height, layer = image.shape
        image_size = (width, height)

        return image_size
    else:
        return None


def read_data(path):
    with open(path, "r") as f:
        for line in f:
            sk_points_str = line.split(" ")
            sk_points = []
            for points in sk_points_str:
                tmp_pts = points.split(",")
                if len(tmp_pts)==2:
                    x = int(tmp_pts[0])
                    y = int(tmp_pts[1])
                    sk_points.append([x,y])
            all_sk_points.append(sk_points)


def process_video(path):
    image_size = calc_video_frame_size(path)

    # Path to video file
    vidObj = cv2.VideoCapture(path)

    out = cv2.VideoWriter('project.mp4', cv2.VideoWriter_fourcc(*'mp4v'), 10, (image_size[1], image_size[0]))

    curr_time = 0

    # checks whether frames were extracted
    success = 1

    while success:
        # vidObj object calls read
        # function extract frames
        success, image = vidObj.read()

        if image_size==None:
            width, height, layer = image.shape
            image_size = (width, height)

        for points in all_sk_points[curr_time]:
            image = cv2.circle(image, (points[0], points[1]), 5, (0, 0, 255), -1)

        curr_time += 1

        out.write(image)

    print("{} frames processed".format(curr_time))
    out.release()


read_data("pose_demo_3_out.txt")

process_video("pose_demo_3.mp4")

