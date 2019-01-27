#version 330 core

uniform vec3 gradientStart;
uniform vec3 gradientEnd;

in vec4 vertex;
out vec4 color;

void main()
{
	float t = vertex.x;
    color = vec4(
		gradientStart.x + t * (gradientEnd.x - gradientStart.x),
	    gradientStart.y + t * (gradientEnd.y - gradientStart.y),
		gradientStart.z + t * (gradientEnd.z - gradientStart.z),
		1
	);
}