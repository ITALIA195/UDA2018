#version 460 core

layout(location = 0) in vec2 position;
uniform mat4 view;

out vec4 vertex;

void main()
{
    vertex = view * vec4(position, 1.0, 1.0);
    gl_Position = vertex;
}