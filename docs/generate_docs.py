import os
import os.path
import subprocess
import argparse
import shutil
from datetime import date
import sys

def run(*args, **kwargs):
    error_code = subprocess.call(*args, **kwargs)
    if error_code != 0: # an error happened
        sys.exit(error_code)

parser = argparse.ArgumentParser(description='Runs the c++ client doc generation script.')
parser.add_argument('game', action='store', help='the name of the game you want to document. Must exist in ../games/')

args = parser.parse_args()

game_name = args.game[0].upper() + args.game[1:]
lower_game_name = game_name[0].lower() + game_name[1:]


with open("./Doxyfile", "r") as f:
    base_doxyfile = f.read()

new_doxyfile = base_doxyfile.replace("$$$GAME_NAME$$$", game_name)

temp_path = "./temp"
if os.path.isdir(temp_path):
    shutil.rmtree(temp_path)
os.makedirs(temp_path)

with open(os.path.join(temp_path, "Doxyfile"), "w+") as f:
    f.write(new_doxyfile)

game_path = os.path.join("..", "Games", game_name)
only_files = [f for f in os.listdir(game_path) if os.path.isfile(os.path.join(game_path, f))]

for name in only_files:
    shutil.copyfile(os.path.join(game_path, name), os.path.join(temp_path, name))


shutil.copyfile(os.path.join("..", "Joueur", "BaseGameObject.cs"), os.path.join(temp_path, "BaseGameObject.cs"))

with open("../README.md", "r") as f:
    readme = f.read()

readme = readme.replace("GAME_NAME", game_name).replace("game_name", lower_game_name)

with open(os.path.join(temp_path, "README.md"), "w+") as f:
    f.write(readme)

#shutil.copyfile(os.path.join("..", "README.md"), os.path.join(temp_path, "README.md"))

run(["doxygen"], shell=True, cwd=temp_path)

output_path = "./output"
if os.path.isdir(output_path):
    shutil.rmtree(output_path)
shutil.copytree(os.path.join(temp_path, "docs", "html"), output_path)

shutil.rmtree(temp_path)
