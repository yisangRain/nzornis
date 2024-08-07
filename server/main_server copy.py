from flask import Flask, request, jsonify
import os
from database_init import initialise_database

app = Flask(__name__)

# Directory to save the uploaded videos
UPLOAD_FOLDER = 'uploads'
if not os.path.exists(UPLOAD_FOLDER):
    os.makedirs(UPLOAD_FOLDER)

@app.route('/upload', methods=['POST'])
def upload_video():
    if 'video' not in request.files:
        return jsonify({"error": "No video file part in the request"}), 400

    video_file = request.files['video']

    if video_file.filename == '':
        return jsonify({"error": "No selected file"}), 400

    # Save the file
    save_path = os.path.join(UPLOAD_FOLDER, video_file.filename)
    video_file.save(save_path)

    return jsonify({"message": "Video uploaded successfully", "path": save_path}), 200

if __name__ == '__main__':
    initialise_database("dev.db")
    app.run(debug=True)
