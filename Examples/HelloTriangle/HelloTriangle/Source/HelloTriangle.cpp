#include <iostream>
#include <glad/gl.h>
#include <GLFW/glfw3.h>

#include "Vertex.h"

int main()
{
    std::cout << "Hello, Triangle!\n";

    glfwInit();
    glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);
    glfwWindowHint(GLFW_RESIZABLE, GLFW_FALSE);
    glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 4);
    glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 6);

    GLFWwindow* window = glfwCreateWindow(800, 600, "Hello Triangle", nullptr, nullptr);
    glfwSwapInterval(1);
    if (!window)
    {
        std::cerr << "Failed to create window!\n";
        return 0;
    }

    glfwMakeContextCurrent(window);
    gladLoadGL(glfwGetProcAddress);

    const vertex vertices[3] = {
        {{-0.5f, -0.5f, 0.0f}, {0.0f, 0.0f}, {1.0f, 1.0f, 1.0f}, {1.0f, 0.0f, 0.0f, 1.0f}},
        {{-0.0f, +0.5f, 0.0f}, {0.0f, 0.0f}, {1.0f, 1.0f, 1.0f}, {0.0f, 1.0f, 0.0f, 1.0f}},
        {{+0.5f, -0.5f, 0.0f}, {0.0f, 0.0f}, {1.0f, 1.0f, 1.0f}, {0.0f, 0.0f, 1.0f, 1.0f}}
    };

    const unsigned int indices[3] = {0, 1, 2};

    unsigned int vao, vbo, ibo;

    glCreateVertexArrays(1, &vao);
    glBindVertexArray(vao);

    glCreateBuffers(1, &vbo);
    glBindBuffer(GL_ARRAY_BUFFER, vbo);
    glBufferData(GL_ARRAY_BUFFER, sizeof vertices, vertices, GL_STATIC_DRAW);

    glCreateBuffers(1, &ibo);
    glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, ibo);
    glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof indices, indices, GL_STATIC_DRAW);


    glEnableVertexAttribArray(0);
    glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, sizeof(vertex), (const void*)offsetof(vertex, position));

    glEnableVertexAttribArray(1);
    glVertexAttribPointer(1, 2, GL_FLOAT, GL_FALSE, sizeof(vertex), (const void*)offsetof(vertex, texcoord));

    glEnableVertexAttribArray(2);
    glVertexAttribPointer(2, 3, GL_FLOAT, GL_FALSE, sizeof(vertex), (const void*)offsetof(vertex, normal));

    glEnableVertexAttribArray(3);
    glVertexAttribPointer(3, 4, GL_FLOAT, GL_FALSE, sizeof(vertex), (const void*)offsetof(vertex, color));


    const char* vertSource = "#version 460 core\n"
                             "\n"
                             "layout(location = 0) in vec3 vposition;\n"
                             "layout(location = 1) in vec2 vtexcoord;\n"
                             "layout(location = 2) in vec3 vnormal;\n"
                             "layout(location = 3) in vec4 vcolor;\n"
                             "\n"
                             "out vec4 color;\n"
                             "\n"
                             "void main()\n"
                             "{\n"
                             "  gl_Position = vec4(vposition, 1.0);\n"
                             "  color = vcolor;\n"
                             "}";

    const char* fragSource = "#version 460 core\n"
                             "\n"
                             "in vec4 color;"
                             "\n"
                             "out vec4 fcolor;\n"
                             "uniform float time;\n"
                             "\n"
                             "void main()\n"
                             "{\n"
                             "  fcolor = color;\n"
                             "}";
    
    unsigned int vshader = glCreateShader(GL_VERTEX_SHADER);
    glShaderSource(vshader, 1, &vertSource, nullptr);

    unsigned int fshader = glCreateShader(GL_FRAGMENT_SHADER);
    glShaderSource(fshader, 1, &fragSource, nullptr);

    unsigned int shaderProgram = glCreateProgram();
    glAttachShader(shaderProgram, vshader);
    glAttachShader(shaderProgram, fshader);
    

    glCompileShader(vshader);
    int compiled = 0;
    glGetShaderiv(vshader, GL_COMPILE_STATUS, &compiled);
    if(!compiled)
    {
        char message[1024];
        int length;
        glGetShaderInfoLog(vshader, 1024, &length, message);
        std::cerr << message << "\n";
        return 0;
    }

    
    glCompileShader(fshader);
    glGetShaderiv(fshader, GL_COMPILE_STATUS, &compiled);
    if(!compiled)
    {
        char message[1024];
        int length;
        glGetShaderInfoLog(fshader, 1024, &length, message);
        std::cerr << message << "\n";
        return 0;
    }

    glLinkProgram(shaderProgram);
    glGetProgramiv(shaderProgram, GL_LINK_STATUS, &compiled);
    if(!compiled)
    {
        char message[1024];
        int length;
        glGetProgramInfoLog(shaderProgram, 1024, &length, message);
        std::cerr << message << "\n";
        return 0;
    }
    
    glValidateProgram(shaderProgram);
    glGetProgramiv(shaderProgram, GL_VALIDATE_STATUS, &compiled);
    if(!compiled)
    {
        char message[1024];
        int length;
        glGetProgramInfoLog(shaderProgram, 1024, &length, message);
        std::cerr << message << "\n";
        return 0;
    }
    
    while (!glfwWindowShouldClose(window))
    {
        glfwPollEvents();
        glUseProgram(shaderProgram);
        glDrawElements(GL_TRIANGLES, 3, GL_UNSIGNED_INT, nullptr);
        glfwSwapBuffers(window);
    }

    glfwTerminate();
    return 0;
}
