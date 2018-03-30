#version 330 core

in vec2 position;
out vec4 color;
uniform vec3 gradientStart;
uniform vec3 gradientEnd;
uniform mat4 modelView;

void main()
{
	gl_Position = modelView * vec4(position.x, position.y, 1, 1);
	float t = gl_Position.x;
	color = vec4(
		gradientStart.x + t * (gradientEnd.x - gradientStart.x),
		gradientStart.y + t * (gradientEnd.y - gradientStart.y),
		gradientStart.z + t * (gradientEnd.z - gradientStart.z),
		1
	);
}