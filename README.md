# DontRuinMyAccount - Sinmai防毁号Mod（Autoplay的成绩不上传）
> 我习惯开着AquaMai的UX.ImmediateSave，有时候开着autoplay看谱面确认，结果AP+的成绩就直接传上去毁号了。为了解决这个问题，有了这个Mod。

### 用法
1. [Release](https://github.com/Starrah/DontRuinMyAccount/releases)下载DLL，或[自己编译](#编译)出DLL文件。
2. DLL文件放到Package/Mods目录下。

### 编译
- 基本上用任何IDE都能直接编译，Visual Studio和Jetbrains Rider，都是直接导入项目就可以了。

### 验证Mod已经正确运行
- 游戏启动后，MelonLoader的Log窗口中会有Log：`[DontRuinMyAccount] Initialized.`
- 进入游戏后开启Autoplay时，会有Log：`[DontRuinMyAccount] Autoplay triggered, will ignore this score.`
- 歌曲结束进入成绩页面时，也会有`"Prevented update DXRating`等相应的Log。

### Credits
Thanks [@Rika](https://github.com/RikaKagurasaka) for providing a lot of helps. 