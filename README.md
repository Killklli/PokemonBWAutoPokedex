# Pokemon Black and White Auto Tracker

This is a pokemon tracker for randomizers for pokemon black running DeSmuME.

## Usage

1. Build and compile the Solution from the git repo to generate the exe
2. Download the latest release of Desmume: http://desmume.org/download/
3. Download the Lua DLL that matches your Desmume: - 
https://sourceforge.net/projects/luabinaries/files/5.1.5/Windows%20Libraries/Dynamic/
    - lua-5.1.5_Win32_dll14_lib.zip for x86 Desmume
    - lua-5.1.5_Win64_dll14_lib.zip for x86-64 Desmume

4. Extract lua5.1.dll from the .zip file to the same folder where your DeSmuME_0.9.11_x86.exe or DeSmuME_0.9.11_x64.exe is

5. Rename lua5.1.dll to lua51.dll
6. In Desmume, open Tools > Lua Scripting > New Lua Script Window and select the included lua script in the same output folder as the exe you made in step 1.
7. Make sure the script is running
8. Start the Exe you made in Step 1.