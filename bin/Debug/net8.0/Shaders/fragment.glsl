#version 460 core

out vec4 FragColor;

uniform vec3 objectColor;
uniform float time;

void main()
{
    float brightness = 0.9 + 0.1 * sin(time * 2.0); // pulsuje miêdzy 0.8 a 1.0
    FragColor = vec4(objectColor * brightness, 1.0);
}
