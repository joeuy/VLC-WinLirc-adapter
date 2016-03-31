# VLC-WinLirc-adapter
Relay WinLirc commands to VLC. This works for Windows with [.NET framework 4.5](https://www.microsoft.com/es-es/download/details.aspx?id=30653) (installed by default on win8.1+)

## Usage
1. Enable web interface on VLC.  Tools -> Preferences -> Show all -> Interfaces -> Main Interfaces -> Check Web
2. Compile sources or [download precompiled](https://github.com/joeuy/VLC-WinLirc-adapter/raw/0.1/Release.zip)
2. Complete config.txt
3. Complete commands.txt
4. Run LircVlc.exe 
5. Enjoy

## config.txt
two simple lines: 

```
lirc 127.0.0.1:8765
vlc http://127.0.0.1:8080/requests/status.xml?command={0}
```

lirc: address to find lirc server (example is default), this setting is available under Server Options on WinLirc
vlc: address where VLC is listening for web commands (example is default)

## commands.txt

specifies each of the relayed commands

```
remote_name button vlccommand
```
 
remote_name: is the remote name specified in WinLirc.
button: is the remote button name specified in WinLirc.
vlccommand: is the command to send to VLC player.

For available commands look at example commands.txt file or under "C:\Program Files (x86)\VideoLAN\VLC\lua\http\requests\README.txt"