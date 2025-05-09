import zmq
from kokoro import KPipeline
import sounddevice as sd

pipeline = KPipeline(lang_code='a')

context = zmq.Context()
socket = context.socket(zmq.PULL)
socket.bind("tcp://*:5555")       # Listen on port 5555

print("TTS Server ready...")

while True:
    message = socket.recv_string()
    print(f"Received request: {message}")
    generator = pipeline(message, voice='af_heart')
    for i, (gs, ps, audio) in enumerate(generator):
        sd.play(audio, samplerate=24000)
        sd.wait()
