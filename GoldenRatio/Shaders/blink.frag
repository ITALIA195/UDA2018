#version 460 core

uniform float time;
out vec4 color;

void main()
{
    float alpha = sin(time) * 0.5 + 0.5;
	color = vec4(0.0, 0.0, 1.0, alpha);
}