# Sonasphere

##[Demo Video](https://www.youtube.com/watch?v=79BfGVj-3uE)
[![Demo Video](http://i.imgur.com/nTN1dlt.png)](https://www.youtube.com/watch?v=79BfGVj-3uE)

### Inspiration

We really believe that VR is going to revolutionize our world and building an app for the Oculus Rift is a great way to both broaden our skill set and participate in the Virtual Reality community. In addition we both wanted a new way to enjoy our music listening sessions.

### What it does

Sonasphere process any audio file, analyzes and isolates key elements in the music to represent visually in an immersive VR landscape.

### How we built it

We separated the audio into over 2000 frequency buckets, performed real-time processing on them. We then created Heuristics which allowed us to differentiate between major sections in the music (Bass, Vocals, Treble, and Snare). Finally we mapped these to meaningful objects within our VR scene using Unity. Using Unity's flexible export features we were able to port this to by viewed in the Oculus Rift and Google Cardboard.

### Challenges we ran into
 - Limitations of Unity for setting up the scene and manipulating graphical properties on the fly.
 - Had to manually fine tune the heuristics due to time constraints.
 - Unity crashing.
 - Prioritizing qualitative tasks for a very open ended project.

### Accomplishments that we're proud of
 - Building a quality experience in VR without prior background in the field.
 - Learning more about the science of sound and music.
 - Sharing the experience with others and making people smile.

### What we learned
 - Musical harmonics and common frequency ranges for different sounds.
 - How to manipulate particles in Unity.
 - How to integrate the Oculus Rift and Google Cardboard into Unity.

### What's next for Sonaphere
 - Pre-processing on the audio file to find large scale pattern changes in the music for the purpose of modifying the overall ambiance of the visualization.
 - Leverage machine learning to replace manual heuristics and to automate the constant tweaking we had to perform.
