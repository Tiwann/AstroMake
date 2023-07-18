#include "Vector.h"

vec2::vec2(float x, float y) : x(x), y(y)
{
}

vec2::vec2() : x(0.0f), y(0.0f)
{
}

vec2::vec2(float n) : x(n), y(n)
{
}

vec3::vec3(float x, float y, float z) : x(x), y(y), z(z)
{
}

vec3::vec3() : x(0.0f), y(0.0f), z(0.0f)
{
}

vec3::vec3(float n): x(n), y(n), z(n)
{
}

vec4::vec4(float x, float y, float z, float w) : x(x), y(y), z(z), w(w)
{
}

vec4::vec4() : x(0), y(0), z(0), w(0)
{
}

vec4::vec4(float n) : x(n), y(n), z(n), w(n)
{
}

