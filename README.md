# Whisp, A real-time virtual assistant  
Whisp was built for speed, a lot of its development time has gone into making things run quickly, this means that responses may be quirky, or not 100% accurate, this is intentional, to a degree, obviously it would be amazing if we were all running military grade hardware with 0 compromises, alas this is not the case for most of us.

## So, what is it, and why  
Inspired by the AI-Vtuber [Neuro-sama](https://en.wikipedia.org/wiki/Neuro-sama) and home assistants like [Alexa](https://en.wikipedia.org/wiki/Amazon_Alexa), Whisp is an AI-Assistant, powered mainly by a LLM.

I've always had issues with Alexa, and even modern day versions powered by their own AI models, and that's that they don't go that one step beyond, playing music and doing basic arithmetic is great and useful, but they can't start programs for me, they can't integrate with systems that we build, they can't do anything outside of their programming, and being closed source, means they never will, this is where Whisp shines.

Alexa and the likes will always be one dimensional home assistants that will do the tasks you tell them to, robot style, Whisp can be given a personality, tailored for whoever wants to deploy it.

## So what can it do?  
I have many many plans, but the things that currently work are:  
- Almost real-time, 2–3s responses from you finishing talking, to the program responding  
- Music requests, will play music if you ask it to, and stop the music if asked to as well  
- A basic timer  
- Program starting, will launch a program that you have as a shortcut (.lnk) on your desktop  

## About that speed thing  
The thing that's kept me up for countless nights, the speed — real-time communication is important, no one wants to talk to someone who only replies on weekends once a fortnight, so how do we achieve this?  
- Step one will be the LLM we choose, in this case I've went with "Llama 3 8B Instruct (Q5_K_M)", it's a solid balance of accuracy and reply speed. I've tried others, and they all have their shortfalls, either being incredibly slow, or incredibly dumb, I feel like this is the best middle ground we have at the moment  
- Step two will be the Speech to Text model, Whisper is a popular one and most will go to it, but from my experience, it falls short in many places, it's insanely accurate at times, and other times, it's slow. VOSK has been a good middle ground between the two, right now we use en-us-0.22, which is on the slower end, but there's nothing more frustrating than saying "whisper" and getting "the respect" back  
- Step three, text to speech, this is honestly a no-brainer decision, Kokoro sounds amazing, generates in often times less than a second, even on long outputs, and is just all-round great to work with. Communication between these two uses ZMQ, websockets are slow, and why reinvent the wheel by adding our own implementation of what already exists  

And there you have it, these 3 are going to be what turns this from that old friend you only keep on your friends list out of nostalgia, and something that actually replies to you in a reasonable time.

## Why not use AI-Agents, are you dumb?  
Yes, but that's not why we don't use agents. Finding a middle ground between something that will respond to you like a friend (or an enemy if that's your thing) and something that will just run functions is the whole basis of Whisp. From my experience with agents, you get a program that's really good at doing the thing you tell it to do, but something that's dryer than Death Valley.

## So, how do I make this thing work?  
Out of the box, you'll need a few things:  
- The VOSK model, as I said before, we use en-us-0.22 which can be found at [alphacephei](https://alphacephei.com/vosk/)  
- The LLM, this can be found on [huggingface](https://huggingface.co/QuantFactory/Meta-Llama-3-8B-Instruct-GGUF/blob/main/Meta-Llama-3-8B-Instruct.Q5_K_M.gguf)  
- Kokoro! Installation instructions can be found on [huggingface](https://huggingface.co/hexgrad/Kokoro-82M)  
- The CUDA toolkit, Whisp is designed to use CUDA, and without it, the whole speed thing is a pipedream. Install it from [developer.nvidia](https://developer.nvidia.com/cuda-downloads). Remember to add `CUDA_HOME`, `CUDA_PATH`, and `CUDA_PATH_V(your cuda version)` to your Environment Variables  

You will also need to generate yourself an `apps.json` file, this is used to allow Whisp to start programs for you. `scan.py` will generate one from your desktop shortcuts.

Then it's as simple as just starting both C# Whisp, and the WhispTTS.py programs.

## Why not just allow the LLM to have access to every program on your PC?  
This should go without saying, but when you ask for it to start Discord and it starts the program for uninstalling it, then come back to me.

## So, why not use OpenAI stuff? That would solve the issue of Agents + Natural language, no?  
Yep! But then it wouldn't be free, it would be at the mercy of a multi-billion dollar company that could pull your little friend from their servers at any point, and it wouldn't work offline — not great, right?

## Why not do everything in Python, that's where all the AI stuff is?  
Good point.

## Footnote things  
This entire project started from someone with absolute 0 experience in anything AI outside of shouting at GPT to find something for me. My practices and choices may not be industry standard, but they're mine, they work, and that's more than can be said for most "industry standard" programs.
