import os
import platform


def pause():
	input("Press any key to quit...")


if platform.system() == "Windows":
	print("Building Visual Studio 2022 Solution...")
	success = os.system("AstroMake /build:vstudio")
	if success != 0:
		print("An Error occurred")
	pause()
