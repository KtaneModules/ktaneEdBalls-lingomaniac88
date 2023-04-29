thickness = 0.003;
frame_size = [0.072864, 0.097152, thickness];

difference() {
    cube(frame_size + [thickness, thickness, 0], center=true);
    cube(frame_size - [thickness, thickness, -1], center=true);
}