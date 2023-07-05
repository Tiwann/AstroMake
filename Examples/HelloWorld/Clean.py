import os
import platform


def pause():
	input("Press any key to quit...")


if platform.system() == "Windows":
	print("Cleaning...")
	success = os.system("AstroMake /clean")
	if success != 0:
		print("An Error occurred")
	pause()
