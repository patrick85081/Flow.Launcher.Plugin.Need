# Flow.Launcher.Plugin.Need
=========================

![img](Flow.Launcher.Plugin.Need\icon.png)

這是 [Wox Need](http://www.wox.one/plugin/87) 的移植 for [Flow launcher](https://github.com/Flow-Launcher/Flow.Launcher)

Get and set infos

![img](http://api.wox.one/media/plugin/BEA0FDFC6D3B4085823A60DC76F28844/Wox.Need-f2e93717-2434-4838-91e3-ce8618c3c4ed.gif)

### How To Use [Need]

| What             | How                             |
| ---------------- | ------------------------------- |
| Get to clipboard | `need Key`                    |
| Save             | `need KeyWithoutSpaces Value` |
| Delete           | `need delete Key`             |
| Open file        | `need open`                   |
| Reload file      | `need reload`                 |

* For example :

| feature              | do                                |
| -------------------- | --------------------------------- |
| Save server_ip first | need server_ip 127.0.0.1          |
| need server_ip       | 127.0.0.1 is now in the clipboard |
| finally delete it    | need delete server_ip             |
