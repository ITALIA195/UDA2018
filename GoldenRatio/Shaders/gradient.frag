#version 460 core

const float PI = 3.1415926535;

uniform float time;

in vec4 vertex;
out vec4 color;

void main()
{
	float phaseX = vertex.x * vertex.y;
	
	float r = sin(time + phaseX) * 0.5 + 0.5;
	float g = sin(time + phaseX + 2 * (PI / 3)) * 0.5 + 0.5;
	float b = sin(time + phaseX + 4 * (PI / 3)) * 0.5 + 0.5;
	
    color = vec4(r, g, b, 1);
}