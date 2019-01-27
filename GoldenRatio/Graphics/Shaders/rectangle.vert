#version 330 core

layout(location = 0) in vec2 position;
uniform mat4 view;

out vec4 vertex;

void main()
{
    gl_Position = vertex = view * vec4(position, 1.0, 1.0);
}