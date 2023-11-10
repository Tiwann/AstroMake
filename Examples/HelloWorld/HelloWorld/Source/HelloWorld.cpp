#include <iostream>


int main()
{
#if defined(HELLOWORLD_DEBUG)
    std::cout << "Hello, AstroMake!\n";
#endif
    
    std::cin.get();
    return 0;
}