#version 120

in vec2 position;
in vec3 colorIn;
varying out vec4 color;
uniform float alpha;
uniform mat4 modelView;

void main()
{
    gl_Position = modelView * vec4(position.x, position.y, 1, 1);
    color = vec4(colorIn, alpha);
}